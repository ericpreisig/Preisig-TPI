/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

using DTO.Entity;
using SQLite.CodeFirst;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;

namespace DAL.Database
{
    [DbConfigurationType(typeof(SqLiteConfiguration))]
    public class DbApplicationContext : DbContext
    {
        #region Public Constructors

        /// <summary>
        /// Init the database : do not work with SqLite
        /// </summary>
        public DbApplicationContext() : base(new SQLiteConnection(@"Data Source = " + Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\GestionAudio\GestionAudio.db"), true)
        {
            System.Data.Entity.Database.SetInitializer(new CreateDatabaseIfNotExists<DbApplicationContext>());
            System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DbApplicationContext, Configuration>());
        }

        #endregion Public Constructors

        #region Public Properties

        public DbSet<Album> Album { get; set; }
        public DbSet<Artist> Artist { get; set; }
        public DbSet<Context> Context { get; set; }
        public DbSet<IncludeFolder> IncludeFolder { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<Playlist> Playlist { get; set; }
        public DbSet<Radio> Radio { get; set; }
        public DbSet<Track> Track { get; set; }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Model the database
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<DbApplicationContext>(modelBuilder);
            //var sqliteConnectionInitializer = new SqliteDropCreateDatabaseAlways<DbApplicationContext>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);
            modelBuilder.Entity<Genre>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("t_Genre");
            });
            modelBuilder.Entity<Playlist>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("t_Playlist");
            });
            modelBuilder.Entity<Radio>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("t_Radio");
            });
            modelBuilder.Entity<Track>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("t_Track");
            });
            modelBuilder.Entity<Album>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("t_Album");
            });

            modelBuilder.Entity<Artist>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("t_Artist");
            });
            modelBuilder.Entity<Context>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("t_Context");
            });
            modelBuilder.Entity<IncludeFolder>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("t_IncludeFolder");
            });
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }

        #endregion Protected Methods
    }
}