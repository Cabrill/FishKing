using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class LinearScaleContainerRuntime
    {
        public float StartX
        {
            get
            {
                return LeftEndRectangle.WorldUnitX;
            }
        }

        public float EndX
        {
            get
            {
                return RightEndRectangle.WorldUnitX;
            }
        }

        partial void CustomInitialize()
        {
            
        }
    }
}
