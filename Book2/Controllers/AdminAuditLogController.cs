using Book2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAuditLogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminAuditLogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(x => x.Timestamp)
                .Take(500)
                .ToListAsync();
            return View(logs);
        }
    }
}
