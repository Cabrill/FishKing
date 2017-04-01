using FlatRedBall.TileCollisions;
using FlatRedBall.TileGraphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishKing.TileCollisions
{
    public static class TileShapeCollectionHelpers
    {
        public static void AddCollisionsFromLayer(this TileShapeCollection tileShapeCollection, 
            LayeredTileMap layeredTileMap, List<string> layerNames)
        {
            var properties = layeredTileMap.Properties;

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
            var properties = layeredTileMap.Properties;

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
            var properties = layeredTileMap.Properties;

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
    }
}
