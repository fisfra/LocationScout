namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Photo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ImageBytes = c.Binary(maxLength: 16, fixedLength: true),
                        ShootingLocation_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ShootingLocations", t => t.ShootingLocation_Id)
                .Index(t => t.ShootingLocation_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "ShootingLocation_Id", "dbo.ShootingLocations");
            DropIndex("dbo.Photos", new[] { "ShootingLocation_Id" });
            DropTable("dbo.Photos");
        }
    }
}
