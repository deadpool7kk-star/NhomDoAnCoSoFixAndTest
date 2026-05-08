using Book2.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Book2.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TheLoai> TheLoais { get; set; }
        public DbSet<Sach> Saches { get; set; }
        public DbSet<TacGia> TacGias { get; set; }
        public DbSet<NhaXuatBan> NhaXuatBans { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        public DbSet<GioHang> GioHangs { get; set; }
        public DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<PhieuNhap> PhieuNhaps { get; set; }
        public DbSet<ChiTietPhieuNhap> ChiTietPhieuNhaps { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // TacGia - Sach
            builder.Entity<Sach>()
                .HasOne(x => x.TacGiaObj)
                .WithMany(x => x.Saches)
                .HasForeignKey(x => x.TacGiaId)
                .OnDelete(DeleteBehavior.SetNull);

            // NhaXuatBan - Sach
            builder.Entity<Sach>()
                .HasOne(x => x.NhaXuatBan)
                .WithMany(x => x.Saches)
                .HasForeignKey(x => x.NhaXuatBanId)
                .OnDelete(DeleteBehavior.SetNull);

            // TheLoai - Sach
            builder.Entity<Sach>()
                .HasOne(x => x.TheLoai)
                .WithMany(x => x.Saches)
                .HasForeignKey(x => x.TheLoaiId)
                .OnDelete(DeleteBehavior.Restrict);

            // Mỗi user chỉ có 1 giỏ hàng
            builder.Entity<GioHang>()
                .HasIndex(x => x.UserId)
                .IsUnique();

            // Mỗi sách chỉ nên xuất hiện 1 lần trong 1 giỏ hàng
            builder.Entity<ChiTietGioHang>()
                .HasIndex(x => new { x.GioHangId, x.SachId })
                .IsUnique();

            builder.Entity<ChiTietGioHang>()
                .HasOne(x => x.GioHang)
                .WithMany(x => x.ChiTietGioHangs)
                .HasForeignKey(x => x.GioHangId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChiTietGioHang>()
                .HasOne(x => x.Sach)
                .WithMany()
                .HasForeignKey(x => x.SachId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ChiTietDonHang>()
                .HasOne(x => x.DonHang)
                .WithMany(x => x.ChiTietDonHangs)
                .HasForeignKey(x => x.DonHangId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ChiTietDonHang>()
                .HasOne(x => x.Sach)
                .WithMany()
                .HasForeignKey(x => x.SachId)
                .OnDelete(DeleteBehavior.Restrict);

            // GioHang - Coupon
            builder.Entity<GioHang>()
                .HasOne(x => x.Coupon)
                .WithMany()
                .HasForeignKey(x => x.CouponId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}