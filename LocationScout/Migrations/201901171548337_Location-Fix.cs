namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationFix : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ParkingLocations", "Coordinates_Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.ParkingLocations", "Coordinates_Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.ShootingLocations", "Coordinates_Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.ShootingLocations", "Coordinates_Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.SubjectLocations", "Coordinates_Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.SubjectLocations", "Coordinates_Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubjectLocations", "Coordinates_Longitude");
            DropColumn("dbo.SubjectLocations", "Coordinates_Latitude");
            DropColumn("dbo.ShootingLocations", "Coordinates_Longitude");
            DropColumn("dbo.ShootingLocations", "Coordinates_Latitude");
            DropColumn("dbo.ParkingLocations", "Coordinates_Longitude");
            DropColumn("dbo.ParkingLocations", "Coordinates_Latitude");
        }
    }
}
