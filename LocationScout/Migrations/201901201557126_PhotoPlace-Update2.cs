namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotoPlaceUpdate2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ParkingLocations", "Coordinates_Latitude", c => c.Double());
            AlterColumn("dbo.ParkingLocations", "Coordinates_Longitude", c => c.Double());
            AlterColumn("dbo.SubjectLocations", "Coordinates_Latitude", c => c.Double());
            AlterColumn("dbo.SubjectLocations", "Coordinates_Longitude", c => c.Double());
            AlterColumn("dbo.ShootingLocations", "Coordinates_Latitude", c => c.Double());
            AlterColumn("dbo.ShootingLocations", "Coordinates_Longitude", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShootingLocations", "Coordinates_Longitude", c => c.Double(nullable: false));
            AlterColumn("dbo.ShootingLocations", "Coordinates_Latitude", c => c.Double(nullable: false));
            AlterColumn("dbo.SubjectLocations", "Coordinates_Longitude", c => c.Double(nullable: false));
            AlterColumn("dbo.SubjectLocations", "Coordinates_Latitude", c => c.Double(nullable: false));
            AlterColumn("dbo.ParkingLocations", "Coordinates_Longitude", c => c.Double(nullable: false));
            AlterColumn("dbo.ParkingLocations", "Coordinates_Latitude", c => c.Double(nullable: false));
        }
    }
}
