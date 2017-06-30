namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CommentAddForm1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommentAddForms",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(nullable: false),
                        Title = c.String(),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CommentAddForms");
        }
    }
}
