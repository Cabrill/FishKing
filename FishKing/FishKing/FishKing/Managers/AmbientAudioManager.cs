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
        private static bool volumeHasChanged = false;
        private static float _volume = 1f;

        public static float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                volumeHasChanged = true;
                UpdateAmbientSoundSources();
            }
        }

        public static FlatRedBall.TileGraphics.LayeredTileMap CurrentTileMap;
        public static Entities.Character CharacterInstance;

        private static SoundEffectInstance _waterFallAmbientSound;
        private static SoundEffectInstance _riverAmbientSound;
        private static SoundEffectInstance _oceanAmbientSound;
        private static SoundEffectInstance _deepOceanAmbientSound;
        private static SoundEffectInstance _lakeAmbientSound;
        private static SoundEffectInstance _caveAmbientSound;
        private static SoundEffectInstance _forestAmbientSound;

        private static readonly AudioListener Listener = new AudioListener();
        private static AudioEmitter _waterfallEmitter;
        private static AudioEmitter _oceanEmitter;
        private static AudioEmitter _riverEmitter;
        private static AudioEmitter _forestEmitter;
        private static AudioEmitter _lakeEmitter;

        public static void UpdateAmbientSoundSources()
        {
            //EARLY OUT
            if (CurrentTileMap == null) return;

            Listener.Position = Vector3.Zero;
            var charPosition = new Point3D(CharacterInstance.Position);

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "WaterfallLines") != null)
            {
                if (_waterFallAmbientSound == null)
                {
                    _waterFallAmbientSound = GlobalContent.WaterfallAmbient.CreateInstance();
                    _waterFallAmbientSound.IsLooped = true;
                }
                if (_waterfallEmitter == null)
                {
                    _waterfallEmitter = new AudioEmitter();
                }

                _waterfallEmitter.Position = FindClosestPolygonPointOnLayer("WaterfallLines", charPosition);
                _waterFallAmbientSound.Volume = Volume * Math.Max(0, 1 - (_waterfallEmitter.Position.Length() / 10));

                _waterFallAmbientSound.Apply3D(Listener, _waterfallEmitter);

                if (_waterFallAmbientSound.State != SoundState.Playing)
                {
                    _waterFallAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "OceanLines") != null)
            {
                if (_oceanAmbientSound == null)
                {
                    _oceanAmbientSound = GlobalContent.OceanAmbient.CreateInstance();
                    _oceanAmbientSound.IsLooped = true;
                }
                if (_oceanEmitter == null)
                {
                    _oceanEmitter = new AudioEmitter();
                }
                var allCloseOceanPoints = FindClosestPointsOnAllPolygonsOnLayer("OceanLines", charPosition);
                var closeOceanPoints = allCloseOceanPoints as Point3D[] ?? allCloseOceanPoints.ToArray();
                var sumDist = (float) closeOceanPoints.Sum(p => p.Length());
                var minDist = (float) closeOceanPoints.Min(p => p.Length());
                var xSum = (float) closeOceanPoints.Sum(p =>
                               p.X * Math.Min(1, Math.Pow((1 / p.Length()), 3))
                           ) / closeOceanPoints.Count();
                var ySum = (float) closeOceanPoints.Sum(p =>
                               p.Y * Math.Min(1, Math.Pow((1 / p.Length()), 3))
                           ) / closeOceanPoints.Count();

                var positionX = xSum;
                var positionY = ySum;
                var positionZ = CharacterInstance.Position.Z;

                _oceanEmitter.Position = new Vector3(positionX, positionY, positionZ);
                _oceanAmbientSound.Volume = Volume * MathHelper.Clamp(1f - (2.5f * minDist / sumDist), 0, 1);

                _oceanAmbientSound.Apply3D(Listener, _oceanEmitter);

                if (_oceanAmbientSound.State != SoundState.Playing)
                {
                    _oceanAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "RiverLines") != null)
            {
                if (_riverAmbientSound == null)
                {
                    _riverAmbientSound = GlobalContent.RiverAmbient.CreateInstance();
                    _riverAmbientSound.IsLooped = true;
                }
                if (_riverEmitter == null)
                {
                    _riverEmitter = new AudioEmitter();
                }
                var allCloseRiverPoints = FindClosestPointsOnAllPolygonsOnLayer("RiverLines", charPosition);
                var sumDist = (float) allCloseRiverPoints.Sum(p => p.Length());
                var minDist = (float) allCloseRiverPoints.Min(p => p.Length());
                var xSum = (float) allCloseRiverPoints.Sum(p =>
                               p.X * Math.Min(1, Math.Pow((1 / p.Length()), 3))
                           ) / allCloseRiverPoints.Count();
                var ySum = (float) allCloseRiverPoints.Sum(p =>
                               p.Y * Math.Min(1, Math.Pow((1 / p.Length()), 3))
                           ) / allCloseRiverPoints.Count();

                var positionX = xSum;
                var positionY = ySum;
                var positionZ = CharacterInstance.Position.Z;

                _riverEmitter.Position = new Vector3(positionX, positionY, positionZ);
                _riverAmbientSound.Volume = Volume * MathHelper.Clamp(1f - (2.5f * minDist / sumDist), 0, 1);

                _riverAmbientSound.Apply3D(Listener, _riverEmitter);

                if (_riverAmbientSound.State != SoundState.Playing)
                {
                    _riverAmbientSound.Play();
                }
            }


            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "CaveAmbient") != null)
            {
                if (_caveAmbientSound == null)
                {
                    _caveAmbientSound = GlobalContent.CaveAmbient.CreateInstance();
                    _caveAmbientSound.IsLooped = true;
                }

                if (volumeHasChanged)
                {
                    _caveAmbientSound.Volume = Volume;
                }

                if (_caveAmbientSound.State != SoundState.Playing)
                {
                    _caveAmbientSound.Volume = Volume;
                    _caveAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "DeepOceanAmbient") != null)
            {
                if (_deepOceanAmbientSound == null)
                {
                    _deepOceanAmbientSound = GlobalContent.DeepOceanAmbient.CreateInstance();
                    _deepOceanAmbientSound.IsLooped = true;
                }

                if (volumeHasChanged)
                {
                    _deepOceanAmbientSound.Volume = Volume;
                }

                if (_deepOceanAmbientSound.State != SoundState.Playing)
                {
                    _deepOceanAmbientSound.Volume = Volume;
                    _deepOceanAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "ForestLines") != null)
            {
                if (_forestAmbientSound == null)
                {
                    _forestAmbientSound = GlobalContent.ForestAmbient.CreateInstance();
                    _forestAmbientSound.IsLooped = true;
                }
                if (_forestEmitter == null)
                {
                    _forestEmitter = new AudioEmitter();
                }

                _forestEmitter.Position = FindClosestPolygonPointOnLayer("ForestLines", charPosition);
                _forestAmbientSound.Volume = Volume * Math.Max(0, 1 - (_forestEmitter.Position.Length() / 10));

                _forestAmbientSound.Apply3D(Listener, _forestEmitter);

                if (_forestAmbientSound.State != SoundState.Playing)
                {
                    _forestAmbientSound.Play();
                }
            }

            if (CurrentTileMap.ShapeCollections.Find(s => s.Name == "LakeLines") != null)
            {
                if (_lakeAmbientSound == null)
                {
                    _lakeAmbientSound = GlobalContent.LakeAmbient.CreateInstance();
                    _lakeAmbientSound.IsLooped = true;
                }
                if (_lakeEmitter == null)
                {
                    _lakeEmitter = new AudioEmitter();
                }

                _lakeEmitter.Position = FindClosestPolygonPointOnLayer("LakeLines", charPosition);
                _lakeAmbientSound.Volume = Volume * Math.Max(0, 1 - (_lakeEmitter.Position.Length() / 10));

                _lakeAmbientSound.Apply3D(Listener, _lakeEmitter);

                if (_lakeAmbientSound.State != SoundState.Playing)
                {
                    _lakeAmbientSound.Play();
                }
            }

            volumeHasChanged = false;
        }

        private static Vector3 FindClosestPolygonPointOnLayer(string layerName, Point3D fromPoint)
        {
            var lines = CurrentTileMap.ShapeCollections.Find(s => s.Name == layerName).Polygons;

            var closestLine =
                lines.Aggregate((x, y) => x.VectorFrom(fromPoint).Length() < y.VectorFrom(fromPoint).Length() ? x : y);
            var closestPoint = closestLine.VectorFrom(fromPoint);

            return new Vector3((float) (closestPoint.X / CurrentTileMap.WidthPerTile),
                (float) (closestPoint.Y / CurrentTileMap.HeightPerTile), Listener.Position.Z);
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
            CurrentTileMap = null;
            CharacterInstance = null;

            if (_waterFallAmbientSound != null)
            {
                _waterFallAmbientSound.Stop();
                _waterfallEmitter = null;
            }
            _deepOceanAmbientSound?.Stop();
            if (_riverAmbientSound != null)
            {
                _riverAmbientSound.Stop();
                _riverEmitter = null;
            }
            if (_oceanAmbientSound != null)
            {
                _oceanAmbientSound.Stop();
                _oceanEmitter = null;
            }
            _caveAmbientSound?.Stop();
            if (_forestAmbientSound != null)
            {
                _forestAmbientSound.Stop();
                _forestEmitter = null;
            }
            if (_lakeAmbientSound != null)
            {
                _lakeAmbientSound.Stop();
                _lakeEmitter = null;
            }
        }
    }
}