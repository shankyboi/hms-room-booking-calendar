
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Policy;
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityModel;

namespace Admin.Controllers
{
	public abstract class BaseController : Controller
	{
		public HttpClient client = new HttpClient();
		public string apiURL = "";
		public BaseController(IConfiguration configuration)
		{
			apiURL = configuration["ApiSettings:apiURL"];

		}
		public virtual HttpClient getHttpClient()
		{
			var newClient = new HttpClient();
			newClient.BaseAddress = new Uri(apiURL);
			newClient.DefaultRequestHeaders.Accept.Clear();
			newClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("NalamVazhatoken"));
			newClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			newClient.DefaultRequestHeaders.Add("AuthProvider", HttpContext.Session.GetString("NalamVazhaAuthProvider"));

			return newClient;
		}

		protected virtual bool HasRoleAuthorization(string controllerName, string actionMethodName)
		{
			if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("NalamVazhaloginUserID")) ||
				string.IsNullOrWhiteSpace(HttpContext.Session.GetString("NalamVazharole_JSON")))
			{
				return false;
			}

			DataTable roleAuthorizations = HttpContext.Session.GetSession<DataTable>("NalamVazharoles");
			if (roleAuthorizations == null || roleAuthorizations.Rows.Count == 0)
			{
				return false;
			}

			if (!roleAuthorizations.Columns.Contains("controllername") ||
				!roleAuthorizations.Columns.Contains("actionmethodname"))
			{
				return false;
			}

			string escapedControllerName = controllerName.Replace("'", "''");
			string escapedActionMethodName = actionMethodName.Replace("'", "''");
			DataView roleView = new DataView(roleAuthorizations);
			roleView.RowFilter = $"controllername='{escapedControllerName}' AND actionmethodname='{escapedActionMethodName}'";

			return roleView.Count > 0;
		}

		protected virtual bool HasUserRole(params string[] allowedRoles)
		{
			if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("NalamVazhaloginUserID")))
			{
				return false;
			}

			string userRole = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
			foreach (string allowedRole in allowedRoles)
			{
				if (string.Equals(userRole, allowedRole, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}

			return false;
		}

	}
}

