namespace AdoNET.EFCodeFirst.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Course",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Descriptions = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Enrollment",
                c => new
                    {
                        EnrollmentId = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        StudentId = c.Int(nullable: false),
                        Grade = c.Int(),
                    })
                .PrimaryKey(t => t.EnrollmentId)
                .ForeignKey("dbo.Course", t => t.CourseId, cascadeDelete: true)
                .ForeignKey("dbo.Student", t => t.StudentId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.StudentId);
            
            CreateTable(
                "dbo.Student",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 255),
                        LastName = c.String(nullable: false, maxLength: 255),
                        Motivation = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Joined = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Enrollment", "StudentId", "dbo.Student");
            DropForeignKey("dbo.Enrollment", "CourseId", "dbo.Course");
            DropIndex("dbo.Enrollment", new[] { "StudentId" });
            DropIndex("dbo.Enrollment", new[] { "CourseId" });
            DropTable("dbo.Student");
            DropTable("dbo.Enrollment");
            DropTable("dbo.Course");
        }
    }
}
