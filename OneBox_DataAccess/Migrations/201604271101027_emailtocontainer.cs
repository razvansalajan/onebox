namespace OneBox_DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emailtocontainer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailToContainers",
                c => new
                    {
                        EmailToContainerId = c.Int(nullable: false, identity: true),
                        Email = c.String(),
                        ContainerName = c.String(),
                    })
                .PrimaryKey(t => t.EmailToContainerId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EmailToContainers");
        }
    }
}
 