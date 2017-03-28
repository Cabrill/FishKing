using FishKing.Entities;
using RenderingLibrary;
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
        private float fallRate = 0.1f;

        public float LineStress
        {
            get; set;
        } = 0;
        public int MaxStress
        {
            get; private set;
        } = 1;

        public bool LineHasSnapped { get { return LineStress >= MaxStress; } }

        public void Update()
        {
            if (Visible)
            {
                if (LineStress >= MaxStress)
                {
                     HandleLineSnapped();
                }
                else
                {
                    if (!AllCloudsLeftRightAnimation.IsPlaying())
                    {
                        AllCloudsLeftRightAnimation.Play();
                    }
                    LineStress = Math.Max(0, LineStress - fallRate);

                    FishingLineStressHeight = LineStress;
                    var stressBracketAlpha = Math.Min(255, (int)(maxAlpha * (Decimal.Divide((decimal)LineStress, MaxStress))));
                    var stressBracketRed = Math.Min(255, (int)(185 + (100 * (LineStress / MaxStress))));

                    LeftStressBracket.Alpha = RightStressBracket.Alpha = stressBracketAlpha;
                    LeftStressBracket.Red = RightStressBracket.Red = stressBracketRed;
                    LeftStressBracket.Green = RightStressBracket.Green = maxAlpha - stressBracketRed;
                    LeftStressBracket.Blue = RightStressBracket.Blue = maxAlpha - stressBracketRed;
                }
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

        private void HandleLineSnapped()
        {

        }
    }
}
