namespace OneBox_DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class email_to_container_table_delete_containername_column : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.EmailToContainers", "ContainerName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EmailToContainers", "ContainerName", c => c.String());
        }
    }
}
