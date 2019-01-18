namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IdsAdded : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CountryAreas", "Area_Name", "dbo.Areas");
            DropForeignKey("dbo.SubAreaAreas", "Area_Name", "dbo.Areas");
            DropForeignKey("dbo.SubjectLocations", "SubjectArea_Name", "dbo.Areas");
            DropForeignKey("dbo.CountryAreas", "Country_Name", "dbo.Countries");
            DropForeignKey("dbo.SubjectLocations", "SubjectCountry_Name", "dbo.Countries");
            DropForeignKey("dbo.SubAreaAreas", "SubArea_Name", "dbo.SubAreas");
            DropForeignKey("dbo.SubjectLocations", "SubjectSubArea_Name", "dbo.SubAreas");
            DropIndex("dbo.SubjectLocations", new[] { "SubjectArea_Name" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectCountry_Name" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectSubArea_Name" });
            DropIndex("dbo.CountryAreas", new[] { "Country_Name" });
            DropIndex("dbo.CountryAreas", new[] { "Area_Name" });
            DropIndex("dbo.SubAreaAreas", new[] { "SubArea_Name" });
            DropIndex("dbo.SubAreaAreas", new[] { "Area_Name" });
            RenameColumn(table: "dbo.CountryAreas", name: "Country_Name", newName: "Country_Id");
            RenameColumn(table: "dbo.CountryAreas", name: "Area_Name", newName: "Area_Id");
            RenameColumn(table: "dbo.SubAreaAreas", name: "SubArea_Name", newName: "SubArea_Id");
            RenameColumn(table: "dbo.SubAreaAreas", name: "Area_Name", newName: "Area_Id");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectArea_Name", newName: "SubjectArea_Id");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectCountry_Name", newName: "SubjectCountry_Id");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectSubArea_Name", newName: "SubjectSubArea_Id");
            DropPrimaryKey("dbo.Areas");
            DropPrimaryKey("dbo.Countries");
            DropPrimaryKey("dbo.SubAreas");
            DropPrimaryKey("dbo.CountryAreas");
            DropPrimaryKey("dbo.SubAreaAreas");
            AddColumn("dbo.Areas", "Id", c => c.Long(nullable: false, identity: true));
            AddColumn("dbo.Countries", "Id", c => c.Long(nullable: false, identity: true));
            AddColumn("dbo.SubAreas", "Id", c => c.Long(nullable: false, identity: true));
            AlterColumn("dbo.Areas", "Name", c => c.String());
            AlterColumn("dbo.Countries", "Name", c => c.String());
            AlterColumn("dbo.SubAreas", "Name", c => c.String());
            AlterColumn("dbo.SubjectLocations", "SubjectArea_Id", c => c.Long());
            AlterColumn("dbo.SubjectLocations", "SubjectCountry_Id", c => c.Long());
            AlterColumn("dbo.SubjectLocations", "SubjectSubArea_Id", c => c.Long());
            AlterColumn("dbo.CountryAreas", "Country_Id", c => c.Long(nullable: false));
            AlterColumn("dbo.CountryAreas", "Area_Id", c => c.Long(nullable: false));
            AlterColumn("dbo.SubAreaAreas", "SubArea_Id", c => c.Long(nullable: false));
            AlterColumn("dbo.SubAreaAreas", "Area_Id", c => c.Long(nullable: false));
            AddPrimaryKey("dbo.Areas", "Id");
            AddPrimaryKey("dbo.Countries", "Id");
            AddPrimaryKey("dbo.SubAreas", "Id");
            AddPrimaryKey("dbo.CountryAreas", new[] { "Country_Id", "Area_Id" });
            AddPrimaryKey("dbo.SubAreaAreas", new[] { "SubArea_Id", "Area_Id" });
            CreateIndex("dbo.SubjectLocations", "SubjectArea_Id");
            CreateIndex("dbo.SubjectLocations", "SubjectCountry_Id");
            CreateIndex("dbo.SubjectLocations", "SubjectSubArea_Id");
            CreateIndex("dbo.CountryAreas", "Country_Id");
            CreateIndex("dbo.CountryAreas", "Area_Id");
            CreateIndex("dbo.SubAreaAreas", "SubArea_Id");
            CreateIndex("dbo.SubAreaAreas", "Area_Id");
            AddForeignKey("dbo.CountryAreas", "Area_Id", "dbo.Areas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubAreaAreas", "Area_Id", "dbo.Areas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubjectLocations", "SubjectArea_Id", "dbo.Areas", "Id");
            AddForeignKey("dbo.CountryAreas", "Country_Id", "dbo.Countries", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubjectLocations", "SubjectCountry_Id", "dbo.Countries", "Id");
            AddForeignKey("dbo.SubAreaAreas", "SubArea_Id", "dbo.SubAreas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.SubjectLocations", "SubjectSubArea_Id", "dbo.SubAreas", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubjectLocations", "SubjectSubArea_Id", "dbo.SubAreas");
            DropForeignKey("dbo.SubAreaAreas", "SubArea_Id", "dbo.SubAreas");
            DropForeignKey("dbo.SubjectLocations", "SubjectCountry_Id", "dbo.Countries");
            DropForeignKey("dbo.CountryAreas", "Country_Id", "dbo.Countries");
            DropForeignKey("dbo.SubjectLocations", "SubjectArea_Id", "dbo.Areas");
            DropForeignKey("dbo.SubAreaAreas", "Area_Id", "dbo.Areas");
            DropForeignKey("dbo.CountryAreas", "Area_Id", "dbo.Areas");
            DropIndex("dbo.SubAreaAreas", new[] { "Area_Id" });
            DropIndex("dbo.SubAreaAreas", new[] { "SubArea_Id" });
            DropIndex("dbo.CountryAreas", new[] { "Area_Id" });
            DropIndex("dbo.CountryAreas", new[] { "Country_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectSubArea_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectCountry_Id" });
            DropIndex("dbo.SubjectLocations", new[] { "SubjectArea_Id" });
            DropPrimaryKey("dbo.SubAreaAreas");
            DropPrimaryKey("dbo.CountryAreas");
            DropPrimaryKey("dbo.SubAreas");
            DropPrimaryKey("dbo.Countries");
            DropPrimaryKey("dbo.Areas");
            AlterColumn("dbo.SubAreaAreas", "Area_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.SubAreaAreas", "SubArea_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.CountryAreas", "Area_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.CountryAreas", "Country_Id", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.SubjectLocations", "SubjectSubArea_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.SubjectLocations", "SubjectCountry_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.SubjectLocations", "SubjectArea_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.SubAreas", "Name", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Countries", "Name", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Areas", "Name", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.SubAreas", "Id");
            DropColumn("dbo.Countries", "Id");
            DropColumn("dbo.Areas", "Id");
            AddPrimaryKey("dbo.SubAreaAreas", new[] { "SubArea_Name", "Area_Name" });
            AddPrimaryKey("dbo.CountryAreas", new[] { "Country_Name", "Area_Name" });
            AddPrimaryKey("dbo.SubAreas", "Name");
            AddPrimaryKey("dbo.Countries", "Name");
            AddPrimaryKey("dbo.Areas", "Name");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectSubArea_Id", newName: "SubjectSubArea_Name");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectCountry_Id", newName: "SubjectCountry_Name");
            RenameColumn(table: "dbo.SubjectLocations", name: "SubjectArea_Id", newName: "SubjectArea_Name");
            RenameColumn(table: "dbo.SubAreaAreas", name: "Area_Id", newName: "Area_Name");
            RenameColumn(table: "dbo.SubAreaAreas", name: "SubArea_Id", newName: "SubArea_Name");
            RenameColumn(table: "dbo.CountryAreas", name: "Area_Id", newName: "Area_Name");
            RenameColumn(table: "dbo.CountryAreas", name: "Country_Id", newName: "Country_Name");
            CreateIndex("dbo.SubAreaAreas", "Area_Name");
            CreateIndex("dbo.SubAreaAreas", "SubArea_Name");
            CreateIndex("dbo.CountryAreas", "Area_Name");
            CreateIndex("dbo.CountryAreas", "Country_Name");
            CreateIndex("dbo.SubjectLocations", "SubjectSubArea_Name");
            CreateIndex("dbo.SubjectLocations", "SubjectCountry_Name");
            CreateIndex("dbo.SubjectLocations", "SubjectArea_Name");
            AddForeignKey("dbo.SubjectLocations", "SubjectSubArea_Name", "dbo.SubAreas", "Name");
            AddForeignKey("dbo.SubAreaAreas", "SubArea_Name", "dbo.SubAreas", "Name", cascadeDelete: true);
            AddForeignKey("dbo.SubjectLocations", "SubjectCountry_Name", "dbo.Countries", "Name");
            AddForeignKey("dbo.CountryAreas", "Country_Name", "dbo.Countries", "Name", cascadeDelete: true);
            AddForeignKey("dbo.SubjectLocations", "SubjectArea_Name", "dbo.Areas", "Name");
            AddForeignKey("dbo.SubAreaAreas", "Area_Name", "dbo.Areas", "Name", cascadeDelete: true);
            AddForeignKey("dbo.CountryAreas", "Area_Name", "dbo.Areas", "Name", cascadeDelete: true);
        }
    }
}
