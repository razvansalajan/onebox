namespace OneBox_DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnecessarytableforsharing : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SharedItem_User",
                c => new
                    {
                        SharedItem_UserId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RootSharedItemId = c.Int(nullable: false),
                        RootItemVirtualPath = c.String(),
                        EmailToContainer_EmailToContainerId = c.Int(),
                    })
                .PrimaryKey(t => t.SharedItem_UserId)
                .ForeignKey("dbo.EmailToContainers", t => t.EmailToContainer_EmailToContainerId)
                .ForeignKey("dbo.RootSharedItems", t => t.RootSharedItemId, cascadeDelete: true)
                .Index(t => t.RootSharedItemId)
                .Index(t => t.EmailToContainer_EmailToContainerId);
            
            CreateTable(
                "dbo.RootSharedItems",
                c => new
                    {
                        RootSharedItemId = c.Int(nullable: false, identity: true),
                        initialRootName = c.String(),
                    })
                .PrimaryKey(t => t.RootSharedItemId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SharedItem_User", "RootSharedItemId", "dbo.RootSharedItems");
            DropForeignKey("dbo.SharedItem_User", "EmailToContainer_EmailToContainerId", "dbo.EmailToContainers");
            DropIndex("dbo.SharedItem_User", new[] { "EmailToContainer_EmailToContainerId" });
            DropIndex("dbo.SharedItem_User", new[] { "RootSharedItemId" });
            DropTable("dbo.RootSharedItems");
            DropTable("dbo.SharedItem_User");
        }
    }
}
