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
using FlatRedBall.Math;
using FlatRedBall.Glue.StateInterpolation;
using StateInterpolationPlugin;
using Microsoft.Xna.Framework;

namespace FishKing.Entities
{
	public partial class FishingLine
	{
        Tweener lineTweener;
        private const double optimalTime = 0.1;
        private double lastLineTime = 0;
        private bool isReelingIn;
        private bool lineIsSettling;
        public bool LineIsSettling
        { get { return lineIsSettling; } }

        private bool lineHasSettled;
        public bool LineHasSettled
        {
            get { return lineHasSettled; }
        }
        private Direction directionCast;
        public Direction DirectionCast
        {
            get { return directionCast; }
            set { directionCast = value; }
        }
        private Vector3 originationVector;
        public Vector3 OriginationVector
        {
            get
            {
                if (FishingLineLinesList.Count > 0)
                {
                    return FishingLineLinesList[0].AbsolutePoint1.ToVector3();
                }
                else
                {
                    return originationVector;
                }
            }
            set
            {
                if (originationVector != value)
                {
                    var priorOrigination = originationVector;
                    originationVector = value;
                    if (FishingLineLinesList.Count > 0)
                    {
                        FishingLineLinesList[0].SetFromAbsoluteEndpoints(new Point3D(originationVector), FishingLineLinesList[0].AbsolutePoint2);
                        if (!LineIsSettling)
                        {
                            //ReactToNewOrigination(priorOrigination);
                        }
                    }
                    
                }
            }
        }

        private Vector3 destinationVector;
        public Vector3 DestinationVector
        {
            get
            {
                if (FishingLineLinesList.Count > 0)
                {
                    return FishingLineLinesList.Last.AbsolutePoint2.ToVector3();
                }
                else
                {
                    return destinationVector;
                }
            }
            set
            {
                if (destinationVector != value)
                {
                    var priorDestination = destinationVector;
                    destinationVector = value;
                    if (FishingLineLinesList.Count > 0)
                    {
                        FishingLineLinesList.Last.SetFromAbsoluteEndpoints(FishingLineLinesList.Last.AbsolutePoint1, new Point3D(destinationVector));
                        if (LineHasSettled && !isReelingIn)
                        {
                            ReactToNewDestination(priorDestination);
                        }
                    }
                }
            }
        }

		private void CustomInitialize()
		{


		}

		private void CustomActivity()
		{
            if (!Visible && FishingLineLinesList.Count > 0)
            {
                Reset();
            }
		}

        public void UpdateLineFromBobberCast(Vector3 bobberPosition, bool isFinalUpdate = false)
        {
            DestinationVector = bobberPosition;
            var currentTime = FlatRedBall.TimeManager.CurrentTime;
            var lastLine = FishingLineLinesList.Last;
            if (lastLine == null || (currentTime-lastLineTime >= optimalTime))
            {
                var newLine = ShapeManager.AddLine();
                newLine.Color = new Color(LineColorRed, LineColorGreen, LineColorBlue, LineColorAlpha);
                if (lastLine == null)
                {
                    newLine.SetFromAbsoluteEndpoints(OriginationVector, DestinationVector);
                }
                else
                {
                    newLine.SetFromAbsoluteEndpoints(lastLine.AbsolutePoint2, new Point3D(DestinationVector));
                }
                FishingLineLinesList.Add(newLine);
                lastLineTime = currentTime;
            }
            else
            {
                lastLine.SetFromAbsoluteEndpoints(lastLine.AbsolutePoint1, new Point3D(bobberPosition));
            }
            if (isFinalUpdate)
            {
                lineIsSettling = true;
                if (DirectionCast == Direction.Left || DirectionCast == Direction.Right)
                {
                    SettleHorizontalFishingLine();
                }
                else
                {
                    SettleVerticalFishingLine();
                }
            }
        }

