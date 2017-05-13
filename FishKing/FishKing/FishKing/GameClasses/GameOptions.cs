using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GameClasses
{
    [Serializable]
    public class GameOptions : INotifyPropertyChanged
    {
        private float soundEffectsVolume;
        public float SoundEffectsVolume
        {
            get
            {
                return soundEffectsVolume;
            }
            set
            {
                soundEffectsVolume = value;
                OnPropertyChanged(nameof(SoundEffectsVolume));
            }
        }

        private float musicVolume;
        public float MusicVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                musicVolume = value;
                OnPropertyChanged(nameof(MusicVolume));
            }
        }

        private float ambientVolume;
        public float AmbientVolume
        {
            get
            {
                return ambientVolume;
            }
            set
            {
                ambientVolume = value;
                OnPropertyChanged(nameof(AmbientVolume));
            }
        }

        private float difficulty;
        public float Difficulty
        {
            get
            {
                return difficulty;
            }
            set
            {
                difficulty = value;
                OnPropertyChanged(nameof(Difficulty));
            }
        }

        private const float DefaultSoundEffectsVolume = 1f;
        private const float DefaultMusicVolume = 0.5f;
        private const float DefaultAmbientVolume = 1f;
        private const float DefaultDifficulty = 0.5f;

        public event PropertyChangedEventHandler PropertyChanged;

        public GameOptions()
        {
            PropertyChanged += GameOptions_PropertyChanged;
            SoundEffectsVolume = DefaultSoundEffectsVolume;
            MusicVolume = DefaultMusicVolume;
            AmbientVolume = DefaultAmbientVolume;
            Difficulty = DefaultDifficulty;
        }

        private void GameOptions_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(AmbientVolume): AmbientAudioManager.Volume = AmbientVolume; break;
                case nameof(MusicVolume): MusicManager.Volume = MusicVolume; break;
            }
            
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
