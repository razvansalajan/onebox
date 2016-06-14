namespace OneBox_DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnecessarytableforsharing_2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SharedItem_User", "EmailToContainer_EmailToContainerId", "dbo.EmailToContainers");
            DropIndex("dbo.SharedItem_User", new[] { "EmailToContainer_EmailToContainerId" });
            RenameColumn(table: "dbo.SharedItem_User", name: "EmailToContainer_EmailToContainerId", newName: "EmailToContainerId");
            AlterColumn("dbo.SharedItem_User", "EmailToContainerId", c => c.Int(nullable: false));
            CreateIndex("dbo.SharedItem_User", "EmailToContainerId");
            AddForeignKey("dbo.SharedItem_User", "EmailToContainerId", "dbo.EmailToContainers", "EmailToContainerId", cascadeDelete: true);
            DropColumn("dbo.SharedItem_User", "UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SharedItem_User", "UserId", c => c.Int(nullable: false));
            DropForeignKey("dbo.SharedItem_User", "EmailToContainerId", "dbo.EmailToContainers");
            DropIndex("dbo.SharedItem_User", new[] { "EmailToContainerId" });
            AlterColumn("dbo.SharedItem_User", "EmailToContainerId", c => c.Int());
            RenameColumn(table: "dbo.SharedItem_User", name: "EmailToContainerId", newName: "EmailToContainer_EmailToContainerId");
            CreateIndex("dbo.SharedItem_User", "EmailToContainer_EmailToContainerId");
            AddForeignKey("dbo.SharedItem_User", "EmailToContainer_EmailToContainerId", "dbo.EmailToContainers", "EmailToContainerId");
        }
    }
}
