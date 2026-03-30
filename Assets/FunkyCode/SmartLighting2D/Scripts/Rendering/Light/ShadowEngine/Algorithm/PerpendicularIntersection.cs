using System;
using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.Rendering.Light.Shadow
{
    public static class PerpendicularIntersection
    {
        public static Pair2 pair = Pair2.Zero();

        private static readonly Pair2D pairA = new(Vector2D.Zero(), Vector2D.Zero());
        private static readonly Pair2D pairB = new(Vector2D.Zero(), Vector2D.Zero());

        public static void Draw(List<Polygon2> polygons, float shadowDistance)
        {
            if (polygons == null) return;

            var fill = new UVRect(0, 0, 1, 1);

            var light = ShadowEngine.light;
            var offset = ShadowEngine.lightOffset + ShadowEngine.objectOffset;
            var lightSizeSquared = ShadowEngine.lightSize * 2;

            if (shadowDistance == 0) shadowDistance = lightSizeSquared;

            var outerAngle = light.outerAngle;

            Vector2 vA, pA, vB, pB, vC, vD;
            float angleA, angleB, rotA, rotB;

            var PolygonCount = polygons.Count;

            Vector2? intersectionLeft;
            Vector2? intersectionRight;

            var intersectionLeftOffset = Vector2.zero;
            var intersectionRightOffset = Vector2.zero;

            GL.Color(Color.black);

            for (var i = 0; i < PolygonCount; i++)
            {
                var pointsList = polygons[i].points;
                var pointsCount = pointsList.Length;

                for (var x = 0; x < pointsCount; x++)
                {
                    var next = (x + 1) % pointsCount;

                    pair.A = pointsList[x];
                    pair.B = pointsList[next];

                    var edgeALocalX = pair.A.x;
                    var edgeALocalY = pair.A.y;

                    var edgeBLocalX = pair.B.x;
                    var edgeBLocalY = pair.B.y;

                    var edgeAWorldX = edgeALocalX + offset.x;
                    var edgeAWorldY = edgeALocalY + offset.y;

                    var edgeBWorldX = edgeBLocalX + offset.x;
                    var edgeBWorldY = edgeBLocalY + offset.y;

                    var lightDirection = Mathf.Atan2((edgeAWorldY + edgeBWorldY) / 2, (edgeAWorldX + edgeBWorldX) / 2) *
                                         Mathf.Rad2Deg;
                    var EdgeDirection =
                        (Mathf.Atan2(edgeALocalY - edgeBLocalY, edgeALocalX - edgeBLocalX) * Mathf.Rad2Deg - 180 +
                         720) % 360;

                    lightDirection -= EdgeDirection;

                    lightDirection = (lightDirection + 720) % 360;

                    angleA = (float)Math.Atan2(edgeAWorldY, edgeAWorldX);
                    angleB = (float)Math.Atan2(edgeBWorldY, edgeBWorldX);

                    rotA = angleA - Mathf.Deg2Rad * light.outerAngle;
                    rotB = angleB + Mathf.Deg2Rad * light.outerAngle;

                    // Right Collision
                    vC.x = edgeAWorldX;
                    vC.y = edgeAWorldY;

                    // Left Collision
                    vD.x = edgeBWorldX;
                    vD.y = edgeBWorldY;

                    // Right Inner
                    vA.x = edgeAWorldX;
                    vA.y = edgeAWorldY;
                    vA.x += Mathf.Cos(angleA) * lightSizeSquared;
                    vA.y += Mathf.Sin(angleA) * lightSizeSquared;

                    // Left Inner
                    vB.x = edgeBWorldX;
                    vB.y = edgeBWorldY;
                    vB.x += Mathf.Cos(angleB) * lightSizeSquared;
                    vB.y += Mathf.Sin(angleB) * lightSizeSquared;

                    // Outer Right
                    pA.x = edgeAWorldX;
                    pA.y = edgeAWorldY;
                    pA.x += Mathf.Cos(rotA) * lightSizeSquared;
                    pA.y += Mathf.Sin(rotA) * lightSizeSquared;

                    // Outer Left
                    pB.x = edgeBWorldX;
                    pB.y = edgeBWorldY;
                    pB.x += Mathf.Cos(rotB) * lightSizeSquared;
                    pB.y += Mathf.Sin(rotB) * lightSizeSquared;

                    // Right Intersection
                    intersectionRight = LineIntersectPolygons(vC - offset, vA - offset, polygons);

                    if (intersectionRight != null)
                    {
                        if (intersectionRight.Value.y < 0)
                        {
                            intersectionRight = null;
                        }
                        else
                        {
                            intersectionRight = intersectionRight + offset;

                            vA.x = intersectionRight.Value.x;
                            vA.y = intersectionRight.Value.y;

                            intersectionRightOffset = intersectionRight.Value;
                            intersectionRightOffset.y += shadowDistance;
                        }
                    }

                    // Left Intersection
                    intersectionLeft = LineIntersectPolygons(vD - offset, vB - offset, polygons);

                    if (intersectionLeft != null)
                    {
                        if (intersectionLeft.Value.y < 0)
                        {
                            intersectionLeft = null;
                        }
                        else
                        {
                            intersectionLeft = intersectionLeft + offset;

                            vB.x = intersectionLeft.Value.x;
                            vB.y = intersectionLeft.Value.y;

                            intersectionLeftOffset = intersectionLeft.Value;
                            intersectionLeftOffset.y += shadowDistance;
                        }
                    }

                    GL.TexCoord3(fill.x0, fill.y0, 0);

                    if (intersectionLeft != null && intersectionRight != null)
                    {
                        // Right
                        GL.Vertex3(vA.x, vA.y, 0);
                        GL.Vertex3(intersectionLeftOffset.x, intersectionLeftOffset.y, 0);
                        GL.Vertex3(intersectionRightOffset.x, intersectionRightOffset.y, 0);

                        //Left
                        GL.Vertex3(vB.x, vB.y, 0);
                        GL.Vertex3(vA.x, vA.y, 0);
                        GL.Vertex3(intersectionLeftOffset.x, intersectionLeftOffset.y, 0);
                    }
                    else
                    {
                        if (intersectionRight != null)
                        {
                            GL.Vertex3(vA.x, vA.y, 0);
                            GL.Vertex3(vB.x, vB.y, 0);
                            GL.Vertex3(intersectionRightOffset.x, intersectionRightOffset.y, 0);
                        }

                        if (intersectionLeft != null)
                        {
                            GL.Vertex3(vB.x, vB.y, 0);
                            GL.Vertex3(vA.x, vA.y, 0);
                            GL.Vertex3(intersectionLeftOffset.x, intersectionLeftOffset.y, 0);
                        }
                    }

                    // Right Fin
                    GL.Vertex3(vA.x, vA.y, 0);
                    GL.Vertex3(vB.x, vB.y, 0);
                    GL.Vertex3(vC.x, vC.y, 0);


                    // Left Fin
                    GL.Vertex3(vB.x, vB.y, 0);
                    GL.Vertex3(vD.x, vD.y, 0);
                    GL.Vertex3(vC.x, vC.y, 0);
                }
            }
        }

        private static Vector2? PolygonClosestIntersection(Polygon2 poly, Vector2 startPoint, Vector2 endPoint)
        {
            float distance = 1000000000;
            Vector2? result = null;

            for (var i = 0; i < poly.points.Length; i++)
            {
                var pa = poly.points[i];
                var pb = poly.points[(i + 1) % poly.points.Length];

                pairA.A.x = startPoint.x;
                pairA.A.y = startPoint.y;
                pairA.B.x = endPoint.x;
                pairA.B.y = endPoint.y;

                pairB.A.x = pa.x;
                pairB.A.y = pa.y;
                pairB.B.x = pb.x;
                pairB.B.y = pb.y;

                var intersection = Math2D.GetPointLineIntersectLine2(pairA, pairB);

                if (intersection != null)
                {
                    var d = Vector2.Distance(intersection.Value, startPoint);

                    if (result != null)
                    {
                        if (d < distance)
                        {
                            result = intersection.Value;
                            d = distance;
                        }
                    }
                    else
                    {
                        result = intersection.Value;
                        distance = d;
                    }
                }
            }

            return result;
        }


        public static Vector2? LineIntersectPolygons(Vector2 startPoint, Vector2 endPoint, List<Polygon2> originlPoly)
        {
            Vector2? result = null;
            float distance = 1000000000;

            foreach (var polygons in ShadowEngine.effectPolygons)
            {
                if (originlPoly == polygons) continue;

                foreach (var polygon in polygons)
                {
                    var intersection = PolygonClosestIntersection(polygon, startPoint, endPoint);

                    if (intersection != null)
                    {
                        var d = Vector2.Distance(intersection.Value, startPoint);
                        if (result != null)
                        {
                            if (d < distance)
                            {
                                result = intersection.Value;
                                d = distance;
                            }
                        }
                        else
                        {
                            result = intersection.Value;
                            distance = d;
                        }
                    }
                }
            }

            return result;
        }
    }
}