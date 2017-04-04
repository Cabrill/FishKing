using FlatRedBall;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class ProgressBarRuntime
    {
        private int maxProgress = 100;
        private int minProgress = 1;
        private int maxTextureWidth = 457;
        private int directionModifier = 1;
        private int progressIncrement = 2;

        public int Progress
        {
            get { return (int)(this.BarFillWidth); }
            set { UpdateProgressBar(value);  }
        }

        partial void CustomInitialize()
        {
            ResetProgress();
        }

        public void Update()
        {
            var newProgress = Progress + (progressIncrement * directionModifier);

            if (newProgress >= maxProgress || newProgress <= minProgress)
            {
                directionModifier *= -1;
                newProgress = Math.Min(maxProgress, newProgress);
                newProgress = Math.Max(minProgress, newProgress);
            }
            Progress = newProgress;
        }

        public void ResetProgress()
        {
            Progress = 1;
            directionModifier = 1;
        }

        public void UpdateProgressBar(int percent)
        {
            this.BarFillWidth = percent;
            this.BarFillTextureWidth =  5 + (int)((percent *.01) * (maxTextureWidth-5));
        }

        public void PositionProgressBarOver(Vector3 objectPosition)
        {
            var camera = FlatRedBall.Camera.Main;
            var position = new Vector3(objectPosition.X - camera.X, objectPosition.Y - camera.Y, objectPosition.Z + 1);
            position.X += camera.OrthogonalWidth / 2;
            position.Y -= camera.OrthogonalHeight / 2;
            IPositionedSizedObject barAsPositionedObject = this as IPositionedSizedObject;
            var trueHeight = barAsPositionedObject.Height;
            var trueWidth = barAsPositionedObject.Width;
            var screenWidth = camera.OrthogonalWidth;

            var proposedY = -(position.Y + (trueHeight * 3));
            if (proposedY < 0)
            {
                proposedY = -(position.Y - trueHeight);
            }

            var proposedX = position.X - (trueWidth / 2);
            if (proposedX < 0)
            {
                proposedX = 0;
            }
            if (proposedX + trueWidth > screenWidth)
            {
                proposedX = screenWidth - trueWidth;
            }
            
            barAsPositionedObject.X = proposedX;
            barAsPositionedObject.Y = proposedY;
        }
    }
}
