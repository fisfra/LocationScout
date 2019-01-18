namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustCountryRelation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubAreas", "Country_Id", c => c.Long());
            CreateIndex("dbo.SubAreas", "Country_Id");
            AddForeignKey("dbo.SubAreas", "Country_Id", "dbo.Countries", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubAreas", "Country_Id", "dbo.Countries");
            DropIndex("dbo.SubAreas", new[] { "Country_Id" });
            DropColumn("dbo.SubAreas", "Country_Id");
        }
    }
}
