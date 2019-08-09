namespace Sandbox_EF6
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PressReleaseDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Fk1 = c.Int(nullable: false),
                        PressReleaseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PressReleases", t => t.Fk1)
                .ForeignKey("dbo.PressReleases", t => t.PressReleaseId)
                .Index(t => t.Fk1)
                .Index(t => t.PressReleaseId);
            
            CreateTable(
                "dbo.PressReleases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PressReleaseDetailPressReleasesId = c.Int(name: "PressReleaseDetail.PressReleasesId", nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PressReleaseDetails", t => t.PressReleaseDetailPressReleasesId)
                .Index(t => t.PressReleaseDetailPressReleasesId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PressReleases", "PressReleaseDetail.PressReleasesId", "dbo.PressReleaseDetails");
            DropForeignKey("dbo.PressReleaseDetails", "PressReleaseId", "dbo.PressReleases");
            DropForeignKey("dbo.PressReleaseDetails", "Fk1", "dbo.PressReleases");
            DropIndex("dbo.PressReleases", new[] { "PressReleaseDetail.PressReleasesId" });
            DropIndex("dbo.PressReleaseDetails", new[] { "PressReleaseId" });
            DropIndex("dbo.PressReleaseDetails", new[] { "Fk1" });
            DropTable("dbo.PressReleases");
            DropTable("dbo.PressReleaseDetails");
        }
    }
}
