
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Net;

namespace Admin.Controllers
{
	[Route("[controller]/[action]")]
	public class SystemController : Controller
	{
		private readonly ILogger<SystemController> _logger;

		public SystemController(ILogger<SystemController> logger)
		{
			_logger = logger;
		}

		private bool IsAjaxRequest()
		{
			var xrw = Request.Headers["X-Requested-With"].ToString();
			var accept = Request.Headers.ContainsKey("Accept") ? Request.Headers["Accept"].ToString() : string.Empty;

			return xrw == "XMLHttpRequest" || accept.Contains("application/json", StringComparison.OrdinalIgnoreCase);
		}

		[Route("/System/Error")]
		public IActionResult Error()
		{
			var exceptionFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

			if (exceptionFeature != null)
			{
				var exception = exceptionFeature.Error;
				_logger.LogError(exception, "Unhandled exception caught by SystemController.");

				if (exception is HttpRequestException)
				{
					if (IsAjaxRequest())
						return Content("Network error. Please check your internet connection or try again later.");
					 
					ViewBag.Message = "Network error. Please check your internet connection or try again later.";
					ViewBag.StatusCode = (int)HttpStatusCode.BadGateway;
					return View("GatewayError");
				}
				else
				{
					if (IsAjaxRequest())
						return Content("An unexpected error occurred. Please try again later.");
					 

					ViewBag.Message = "An unexpected error occurred. Please try again later.";
					ViewBag.StatusCode = 500;
					return View("Error");
				}
			}
			if (IsAjaxRequest())
				return Content("An unknown error occurred.");

			 
			ViewBag.Message = "An unknown error occurred.";
			ViewBag.StatusCode = 500;
			return View("Error");
		}

		[Route("/System/StatusCode")]
		public IActionResult StatusCodeHandler(int code)
		{
			ViewBag.StatusCode = code;
			string message;

			switch (code)
			{
				case 404:
					message = "The requested information couldn't be loaded right now";
					break;
				case 502:
					message = "Bad Gateway. The server is not responding properly.";
					break;
				case 503:
					message = "Service unavailable. Please try again later.";
					break;
				case 504:
					message = "Gateway Timeout. The server took too long to respond.";
					break;
				case 500:
					message = "Something went wrong while processing your request. Please try again.";
					break;
				default:
					message = "Something went wrong while processing your request. Please try again.";
					break;
			}

			if (IsAjaxRequest())
				return Content(message);



			ViewBag.Message = message;

			if (code == 404)
				return View("NotFound");
			else if (code == 502 || code == 504)
				return View("GatewayError");
			else if (code == 503)
				return View("ServiceUnavailable");
			else
				return View("Error");
		}

		[HttpGet("/System/CheckNetwork")]
		public async Task<IActionResult> CheckNetwork()
		{
			try
			{
				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromSeconds(3);
					var response = await client.GetAsync("https://www.google.com");

					if (response.IsSuccessStatusCode)
						return Ok(new { success = true, message = "Internet connection is active." });
					else
						return StatusCode((int)response.StatusCode, new {message = "Network reachable but service returned error." });
				}
			}
			catch (HttpRequestException)
			{
				return StatusCode(502, new {message = "Internet connection is down or unstable." });
			}
			catch (TaskCanceledException)
			{
				return StatusCode(504, new {message = "Network timeout. Please check your connection." });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while checking network connectivity.");
				return StatusCode(500, new {message = "Unexpected error while checking network." });
			}
		}
	}
}
 

