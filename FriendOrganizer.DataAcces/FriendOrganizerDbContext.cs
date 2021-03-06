﻿using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using FriendOrganizer.Model;

namespace FriendOrganizer.DataAcces
{
    public class FriendOrganizerDbContext : DbContext
    {
        //TODO: Make db connection setting in settings
        public FriendOrganizerDbContext() : base("FriendOrganizerDb")
        {
            
        }

        /// <summary>
        /// Lista
        /// </summary>
        public DbSet<Friend> Friends { get; set; }

        /// <summary>
        /// Ulubione języki
        /// </summary>
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }

        /// <summary>
        /// Ulubione telefony, relacja one-to-many
        /// </summary>
        public DbSet<FriendPhoneNumber> FriendPhoneNumbers { get; set; }

        public DbSet<Meeting> Meetings { get; set; }

        /// <summary>
        /// Tutaj ustawiamy jak ma być tworzona baza
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //Fluent API - zakomentowane bo użyjemy atrybutów, a następnie update-migration
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
