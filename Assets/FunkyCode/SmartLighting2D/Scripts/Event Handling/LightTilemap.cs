using System;
using System.Collections.Generic;
using FunkyCode.LightingSettings;
using FunkyCode.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace FunkyCode.EventHandling
{
    public class LightTilemap : Base
    {
        public static Vector2[] removePointsColliding = new Vector2[100];
        public static int removePointsCollidingCount;

        public static LightCollision2D[] removeCollisions = new LightCollision2D[100];
        public static int removeCollisionsCount;

        public static List<LightCollision2D> RemoveHiddenPoints(List<LightCollision2D> collisions, Light2D light,
            EventPreset eventPreset)
        {
            var lightSizeSquared = Mathf.Sqrt(light.size * light.size + light.size * light.size);
            float rotLeft, rotRight;

            var testPolygon = GetPolygon();
            Vector2 lightPosition = -light.transform.position;
            int next;

            for (var iid = 0; iid < eventPreset.layerSetting.list.Length; iid++)
            {
                var layerId = eventPreset.layerSetting.list[iid].layerID;

                var tilemapColliderList = LightTilemapCollider2D.GetShadowList(layerId);
                foreach (var id in tilemapColliderList)
                {
                    var tilemapCollider = id.GetCurrentTilemap();
                    var count = tilemapCollider.chunkManager.GetTiles(light.transform2D.WorldRect);

                    for (var t = 0; t < count; t++)
                    {
                        var tile = tilemapCollider.chunkManager.display[t];

                        if (tile.occluded) continue;

                        switch (id.shadowTileType)
                        {
                            case ShadowTileType.AllTiles:
                                break;

                            case ShadowTileType.ColliderOnly:

                                if (tile.colliderType == Tile.ColliderType.None)
                                    continue;

                                break;
                        }

                        var polygons = tile.GetWorldPolygons(tilemapCollider);
                        if (polygons.Count < 1)
                            continue;

                        var tilePosition = tile.GetWorldPosition(tilemapCollider) + lightPosition;
                        if (tile.NotInRange(tilePosition, light.size))
                            continue;

                        removePointsCollidingCount = 0;
                        removeCollisionsCount = 0;

                        for (var i = 0; i < polygons.Count; i++)
                        {
                            var pointsList = polygons[i].points;
                            var pointsCount = pointsList.Length;

                            for (var x = 0; x < pointsCount; x++)
                            {
                                next = (x + 1) % pointsCount;

                                var left = pointsList[x];
                                var right = pointsList[next];

                                edgeLeft.x = left.x + lightPosition.x;
                                edgeLeft.y = left.y + lightPosition.y;

                                edgeRight.x = right.x + lightPosition.x;
                                edgeRight.y = right.y + lightPosition.y;

                                rotLeft = (float)Math.Atan2(edgeLeft.y, edgeLeft.x);
                                rotRight = (float)Math.Atan2(edgeRight.y, edgeRight.x);

                                projectionLeft.x = edgeLeft.x + (float)Math.Cos(rotLeft) * lightSizeSquared;
                                projectionLeft.y = edgeLeft.y + (float)Math.Sin(rotLeft) * lightSizeSquared;

                                projectionRight.x = edgeRight.x + (float)Math.Cos(rotRight) * lightSizeSquared;
                                projectionRight.y = edgeRight.y + (float)Math.Sin(rotRight) * lightSizeSquared;

                                testPolygon.points[0] = projectionLeft;
                                testPolygon.points[1] = projectionRight;
                                testPolygon.points[2] = edgeRight;
                                testPolygon.points[3] = edgeLeft;

                                float collisionCount = collisions.Count;
                                for (var c = 0; c < collisionCount; c++)
                                {
                                    var col = collisions[c];
                                    if (col.collider == id) continue;

                                    // Check if event handling objects are inside shadow
                                    // Add it to remove list
                                    var pCount = col.points.Count;
                                    for (var p = 0; p < pCount; p++)
                                    {
                                        var point = col.points[p];

                                        if (Math2D.PointInPoly(point, testPolygon))
                                        {
                                            removePointsColliding[removePointsCollidingCount] = point;
                                            removePointsCollidingCount++;
                                        }
                                    }

                                    // Remove Event Handling points with remove list
                                    for (var p = 0; p < removePointsCollidingCount; p++)
                                        col.points.Remove(removePointsColliding[p]);

                                    removePointsCollidingCount = 0;

                                    // If there is no points left
                                    // collision object should be removed
                                    if (col.points.Count < 1)
                                    {
                                        removeCollisions[removeCollisionsCount] = col;
                                        removeCollisionsCount++;
                                    }
                                }

                                for (var p = 0; p < removeCollisionsCount; p++) collisions.Remove(removeCollisions[p]);

                                removeCollisionsCount = 0;
                            }
                        }
                    }
                }
            }

            return collisions;
        }
    }
}