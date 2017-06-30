namespace WebApplication4.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PostList : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PostViewModels", "PostUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PostViewModels", new[] { "PostUser_Id" });
            CreateTable(
                "dbo.PostAddForms",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        BlogPost = c.Boolean(nullable: false),
                        ParentId = c.Int(nullable: false),
                        Title = c.String(),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.PostId);
            
            DropTable("dbo.PostViewModels");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PostViewModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        BlogPost = c.Boolean(nullable: false),
                        Title = c.String(),
                        Message = c.String(),
                        ParentId = c.Int(nullable: false),
                        PostUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.PostAddForms");
            CreateIndex("dbo.PostViewModels", "PostUser_Id");
            AddForeignKey("dbo.PostViewModels", "PostUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
