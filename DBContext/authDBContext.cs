using churchWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace churchWebAPI.DBContext
{
    public class authDBContext : DbContext
    {
        public authDBContext(DbContextOptions<authDBContext> options) : base(options)
        {
        }

        public DbSet<clsRegister> mstRegister { get; set; }
        public DbSet<clsBibleVerse> trnBibleVerses { get; set; }
        public DbSet<BibleVerseV2> trnBibleVersesV2 { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<clsRegister>(entity =>
            {
                entity.HasKey(e => e.MstUserId);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ConfirmPassword).IsRequired().HasMaxLength(255);
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
            });
        }
    }
}
