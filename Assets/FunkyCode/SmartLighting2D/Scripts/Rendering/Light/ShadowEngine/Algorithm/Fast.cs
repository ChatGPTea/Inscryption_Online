using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.Rendering.Light.Shadow
{
    public static class Fast
    {
        public static Pair2 pair = Pair2.Zero();
        public static Color segmentData;

        public static void Draw(List<Polygon2> polygons, float translucency)
        {
            if (polygons == null) return;

            var position = ShadowEngine.lightOffset;

            var PolygonCount = polygons.Count;

            for (var i = 0; i < PolygonCount; i++)
            {
                var pointsList = polygons[i].points;
                var pointsCount = pointsList.Length;

                if (ShadowEngine.ignoreInside)
                {
                    // change to sides of vertices?
                    if (Math2D.PointInPoly(-position, polygons[i])) continue;
                }
                else if (ShadowEngine.dontdrawInside)
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

                    segmentData = new Color(pair.A.x + position.x, pair.A.y + position.y, pair.B.x + position.x,
                        pair.B.y + position.y);

                    GL.Color(segmentData);

                    GL.Vertex3(0, 0, translucency);
                    GL.Vertex3(1, 0, translucency);
                    GL.Vertex3(1, 1, translucency);
                    GL.Vertex3(0, 1, translucency);
                }
            }
        }
    }
}