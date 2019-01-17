namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Location : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ParkingLocations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        PhotoPlace_Id = c.Long(),
                        SubjectLocation_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PhotoPlaces", t => t.PhotoPlace_Id)
                .ForeignKey("dbo.SubjectLocations", t => t.SubjectLocation_Id)
                .Index(t => t.PhotoPlace_Id)
                .Index(t => t.SubjectLocation_Id);
            
            CreateTable(
                "dbo.PhotoPlaces",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShootingLocations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ParkingLocation_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ParkingLocations", t => t.ParkingLocation_Id)
                .Index(t => t.ParkingLocation_Id);
            
            CreateTable(
                "dbo.SubjectLocations",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LocationName = c.String(),
                        SubjectArea_Name = c.String(maxLength: 128),
                        SubjectCountry_Name = c.String(maxLength: 128),
                        SubjectSubArea_Name = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.SubjectArea_Name)
                .ForeignKey("dbo.Countries", t => t.SubjectCountry_Name)
                .ForeignKey("dbo.SubAreas", t => t.SubjectSubArea_Name)
                .Index(t => t.SubjectArea_Name)
                .Index(t => t.SubjectCountry_Name)
                .Index(t => t.SubjectSubArea_Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubjectLocations", "SubjectSubArea_Name", "dbo.SubAreas");
            DropForeignKey("dbo.SubjectLocations", "SubjectCountry_Name", "dbo.Countries");
            DropForeignKey("dbo.SubjectLocations", "SubjectArea_Name", "dbo.Areas");
            DropForeignKey("dbo.ParkingLocations", "SubjectLocation_Id", "dbo.SubjectLocations");
            DropForeignKey("dbo.ShootingLocations", "ParkingLocation_Id", "dbo.ParkingLocations");
            DropForeignKey("dbo.ParkingLocations", "PhotoPlace_Id", "dbo.PhotoPlaces");
            DropIndex("dbo.SubjectLocations", new[] { "SubjectSubArea_Name" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectCountry_Name" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectArea_Name" });
            DropIndex("dbo.ShootingLocations", new[] { "ParkingLocation_Id" });
            DropIndex("dbo.ParkingLocations", new[] { "SubjectLocation_Id" });
            DropIndex("dbo.ParkingLocations", new[] { "PhotoPlace_Id" });
            DropTable("dbo.SubjectLocations");
            DropTable("dbo.ShootingLocations");
            DropTable("dbo.PhotoPlaces");
            DropTable("dbo.ParkingLocations");
        }
    }
}
