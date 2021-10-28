namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            //Test your solutions here
            var result = ExportSongsAboveDuration(context, 4);

            File.WriteAllText("../../../result.txt", result);

        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {

            var sb = new StringBuilder();

            var albums = context.Albums.Where(x => x.ProducerId == producerId).Select(x => new
            {
                albumName = x.Name,
                releaseDate = x.ReleaseDate,
                producerName = x.Producer.Name,
                albumSongs = x.Songs.Select(x => new
                {
                    songName = x.Name,
                    songPrice = x.Price,
                    songWriter = x.Writer.Name
                }).ToList()
            }).ToList();


            foreach (var album in albums.OrderByDescending(x => x.albumSongs.Sum(x => x.songPrice)))
            {
                sb.AppendLine($"-AlbumName: {album.albumName}");
                sb.AppendLine($"-ReleaseDate: {album.releaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}");
                sb.AppendLine($"-ProducerName: {album.producerName}");
                sb.AppendLine("-Songs:");

                var songCount = 1;

                foreach (var song in album.albumSongs.OrderByDescending(x => x.songName).ThenBy(x => x.songWriter))
                {

                    sb.AppendLine($"---#{songCount++}");
                    sb.AppendLine($"---SongName: {song.songName}");
                    sb.AppendLine($"---Price: {song.songPrice:F2}");
                    sb.AppendLine($"---Writer: {song.songWriter}");
                }
                var totalPrice = album.albumSongs.Sum(x => x.songPrice);
                sb.AppendLine($"-AlbumPrice: {totalPrice:F2}");

            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {

            var sb = new StringBuilder();

            var songs = context.Songs
                .Select(x => new
                {
                    songName = x.Name,
                    writerName = x.Writer.Name,
                    performerFullName = x.SongPerformers
                    .Select(x => x.Performer.FirstName + " " + x.Performer.LastName)
                    .FirstOrDefault(),
                    albumProducer = x.Album.Producer.Name,
                    duration = x.Duration
                })
                .OrderBy(x => x.songName)
                .ThenBy(x => x.writerName)
                .ThenBy(x => x.performerFullName)
                .ToList();

            var filter = songs.Where(x => x.duration.TotalSeconds > duration).ToList();


            var counter = 1;
            foreach (var song in filter)
            {
                sb.AppendLine($"-Song #{counter++}");
                sb.AppendLine($"---SongName: {song.songName}");
                sb.AppendLine($"---Writer: {song.writerName}");
                sb.AppendLine($"---Performer: {song.performerFullName}");
                sb.AppendLine($"---Duration: {song.duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
