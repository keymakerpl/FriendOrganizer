using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using FriendOrganizer.Model;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace FriendOrganizer.DataAcces
{
    public class FriendOrganizerDbContext : DbContext
    {
        //TODO: Make db connection setting in settings
        public FriendOrganizerDbContext() : base("FriendOrganizerDb")
        {
            
        }

        public DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Fluent API
            //modelBuilder.Configurations.Add(new FriendConfiguration());

        }
    }


    /// <summary>
    /// Fluent Api cfg example
    /// </summary>
    public class FriendConfiguration : EntityTypeConfiguration<Friend>
    {
        public FriendConfiguration()
        {
            Property(f => f.FirstName)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
