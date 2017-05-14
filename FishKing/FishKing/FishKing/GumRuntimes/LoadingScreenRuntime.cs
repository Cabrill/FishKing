using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.GumRuntimes
{
    partial class LoadingScreenRuntime
    {
        partial void CustomInitialize()
        {
            SetFishGraphicOrigin();
        }

        private void SetFishGraphicOrigin()
        {
            this.FishSprite.XOrigin = this.FishSprite.XOrigin;
            this.FishSprite.YOrigin = this.FishSprite.YOrigin;
        }
    }
}
