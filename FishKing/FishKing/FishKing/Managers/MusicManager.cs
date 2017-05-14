﻿using Microsoft.Xna.Framework.Media;
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
        public static bool PlayingSong
        {
            get { return playingSong; }
        }

        private static float volumeModifier = 0.35f;
        public static float Volume
        {
            get { return Microsoft.Xna.Framework.Media.MediaPlayer.Volume; }
            set { Microsoft.Xna.Framework.Media.MediaPlayer.Volume = (value* volumeModifier); }
        }

        private static List<Song> playList;
        public static List<Song> PlayList
        {
            get
            {
                if (playList == null)
                {
                    playList = LoadAllSongs();
                }
                return playList;
            }
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

        private static List<Song> LoadAllSongs()
        {
            var songList = new List<Song>();

            songList.Add(GlobalContent.Audionautix_AcousticGuitar1);
            songList.Add(GlobalContent.Audionautix_OneFineDay);
            songList.Add(GlobalContent.Audionautix_Serenity);
            songList.Add(GlobalContent.Clean_Soul_Calming_Kevin_MacLeod);
            songList.Add(GlobalContent.Ketsa_tide_will_take_us_home);
            songList.Add(GlobalContent.Little_Glass_Men_The_Dweller_on_Coyote_Hill);

            return songList;
        }

        public static void PlaySong()
        {
            if (PlayList.Count > 0)
            {
                if (currentTrackNumber > PlayList.Count - 1) currentTrackNumber = 0;

                var nextSong = PlayList.ElementAt(currentTrackNumber++);
                if (!nextSong.IsDisposed)
                {
                    FlatRedBall.Audio.AudioManager.PlaySong(nextSong, true, false);
                    playingSong = true;
                    if (currentTrackNumber > TrackCount - 1)
                    {
                        currentTrackNumber = 0;
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
