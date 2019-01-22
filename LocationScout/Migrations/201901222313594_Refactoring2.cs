namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactoring2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PhotoPlaces", "ShootingLocation_Id", "dbo.ShootingLocations");
            DropIndex("dbo.PhotoPlaces", new[] { "ShootingLocation_Id" });
            DropTable("dbo.PhotoPlaces");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PhotoPlaces",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        ShootingLocation_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.PhotoPlaces", "ShootingLocation_Id");
            AddForeignKey("dbo.PhotoPlaces", "ShootingLocation_Id", "dbo.ShootingLocations", "Id", cascadeDelete: true);
        }
    }
}
