namespace LocationScout.DataAccess
{
    using LocationScout.Model;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class LocationScoutContext : DbContext
    {
        // Your context has been configured to use a 'LoactionSout' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'LocationScout.DataAccess.LocationScout' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'LocationScout' 
        // connection string in the application configuration file.
        public LocationScoutContext()
            : base("name=LocationScout")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<SubArea> SubAreas { get; set; }

        public DbSet<SubjectLocation> SubjectLocations { get; set; }
        public DbSet<ParkingLocation> ParkingLocations { get; set; }
        public DbSet<ShootingLocation> ShootingLocations { get; set; }  
        public DbSet<Photo> Photos { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SubjectLocation>()
                        .HasRequired(s => s.Country);

            modelBuilder.Entity<SubjectLocation>()
                        .HasRequired(s => s.Area);

            modelBuilder.Entity<SubjectLocation>()
                        .HasRequired(s => s.SubArea);

            modelBuilder.Entity<Photo>()
                        .HasRequired(p => p.ShootingLocation);


            /*       
            modelBuilder.Entity<Country>()
                        .HasRequired(c => c.Name);

            modelBuilder.Entity<Area>()
                        .HasRequired(a => a.Name);

            modelBuilder.Entity<SubArea>()
                        .HasRequired(s => s.Name);

            modelBuilder.Entity<SubjectLocation>()
                        .HasRequired(s => s.Name);

            modelBuilder.Entity<ShootingLocation>()
                        .HasRequired(s => s.Name);

            modelBuilder.Entity<ParkingLocation>()
            .HasRequired(s => s.Name);*/
        }

    }
}

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
   
