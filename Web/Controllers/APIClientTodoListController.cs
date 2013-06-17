using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers {
	[Authorize]
	public class APIClientTodoListController : APIClientControllerBase {
		public ActionResult Index() {
			var response = CallAPIGet("TodoList");
			if (response.IsSuccessStatusCode) {
				var todoLists = response.Content.ReadAsAsync<IEnumerable<TodoListDto>>().Result;
				return View(todoLists);
			} else {
				return View();
			}
		}

		public ActionResult Create() {
			var model = new TodoListDto() { UserId = User.Identity.Name };
			return View(model);
		}

		[HttpPost]
		public ActionResult Create(TodoListDto list) {
			if (!ModelState.IsValid) {
				return View();
			}
			list.Todos = new List<TodoItemDto>();
			var result = CallAPIPost("TodoList", list);
			return RedirectToAction("Index");
		}

		public ActionResult Delete(int id) {
			var response = CallAPIDelete("TodoList", id);
			return RedirectToAction("Index");
		}

		public ActionResult Edit(int id) {
			var response = CallAPIGet("TodoList", id);
			if (response.IsSuccessStatusCode) {
				var todoList = response.Content.ReadAsAsync<TodoListDto>().Result;
				return View(todoList);
			} else {
				return RedirectToAction("Index");
			}
		}

		[HttpPost]
		public ActionResult Edit(int id, TodoListDto list) {
			if (!ModelState.IsValid) {
				return RedirectToAction("Index");
			}
			var response = CallAPIGet("TodoList", id);
			if (!response.IsSuccessStatusCode) {
				return RedirectToAction("Index");
			}
			var todoList = response.Content.ReadAsAsync<TodoListDto>().Result;
			list.Todos = todoList.Todos;

			response = CallAPIPut("TodoList", id, list);
			if (response.IsSuccessStatusCode) {
				return RedirectToAction("Index");
			}
			return View();
		}
	}
}
