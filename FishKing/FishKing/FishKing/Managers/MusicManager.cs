using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FishKing
{
    static class MusicManager
    {
        private static bool playingSong;
        public static bool PlayingSong => playingSong;

        private static float volumeModifier = 0.35f;
        public static float Volume
        {
            get { return Microsoft.Xna.Framework.Media.MediaPlayer.Volume; }
            set { Microsoft.Xna.Framework.Media.MediaPlayer.Volume = (value* volumeModifier); }
        }

        private static List<Song> _playList;
        public static List<Song> PlayList
        {
            get { return _playList; }
            set { _playList = value;  _currentTrackNumber = 0; NumberOfPlayListLoops = 0; }
        }

        public static int TrackCount => PlayList.Count;
        private static int _currentTrackNumber = 0;

        public static int NumberOfPlayListLoops
        {
            get; set;
        }

        internal static void LoadPlayListByMapName(string mapName, bool playImmediately = true)
        {
            var newPlaylist = new List<Song>();
            var songStringList = GlobalContent.MapMusic[mapName].Songs;
            
            foreach (var songName in songStringList)
            {
                newPlaylist.Add((Song)GlobalContent.GetFile(songName));
            }

            _playList = newPlaylist;
            if (playImmediately) PlaySong();
        }

        public static void PlaySong()
        {
            if (PlayList.Count > 0)
            {
                if (_currentTrackNumber > PlayList.Count - 1) _currentTrackNumber = 0;

                var nextSong = PlayList.ElementAt(_currentTrackNumber++);
                if (!nextSong.IsDisposed)
                {
                    FlatRedBall.Audio.AudioManager.PlaySong(nextSong, true, false);
                    playingSong = true;
                    if (_currentTrackNumber > TrackCount - 1)
                    {
                        _currentTrackNumber = 0;
                        NumberOfPlayListLoops++;
                    }
                }
            }
        }

        public static void Stop()
        {
            if (FlatRedBall.Audio.AudioManager.CurrentlyPlayingSong != null)
            {
                FlatRedBall.Audio.AudioManager.StopSong();
            }
        }

        public static void Update()
        {
            if (playingSong)
            {
                var currentSong = FlatRedBall.Audio.AudioManager.CurrentlyPlayingSong;
                if (currentSong == null)
                {
                    PlaySong();
                }
            }
        }
    }
}
