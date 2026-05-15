using Book2.Data;
using Book2.Models;
using System.Security.Claims;

namespace Book2.Services
{
    public interface ILogService
    {
        Task LogAsync(string action, string controller, string detail, ClaimsPrincipal user, string? ip = null);
    }

    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string action, string controller, string detail, ClaimsPrincipal user, string? ip = null)
        {
            var log = new AuditLog
            {
                Action = action,
                Controller = controller,
                Detail = detail,
                Timestamp = DateTime.Now,
                IPAddress = ip,
                UserId = user.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = user.Identity?.Name
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
