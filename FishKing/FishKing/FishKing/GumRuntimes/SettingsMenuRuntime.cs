using FishKing.Managers;
using FishKing.UtilityClasses;
using FlatRedBall.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class SettingsMenuRuntime
    {
        private SoundEffectInstance testSound;

        partial void CustomInitialize()
        {
            CloseButton.Click += CloseButton_Click;

            SoundSettingSlider.SettingValue = OptionsManager.Options.SoundEffectsVolume;
            MusicSettingSlider.SettingValue = OptionsManager.Options.MusicVolume;
            AmbientSettingSlider.SettingValue = OptionsManager.Options.AmbientVolume;
            DifficultySettingSlider.SettingValue = OptionsManager.Options.Difficulty;

            testSound = GlobalContent.LineSnap.CreateInstance();

            SoundSettingSlider.OnSettingChanged = (float newValue) => {
                OptionsManager.Options.SoundEffectsVolume = newValue;
                testSound.Volume = newValue;
                testSound.Play();
            };
            MusicSettingSlider.OnSettingChanged = (float newValue) => { OptionsManager.Options.MusicVolume = newValue; };
            AmbientSettingSlider.OnSettingChanged = (float newValue) => { OptionsManager.Options.AmbientVolume = newValue; };
            DifficultySettingSlider.OnSettingChanged = (float newValue) => { OptionsManager.Options.Difficulty = newValue; };
        }

        private void CloseButton_Click(FlatRedBall.Gui.IWindow window)
        {
            this.Visible = false;
        }

        internal void HandleMovement(I2DInput input)
        {
            var direction = CardinalTimedDirection.GetDesiredDirection(input);
            var sliders = SettingsContainer.Children.Cast<SettingsSliderRuntime>();

            if (direction == Entities.Direction.Left || direction == Entities.Direction.Right)
            {
                var settingSlider = sliders.Where(slider => slider.IsHighlighted).FirstOrDefault();
                settingSlider?.HandleDirection(direction);
            }
            else if (direction != Entities.Direction.None)
            {
                if (CloseButton.IsHighlighted && direction == Entities.Direction.Up)
                {
                    CloseButton.UnhighlightButton();
                    var bottomSlider = sliders.LastOrDefault();
                    bottomSlider.HighlightSlider();
                }
                else if (sliders.Where(slider => (slider as SettingsSliderRuntime).IsHighlighted).Any())
                {
                    var slidersAsList = sliders.ToList();
                    var currentSlider = sliders.Where(slider => slider.IsHighlighted).FirstOrDefault();
                    var currentIdx = slidersAsList.IndexOf(currentSlider);
                    currentSlider.UnhighlightSlider();
                    if (currentIdx > 0 && direction == Entities.Direction.Up)
                    {
                        slidersAsList.ElementAt(currentIdx - 1).HighlightSlider();
                    } else if (direction == Entities.Direction.Down)
                    {
                        if (currentIdx < slidersAsList.Count - 1)
                        {
                            slidersAsList.ElementAt(currentIdx + 1).HighlightSlider();
                        }
                        else
                        {
                            CloseButton.HighlightButton();
                        }
                    }
                }
            }
        }


        internal void HandleSelect()
        {
            if (CloseButton.IsHighlighted)
            {
                CloseButton.CallClick();
            }
        }
    }
}
