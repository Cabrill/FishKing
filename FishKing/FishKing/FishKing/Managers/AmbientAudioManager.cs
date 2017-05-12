using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing
{
    public static class AmbientAudioManager
    {
        public static FlatRedBall.TileGraphics.LayeredTileMap CurrentTileMap;
        public static Entities.Character CharacterInstance;

        private static SoundEffectInstance waterFallAmbientSound;
        private static SoundEffectInstance riverAmbientSound;
        private static SoundEffectInstance oceanAmbientSound;
        private static SoundEffectInstance deepOceanAmbientSound;
        private static SoundEffectInstance lakeAmbientSound;
        private static SoundEffectInstance caveAmbientSound;
        private static SoundEffectInstance forestAmbientSound;

        private static AudioListener listener = new AudioListener();
        private static AudioEmitter waterfallEmitter;
        private static AudioEmitter oceanEmitter;
        private static AudioEmitter riverEmitter;
        private static AudioEmitter forestEmitter;

        public static void UpdateAmbientSoundSources()
        {
            listener.Position = Vector3.Zero;
            Point3D charPosition = new Point3D(CharacterInstance.Position);

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "WaterfallLines") != null)
            {
                if (waterFallAmbientSound == null)
                {
                    waterFallAmbientSound = GlobalContent.WaterfallAmbient.CreateInstance();
                    waterFallAmbientSound.IsLooped = true;
                    waterfallEmitter = new AudioEmitter();
                }

                waterfallEmitter.Position = FindClosestPolygonPointOnLayer("WaterfallLines", charPosition);
                waterFallAmbientSound.Volume = Math.Max(0, 1 - (waterfallEmitter.Position.Length() / 10));

                waterFallAmbientSound.Apply3D(listener, waterfallEmitter);

                if (waterFallAmbientSound.State != SoundState.Playing)
                {
                    waterFallAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "OceanLines") != null)
            {
                if (oceanAmbientSound == null)
                {
                    oceanAmbientSound = GlobalContent.OceanAmbient.CreateInstance();
                    oceanAmbientSound.IsLooped = true;
                    oceanEmitter = new AudioEmitter();
                }
                var allCloseOceanPoints = FindClosestPointsOnAllPolygonsOnLayer("OceanLines", charPosition);
                var sumDist = (float)allCloseOceanPoints.Sum(p => p.Length());
                var minDist = (float)allCloseOceanPoints.Min(p => p.Length());
                var xSum = (float)allCloseOceanPoints.Sum(p =>
                p.X * Math.Min(1, Math.Pow((1 / p.Length()), 3))
                ) / allCloseOceanPoints.Count();
                var ySum = (float)allCloseOceanPoints.Sum(p =>
                p.Y * Math.Min(1, Math.Pow((1 / p.Length()), 3))
                ) / allCloseOceanPoints.Count();


                //var combineDistance = 5;
                float positionX = xSum;
                float positionY = ySum;
                float positionZ = CharacterInstance.Position.Z;

                oceanEmitter.Position = new Vector3(positionX, positionY, positionZ);
                oceanAmbientSound.Volume = MathHelper.Clamp(1f - (2.5f * minDist / sumDist), 0, 1);

                oceanAmbientSound.Apply3D(listener, oceanEmitter);

                if (oceanAmbientSound.State != SoundState.Playing)
                {
                    oceanAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "RiverLines") != null)
            {
                if (riverAmbientSound == null)
                {
                    riverAmbientSound = GlobalContent.RiverAmbient.CreateInstance();
                    riverAmbientSound.IsLooped = true;
                    riverEmitter = new AudioEmitter();
                }
                var allCloseRiverPoints = FindClosestPointsOnAllPolygonsOnLayer("RiverLines", charPosition);
                var sumDist = (float)allCloseRiverPoints.Sum(p => p.Length());
                var minDist = (float)allCloseRiverPoints.Min(p => p.Length());
                var xSum = (float)allCloseRiverPoints.Sum(p =>
                p.X * Math.Min(1, Math.Pow((1 / p.Length()), 3))
                ) / allCloseRiverPoints.Count();
                var ySum = (float)allCloseRiverPoints.Sum(p =>
                p.Y * Math.Min(1, Math.Pow((1 / p.Length()), 3))
                ) / allCloseRiverPoints.Count();


                //var combineDistance = 5;
                float positionX = xSum;
                float positionY = ySum;
                float positionZ = CharacterInstance.Position.Z;

                riverEmitter.Position = new Vector3(positionX, positionY, positionZ);
                riverAmbientSound.Volume = MathHelper.Clamp(1f - (2.5f * minDist / sumDist), 0, 1);

                riverAmbientSound.Apply3D(listener, riverEmitter);

                if (riverAmbientSound.State != SoundState.Playing)
                {
                    riverAmbientSound.Play();
                }
            }


            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "CaveAmbient") != null)
            {
                if (caveAmbientSound == null)
                {
                    caveAmbientSound = GlobalContent.CaveAmbient.CreateInstance();
                    caveAmbientSound.IsLooped = true;
                }

                if (caveAmbientSound.State != SoundState.Playing)
                {
                    caveAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "DeepOceanAmbient") != null)
            {
                if (deepOceanAmbientSound == null)
                {
                    deepOceanAmbientSound = GlobalContent.DeepOceanAmbient.CreateInstance();
                    deepOceanAmbientSound.IsLooped = true;
                }

                if (deepOceanAmbientSound.State != SoundState.Playing)
                {
                    deepOceanAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "ForestLines") != null)
            {
                if (forestAmbientSound == null)
                {
                    forestAmbientSound = GlobalContent.ForestAmbient.CreateInstance();
                    forestAmbientSound.IsLooped = true;
                    forestEmitter = new AudioEmitter();
                }

                forestEmitter.Position = FindClosestPolygonPointOnLayer("ForestLines", charPosition);
                forestAmbientSound.Volume = Math.Max(0, 1 - (forestEmitter.Position.Length() / 10));

                forestAmbientSound.Apply3D(listener, waterfallEmitter);

                if (forestAmbientSound.State != SoundState.Playing)
                {
                    forestAmbientSound.Play();
                }
            }
        }

        private static Vector3 FindClosestPolygonPointOnLayer(string layerName, Point3D fromPoint)
        {
            var lines = CurrentTileMap.ShapeCollections.Find(s => s.Name == layerName).Polygons;

            var closestLine = lines.Aggregate((x, y) => x.VectorFrom(fromPoint).Length() < y.VectorFrom(fromPoint).Length() ? x : y);
            var closestPoint = closestLine.VectorFrom(fromPoint);

            return new Vector3((float)(closestPoint.X / CurrentTileMap.WidthPerTile), (float)(closestPoint.Y / CurrentTileMap.HeightPerTile), listener.Position.Z);
        }

        private static IEnumerable<Point3D> FindClosestPointsOnAllPolygonsOnLayer(string layerName, Point3D fromPoint)
        {
            var allClosestPoints = new List<Point3D>();
            for (int i = 0; i < 10; i++)
            {
                var layerNumName = layerName + (i == 0 ? "" : (i + 1).ToString());
                var lines = CurrentTileMap.ShapeCollections.Find(s => s.Name == layerNumName)?.Polygons;

                if (lines == null)
                {
                    break;
                }
                else
                {
                    allClosestPoints.AddRange(lines.Select(line => line.VectorFrom(fromPoint)));
                }
            }
            return allClosestPoints;
        }

        public static void RemoveAmbientAudioSources()
        {
            if (waterFallAmbientSound != null)
            {
                waterFallAmbientSound.Stop();
            }
            if (deepOceanAmbientSound != null)
            {
                deepOceanAmbientSound.Stop();
            }
            if (riverAmbientSound != null)
            {
                riverAmbientSound.Stop();
            }
            if (oceanAmbientSound != null)
            {
                oceanAmbientSound.Stop();
            }

            if (caveAmbientSound != null)
            {
                caveAmbientSound.Stop();
            }
        }

    }
}
