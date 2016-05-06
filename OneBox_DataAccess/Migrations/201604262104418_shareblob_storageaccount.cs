namespace OneBox_DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shareblob_storageaccount : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SharedBlobs",
                c => new
                    {
                        SharedBlobId = c.Int(nullable: false, identity: true),
                        BlobPath = c.String(),
                    })
                .PrimaryKey(t => t.SharedBlobId);
            
            CreateTable(
                "dbo.StorageAccounts",
                c => new
                    {
                        StorageAccountId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.StorageAccountId);
            
            CreateTable(
                "dbo.StorageAccountSharedBlobs",
                c => new
                    {
                        StorageAccount_StorageAccountId = c.Int(nullable: false),
                        SharedBlob_SharedBlobId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StorageAccount_StorageAccountId, t.SharedBlob_SharedBlobId })
                .ForeignKey("dbo.StorageAccounts", t => t.StorageAccount_StorageAccountId, cascadeDelete: true)
                .ForeignKey("dbo.SharedBlobs", t => t.SharedBlob_SharedBlobId, cascadeDelete: true)
                .Index(t => t.StorageAccount_StorageAccountId)
                .Index(t => t.SharedBlob_SharedBlobId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StorageAccountSharedBlobs", "SharedBlob_SharedBlobId", "dbo.SharedBlobs");
            DropForeignKey("dbo.StorageAccountSharedBlobs", "StorageAccount_StorageAccountId", "dbo.StorageAccounts");
            DropIndex("dbo.StorageAccountSharedBlobs", new[] { "SharedBlob_SharedBlobId" });
            DropIndex("dbo.StorageAccountSharedBlobs", new[] { "StorageAccount_StorageAccountId" });
            DropTable("dbo.StorageAccountSharedBlobs");
            DropTable("dbo.StorageAccounts");
            DropTable("dbo.SharedBlobs");
        }
    }
}
