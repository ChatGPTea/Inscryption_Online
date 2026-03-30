using System;
using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.Rendering.Light.Shadow
{
    public static class SpriteProjection
    {
        public static Pair2 pair = Pair2.Zero();


        private static readonly Pair2D pairA = new(Vector2D.Zero(), Vector2D.Zero());
        private static readonly Pair2D pairB = new(Vector2D.Zero(), Vector2D.Zero());

        public static void Draw(List<Polygon2> polygons, float shadowDistanceMin, float shadowDistanceMax,
            float translucency)
        {
            if (polygons == null) return;

            var uv = new Vector2[4];
            var vertices = new Vector2[4];

            var mat = Lighting2D.Materials.shadow.GetSpriteProjectionMaterial();

            if (ShadowEngine.spriteProjection == null) return;

            mat.mainTexture = ShadowEngine.spriteProjection.texture;
            mat.SetPass(0);

            GL.Begin(GL.QUADS);

            GL.Color(new Color(translucency, 0, 0, 0));

            var light = ShadowEngine.light;
            var offset = ShadowEngine.lightOffset + ShadowEngine.objectOffset;
            var lightSize = ShadowEngine.lightSize;

            var pivotY = ShadowEngine.spriteProjection.pivot.y / ShadowEngine.spriteProjection.texture.height;

            var virtualSpriteRenderer = new VirtualSpriteRenderer();

            virtualSpriteRenderer.sprite = ShadowEngine.spriteProjection;
            virtualSpriteRenderer.flipX = ShadowEngine.flipX;
            //virtualSpriteRenderer.flipY = spriteRenderer.flipY;

            var spriteTransform = new SpriteTransform(virtualSpriteRenderer, Vector2.zero, Vector2.one, 0);

            var uvRect = spriteTransform.uv;
            pivotY = uvRect.y + pivotY;

            // if (shadowDistance == 0) {
            //     shadowDistance = lightSize;
            // }

            var outerAngle = light.outerAngle;


            Vector2 vA, vB, vC, vD;
            float angleA, angleB, rotA, rotB;

            var PolygonCount = polygons.Count;

            for (var i = 0; i < PolygonCount; i++)
            {
                var pointsList = polygons[i].points;
                var pointsCount = pointsList.Length;

                SoftShadowSorter.Set(polygons[i], light);


                pair.A.x = SoftShadowSorter.minPoint.x;
                pair.A.y = SoftShadowSorter.minPoint.y;

                pair.B.x = SoftShadowSorter.maxPoint.x;
                pair.B.y = SoftShadowSorter.maxPoint.y;

                var edgeALocalX = pair.A.x;
                var edgeALocalY = pair.A.y;

                var edgeBLocalX = pair.B.x;
                var edgeBLocalY = pair.B.y;

                var edgeAWorldX = edgeALocalX + offset.x;
                var edgeAWorldY = edgeALocalY + offset.y;

                var edgeBWorldX = edgeBLocalX + offset.x;
                var edgeBWorldY = edgeBLocalY + offset.y;

                var mx = (edgeAWorldX + edgeBWorldX) / 2;
                var my = (edgeAWorldY + edgeBWorldY) / 2;

                var step = Mathf.Sqrt(mx * mx + my * my) / lightSize;
                var length = Mathf.Lerp(shadowDistanceMin, shadowDistanceMax, step);

                var lightDirection = Mathf.Atan2(my, mx) * Mathf.Rad2Deg;
                var EdgeDirection = (Mathf.Atan2(edgeALocalY - edgeBLocalY, edgeALocalX - edgeBLocalX) * Mathf.Rad2Deg -
                    180 + 720) % 360;

                lightDirection -= EdgeDirection;
                lightDirection = (lightDirection + 720) % 360;

                if (lightDirection > 180)
                {
                    // continue;
                }

                for (float s = 0; s <= 1; s += 0.1f)
                {
                    var step0 = s;
                    var step1 = s + 0.1f;

                    var dir = SoftShadowSorter.minPoint.Atan2(SoftShadowSorter.maxPoint) - Mathf.PI;
                    var distance = Vector2.Distance(SoftShadowSorter.minPoint, SoftShadowSorter.maxPoint);

                    pair.A.x = SoftShadowSorter.minPoint.x + Mathf.Cos(dir) * distance * step0;
                    pair.A.y = SoftShadowSorter.minPoint.y + Mathf.Sin(dir) * distance * step0;

                    pair.B.x = SoftShadowSorter.minPoint.x + Mathf.Cos(dir) * distance * step1;
                    pair.B.y = SoftShadowSorter.minPoint.y + Mathf.Sin(dir) * distance * step1;

                    edgeALocalX = pair.A.x;
                    edgeALocalY = pair.A.y;

                    edgeBLocalX = pair.B.x;
                    edgeBLocalY = pair.B.y;

                    edgeAWorldX = edgeALocalX + offset.x;
                    edgeAWorldY = edgeALocalY + offset.y;

                    edgeBWorldX = edgeBLocalX + offset.x;
                    edgeBWorldY = edgeBLocalY + offset.y;

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
                    vA.x += Mathf.Cos(angleA) * lightSize * length;
                    vA.y += Mathf.Sin(angleA) * lightSize * length;

                    // Left Inner
                    vB.x = edgeBWorldX;
                    vB.y = edgeBWorldY;
                    vB.x += Mathf.Cos(angleB) * lightSize * length;
                    vB.y += Mathf.Sin(angleB) * lightSize * length;

                    vertices[0] = new Vector2(vD.x, vD.y);
                    vertices[1] = new Vector2(vC.x, vC.y);
                    vertices[2] = new Vector2(vA.x, vA.y);
                    vertices[3] = new Vector2(vB.x, vB.y);

                    float x0;
                    float x1;

                    var mode = edgeAWorldY < 0;

                    if (virtualSpriteRenderer.flipX) mode = !mode;

                    if (mode)
                    {
                        x0 = Mathf.Lerp(uvRect.width, uvRect.x, step0);
                        x1 = Mathf.Lerp(uvRect.width, uvRect.x, step1);
                    }
                    else
                    {
                        x0 = Mathf.Lerp(uvRect.x, uvRect.width, step0);
                        x1 = Mathf.Lerp(uvRect.x, uvRect.width, step1);
                    }


                    var y0 = Mathf.Lerp(uvRect.y, uvRect.height, pivotY);
                    var y1 = Mathf.Lerp(uvRect.y, uvRect.height, 1);

                    uv[0] = new Vector2(x1, y0);
                    uv[1] = new Vector2(x0, y0);
                    uv[2] = new Vector2(x0, y1);
                    uv[3] = new Vector2(x1, y1);

                    // Right Fin
                    GL.MultiTexCoord3(0, uv[2].x, uv[2].y, 0);
                    GL.Vertex3(vertices[2].x, vertices[2].y, 0);

                    GL.MultiTexCoord3(0, uv[3].x, uv[3].y, 0);
                    GL.Vertex3(vertices[3].x, vertices[3].y, 0);

                    GL.MultiTexCoord3(0, uv[0].x, uv[0].y, 0);
                    GL.Vertex3(vertices[0].x, vertices[0].y, 0);

                    GL.MultiTexCoord3(0, uv[1].x, uv[1].y, 0);
                    GL.Vertex3(vertices[1].x, vertices[1].y, 0);
                }
            }

            GL.End();
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