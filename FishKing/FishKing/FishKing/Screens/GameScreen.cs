#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;

using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;
using FlatRedBall.Localization;
using Microsoft.Xna.Framework;
using FishKing.Entities;
using FishKing.DataTypes;
using System.Collections.Specialized;
using System.Linq;
using FlatRedBall.TileCollisions;
using FishKing.Enums;
using static FishKing.Enums.WaterTypes;
using Microsoft.Xna.Framework.Audio;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
#endif
#endregion

namespace FishKing.Screens
{
	public partial class GameScreen
	{
        static string levelToLoad = "DesertIsland";
        static string startPointName = "FirstSpawn";
        private bool wasFishing = false;

        SoundEffectInstance waterFallAmbientSound;
        SoundEffectInstance riverAmbientSound;
        SoundEffectInstance oceanAmbientSound;
        SoundEffectInstance deepOceanAmbientSound;
        SoundEffectInstance lakeAmbientSound;
        SoundEffectInstance caveAmbientSound;

        AudioListener listener = new AudioListener();
        AudioEmitter waterfallEmitter;
        AudioEmitter oceanEmitter;
        AudioEmitter riverEmitter;

        bool CanMoveCharacter
        {
            get
            {
                return DialogDisplayInstance.Visible == false &&
                    !CharacterInstance.IsPullingInCatch &&
                    (!CharacterInstance.IsDisplayingCatch || CharacterInstance.HasFinishedDisplayingCatch) &&
                    (!FishCatchingInterfaceInstance.LineHasSnapped || FishCatchingInterfaceInstance.FishHasEscaped);
            }
        }

		void CustomInitialize()
        {
            LoadLevel(levelToLoad);

            InitializeCharacter();
        }


        private void HandleNewNpc(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach(var item in e.NewItems)
            {
                (item as Character).ReactToReposition();
            }
        }

        void LoadLevel(string levelToLoad)
        {
            InitializeLevel(levelToLoad);
            AddCollisions();
            RemoveBridgedCollisions();
            AddWaterTiles();
            RemoveBlockeddWaterTiles();
            AdjustNpcs();
            UpdateCamera();

#if DEBUG
            this.SolidCollisions.Visible =
                DebuggingVariables.ShowShapes;
#endif
            
            if (levelToLoad.Contains("Cave"))
            {
                CaveLightConeSprite.Position = Camera.Main.Position;
                CaveLightConeSprite.Visible = true;
                CaveLightConeSprite.Z = -1;
            }
            else
            {
                CaveLightConeSprite.Visible = false;
            }
        }

        private void AdjustNpcs()
        {
            foreach(var character in NpcCharacterList)
            {
                character.ReactToReposition();
            }
        }
        
        private void InitializeCharacter()
        {
            var foundStartPoint = this.StartPointList.FirstOrDefault(item => item.Name == startPointName);
            if(foundStartPoint == null)
            {
                throw new Exception($"Could not find start point with a name of {startPointName}");
            }
            this.CharacterInstance.X = foundStartPoint.X;
            this.CharacterInstance.Y = foundStartPoint.Y;

            this.CharacterInstance.ReactToReposition();

            this.CharacterInstance.MovementInput = InputManager.Keyboard.Get2DInput(
                Keys.A, Keys.D, Keys.W, Keys.S);

            this.CharacterInstance.ActionInput = InputManager.Keyboard.GetKey(Keys.Space);
            this.CharacterInstance.FishingAlignmentInput = InputManager.Mouse.GetButton(Mouse.MouseButtons.LeftButton);

            var overlayLayer = CurrentTileMap.MapLayers.FindByName("Overlay");
            if (overlayLayer != null)
            {
                this.CharacterInstance.Position.Z = overlayLayer.Z - 0.5f;
            }
            else
            {
                this.CharacterInstance.Position.Z = CurrentTileMap.MapLayers.Count - 2;
            }
            FindNearestAmbientEmitters();
        }

        void CustomActivity(bool firstTimeCalled)
		{
            UpdateCamera();
            DialogActivity();
            FishingActivity();

            bool characterMoved = false;
            if (CanMoveCharacter)
            {
                characterMoved = this.CharacterInstance.PerformMovementActivity(this.SolidCollisions, NpcCharacterList);
            }

            CharacterInstance.UpdateFishingStatus(characterMoved);
            this.CharacterInstance.SetSpriteOffset();

            CollisionActivity();
            if (CharacterInstance.IsMoving)
            {
                FindNearestAmbientEmitters();
            }

        }

