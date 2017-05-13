using FishKing.Enums;
using FlatRedBall.Math;
using FlatRedBall.Math.Geometry;
using FlatRedBall.TileCollisions;
using FlatRedBall.TileGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FishKing.Enums.WaterTypes;

namespace FlatRedBall.TileCollisions
{
    public partial class TileShapeCollection
    {
        public void AddWaterAtWorld(float x, float y, string givenName)
        {
            // Make sure there isn't already collision here
            if (GetTileAt(x, y) == null)
            {
                // x and y
                // represent
                // the center
                // of the tile
                // where the user
                // may want to add 
                // collision.  Let's
                // subtract half width/
                // height so we can use the
                // bottom/left
                float roundedX = MathFunctions.RoundFloat(x - GridSize / 2.0f, GridSize, mLeftSeedX);
                float roundedY = MathFunctions.RoundFloat(y - GridSize / 2.0f, GridSize, mBottomSeedY);

                AxisAlignedRectangle newAar = new AxisAlignedRectangle();
                newAar.Width = GridSize;
                newAar.Height = GridSize;
                newAar.Left = roundedX;
                newAar.Bottom = roundedY;
                newAar.Name = givenName;

                if (this.mVisible)
                {
                    newAar.Visible = true;
                }

                float keyValue = GetKeyValue(roundedX, roundedY);

                int index = mShapes.AxisAlignedRectangles.GetFirstAfter(keyValue, mSortAxis,
                    0, mShapes.AxisAlignedRectangles.Count);

                mShapes.AxisAlignedRectangles.Insert(index, newAar);

                UpdateRepositionDirectionsFor(newAar);
            }
        }
    }

    public static class TileShapeCollectionLayeredTileMapHelperExtensions
    {
        public static List<WaterType> GetAllWaterTypes(this LayeredTileMap layeredTileMap)
        {
            var waterTypes = new List<WaterType>();
            var properties = layeredTileMap.TileProperties;

            foreach (var kvp in properties)
            {
                string name = kvp.Key;
                var namedValues = kvp.Value;
                WaterType water;

                foreach (var nameValue in namedValues)
                {
                    water = WaterTypes.WaterTypeNameToEnum(nameValue.Name);

                    if (water != WaterType.None && !waterTypes.Contains(water))
                    {
                        waterTypes.Add(water);
                    }
                }
                
            }
            return waterTypes;
        }

        public static void AddCollisionFromLayer(this TileShapeCollection tileShapeCollection,
        LayeredTileMap layeredTileMap, List<string> layerNames)
        {
            var properties = layeredTileMap.TileProperties;

            foreach (var kvp in properties)
            {
                string name = kvp.Key;
                var namedValues = kvp.Value;

                float dimension = float.NaN;
                float dimensionHalf = 0;
                foreach (var layer in layeredTileMap.MapLayers)
                {
                    var layerToCollide = layerNames.Contains(layer.Name);
                    var dictionary = layer.NamedTileOrderedIndexes;

                    if (dictionary.ContainsKey(name) && layerToCollide)
                    {
                        var indexList = dictionary[name];

                        foreach (var index in indexList)
                        {
                            float left;
                            float bottom;
                            layer.GetBottomLeftWorldCoordinateForOrderedTile(index, out left, out bottom);

                            if (float.IsNaN(dimension))
                            {
                                dimension = layer.Vertices[(index * 4) + 1].Position.X - left;
                                dimensionHalf = dimension / 2.0f;
                                tileShapeCollection.GridSize = dimension;
                            }

                            tileShapeCollection.AddCollisionAtWorld(left + dimensionHalf,
                                bottom + dimensionHalf);
                        }
                    }
                }
            }
        }

