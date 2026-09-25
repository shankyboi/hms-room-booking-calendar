using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace Admin
{
		public class ErrorLoggingMiddleware
		{
				private readonly RequestDelegate _next;
				private readonly ILogger _logger;
				public ErrorLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
				{
				_logger = loggerFactory.CreateLogger<ErrorLoggingMiddleware>();
				_next = next;
				}
				public async Task Invoke(HttpContext context)
				{
				try
				{
				await _next(context);
				}
				catch (Exception ex)
				{
				_logger.LogError(ex, "An unhandled exception occurred in - Admin, Error Message: " + (ex.InnerException != null ? $"Inner Exception Message: {ex.InnerException.Message}, Inner Exception Stack Trace: {ex.InnerException.StackTrace}" : $"Exception Message: {ex.Message}, Stack Trace: {ex.StackTrace}"));
				throw;
}
		}
		}
}

