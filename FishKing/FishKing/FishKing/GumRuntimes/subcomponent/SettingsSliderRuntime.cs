using FishKing.Entities;
using FlatRedBall.Gui;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class SettingsSliderRuntime : INotifyPropertyChanged
    {

        private float _settingValue;
        public float SettingValue
        {
            get
            {
                return _settingValue;
            }
            set
            {
                _settingValue = value;
                OnPropertyChanged(nameof(SettingValue));
            }
        }

        public bool IsHighlighted
        {
            get { return CurrentHighlightState == Highlight.Highlighted; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Action<float> OnSettingChanged;

        private void UnhighlightSlider(IWindow window)
        {
            UnhighlightSlider();
        }

        private void HighlightSlider(IWindow window)
        {
            HighlightSlider();
        }

        public void HighlightSlider()
        {
            this.CurrentHighlightState = Highlight.Highlighted;
        }

        public void UnhighlightSlider()
        {
            this.CurrentHighlightState = Highlight.NotHighlighted;
        }

        partial void CustomInitialize()
        {
            PropertyChanged += SettingsSliderRuntime_PropertyChanged;
            this.Click += SettingsSliderRuntime_Click;
            this.RollOn += HighlightSlider;
            this.RollOff += UnhighlightSlider;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void SettingsSliderRuntime_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingValue))
            {
                if (OnSettingChanged != null) OnSettingChanged(SettingValue);
                UpdateArrow();
            }
        }

        private void SettingsSliderRuntime_Click(FlatRedBall.Gui.IWindow window)
        {
            var newPct = GetPercentFromClick();
            SettingValue = newPct;
        }

        public void HandleDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left: decreaseSlider(); break;
                case Direction.Right: increaseSlider(); break;
            }
        }

        private void increaseSlider()
        {
            SettingValue = (float)Math.Round(Math.Min(1f, SettingValue + 0.05f) * 20) / 20;
        }

        private void decreaseSlider()
        {
            SettingValue = (float)Math.Round(Math.Max(0f, SettingValue - 0.05f) * 20) / 20;
        }

        private void UpdateArrow()
        {
            LinearScaleContainerInstance.ArrowPercent = SettingValue * 100;
        }

        private float GetPercentFromClick()
        {
            var mouseX = FlatRedBall.Input.InputManager.Mouse.WorldXAt(1f);
            var startX = LinearScaleContainerInstance.StartX;
            var endX = LinearScaleContainerInstance.EndX;
            var pct = (mouseX - startX) / (endX - startX);
            pct = (float)Math.Round(MathHelper.Clamp(pct, 0f, 1f) * 20) / 20;
            return pct;
        }
    }
}
