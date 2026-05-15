using Book2.Data;
using Book2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSystemSettingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSystemSettingController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.SystemSettings.ToListAsync();
            if (!settings.Any())
            {
                var defaults = new List<SystemSetting>
                {
                    new SystemSetting { Key = "StoreName", Value = "BookShop Online", Description = "Tên cửa hàng" },
                    new SystemSetting { Key = "Hotline", Value = "1900 1234", Description = "Số điện thoại hỗ trợ" },
                    new SystemSetting { Key = "Email", Value = "support@bookshop.com", Description = "Email liên hệ" },
                    new SystemSetting { Key = "Address", Value = "123 Đường ABC, Hà Nội", Description = "Địa chỉ cửa hàng" }
                };
                _context.SystemSettings.AddRange(defaults);
                await _context.SaveChangesAsync();
                settings = defaults;
            }
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Dictionary<int, string> settings)
        {
            foreach (var item in settings)
            {
                var setting = await _context.SystemSettings.FindAsync(item.Key);
                if (setting != null)
                {
                    setting.Value = item.Value;
                }
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Lưu cấu hình hệ thống thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
