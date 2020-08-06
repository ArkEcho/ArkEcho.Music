using System;
using System.IO;
using System.Runtime.Serialization;
using TagLib;

namespace ArkEcho.Core
{
    public class MusicFile
    {
        public long ID { get; set; } = 0;

        public string Title { get; set; } = string.Empty;

        public string Performer { get; private set; } = string.Empty;

        public string Album { get; private set; } = string.Empty;

        public string AlbumArtist { get; private set; } = string.Empty;

        public uint Disc { get; private set; } = 0;

        public uint DiscCount { get; private set; } = 0;

        public uint Track { get; private set; } = 0;

        public uint TrackCount { get; private set; } = 0;

        public uint Year { get; private set; } = 0;

        public string FilePath { get; set; } = string.Empty;

        public MusicFile(string FilePath)
        {
            this.FilePath = FilePath;
        }

        public bool Init()
        {
            using (TagLib.File file = TagLib.File.Create(FilePath))
            {
                Title = file.Tag.Title;
                Performer = file.Tag.FirstPerformer;
                Album = file.Tag.Album;
                AlbumArtist = file.Tag.FirstAlbumArtist;
                Disc = file.Tag.Disc;
                DiscCount = file.Tag.DiscCount;
                Track = file.Tag.Track;
                TrackCount = file.Tag.TrackCount;
                Year = file.Tag.Year;
            }
            return true;
        }
    }
}
