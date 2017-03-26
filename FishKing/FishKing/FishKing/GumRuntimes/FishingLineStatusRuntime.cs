using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class FishingLineStatusRuntime
    {
        private int maxAlpha = 255;
        private float riseRate = 2f;
        private float fallRate = 0.5f;
        public float LineStress
        {
            get; set;
        }
        public int MaxStress
        {
            get; private set;
        }

        public void Update()
        {
            if (Visible)
            {
                if (!AllCloudsLeftRightAnimation.IsPlaying())
                {
                    AllCloudsLeftRightAnimation.Play();
                }
                LineStress = Math.Max(0, LineStress - fallRate);

                FishingLineStressHeight = LineStress;
                var stressBracketAlpha = (int)(maxAlpha * (Decimal.Divide((decimal)LineStress, MaxStress)));

                LeftStressBracket.Alpha = stressBracketAlpha;
                RightStressBracket.Alpha = stressBracketAlpha;

            }
            else if (AllCloudsLeftRightAnimation.IsPlaying())
            {
                AllCloudsLeftRightAnimation.Stop();
            }
        }

        public void IncreaseStress()
        {
            LineStress += riseRate;
        }

        public void Reset()
        {
            LineStress = 0;
            MaxStress = (int)LeftStressBracket.Height;
        }
    }
}
