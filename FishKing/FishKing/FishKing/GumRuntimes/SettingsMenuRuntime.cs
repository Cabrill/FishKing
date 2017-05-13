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

#if WINDOWS
            FullScreenButton.Click += FullScreenButton_Click;
            FullScreenButton.ButtonText = "Windowed";
#else
            FullScreenButton.Visible = false;
#endif
        }

        private void FullScreenButton_Click(FlatRedBall.Gui.IWindow window)
        {
#if WINDOWS
            System.IntPtr hWnd = FlatRedBall.FlatRedBallServices.Game.Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(hWnd);
            var form = control.FindForm();

            if (form.WindowState == System.Windows.Forms.FormWindowState.Maximized)
            {
                form.WindowState = System.Windows.Forms.FormWindowState.Normal;
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                form.SetBounds(0, 0, 1280, 720);
                FullScreenButton.ButtonText = "Full Screen";
            }
            else
            {
                var graphics = FlatRedBall.FlatRedBallServices.GraphicsOptions;
                form.SetDesktopBounds(0, 0, graphics.ResolutionWidth, graphics.ResolutionHeight);
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
                FullScreenButton.ButtonText = "Windowed";
            }
#endif
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
                if (CloseButton.IsHighlighted && direction == Entities.Direction.Left)
                {
                    CloseButton.UnhighlightButton();
                    FullScreenButton.HighlightButton();
                }
                else if (FullScreenButton.IsHighlighted && direction == Entities.Direction.Right)
                {
                    FullScreenButton.UnhighlightButton();
                    CloseButton.HighlightButton();
                }
                else if (!FullScreenButton.IsHighlighted && !CloseButton.IsHighlighted)
                {
                    var settingSlider = sliders.Where(slider => slider.IsHighlighted).FirstOrDefault();
                    settingSlider?.HandleDirection(direction);
                }
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
                    }
                    else if (direction == Entities.Direction.Down)
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
            else if (FullScreenButton.IsHighlighted)
            {
                FullScreenButton.CallClick();
            }
        }
    }
}