        public static void RemoveCollisionsFromLayer(this TileShapeCollection tileShapeCollection,
                    LayeredTileMap layeredTileMap, List<string> layerNames)
        {
            var properties = layeredTileMap.TileProperties;

            foreach (var kvp in properties)
            {
                string name = kvp.Key;
                var namedValues = kvp.Value;

                float dimension = float.NaN;
                float dimensionHalf = 0;
                foreach (var layer in layeredTileMap.MapLayers)
                {
                    var layerToRemoveCollisions = layerNames.Contains(layer.Name);
                    var dictionary = layer.NamedTileOrderedIndexes;

                    if (dictionary.ContainsKey(name) && layerToRemoveCollisions)
                    {
                        var indexList = dictionary[name];

                        foreach (var index in indexList)
                        {
                            float left;
                            float bottom;
                            layer.GetBottomLeftWorldCoordinateForOrderedTile(index, out left, out bottom);

                            if (float.IsNaN(dimension))
                            {
                                dimension = layer.Vertices[(index * 4) + 1].Position.X - left;
                                dimensionHalf = dimension / 2.0f;
                                tileShapeCollection.GridSize = dimension;
                            }

                            tileShapeCollection.RemoveCollisionAtWorld(left + dimensionHalf,
                                bottom + dimensionHalf);
                        }
                    }
                }
            }
        }

        public static void RemoveCollisionFrom(this TileShapeCollection tileShapeCollection,
            LayeredTileMap layeredTileMap, string nameToUse)
        {
            RemoveCollisionFrom(tileShapeCollection, layeredTileMap,
                new List<string> { nameToUse });
        }

        public static void RemoveCollisionFrom(this TileShapeCollection tileShapeCollection,
            LayeredTileMap layeredTileMap, IEnumerable<string> namesToUse)
        {
            Func<List<TMXGlueLib.DataTypes.NamedValue>, bool> predicate = (list) =>
            {
                var nameProperty = list.FirstOrDefault(item => item.Name.ToLower() == "name");

                return namesToUse.Contains(nameProperty.Value);
            };

            RemoveCollisionFrom(tileShapeCollection, layeredTileMap, predicate);

        }

        public static void RemoveCollisionFrom(this TileShapeCollection tileShapeCollection, LayeredTileMap layeredTileMap,
            Func<List<TMXGlueLib.DataTypes.NamedValue>, bool> predicate)
        {
            var properties = layeredTileMap.TileProperties;

            foreach (var kvp in properties)
            {
                string name = kvp.Key;
                var namedValues = kvp.Value;

                if (predicate(namedValues))
                {
                    float dimension = float.NaN;
                    float dimensionHalf = 0;
                    foreach (var layer in layeredTileMap.MapLayers)
                    {
                        var dictionary = layer.NamedTileOrderedIndexes;

                        if (dictionary.ContainsKey(name))
                        {
                            var indexList = dictionary[name];

                            foreach (var index in indexList)
                            {
                                float left;
                                float bottom;
                                layer.GetBottomLeftWorldCoordinateForOrderedTile(index, out left, out bottom);

                                if (float.IsNaN(dimension))
                                {
                                    dimension = layer.Vertices[(index * 4) + 1].Position.X - left;
                                    dimensionHalf = dimension / 2.0f;
                                    tileShapeCollection.GridSize = dimension;
                                }

                                tileShapeCollection.RemoveCollisionAtWorld(left + dimensionHalf,
                                    bottom + dimensionHalf);
                            }
                        }
                    }
                }
            }

        }

        public static void AddWaterFrom(this TileShapeCollection tileShapeCollection, LayeredTileMap layeredTileMap,
    Func<List<TMXGlueLib.DataTypes.NamedValue>, bool> predicate)
        {
            var properties = layeredTileMap.TileProperties;

            foreach (var kvp in properties)
            {
                string name = kvp.Key;
                var namedValues = kvp.Value;

                if (predicate(namedValues))
                {
                    float dimension = float.NaN;
                    float dimensionHalf = 0;
                    foreach (var layer in layeredTileMap.MapLayers)
                    {
                        var dictionary = layer.NamedTileOrderedIndexes;

                        if (dictionary.ContainsKey(name))
                        {
                            var indexList = dictionary[name];

                            foreach (var index in indexList)
                            {
                                float left;
                                float bottom;
                                layer.GetBottomLeftWorldCoordinateForOrderedTile(index, out left, out bottom);

                                if (float.IsNaN(dimension))
                                {
                                    dimension = layer.Vertices[(index * 4) + 1].Position.X - left;
                                    dimensionHalf = dimension / 2.0f;
                                    tileShapeCollection.GridSize = dimension;
                                }

                                tileShapeCollection.AddWaterAtWorld(left + dimensionHalf,
                                    bottom + dimensionHalf, namedValues[0].Name);
                            }
                        }
                    }
                }
            }
        }
    }
}