        private void UpdateCamera()
        {
            
            Camera camera = Camera.Main;
            if (CurrentTileMap.Width > camera.OrthogonalWidth && CurrentTileMap.Height > camera.OrthogonalHeight)
            {
                var CameraMinX = (camera.OrthogonalWidth / 2);
                var CameraMaxX = (CurrentTileMap.Width) - (camera.OrthogonalWidth / 2);
                var CameraMinY = -CurrentTileMap.Height + camera.OrthogonalHeight / 2;
                var CameraMaxY = -(camera.OrthogonalHeight / 2);
                camera.X = this.CharacterInstance.X;
                camera.Y = this.CharacterInstance.Y;
                // assuming CameraMinX, CameraMaxX, CameraMinY, and CameraMaxY are all defined:
                camera.X = MathHelper.Clamp(camera.X, CameraMinX, CameraMaxX);
                camera.Y = MathHelper.Clamp(camera.Y, CameraMinY, CameraMaxY);
            }
            else
            {
                camera.ClearBorders();
                camera.ClearMinimumsAndMaximums();
                camera.X = CurrentTileMap.Width/2;
                camera.Y = -CurrentTileMap.Height/2;
            }
        }

        private void DialogActivity()
        {
            if (CharacterInstance.IsAttemptingAction)
            {
                if (this.DialogDisplayInstance.Visible)
                {
                    this.DialogDisplayInstance.Visible = false;
                }
                else
                {
                    Character npcTalkingTo = null;
                    foreach (var npc in this.NpcCharacterList)
                    {
                        if (CharacterInstance.ActionCollision.CollideAgainst(npc.BackwardCollision))
                        {
                            npcTalkingTo = npc;
                            break;
                        }
                    }

                    if(npcTalkingTo != null)
                    {
                        ShowDialog(npcTalkingTo.Dialog);
                    }
                }
            }
            CharacterInstance.IsInDialog = DialogDisplayInstance.Visible;
        }

