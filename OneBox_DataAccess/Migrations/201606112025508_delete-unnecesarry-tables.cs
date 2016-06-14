namespace OneBox_DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleteunnecesarrytables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StorageAccountSharedBlobs", "StorageAccount_StorageAccountId", "dbo.StorageAccounts");
            DropForeignKey("dbo.StorageAccountSharedBlobs", "SharedBlob_SharedBlobId", "dbo.SharedBlobs");
            DropIndex("dbo.StorageAccountSharedBlobs", new[] { "StorageAccount_StorageAccountId" });
            DropIndex("dbo.StorageAccountSharedBlobs", new[] { "SharedBlob_SharedBlobId" });
            DropTable("dbo.SharedBlobs");
            DropTable("dbo.StorageAccounts");
            DropTable("dbo.StorageAccountSharedBlobs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StorageAccountSharedBlobs",
                c => new
                    {
                        StorageAccount_StorageAccountId = c.Int(nullable: false),
                        SharedBlob_SharedBlobId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.StorageAccount_StorageAccountId, t.SharedBlob_SharedBlobId });
            
            CreateTable(
                "dbo.StorageAccounts",
                c => new
                    {
                        StorageAccountId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.StorageAccountId);
            
            CreateTable(
                "dbo.SharedBlobs",
                c => new
                    {
                        SharedBlobId = c.Int(nullable: false, identity: true),
                        BlobPath = c.String(),
                    })
                .PrimaryKey(t => t.SharedBlobId);
            
            CreateIndex("dbo.StorageAccountSharedBlobs", "SharedBlob_SharedBlobId");
            CreateIndex("dbo.StorageAccountSharedBlobs", "StorageAccount_StorageAccountId");
            AddForeignKey("dbo.StorageAccountSharedBlobs", "SharedBlob_SharedBlobId", "dbo.SharedBlobs", "SharedBlobId", cascadeDelete: true);
            AddForeignKey("dbo.StorageAccountSharedBlobs", "StorageAccount_StorageAccountId", "dbo.StorageAccounts", "StorageAccountId", cascadeDelete: true);
        }
    }
}
