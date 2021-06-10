namespace PassionProjectV1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tet : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GenresSongs", newName: "SongGenres");
            DropPrimaryKey("dbo.SongGenres");
            AddPrimaryKey("dbo.SongGenres", new[] { "Song_SongId", "Genres_GenreId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SongGenres");
            AddPrimaryKey("dbo.SongGenres", new[] { "Genres_GenreId", "Song_SongId" });
            RenameTable(name: "dbo.SongGenres", newName: "GenresSongs");
        }
    }
}
