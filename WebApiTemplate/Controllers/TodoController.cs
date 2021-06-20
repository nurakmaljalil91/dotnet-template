using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Threading.Tasks;
using WebApiTemplate.Models;
using WebApiTemplate.Services;

namespace WebApiTemplate.Controllers
{
    public class TodoController : BaseApiController
    {
        private readonly ITodoRepository _todoRepository;

        public TodoController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTodo([FromQuery] string id)
        {

            if (id == null)
            {
                var allTodo = await _todoRepository.GetAllTodo();
                return Ok(allTodo);
            }
            else
            {
                var todo = await _todoRepository.GetTodo(id);
                return Ok(todo);
            }
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Response>> CreateTodo([FromBody] Todo todo)
        {
            var created = await _todoRepository.PostTodo(todo);

            return Ok(created);
        }

        [HttpPut()]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Response>> UpdateTodo([FromBody] Todo todo)
        {
            var updated = await _todoRepository.PutTodo(todo);

            return Ok(updated);
        }

        [HttpDelete()]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Response>> DeleteTodo([FromQuery] string id)
        {
            if (!await _todoRepository.CheckId(id))
            {
                return BadRequest();
            }
            var deleted = await _todoRepository.DeleteTodo(id);

            return Ok(deleted);
        }

    }
}
