namespace OneBox_DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resetEmailTabledelete : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.EmailToContainers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EmailToContainers",
                c => new
                    {
                        EmailToContainerId = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.EmailToContainerId);
            
        }
    }
}
