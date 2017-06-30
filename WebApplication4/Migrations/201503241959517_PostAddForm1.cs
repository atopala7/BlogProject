namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostAddForm1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PostAddForms", "Date");
            DropColumn("dbo.PostAddForms", "BlogPost");
            DropColumn("dbo.PostAddForms", "ParentId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PostAddForms", "ParentId", c => c.Int(nullable: false));
            AddColumn("dbo.PostAddForms", "BlogPost", c => c.Boolean(nullable: false));
            AddColumn("dbo.PostAddForms", "Date", c => c.DateTime(nullable: false));
        }
    }
}
