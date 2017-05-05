/*
Author : Eric Preisig
Version : 1.0.0
Date : 06.04.2017
*/

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

        //public DbSet<AppSettings> AppSettings { get; set; }
       // public DbSet<FileTrack> FileTrack { get; set; }
       // public DbSet<User> Users { get; set; }

        #endregion Public Properties

        #region Protected Methods

        /// <summary>
        /// Model the database
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<DbApplicationContext>(modelBuilder);
            System.Data.Entity.Database.SetInitializer(sqliteConnectionInitializer);

            /*modelBuilder.Entity<User>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("Users");
            });
            modelBuilder.Entity<AppSettings>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("AppSettings");
            });
            modelBuilder.Entity<FileTrack>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("FileTrack");
            });*/

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }

        #endregion Protected Methods
    }
}