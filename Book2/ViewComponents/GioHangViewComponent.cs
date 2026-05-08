using Book2.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Book2.ViewComponents
{
    public class GioHangViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public GioHangViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int soLuong = 0;

            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {
                    soLuong = await _context.ChiTietGioHangs
                        .Where(x => x.GioHang.UserId == userId)
                        .SumAsync(x => (int?)x.SoLuong) ?? 0;
                }
            }

            return View(soLuong);
        }
    }
}