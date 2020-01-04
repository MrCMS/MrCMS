namespace MrCMS.DbConfiguration
{
    //public class CreateIdentityModel : ICreateModel
    //{
    //    public void Create(ModelBuilder builder)
    //    {
    //        builder.Entity<User>(b =>
    //        {
    //            b.HasKey(u => u.Id);
    //            b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
    //            b.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");
    //            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
    //            b.Property(u => u.UserName).HasMaxLength(256);
    //            b.Property(u => u.NormalizedUserName).HasMaxLength(256);
    //            b.Property(u => u.Email).HasMaxLength(256);
    //            b.Property(u => u.NormalizedEmail).HasMaxLength(256);
    //            b.Property(u => u.Name).HasMaxLength(256);
    //            b.Property(x => x.Latitude).HasColumnType("decimal(18,5)");
    //            b.Property(x => x.Longitude).HasColumnType("decimal(18,5)");
    //            b.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
    //            b.HasMany<UserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
    //            b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
    //        });
    //        builder.Entity<Role>(b =>
    //        {
    //            b.HasKey(r => r.Id);
    //            b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();
    //            b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
    //            b.Property(u => u.Name).HasMaxLength(256);
    //            b.Property(u => u.NormalizedName).HasMaxLength(256);
    //            b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
    //            b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
    //        });
    //        builder.Entity<UserClaim>(b =>
    //        {
    //            b.HasKey(uc => uc.Id);
    //        });
    //        builder.Entity<RoleClaim>(b =>
    //        {
    //            b.HasKey(rc => rc.Id);
    //        });
    //        builder.Entity<UserRole>(b =>
    //        {
    //            b.HasKey(r => new
    //            {
    //                r.UserId,
    //                r.RoleId
    //            });
    //        });
    //        builder.Entity<UserLogin>(b =>
    //        {
    //            b.HasKey(l => new
    //            {
    //                l.LoginProvider,
    //                l.ProviderKey
    //            });
    //        });
    //        builder.Entity<UserToken>(b =>
    //        {
    //            b.HasKey(l => new
    //            {
    //                l.UserId,
    //                l.LoginProvider,
    //                l.Name
    //            });
    //        });
    //        builder.Entity<AclRole>(b =>
    //        {
    //            b.HasKey(rule => rule.Id);
    //        });
    //    }
    //}
}