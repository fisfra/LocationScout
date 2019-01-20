namespace LocationScout.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Photo4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Photos", "ImageBytes", c => c.Binary());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Photos", "ImageBytes", c => c.Binary(maxLength: 16, fixedLength: true));
        }
    }
}