        public void ReactToTugging()
        {
            TweenerManager.Self.StopAllTweenersOwnedBy(this);
            isReelingIn = true;

            var startX = FishingLineLinesList[0].AbsolutePoint1.X;
            var endX = FishingLineLinesList.Last.AbsolutePoint2.X;
            var startY = FishingLineLinesList[0].AbsolutePoint1.Y;
            var endY = FishingLineLinesList.Last.AbsolutePoint2.Y;

            var rise = endY - startY;
            var run = endX - startX;
            var slope = rise / run;

            PositionedObjectList<Line> settledFishingLineLines = new PositionedObjectList<Line>();
            PositionedObjectList<Line> originalLines = new PositionedObjectList<Line>();
            Line clonedLine;
            Point3D point1, point2;
            double pointX;
            for (int i = 0; i < FishingLineLinesList.Count; i++)
            {
                clonedLine = FishingLineLinesList[i].Clone();
                originalLines.Add(clonedLine.Clone());
                if (i == 0)
                {
                    point1 = FishingLineLinesList[0].AbsolutePoint1;
                }
                else
                {
                    point1 = settledFishingLineLines[i - 1].AbsolutePoint2;
                }
                if (i == FishingLineLinesList.Count - 1)
                {
                    point2 = FishingLineLinesList.Last.AbsolutePoint2;
                }
                else
                {
                    pointX = FishingLineLinesList[i].AbsolutePoint2.X;
                    point2 = new Point3D(pointX, startY + (slope * (pointX- startX)));
                }
                clonedLine.SetFromAbsoluteEndpoints(point1, point2);
                settledFishingLineLines.Add(clonedLine);
            }
            TweenAllLines(originalLines, settledFishingLineLines, 0.2f, SwitchToSingleLine, InterpolationType.Linear);
        }

        private void SwitchToSingleLine()
        {
            TweenerManager.Self.StopAllTweenersOwnedBy(this);
            while (FishingLineLinesList.Count > 1)
            {
                ShapeManager.Remove(FishingLineLinesList.Last);
            }
            if (FishingLineLinesList.Last != null)
            {
                FishingLineLinesList.Last.SetFromAbsoluteEndpoints(originationVector, destinationVector);
            }
        }

        private void TweenAllLines(PositionedObjectList<Line> fromLines, PositionedObjectList<Line> toLines, float inTime, Action afterAction = null, InterpolationType ipt = InterpolationType.Back, Easing e = Easing.Out)
        {
            Line originalLine;
            Line lineToChange;
            Line settledLine;
            float mod = 0f;
            lineTweener = new Tweener(0, 1, inTime, ipt, e);
            lineTweener.Owner = this;
            lineTweener.PositionChanged += (a) => {
                if (!fromLines[0].AbsolutePoint1.Equals(FishingLineLinesList[0].AbsolutePoint1))
                {
                    fromLines[0].SetFromAbsoluteEndpoints(FishingLineLinesList[0].AbsolutePoint1, fromLines[0].AbsolutePoint2);
                    toLines[0].SetFromAbsoluteEndpoints(FishingLineLinesList[0].AbsolutePoint1, toLines[0].AbsolutePoint2);
                }
                if (!fromLines.Last.AbsolutePoint2.Equals(FishingLineLinesList.Last.AbsolutePoint2))
                {
                    fromLines.Last.SetFromAbsoluteEndpoints(fromLines.Last.AbsolutePoint1, FishingLineLinesList.Last.AbsolutePoint2);
                    toLines.Last.SetFromAbsoluteEndpoints(toLines.Last.AbsolutePoint1, FishingLineLinesList.Last.AbsolutePoint2);
                }

                mod = 1 - a;
                for (int i = 0; i < toLines.Count; i++)
                {
                    if (i < FishingLineLinesList.Count)
                    {
                        lineToChange = FishingLineLinesList[i];
                        originalLine = fromLines[i];
                        settledLine = toLines[i];

                        if (lineToChange != null)
                        {
                            lineToChange.SetFromAbsoluteEndpoints(
                            new Point3D(
                                (originalLine.AbsolutePoint1.X * mod) + (settledLine.AbsolutePoint1.X * a),
                                (originalLine.AbsolutePoint1.Y * mod) + (settledLine.AbsolutePoint1.Y * a)),
                            new Point3D(
                                (originalLine.AbsolutePoint2.X * mod) + (settledLine.AbsolutePoint2.X * a),
                                (originalLine.AbsolutePoint2.Y * mod) + (settledLine.AbsolutePoint2.Y * a))
                            );
                        }
                    }
                }
            };
            lineTweener.Ended += afterAction;
            lineTweener.Start();
            TweenerManager.Self.Add(lineTweener);
        }

