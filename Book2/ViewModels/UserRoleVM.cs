namespace Book2.ViewModels
{
    public class UserRoleVM
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? HoTen { get; set; }
        public string? DiaChi { get; set; }
        public string Role { get; set; } = "User";
        public bool IsLocked { get; set; }
    }
}