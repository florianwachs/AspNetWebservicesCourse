namespace WebAPI.Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NameRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.People", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.People", "Name", c => c.String());
        }
    }
}
