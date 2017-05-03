using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing
{
    static class MusicManager
    {
        private static bool playingSong;
        public static bool PlayingSong
        {
            get { return playingSong; }
        }

        public static float Volume
        {
            get { return Microsoft.Xna.Framework.Media.MediaPlayer.Volume; }
            set { Microsoft.Xna.Framework.Media.MediaPlayer.Volume = value; }
        }

        private static List<Song> playList;
        public static List<Song> PlayList
        {
            get { return playList; }
            set { playList = value;  currentTrackNumber = 0; NumberOfPlayListLoops = 0; }
        }
        public static int TrackCount
        {
            get { return PlayList.Count; }
        }
        private static int currentTrackNumber = 0;
        public static int CurrenTrackNumber
        {
            get { return currentTrackNumber; }
        }
        public static int NumberOfPlayListLoops
        {
            get; set;
        }

        public static void PlaySong()
        {
            if (PlayList.Count > 0)
            {
                FlatRedBall.Audio.AudioManager.PlaySong(PlayList.ElementAt(currentTrackNumber++), true, false);
                playingSong = true;
                if (currentTrackNumber > TrackCount-1)
                {
                    currentTrackNumber = 0;
                    NumberOfPlayListLoops++;
                }
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
