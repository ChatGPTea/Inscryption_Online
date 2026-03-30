using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.Rendering.Light.Shadow
{
    public static class LegacyGPU
    {
        public static Pair2 pair = Pair2.Zero();
        public static Vector2 edgeAWorld, edgeBWorld;

        public static void Draw(List<Polygon2> polygons, float distance, float translucency)
        {
            if (polygons == null) return;

            var light = ShadowEngine.light;

            var position = ShadowEngine.lightOffset;
            position.x += ShadowEngine.objectOffset.x;
            position.y += ShadowEngine.objectOffset.y;

            var outerAngle = Mathf.Deg2Rad * light.outerAngle;
            var shadowDistance = ShadowEngine.lightSize;

            var PolygonCount = polygons.Count;

            var ignoreInside = ShadowEngine.ignoreInside;
            var dontdrawInside = ShadowEngine.dontdrawInside;

            var draw = ShadowEngine.drawOffset;

            if (distance > 0)
            {
                shadowDistance = distance;
                outerAngle = 0;
            }

            for (var i = 0; i < PolygonCount; i++)
            {
                var pointsList = polygons[i].points;
                var pointsCount = pointsList.Length;

                if (ignoreInside)
                {
                    // change to sides of vertices?
                    if (Math2D.PointInPoly(-position, polygons[i])) continue;
                }
                else if (dontdrawInside)
                {
                    if (Math2D.PointInPoly(-position, polygons[i]))
                    {
                        ShadowEngine.continueDrawing = false;
                        return;
                    }
                }

                for (var x = 0; x < pointsCount; x++)
                {
                    var next = (x + 1) % pointsCount;

                    pair.A = pointsList[x];
                    pair.B = pointsList[next];

                    edgeAWorld.x = pair.A.x + position.x;
                    edgeAWorld.y = pair.A.y + position.y;

                    edgeBWorld.x = pair.B.x + position.x;
                    edgeBWorld.y = pair.B.y + position.y;

                    GL.Vertex3(draw.x + edgeAWorld.x, draw.y + edgeAWorld.y, 0);
                    GL.Vertex3(draw.x + edgeBWorld.x, draw.y + edgeBWorld.y, 0);
                    GL.Vertex3(shadowDistance, outerAngle, translucency);
                }
            }
        }
    }
}