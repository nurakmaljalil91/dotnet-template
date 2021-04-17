using System.Collections.Generic;
using System.Threading.Tasks;
using WebApiTemplate.Models;

namespace WebApiTemplate.Services
{
    public interface ITodoRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Todo>> GetAllTodo();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Todo> GetTodo(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        ValueTask<Response> PostTodo(Todo todo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        ValueTask<Response> PutTodo(Todo todo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ValueTask<Response> DeleteTodo(string id);

        ValueTask<bool> CheckId(string id);
    }
}