        public void UpdateLineFromFishReelIn()
        {
            //TweenerManager.Self.StopAllTweenersOwnedBy(this);
        }
        
        private void SettleHorizontalFishingLine()
        {
            var totalX = FishingLineLinesList[0].AbsolutePoint1.X - FishingLineLinesList.Last.AbsolutePoint2.X;
            var totalY = FishingLineLinesList[0].AbsolutePoint1.Y - FishingLineLinesList.Last.AbsolutePoint2.Y;

            double pointX, relativeX, pointY, newY;
            Point3D point1, point2;

            PositionedObjectList<Line> settledFishingLineLines = new PositionedObjectList<Line>();
            PositionedObjectList<Line> originalLines = new PositionedObjectList<Line>();
            Line clonedLine, previousLine;
            for (int i = 0; i < FishingLineLinesList.Count; i++)
            {
                clonedLine = FishingLineLinesList[i].Clone();
                originalLines.Add(clonedLine.Clone());

                if (i == 0)
                {
                    point1 = clonedLine.AbsolutePoint1;
                }
                else
                {
                    pointX = FishingLineLinesList[0].AbsolutePoint1.X - clonedLine.AbsolutePoint1.X;
                    relativeX = (pointX / totalX) - 1;
                    pointY = CalculateCatenaryHeight(relativeX);
                    newY = pointY * totalY;

                    point1 = new Point3D(clonedLine.AbsolutePoint1.X, FishingLineLinesList.Last.AbsolutePoint2.Y + newY);

                    previousLine = settledFishingLineLines[i - 1];
                    previousLine.SetFromAbsoluteEndpoints(previousLine.AbsolutePoint1, point1);
                }

                pointX = FishingLineLinesList[0].AbsolutePoint1.X - clonedLine.AbsolutePoint1.X;
                relativeX = (pointX / totalX) - 1;
                pointY = CalculateCatenaryHeight(relativeX);
                newY = pointY * totalY;

                point2 = new Point3D(clonedLine.AbsolutePoint2.X, FishingLineLinesList.Last.AbsolutePoint2.Y + newY);

                clonedLine.SetFromAbsoluteEndpoints(point1, point2);

                settledFishingLineLines.Add(clonedLine);
            }
            
            TweenAllLines(originalLines, settledFishingLineLines, 2f, () => { lineIsSettling = false; lineHasSettled = true; });
        }

        private void SettleVerticalFishingLine()
        {
            lineHasSettled = true;
            var totalX = FishingLineLinesList[0].AbsolutePoint1.X - FishingLineLinesList.Last.AbsolutePoint2.X;
            var totalY = FishingLineLinesList[0].AbsolutePoint1.Y - FishingLineLinesList.Last.AbsolutePoint2.Y;

            double pointY, relativeY, pointX, newX;
            Point3D point1, point2;

            PositionedObjectList<Line> settledFishingLineLines = new PositionedObjectList<Line>();
            PositionedObjectList<Line> originalLines = new PositionedObjectList<Line>();
            Line clonedLine;
            for (int i = 0; i < FishingLineLinesList.Count; i++)
            {
                clonedLine = FishingLineLinesList[i].Clone();
                originalLines.Add(clonedLine.Clone());

                if (i == 0)
                {
                    point1 = clonedLine.AbsolutePoint1;
                }
                else
                {
                    pointY = FishingLineLinesList[0].AbsolutePoint1.Y - clonedLine.AbsolutePoint1.Y;
                    relativeY = (pointY / totalY) - 1;
                    pointX = CalculateCatenaryHeight(relativeY);
                    newX = pointX * totalX;

                    point1 = new Point3D(FishingLineLinesList.Last.AbsolutePoint2.X + newX, clonedLine.AbsolutePoint1.Y);
                }

                if (i > 0)
                {
                    var previousLine = settledFishingLineLines[i - 1];
                    previousLine.SetFromAbsoluteEndpoints(previousLine.AbsolutePoint1, point1);
                }

                pointY = FishingLineLinesList[0].AbsolutePoint1.Y - clonedLine.AbsolutePoint1.Y;
                relativeY = (pointY / totalY) - 1;
                pointX = CalculateCatenaryHeight(relativeY);
                newX = pointX * totalX;

                point2 = new Point3D(FishingLineLinesList.Last.AbsolutePoint2.X + newX, clonedLine.AbsolutePoint2.Y);

                clonedLine.SetFromAbsoluteEndpoints(point1, point2);

                settledFishingLineLines.Add(clonedLine);
            }

            TweenAllLines(originalLines, settledFishingLineLines, 2f, () => { lineIsSettling = false; lineHasSettled = true; });
        }

