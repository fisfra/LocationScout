namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactoring : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ParkingLocations", "PhotoPlace_Id", "dbo.PhotoPlaces");
            DropForeignKey("dbo.ParkingLocations", "SubjectLocation_Id", "dbo.SubjectLocations");
            DropForeignKey("dbo.PhotoPlaces", "PlaceSubjectLocation_Id", "dbo.SubjectLocations");
            DropForeignKey("dbo.ShootingLocations", "ParkingLocation_Id", "dbo.ParkingLocations");
            DropForeignKey("dbo.Photos", "ShootingLocation_Id", "dbo.ShootingLocations");
            DropForeignKey("dbo.SubjectLocations", "SubjectArea_Id", "dbo.Areas");
            DropForeignKey("dbo.SubjectLocations", "SubjectCountry_Id", "dbo.Countries");
            DropForeignKey("dbo.SubjectLocations", "SubjectSubArea_Id", "dbo.SubAreas");
            DropIndex("dbo.ParkingLocations", new[] { "PhotoPlace_Id" });
            DropIndex("dbo.ParkingLocations", new[] { "SubjectLocation_Id" });
            DropIndex("dbo.PhotoPlaces", new[] { "PlaceSubjectLocation_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectArea_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectCountry_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectSubArea_Id" });
            DropIndex("dbo.ShootingLocations", new[] { "ParkingLocation_Id" });
            DropIndex("dbo.Photos", new[] { "ShootingLocation_Id" });
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectArea_Id", newName: "Area_Id");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectCountry_Id", newName: "Country_Id");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectSubArea_Id", newName: "SubArea_Id");
            CreateTable(
                "dbo.ParkingLocationShootingLocations",
                c => new
                    {
                        ParkingLocation_Id = c.Long(nullable: false),
                        ShootingLocation_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ParkingLocation_Id, t.ShootingLocation_Id })
                .ForeignKey("dbo.ParkingLocations", t => t.ParkingLocation_Id, cascadeDelete: true)
                .ForeignKey("dbo.ShootingLocations", t => t.ShootingLocation_Id, cascadeDelete: true)
                .Index(t => t.ParkingLocation_Id)
                .Index(t => t.ShootingLocation_Id);
            
            CreateTable(
                "dbo.ShootingLocationSubjectLocations",
                c => new
                    {
                        ShootingLocation_Id = c.Long(nullable: false),
                        SubjectLocation_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.ShootingLocation_Id, t.SubjectLocation_Id })
                .ForeignKey("dbo.ShootingLocations", t => t.ShootingLocation_Id, cascadeDelete: true)
                .ForeignKey("dbo.SubjectLocations", t => t.SubjectLocation_Id, cascadeDelete: true)
                .Index(t => t.ShootingLocation_Id)
                .Index(t => t.SubjectLocation_Id);
            
            AddColumn("dbo.ParkingLocations", "Name", c => c.String());
            AddColumn("dbo.PhotoPlaces", "Name", c => c.String());
            AddColumn("dbo.PhotoPlaces", "ShootingLocation_Id", c => c.Long(nullable: false));
            AddColumn("dbo.SubjectLocations", "Name", c => c.String());
            AddColumn("dbo.ShootingLocations", "Name", c => c.String());
            AlterColumn("dbo.SubjectLocations", "Area_Id", c => c.Long(nullable: false));
            AlterColumn("dbo.SubjectLocations", "Country_Id", c => c.Long(nullable: false));
            AlterColumn("dbo.SubjectLocations", "SubArea_Id", c => c.Long(nullable: false));
            AlterColumn("dbo.Photos", "ShootingLocation_Id", c => c.Long(nullable: false));
            CreateIndex("dbo.SubjectLocations", "Area_Id");
            CreateIndex("dbo.SubjectLocations", "Country_Id");
            CreateIndex("dbo.SubjectLocations", "SubArea_Id");
            CreateIndex("dbo.Photos", "ShootingLocation_Id");
            CreateIndex("dbo.PhotoPlaces", "ShootingLocation_Id");
            AddForeignKey("dbo.PhotoPlaces", "ShootingLocation_Id", "dbo.ShootingLocations", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Photos", "ShootingLocation_Id", "dbo.ShootingLocations", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubjectLocations", "Area_Id", "dbo.Areas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubjectLocations", "Country_Id", "dbo.Countries", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubjectLocations", "SubArea_Id", "dbo.SubAreas", "Id", cascadeDelete: true);
            DropColumn("dbo.ParkingLocations", "PhotoPlace_Id");
            DropColumn("dbo.ParkingLocations", "SubjectLocation_Id");
            DropColumn("dbo.PhotoPlaces", "PlaceSubjectLocation_Id");
            DropColumn("dbo.SubjectLocations", "LocationName");
            DropColumn("dbo.ShootingLocations", "ParkingLocation_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShootingLocations", "ParkingLocation_Id", c => c.Long());
            AddColumn("dbo.SubjectLocations", "LocationName", c => c.String());
            AddColumn("dbo.PhotoPlaces", "PlaceSubjectLocation_Id", c => c.Long());
            AddColumn("dbo.ParkingLocations", "SubjectLocation_Id", c => c.Long());
            AddColumn("dbo.ParkingLocations", "PhotoPlace_Id", c => c.Long());
            DropForeignKey("dbo.SubjectLocations", "SubArea_Id", "dbo.SubAreas");
            DropForeignKey("dbo.SubjectLocations", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.SubjectLocations", "Area_Id", "dbo.Areas");
            DropForeignKey("dbo.Photos", "ShootingLocation_Id", "dbo.ShootingLocations");
            DropForeignKey("dbo.PhotoPlaces", "ShootingLocation_Id", "dbo.ShootingLocations");
            DropForeignKey("dbo.ShootingLocationSubjectLocations", "SubjectLocation_Id", "dbo.SubjectLocations");
            DropForeignKey("dbo.ShootingLocationSubjectLocations", "ShootingLocation_Id", "dbo.ShootingLocations");
            DropForeignKey("dbo.ParkingLocationShootingLocations", "ShootingLocation_Id", "dbo.ShootingLocations");
            DropForeignKey("dbo.ParkingLocationShootingLocations", "ParkingLocation_Id", "dbo.ParkingLocations");
            DropIndex("dbo.ShootingLocationSubjectLocations", new[] { "SubjectLocation_Id" });
            DropIndex("dbo.ShootingLocationSubjectLocations", new[] { "ShootingLocation_Id" });
            DropIndex("dbo.ParkingLocationShootingLocations", new[] { "ShootingLocation_Id" });
            DropIndex("dbo.ParkingLocationShootingLocations", new[] { "ParkingLocation_Id" });
            DropIndex("dbo.PhotoPlaces", new[] { "ShootingLocation_Id" });
            DropIndex("dbo.Photos", new[] { "ShootingLocation_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "SubArea_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "Country_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "Area_Id" });
            AlterColumn("dbo.Photos", "ShootingLocation_Id", c => c.Long());
            AlterColumn("dbo.SubjectLocations", "SubArea_Id", c => c.Long());
            AlterColumn("dbo.SubjectLocations", "Country_Id", c => c.Long());
            AlterColumn("dbo.SubjectLocations", "Area_Id", c => c.Long());
            DropColumn("dbo.ShootingLocations", "Name");
            DropColumn("dbo.SubjectLocations", "Name");
            DropColumn("dbo.PhotoPlaces", "ShootingLocation_Id");
            DropColumn("dbo.PhotoPlaces", "Name");
            DropColumn("dbo.ParkingLocations", "Name");
            DropTable("dbo.ShootingLocationSubjectLocations");
            DropTable("dbo.ParkingLocationShootingLocations");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubArea_Id", newName: "SubjectSubArea_Id");
            RenameColumn(table: "dbo.SubjectLocations", name: "Country_Id", newName: "SubjectCountry_Id");
            RenameColumn(table: "dbo.SubjectLocations", name: "Area_Id", newName: "SubjectArea_Id");
            CreateIndex("dbo.Photos", "ShootingLocation_Id");
            CreateIndex("dbo.ShootingLocations", "ParkingLocation_Id");
            CreateIndex("dbo.SubjectLocations", "SubjectSubArea_Id");
            CreateIndex("dbo.SubjectLocations", "SubjectCountry_Id");
            CreateIndex("dbo.SubjectLocations", "SubjectArea_Id");
            CreateIndex("dbo.PhotoPlaces", "PlaceSubjectLocation_Id");
            CreateIndex("dbo.ParkingLocations", "SubjectLocation_Id");
            CreateIndex("dbo.ParkingLocations", "PhotoPlace_Id");
            AddForeignKey("dbo.SubjectLocations", "SubjectSubArea_Id", "dbo.SubAreas", "Id");
            AddForeignKey("dbo.SubjectLocations", "SubjectCountry_Id", "dbo.Countries", "Id");
            AddForeignKey("dbo.SubjectLocations", "SubjectArea_Id", "dbo.Areas", "Id");
            AddForeignKey("dbo.Photos", "ShootingLocation_Id", "dbo.ShootingLocations", "Id");
            AddForeignKey("dbo.ShootingLocations", "ParkingLocation_Id", "dbo.ParkingLocations", "Id");
            AddForeignKey("dbo.PhotoPlaces", "PlaceSubjectLocation_Id", "dbo.SubjectLocations", "Id");
            AddForeignKey("dbo.ParkingLocations", "SubjectLocation_Id", "dbo.SubjectLocations", "Id");
            AddForeignKey("dbo.ParkingLocations", "PhotoPlace_Id", "dbo.PhotoPlaces", "Id");
        }
    }
}
