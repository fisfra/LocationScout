namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Areas",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.SubAreas",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.CountryAreas",
                c => new
                    {
                        Country_Name = c.String(nullable: false, maxLength: 128),
                        Area_Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Country_Name, t.Area_Name })
                .ForeignKey("dbo.Countries", t => t.Country_Name, cascadeDelete: true)
                .ForeignKey("dbo.Areas", t => t.Area_Name, cascadeDelete: true)
                .Index(t => t.Country_Name)
                .Index(t => t.Area_Name);
            
            CreateTable(
                "dbo.SubAreaAreas",
                c => new
                    {
                        SubArea_Name = c.String(nullable: false, maxLength: 128),
                        Area_Name = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SubArea_Name, t.Area_Name })
                .ForeignKey("dbo.SubAreas", t => t.SubArea_Name, cascadeDelete: true)
                .ForeignKey("dbo.Areas", t => t.Area_Name, cascadeDelete: true)
                .Index(t => t.SubArea_Name)
                .Index(t => t.Area_Name);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubAreaAreas", "Area_Name", "dbo.Areas");
            DropForeignKey("dbo.SubAreaAreas", "SubArea_Name", "dbo.SubAreas");
            DropForeignKey("dbo.CountryAreas", "Area_Name", "dbo.Areas");
            DropForeignKey("dbo.CountryAreas", "Country_Name", "dbo.Countries");
            DropIndex("dbo.SubAreaAreas", new[] { "Area_Name" });
            DropIndex("dbo.SubAreaAreas", new[] { "SubArea_Name" });
            DropIndex("dbo.CountryAreas", new[] { "Area_Name" });
            DropIndex("dbo.CountryAreas", new[] { "Country_Name" });
            DropTable("dbo.SubAreaAreas");
            DropTable("dbo.CountryAreas");
            DropTable("dbo.SubAreas");
            DropTable("dbo.Countries");
            DropTable("dbo.Areas");
        }
    }
}
