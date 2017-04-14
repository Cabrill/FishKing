using FlatRedBall;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.Extensions
{
    public static class SpriteExtensions
    {
        public static Vector3 GetKeyPixelPosition(this Sprite sprite)
        {
            var spriteName = sprite.Name;
            var chainIndex = sprite.CurrentChainIndex;
            var frameIndex = sprite.CurrentFrameIndex;

            var coordResults = GlobalContent.keypixels.Where(kp => kp.SpriteName == spriteName && kp.ChainIndex == chainIndex && kp.FrameIndex == frameIndex);

            if (coordResults == null || coordResults.Count() != 1)
            {
                throw new IndexOutOfRangeException(string.Format("No key pixels found for sprite {0}, chain {1}, and frame index {2}", spriteName, sprite.AnimationChains[chainIndex].Name, frameIndex));
            }

            var scale = sprite.TextureScale;
            var coord = coordResults.FirstOrDefault();

            var horizontalFlip = (sprite.FlipHorizontal ? -1 : 1);
            var verticaleFlip = (sprite.FlipVertical ? -1 : 1);

            var keyPixelPosition = new Vector3(
                sprite.Position.X + (coord.OffsetX * scale * horizontalFlip), 
                sprite.Position.Y - (coord.OffsetY * scale * verticaleFlip), 
                sprite.Z
                );

            return keyPixelPosition;
        }

        public static Vector3 GetKeyPixelPosition(this Sprite sprite, string FishName)
        {
            var fishType = GlobalContent.Fish_Types[FishName];

            var scale = sprite.TextureScale;

            var horizontalFlip = (sprite.FlipHorizontal ? -1 : 1);
            var verticaleFlip = (sprite.FlipVertical ? -1 : 1);

            var keyPixelPosition = new Vector3(
                sprite.Position.X + (fishType.OffsetX * scale * horizontalFlip),
                sprite.Position.Y - (fishType.OffsetY * scale * verticaleFlip),
                sprite.Z
                );

            return keyPixelPosition;
        }
    }
}