        private void ReactToNewOrigination(Vector3 priorOrigination)
        {
            TweenerManager.Self.StopAllTweenersOwnedBy(this);

            var changeVector = (OriginationVector - priorOrigination);
            Vector3 incrementVector;
            float aprior = 0f;
            float modValue;
            Point3D point1, point2;
            int iterations;

            lineTweener = new Tweener(0, 1f, 0.2f, InterpolationType.Back, Easing.Out);
            lineTweener.Owner = this;
            lineTweener.PositionChanged += (a) =>
            {
                iterations = FishingLineLinesList.Count - 1;
                modValue = (a - aprior);
                incrementVector = changeVector * modValue / (iterations);
                aprior = a;
                for (int i = 0; i < iterations; i++)
                {
                    if (i == 0)
                    {
                        point1 = FishingLineLinesList[0].AbsolutePoint1;
                    }
                    else
                    {
                        point1 = FishingLineLinesList[i-1].AbsolutePoint2;
                    }
                    if (i == iterations)
                    {
                        point2 = FishingLineLinesList.Last.AbsolutePoint2;
                    }
                    else
                    {
                        point2 = FishingLineLinesList[i].AbsolutePoint2 + (incrementVector * (iterations-i));
                    }

                    FishingLineLinesList[i].SetFromAbsoluteEndpoints(point1, point2);
                }
            };
            lineTweener.Start();
            TweenerManager.Self.Add(lineTweener);
        }

        private void ReactToNewDestination(Vector3 priorDestination)
        {
            TweenerManager.Self.StopAllTweenersOwnedBy(this);

            var changeVector = (DestinationVector - priorDestination);
            Vector3 incrementVector;
            float aprior = 0f;
            float modValue;
            Point3D point1, point2;
            int iterations;
            
            lineTweener = new Tweener(0, 1f, 0.2f, InterpolationType.Back, Easing.Out);
            lineTweener.Owner = this;
            lineTweener.PositionChanged += (a) =>
            {
                iterations = FishingLineLinesList.Count-1;
                modValue = (a - aprior);
                incrementVector = changeVector * modValue / (iterations);
                aprior = a;
                for (int i = iterations; i > 0; i--)
                {
                    point1 = FishingLineLinesList[i].AbsolutePoint1 + (incrementVector * i);
                    if (i == iterations)
                    {
                        point2 = FishingLineLinesList.Last.AbsolutePoint2;
                    }
                    else
                    {
                        point2 = FishingLineLinesList[i + 1].AbsolutePoint1;
                    }
                    
                    FishingLineLinesList[i].SetFromAbsoluteEndpoints(point1, point2);
                }
                if (FishingLineLinesList.Count >= 2)
                {
                    FishingLineLinesList[0].SetFromAbsoluteEndpoints(FishingLineLinesList[0].AbsolutePoint1, FishingLineLinesList[1].AbsolutePoint1);
                }
            };
            lineTweener.Start();
            TweenerManager.Self.Add(lineTweener);
        }

        public void StartCasting(Direction directionFacing, Vector3 castOrigination)
        {
            DirectionCast = directionFacing;
            originationVector = castOrigination;
            destinationVector = castOrigination;
        }

        public void Reset()
        {
            TweenerManager.Self.StopAllTweenersOwnedBy(this);
            while (FishingLineLinesList.Count > 0)
            {
                ShapeManager.Remove(FishingLineLinesList.Last);
            }
            DirectionCast = Direction.None;
            lineHasSettled = false;
            lineIsSettling = false;
            isReelingIn = false;
            lastLineTime = 0;
        }

        private double CalculateCatenaryHeight(double x, double a = 0.5)
        {
            var maxY = 1.38109;
            var minY = 0.5;
            return ((a * Math.Cosh(x / a)) - minY) / maxY;
        }

        private void CustomDestroy()
        {
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {
        }
    }
}
