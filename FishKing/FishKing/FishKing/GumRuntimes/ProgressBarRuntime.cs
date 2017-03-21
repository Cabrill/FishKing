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
        private int maxTextureWidth = 457;

        public int Progress
        {
            get { return (int)(this.BarFillWidth); }
            set { UpdateProgressBar(value);  }
        }

        partial void CustomInitialize()
        {
            ResetProgress();
        }

        public void ResetProgress()
        {
            this.BarFillWidth = 1;
            this.BarFillTextureWidth = 1;
        }

        public void UpdateProgressBar(int percent)
        {
            this.BarFillWidth = percent;
            this.BarFillTextureWidth =  (int)((percent *.01) * maxTextureWidth);
        }

        public void PositionProgressBarOver(Vector3 position)
        {
            IPositionedSizedObject barAsPositionedObject = this as IPositionedSizedObject;
            var trueHeight = barAsPositionedObject.Height;
            var trueWidth = barAsPositionedObject.Width;
            var screenWidth = FlatRedBallServices.GraphicsOptions.ResolutionWidth;

            var proposedY = -(position.Y + trueHeight * 3);
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
