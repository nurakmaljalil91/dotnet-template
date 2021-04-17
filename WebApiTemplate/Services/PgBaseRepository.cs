using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace WebApiTemplate.Services
{
    /// <summary>
    /// Act as base class for all the service that use Postgresql database.
    /// Open and close connection 
    /// </summary>
    public class PgBaseRepository
    {
        private readonly DbConnection _connection;
        private readonly ILogger<PgBaseRepository> _logger;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="logger"></param>
        public PgBaseRepository(
            DbConnection connection, 
            ILogger<PgBaseRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        // Use for buffered queries that return a type
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="getData"></param>
        /// <returns></returns>
        protected async Task<T> WithConnection<T>(Func<DbConnection, Task<T>> getData)
        {
            try
            {
                await _connection.OpenAsync();
                return await getData(_connection);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, $"{ex}");
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout",
                    ex);

            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, $"{ex}");
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getData"></param>
        /// <returns></returns>
        protected async Task WithConnection(Func<DbConnection, Task> getData)
        {
            try
            {
                await _connection.OpenAsync();
                await getData(_connection);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, $"{ex}");
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout",
                    ex);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, $"{ex}");
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TRead"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="getData"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        protected async Task<TResult> WithConnection<TRead, TResult>(Func<DbConnection, Task<TRead>> getData,
            Func<TRead, Task<TResult>> process)
        {
            try
            {
                await _connection.OpenAsync();
                var data = await getData(_connection);
                return await process(data);
            }
            catch (TimeoutException ex)
            {
                _logger.LogError(ex, $"{ex}");
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout",
                    ex);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, $"{ex}");
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
            finally
            {
                await _connection.CloseAsync();
            }
        }
    }
}