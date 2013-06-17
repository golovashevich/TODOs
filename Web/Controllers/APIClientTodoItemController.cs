using System.Net.Http;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers {
	[Authorize]
	public class APIClientTodoItemController : APIClientControllerBase {
		public ActionResult Index(int listId) {
			var response = CallAPIGet("TodoList", listId);
			if (response.IsSuccessStatusCode) {
				var list = response.Content.ReadAsAsync<TodoListDto>().Result;
				return View(list.Todos);
			} else {
				return RedirectToAction("Index", "APIClientTodoListController");
			}
		}

		public ActionResult Create(int listId) {
			var model = new TodoItemDto() { TodoListId = listId };
			return View(model);
		}

		[HttpPost]
		public ActionResult Create(int listId, TodoItemDto model) {
			if (!ModelState.IsValid) {
				return View();
			}
			var response = CallAPIPost("Todo", model);
			return RedirectToAction("Index", new { listId = model.TodoListId });
		}

		public ActionResult Delete(int id, int listId) {
			var response = CallAPIDelete("Todo", id);
			return RedirectToAction("Index", new { listId = listId });
		}

		public ActionResult Edit(int id, int listId) {
			var response = CallAPIGet("Todo", id);
			if (response.IsSuccessStatusCode) {
				var todo = response.Content.ReadAsAsync<TodoItemDto>().Result;
				return View(todo);
			} else {
				return RedirectToAction("Index", new { listId = listId });
			}
		}

		[HttpPost]
		public ActionResult Edit(int id, int listId, TodoItemDto item) {
			if (!ModelState.IsValid) {
				return RedirectToAction("Index", new { listId = listId });
			}
			var response = CallAPIPut("Todo", id, item);
			if (response.IsSuccessStatusCode) {
				return RedirectToAction("Index", new { listId = listId });
			}
			return View();
		}
	}
}
