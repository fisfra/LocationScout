namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PhotoPlaceUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PhotoPlaces", "PlaceSubjectLocation_Id", c => c.Long());
            CreateIndex("dbo.PhotoPlaces", "PlaceSubjectLocation_Id");
            AddForeignKey("dbo.PhotoPlaces", "PlaceSubjectLocation_Id", "dbo.SubjectLocations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PhotoPlaces", "PlaceSubjectLocation_Id", "dbo.SubjectLocations");
            DropIndex("dbo.PhotoPlaces", new[] { "PlaceSubjectLocation_Id" });
            DropColumn("dbo.PhotoPlaces", "PlaceSubjectLocation_Id");
        }
    }
}
