using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace Web.Filters {
	public class ValidateHttpAntiForgeryTokenAttribute : AuthorizationFilterAttribute {
		public const string ANTI_FORGERY_NAME = "RequestVerificationToken";
		public override void OnAuthorization(HttpActionContext actionContext) {
			HttpRequestMessage request = actionContext.ControllerContext.Request;

			try {
				if (IsAjaxRequest(request)) {
					ValidateRequestHeader(request);
				} else {
					var cookies = request.Headers.GetCookies();
					var cookie = cookies.Select(c => c[AntiForgeryConfig.CookieName])
						.FirstOrDefault();

					if (cookie != null) {
						string cookieToken = String.Empty;
						string formToken = String.Empty;
						TryGetTokens(cookie.Value, ref cookieToken, ref formToken);
						AntiForgery.Validate(cookieToken, formToken);
					} else {
						AntiForgery.Validate();
					}
				}
			}
			catch (HttpAntiForgeryException e) {
				actionContext.Response = request.CreateErrorResponse(HttpStatusCode.Forbidden, e);
			}
		}

		private bool IsAjaxRequest(HttpRequestMessage request) {
			IEnumerable<string> xRequestedWithHeaders;
			if (request.Headers.TryGetValues("X-Requested-With", out xRequestedWithHeaders)) {
				string headerValue = xRequestedWithHeaders.FirstOrDefault();
				if (!String.IsNullOrEmpty(headerValue)) {
					return String.Equals(headerValue, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
				}
			}

			return false;
		}

		private void ValidateRequestHeader(HttpRequestMessage request) {
			string cookieToken = String.Empty;
			string formToken = String.Empty;

			IEnumerable<string> tokenHeaders;
			if (request.Headers.TryGetValues(ANTI_FORGERY_NAME, out tokenHeaders)) {
				string tokenValue = tokenHeaders.FirstOrDefault();
				TryGetTokens(tokenValue, ref cookieToken, ref formToken);
			}

			AntiForgery.Validate(cookieToken, formToken);
		}


		private static bool TryGetTokens(string value, ref string cookieToken, ref string formToken) {
			if (String.IsNullOrEmpty(value)) {
				return false;
			}
			string[] tokens = value.Split(':');
			if (tokens.Length != 2) {
				return false; 
			}
			cookieToken = tokens[0].Trim();
			formToken = tokens[1].Trim();
			return true; 
		}
	}
}