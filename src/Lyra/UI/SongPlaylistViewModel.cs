using System;
using System.Collections.Generic;
using System.Windows.Data;
using ReactiveUI;

namespace Lyra.UI
{
    public class SongPlaylistViewModel : ReactiveObject
    {
        public string Name { get; set; }

        public IList<SongViewModel> Songs { get; set; } = new List<SongViewModel>();

        public DateTime StartedAt { get; set; }

        public SongViewModel FindSong(SongViewModel current, int offsetToCurrent)
        {
            if (!Songs.Contains(current))
            {
                throw new ArgumentException("Current song must be contained in given song playlist.", nameof(current));
            }

            if (Songs.Count == 1)
            {
                return current;
            }

            var currentIndex = Songs.IndexOf(current);
            return Songs[(currentIndex + offsetToCurrent) % Songs.Count];
        }

        public static SongPlaylistViewModel FromSongsView(string name, ListCollectionView songs)
        {
            var playlistViewModel = new SongPlaylistViewModel { Name = name, StartedAt = DateTime.Now };
            for (var i = 0; i < songs.Count; i++)
            {
                playlistViewModel.Songs.Add((SongViewModel)songs.GetItemAt(i));
            }

            return playlistViewModel;
        }
    }
}