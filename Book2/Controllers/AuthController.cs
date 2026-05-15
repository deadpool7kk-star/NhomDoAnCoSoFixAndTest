using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book2.Controllers
{
    [Authorize]
    public class AuthController : Controller
    {
        public IActionResult RedirectByRole()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            if (User.IsInRole("Staff"))
            {
                return RedirectToAction("Dashboard", "Staff");
            }

            if (User.IsInRole("Shipper"))
            {
                return RedirectToAction("Dashboard", "Shipper");
            }

            return RedirectToAction("Index", "Home");
        }
    }
}