using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Threading;

namespace Admin
{
    
    public class LanguageActionFilter : IActionFilter
    {
        private readonly IConfiguration _configuration;

        public LanguageActionFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {


            string culture = "en-GB";

            if (context.HttpContext.Request.Cookies.ContainsKey("NalamVazhaCurrentCulture"))
            {
                culture = context.HttpContext.Request.Cookies["NalamVazhaCurrentCulture"];
            }
             

            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture;



        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // You can implement any logic after the action is executed if needed
        }
    }

}

