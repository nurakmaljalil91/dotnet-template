using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebApiTemplate.Models;

namespace WebApiTemplate.Services
{
    public class TodoRepository : BaseRepository,  ITodoRepository
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="logger"></param>
        public TodoRepository(
            DbConnection connection,
            ILogger<TodoRepository> logger) : base(connection, logger) { }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Todo>> GetAllTodo()
        {
            return await WithConnection(async conn =>
            {
                var allTodo = new List<Todo>(); // declare items to return

                var command = conn.CreateCommand();

                var sql = $"SELECT * FROM todo";

                command.CommandText = sql;

                command.CommandType = CommandType.Text;

                await using var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        allTodo.Add(new Todo
                        {
                            Id = await reader.IsDBNullAsync(reader.GetOrdinal("id"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("id")),
                            Name = await reader.IsDBNullAsync(reader.GetOrdinal("name"))
                                ? string.Empty
                                : reader.GetString(reader.GetOrdinal("name")),
                            IsComplete = reader.GetBoolean(reader.GetOrdinal("complete")),


                        });
                    }
                }
                await reader.CloseAsync();

                return allTodo;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Todo> GetTodo(string id)
        {
            return await WithConnection(async conn =>
            {
                var todo = new Todo();

                var command = conn.CreateCommand();

                command.CommandText = $"SELECT * FROM todo WHERE id='{id}';";

                command.CommandType = CommandType.Text;

                await using var reader = await command.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        todo.Id = await reader.IsDBNullAsync(reader.GetOrdinal("id"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("id"));
                        todo.Name = await reader.IsDBNullAsync(reader.GetOrdinal("name"))
                            ? string.Empty
                            : reader.GetString(reader.GetOrdinal("name"));
                        todo.IsComplete = reader.GetBoolean(reader.GetOrdinal("complete"));

                    }
                }

                await reader.CloseAsync();

                return todo;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public async ValueTask<Response> PostTodo(Todo todo)
        {
            return await WithConnection(async conn =>
            {
                var id = Guid.NewGuid().ToString();

                var sql = $"INSERT INTO todo (id, name, complete) " +
                          $"VALUES ('{id}','{todo.Name}','false');";

                await using (var command = conn.CreateCommand())
                {
                    command.CommandText = sql;

                    command.CommandType = CommandType.Text;

                    await command.ExecuteNonQueryAsync();
                }

                var respond = new Response()
                {
                    Message = $"Success create new todo with id {id}",
                    CreateTime = DateTime.Now
                };

                return respond;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public async ValueTask<Response> PutTodo(Todo todo)
        {
            return await WithConnection(async conn =>
            {
                var checkId = $"SELECT EXISTS(SELECT 1 FROM todo WHERE todo_id='{todo.Id}')";

                var sqlString = $"UPDATE todo SET " +
                                $"name='{todo.Name}', " +
                                $"complete='{todo.IsComplete}' " +
                                $"WHERE id='{todo.Id}';";

                var success = false;

                var respond = new Response();

                await using (var command = conn.CreateCommand())
                {

                    command.CommandText = checkId;
                    command.CommandType = CommandType.Text;

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                success = reader.GetBoolean(reader.GetOrdinal("exists"));
                            }
                        }
                        await reader.CloseAsync();
                    }

                    if (success)
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        await command.ExecuteNonQueryAsync();
                        respond.Message = $"Success update the todo with todo id {todo.Id}";
                    }
                    else
                    {
                        respond.Message = $"No todo with id {todo.Id}";
                    }
                }

                respond.CreateTime = DateTime.Now;

                return respond;
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async ValueTask<Response> DeleteTodo(string id)
        {
            return await WithConnection(async conn =>
            {
                var checkId = $"SELECT EXISTS(SELECT 1 FROM todo WHERE id='{id}');";
                var sqlString = $"DELETE FROM todo WHERE id='{id}';";
                var success = false;
                var respond = new Response();
                await using (var command = conn.CreateCommand())
                {

                    command.CommandText = checkId;
                    command.CommandType = CommandType.Text;

                    await using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            while (await reader.ReadAsync())
                            {
                                success = reader.GetBoolean(reader.GetOrdinal("exists"));
                            }
                        }
                        await reader.CloseAsync();
                    }

                    if (success)
                    {
                        command.CommandText = sqlString;
                        command.CommandType = CommandType.Text;
                        await command.ExecuteNonQueryAsync();
                        respond.Message = $"Success delete the todo with todo id {id}";
                    }
                    else
                    {
                        respond.Message = $"No todo with todo id {id}";
                    }

                }

                respond.CreateTime = DateTime.Now;

                return respond;
            });
        }

        public async ValueTask<bool> CheckId(string id)
        {
            return await WithConnection(async conn =>
            {
                var exist = false;

                var checkId = $"SELECT EXISTS(SELECT 1 FROM todo WHERE id='{id}');";

                await using var command = conn.CreateCommand();

                command.CommandText = checkId;

                command.CommandType = CommandType.Text;

                await using var reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        exist = reader.GetBoolean(reader.GetOrdinal("exists"));
                    }
                }
                await reader.CloseAsync();

                return exist;
            });
        }


    }
}