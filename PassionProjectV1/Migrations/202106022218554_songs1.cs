namespace PassionProjectV1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class songs1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        ReviewId = c.Int(nullable: false, identity: true),
                        ReviewText = c.String(),
                        fname = c.String(),
                        lname = c.String(),
                        SongId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReviewId)
                .ForeignKey("dbo.Songs", t => t.SongId, cascadeDelete: true)
                .Index(t => t.SongId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reviews", "SongId", "dbo.Songs");
            DropIndex("dbo.Reviews", new[] { "SongId" });
            DropTable("dbo.Reviews");
        }
    }
}