        private void FishingActivity()
        {
#if DEBUG
            if (DebuggingVariables.ImmediatelyStartFishing && !CharacterInstance.IsFishing && !CharacterInstance.IsMoving)
            {
                FishCatchingInterfaceInstance.Reset();
                CharacterInstance.ResetFishingStatus();
                CharacterInstance.IsFishing = true;
                var fish = FishGenerator.CreateFish();
                CharacterInstance.FishOnTheLine = fish;
                CharacterInstance.HasInitiatedCatching = true;
            }
#endif
            if ((CharacterInstance.IsMoving && FishCatchingInterfaceInstance.HasAttachedFish) || 
                (CharacterInstance.IsAttemptingAction && CharacterInstance.HasFinishedDisplayingCatch))
            {
                FishCatchingInterfaceInstance.Reset();
                CharacterInstance.ResetFishingStatus();
            }
            
            var characterJustReleased = CharacterInstance.IsCastingRod && CharacterInstance.ActionInput.WasJustReleased;

            var characterJustStartedfishing = CharacterInstance.IsAttemptingAction && !CharacterInstance.IsInDialog &&
                !CharacterInstance.IsFishing && !CharacterInstance.IsCastingRod && !CharacterInstance.IsMoving && 
                !CharacterInstance.IsAttemptingMovement;

            var tileSize = (float)CurrentTileMap.WidthPerTile;

            if (characterJustStartedfishing)
            {
                CharacterInstance.HasInitiatedCatching = false;
                CharacterInstance.IsCastingRod = true;

                ProgressBarInstance.ResetProgress();
                ProgressBarInstance.PositionProgressBarOver(CharacterInstance.Position);

                var targetStartX = CharacterInstance.Position.X;
                var targetStartY = CharacterInstance.Position.Y;
                switch (CharacterInstance.DirectionFacing)
                {
                    case Direction.Left: targetStartX -= tileSize*2.5f; break;
                    case Direction.Right: targetStartX += tileSize * 2.5f; break;
                    case Direction.Up: targetStartY += tileSize * 2.5f; break;
                    case Direction.Down: targetStartY -= tileSize * 2.5f; break;
                }

                TargetingSpriteInstance.Position = new Vector3(targetStartX, targetStartY, CharacterInstance.Position.Z + 1);
                CharacterInstance.TargetPosition = TargetingSpriteInstance.Position;
            }
            else if (CharacterInstance.IsOnWindUp)
            {
                ProgressBarInstance.Update();

                var percentPower = (float)Decimal.Divide(ProgressBarInstance.Progress, 100);
                var maxDistance = CharacterInstance.MaxDistanceTileCast * tileSize;
                var effectiveDistance = (tileSize*2.5f) + (maxDistance * percentPower);

                var targetNewX = CharacterInstance.Position.X;
                var targetNewY = CharacterInstance.Position.Y;

                switch (CharacterInstance.DirectionFacing)
                {
                    case Direction.Left: targetNewX += -effectiveDistance; break;
                    case Direction.Right: targetNewX += effectiveDistance; break;
                    case Direction.Up: targetNewY += effectiveDistance; break;
                    case Direction.Down: targetNewY += -effectiveDistance; break;
                }
                TargetingSpriteInstance.Position = new Vector3(targetNewX, targetNewY, CharacterInstance.Position.Z + 1);
                CharacterInstance.TargetPosition = TargetingSpriteInstance.Position;
            }
            else if (CharacterInstance.JustReleasedCast)
            {
                var waterType = GetWaterType();
                if (waterType == WaterType.None)
                {
                    CharacterInstance.ResetFishingStatus();
                }
            }
            else if (CharacterInstance.IsFishing)
            {
                if (CharacterInstance.HasFishOnTheLine)
                {
                    if (CharacterInstance.HasInitiatedCatching)
                    {
                        if (!FishCatchingInterfaceInstance.HasAttachedFish)
                        {
                             FishCatchingInterfaceInstance.AttachFish(CharacterInstance.FishOnTheLine, GetWaterType());
                        }
                        if (FishCatchingInterfaceInstance.FishIsCaught)
                        {
                            FishCatchingInterfaceInstance.Stop();
                            if (!CharacterInstance.IsPullingInCatch && !CharacterInstance.IsDisplayingCatch)
                            {
                                CharacterInstance.HandleFishCaught();
                            } else if(CharacterInstance.IsDisplayingCatch && CharacterInstance.IsOnFinalFrameOfAnimationChain && !FishCatchDisplayInstance.Visible)
                            {
                                FishCatchDisplayInstance.ShowFish(CharacterInstance.FishOnTheLine);
                            }
                        }
                        else
                        {
                            if (FishCatchingInterfaceInstance.LineHasSnapped)
                            {
                                if (FishCatchingInterfaceInstance.FishHasEscaped)
                                {
                                    CharacterInstance.ResetFishingStatus();
                                    CharacterInstance.StandStill();
                                    FishCatchingInterfaceInstance.Reset();
                                    FishCatchingInterfaceInstance.Visible = false;
                                }
                            }
                            else
                            {
                                if (CharacterInstance.IsHoldingAlignButton)
                                {
                                    FishCatchingInterfaceInstance.RaiseAlignmentBar();
                                }
                                if (CharacterInstance.IsHoldingAction)
                                {
                                    FishCatchingInterfaceInstance.SpinReel();
                                }
                            }
                            FishCatchingInterfaceInstance.Update();
                        }
                    }
                }
                else
                {
                    var rnd = new Random();
                    var catchChance = 0.003;
                    var catchRoll = rnd.NextDouble();

                    if (
#if DEBUG
                    DebuggingVariables.ImmediatelyCatchFish ||
#endif
                        catchRoll <= catchChance)
                    {
                        var fish = FishGenerator.CreateFish();
                        CharacterInstance.FishOnTheLine = fish;
                    }
                }
            }

            FishCatchingInterfaceInstance.Visible = CharacterInstance.HasInitiatedCatching && !FishCatchingInterfaceInstance.FishIsCaught;
            ProgressBarInstance.Visible = CharacterInstance.IsOnWindUp;
            TargetingSpriteInstance.Visible = CharacterInstance.IsOnWindUp;
            FishCatchDisplayInstance.Visible = (CharacterInstance.IsDisplayingCatch && CharacterInstance.IsOnFinalFrameOfAnimationChain);

            if (wasFishing  && !CharacterInstance.IsFishing)
            {
                FishCatchingInterfaceInstance.Stop();
            }

            wasFishing = CharacterInstance.IsFishing;
        }

