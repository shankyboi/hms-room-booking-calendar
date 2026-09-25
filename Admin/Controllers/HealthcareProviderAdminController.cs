namespace Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class HealthcareProviderAdminController : BaseController
    {
        public HealthcareProviderAdminController(IConfiguration configuration) : base(configuration) { }

        public IActionResult Dashboard()
        {
            if (!HasRoleAuthorization("HealthcareProviderAdmin", "Dashboard") &&
                !HasUserRole("Healthcare Provider Admin"))
                return RedirectToAction("RoleAuthorizationFailed", "users");

            return View("~/Views/FrontDesk/Dashboard.cshtml");
        }
    }
}
