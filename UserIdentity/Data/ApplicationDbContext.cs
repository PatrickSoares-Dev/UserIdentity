using Microsoft.EntityFrameworkCore;
using ConferenciaTelecall.Models.Entities;

namespace ConferenciaTelecall.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UserSystemRole> UserSystemRoles { get; set; }
        public DbSet<Systems> Systems { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserSystemRole>(entity =>
            {
                entity.ToTable("UserSystemRoles");

                entity.HasKey(usr => usr.Id);

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.SystemId).HasColumnName("system_id");
                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(usr => usr.User)
                      .WithMany()
                      .HasForeignKey(usr => usr.UserId);

                entity.HasOne(usr => usr.Systems) 
                      .WithMany(s => s.UserSystemRoles)
                      .HasForeignKey(usr => usr.SystemId);

                entity.HasOne(usr => usr.Role)
                      .WithMany()
                      .HasForeignKey(usr => usr.RoleId);
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Setor)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.SetorId);
        }
    }
}