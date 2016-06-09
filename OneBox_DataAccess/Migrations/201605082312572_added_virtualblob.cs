namespace OneBox_DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_virtualblob : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VirtualBlobs",
                c => new
                    {
                        VirtualBlobId = c.Int(nullable: false, identity: true),
                        VirtualPath = c.String(),
                        FullAzureBlobPath = c.String(),
                    })
                .PrimaryKey(t => t.VirtualBlobId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.VirtualBlobs");
        }
    }
}
