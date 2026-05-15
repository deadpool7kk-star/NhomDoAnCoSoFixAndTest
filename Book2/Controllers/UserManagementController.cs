using Book2.ViewModels;
using Book2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var result = new List<UserRoleVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                result.Add(new UserRoleVM
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    HoTen = user.HoTen,
                    DiaChi = user.DiaChi,
                    Role = roles.FirstOrDefault() ?? "User",
                    IsLocked = await _userManager.IsLockedOutAsync(user)
                });
            }

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> LockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(100));
            TempData["Success"] = "Đã khóa tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UnlockUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);
            TempData["Success"] = "Đã mở khóa tài khoản thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            ViewBag.Roles = new List<string> { "Admin", "Staff", "Shipper", "User" };
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string email, string password, string role, string hoTen, string diaChi)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin.";
                return RedirectToAction(nameof(CreateUser));
            }

            var user = new ApplicationUser 
            { 
                UserName = email, 
                Email = email, 
                EmailConfirmed = true,
                HoTen = hoTen,
                DiaChi = diaChi
            };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                TempData["Success"] = "Tạo tài khoản thành công.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(CreateUser));
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string id, string email, string userName, string hoTen, string diaChi)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = email;
            user.UserName = userName;
            user.HoTen = hoTen;
            user.DiaChi = diaChi;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Cập nhật thông tin thành công.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Cập nhật thất bại.";
            return RedirectToAction(nameof(EditUser), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Xóa tài khoản thành công.";
            }
            else
            {
                TempData["Error"] = "Xóa tài khoản thất bại.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = new List<string> { "Admin", "Staff", "Shipper", "User" };
            ViewBag.CurrentRoles = await _userManager.GetRolesAsync(user);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, string selectedRole)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, selectedRole);

            TempData["Success"] = "Gán quyền thành công.";
            return RedirectToAction(nameof(Index));
        }

    }
}