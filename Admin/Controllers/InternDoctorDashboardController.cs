using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Admin.Controllers
{
    public class InternDoctorDashboardController : BaseController
    {
        public InternDoctorDashboardController(IConfiguration configuration) : base(configuration) { }

        [HttpGet]
        public IActionResult Index()
        {
            if (!HasRoleAuthorization("InternDoctorDashboard", "Index") &&
                !HasUserRole("Intern Doctor"))
                return RedirectToAction("RoleAuthorizationFailed", "users");

            ViewBag.LoginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID") ?? "";
            ViewBag.TenantId    = HttpContext.Session.GetString("NalamVazhachoosedtenantid") ?? "";
            ViewBag.UserRole    = HttpContext.Session.GetString("NalamVazhauserrole") ?? "";
            ViewBag.UserName    = HttpContext.Session.GetString("NalamVazhaloggedinusername") ?? "";
            return View();
        }
    }
}
