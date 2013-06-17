using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers {
	public class APIClientControllerBase : Controller {
		protected HttpResponseMessage CallAPIGet(string methodName, int? id = null) {
			using (var client = CreateHttpClient()) {
				if (null != id) {
					methodName += "/" + id;
				}
				return client.GetAsync(methodName).Result;  // Blocking call!
			}
		}

		protected HttpResponseMessage CallAPIPost(string methodName, object value) {
			using (var client = CreateHttpClient()) {
				return client.PostAsJsonAsync(methodName, value).Result;  // Blocking call!
			}
		}

		protected HttpResponseMessage CallAPIPut(string methodName, int id, object value) {
			using (var client = CreateHttpClient()) {
				methodName += "/" + id;
				return client.PutAsJsonAsync(methodName, value).Result;  // Blocking call!
			}
		}

		protected HttpResponseMessage CallAPIDelete(string methodName, int? id = null) {
			using (var client = CreateHttpClient()) {
				if (null != id) {
					methodName += "/" + id;
				}
				return client.DeleteAsync(methodName).Result;  // Blocking call!
			}
		}

		private HttpClient CreateHttpClient() {
			NetworkCredential credentials;
			using (var context = new UsersContext()) {
				var user = context.UserProfiles.Include("Membership").
					SingleOrDefault(u => u.UserName == User.Identity.Name);
				credentials = new NetworkCredential(user.UserName, user.Membership.Password);
			}
			var baseAddress = new Uri(ConfigurationManager.AppSettings["ApiURL"]);

			var handler = new WebRequestHandler() { Credentials = credentials };
			var client = new HttpClient(handler) { BaseAddress = baseAddress };

			string token = GetAntiForgeryToken();
			var cookieContainer = new CookieCollection();
			cookieContainer.Add(new Cookie(AntiForgeryConfig.CookieName, token));
			handler.CookieContainer.Add(baseAddress, cookieContainer);

			client.DefaultRequestHeaders.Accept.Add(
				new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}

		public static string GetAntiForgeryToken() {
			string cookieToken, formToken;
			AntiForgery.GetTokens(null, out cookieToken, out formToken);
			return cookieToken + ":" + formToken;
		}
	}
}