        private void ShowDialog(string stringId)
        {
            this.DialogDisplayInstance.Visible = true;
            this.DialogDisplayInstance.Text = LocalizationManager.Translate(stringId);
        }

        private void CollisionActivity()
        {
            foreach(var trigger in MapNavigationTriggerList)
            {
                if(CharacterInstance.BackwardCollision.CollideAgainst(trigger.Collision))
                {
                    levelToLoad = trigger.TargetMap;
                    startPointName = trigger.StartPointName;

                    if(string.IsNullOrEmpty(startPointName))
                    {
                        throw new Exception("Trigger has an empty StartPointName");
                    }

                    RestartScreen(reloadContent: false);
                }
            }

        }

        private void FindNearestAmbientEmitters()
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
        }

        private Vector3 FindClosestPolygonPointOnLayer(string layerName, Point3D fromPoint)
        {
            var lines = CurrentTileMap.ShapeCollections.Find(s => s.Name == layerName).Polygons;

            var closestLine = lines.Aggregate((x, y) => x.VectorFrom(fromPoint).Length() < y.VectorFrom(fromPoint).Length() ? x : y);
            var closestPoint = closestLine.VectorFrom(fromPoint);

            return new Vector3((float)(closestPoint.X / CurrentTileMap.WidthPerTile), (float)(closestPoint.Y / CurrentTileMap.HeightPerTile), listener.Position.Z);
        }

        private IEnumerable<Point3D> FindClosestPointsOnAllPolygonsOnLayer(string layerName, Point3D fromPoint)
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

        private WaterType GetWaterType()
        {
            var tile = WaterTiles.GetTileAt(TargetingSpriteInstance.X, TargetingSpriteInstance.Y);
            if (tile == null)
            {
                return WaterType.None;
            }
            else
            {
                return WaterTypeNameToEnum(tile.Name);
            }
        }

        private void AddWaterTiles()
        {
            var waterNames = new List<string>() { "IsOcean", "IsDeepOcean", "IsLake", "IsRiver", "IsPond", "IsDeepOcean", "InWaterfall" };
            WaterTiles.AddWaterFrom(CurrentTileMap, (List =>
                List.Any(item => waterNames.Contains(item.Name))
                ));
            WaterTiles.Visible = false;
        }

        private void RemoveBlockeddWaterTiles()
        {
            var nonWaterLayers = new System.Collections.Generic.List<string>() { "Bridge", "Boat" };
            WaterTiles.RemoveCollisionsFromLayer(CurrentTileMap, nonWaterLayers);
        }

        private void AddCollisions()
        {
            SolidCollisions.AddCollisionFrom(CurrentTileMap,
                 (list =>
                 list.Any(item => item.Name == "HasCollision")));

            var collisionLayers = new System.Collections.Generic.List<string>() { "Walls", "Water" };
            SolidCollisions.AddCollisionFromLayer(CurrentTileMap, collisionLayers);
        }

        private void RemoveBridgedCollisions()
        {
            var nonCollisionLayers = new System.Collections.Generic.List<string>() { "Bridge", "Boat" };
            SolidCollisions.RemoveCollisionsFromLayer(CurrentTileMap, nonCollisionLayers);
            //SolidCollisions.RemoveCollisionFrom(CurrentTileMap,
            //    (List => 
            //    List.Any(item => item.Name == "IsBridge")
            //    ));
        }

        void CustomDestroy()
        {
            if (waterFallAmbientSound != null)
            {
                waterFallAmbientSound.Stop();
                waterFallAmbientSound.Dispose();
            }
            if (oceanAmbientSound != null)
            {
                oceanAmbientSound.Stop();
                oceanAmbientSound.Dispose();
            }
            if (riverAmbientSound != null)
            {
                riverAmbientSound.Stop();
                riverAmbientSound.Dispose();
            }
            if (oceanAmbientSound != null)
            {
                oceanAmbientSound.Stop();
                oceanAmbientSound.Dispose();
            }

            if (caveAmbientSound != null)
            {
                caveAmbientSound.Stop();
                caveAmbientSound.Dispose();
            }
        }

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
