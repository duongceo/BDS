
using HappyRE.Core.Entities.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace HappyRE.Core.Model
{
    public partial class HappyREContext : DbContext
    {
        public HappyREContext() : base("DBC_HappyRE")
        {
            //Disable initializer
            Database.SetInitializer<HappyREContext>(null);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SetupEntities(modelBuilder);
        }
        private void SetupEntities(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<File> File { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }       
        public DbSet<Token> Token { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<RoleGroup> RoleGroup { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<Ward> Ward { get; set; }
        public DbSet<Street> Street { get; set; }
        public DbSet<IpAlloweds> IpAlloweds { get; set; }
        public DbSet<SysCode> SysCode { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<CustomerInfo> CustomerInfo { get; set; }
        public DbSet<CustomerRegionTarget> CustomerRegionTarget { get; set; }
        public DbSet<Property> Property { get; set; }
        public DbSet<SaleOrder> SaleOrder { get; set; }
        public DbSet<PropertyImage> PropertyImage { get; set; }
        public DbSet<ImageFile> ImageFile { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<NotificationRead> NotificationRead { get; set; }
    }
}
