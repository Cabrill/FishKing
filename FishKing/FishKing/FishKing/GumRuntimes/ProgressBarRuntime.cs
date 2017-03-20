using Microsoft.Xna.Framework;
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
            this.X = position.X - 150;
            this.Y = position.Y - 90;
        }
    }
}
