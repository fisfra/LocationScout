namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustCountryRelation2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SubAreas", "Country_Id", "dbo.Countries");
            DropIndex("dbo.SubAreas", new[] { "Country_Id" });
            CreateTable(
                "dbo.SubAreaCountries",
                c => new
                    {
                        SubArea_Id = c.Long(nullable: false),
                        Country_Id = c.Long(nullable: false),
                    })
                .PrimaryKey(t => new { t.SubArea_Id, t.Country_Id })
                .ForeignKey("dbo.SubAreas", t => t.SubArea_Id, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.Country_Id, cascadeDelete: true)
                .Index(t => t.SubArea_Id)
                .Index(t => t.Country_Id);
            
            DropColumn("dbo.SubAreas", "Country_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SubAreas", "Country_Id", c => c.Long());
            DropForeignKey("dbo.SubAreaCountries", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.SubAreaCountries", "SubArea_Id", "dbo.SubAreas");
            DropIndex("dbo.SubAreaCountries", new[] { "Country_Id" });
            DropIndex("dbo.SubAreaCountries", new[] { "SubArea_Id" });
            DropTable("dbo.SubAreaCountries");
            CreateIndex("dbo.SubAreas", "Country_Id");
            AddForeignKey("dbo.SubAreas", "Country_Id", "dbo.Countries", "Id");
        }
    }
}
