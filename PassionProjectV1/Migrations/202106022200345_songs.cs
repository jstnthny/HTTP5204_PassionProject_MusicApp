namespace PassionProjectV1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class songs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongId = c.Int(nullable: false, identity: true),
                        SongName = c.String(),
                        Album = c.String(),
                        AlbumUrl = c.String(),
                        Artist = c.String(),
                    })
                .PrimaryKey(t => t.SongId);
            
            CreateTable(
                "dbo.Genres",
                c => new
                    {
                        GenreId = c.Int(nullable: false, identity: true),
                        Genre = c.String(),
                    })
                .PrimaryKey(t => t.GenreId);
            
            CreateTable(
                "dbo.GenresSongs",
                c => new
                    {
                        Genres_GenreId = c.Int(nullable: false),
                        Song_SongId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Genres_GenreId, t.Song_SongId })
                .ForeignKey("dbo.Genres", t => t.Genres_GenreId, cascadeDelete: true)
                .ForeignKey("dbo.Songs", t => t.Song_SongId, cascadeDelete: true)
                .Index(t => t.Genres_GenreId)
                .Index(t => t.Song_SongId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GenresSongs", "Song_SongId", "dbo.Songs");
            DropForeignKey("dbo.GenresSongs", "Genres_GenreId", "dbo.Genres");
            DropIndex("dbo.GenresSongs", new[] { "Song_SongId" });
            DropIndex("dbo.GenresSongs", new[] { "Genres_GenreId" });
            DropTable("dbo.GenresSongs");
            DropTable("dbo.Genres");
            DropTable("dbo.Songs");
        }
    }
}
