using System.Collections.Generic;
using FunkyCode.SpriteExtension;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    public class LightingOcclussion
    {
        public List<List<Pair2D>> outlinePoints = new();
        public List<List<DoublePair2>> polygonPairs = new();

        public List<List<Pair2D>> polygonPoints = new();
        private readonly VirtualSpriteRenderer spriteRenderer = new();

        public LightingOcclussion(LightingOcclusionShape shape, float size)
        {
            spriteRenderer.sprite = shape.spritePhysicsShape.GetOriginalSprite();

            if (shape.spritePhysicsShape.GetSpriteRenderer() != null)
            {
                spriteRenderer.flipX = shape.spritePhysicsShape.GetSpriteRenderer().flipX;
                spriteRenderer.flipY = shape.spritePhysicsShape.GetSpriteRenderer().flipY;
            }
            else
            {
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = false;
            }

            polygonPoints.Clear();
            outlinePoints.Clear();
            polygonPairs.Clear();

            List<Polygon2> polygons = null;

            switch (shape.shadowType)
            {
                case LightOcclusion2D.ShadowType.Collider:
                    polygons = new List<Polygon2>();

                    var polygons3 = shape.GetPolygonsLocal();

                    foreach (var p in polygons3)
                    {
                        var poly = p.Copy();

                        polygons.Add(poly);
                    }


                    break;

                case LightOcclusion2D.ShadowType.SpritePhysicsShape:

                    var sRenderer = shape.spritePhysicsShape.GetSpriteRenderer();

                    var customShape = PhysicsShapeManager.RequestCustomShape(sRenderer.sprite);

                    var polygons2 = customShape.Get();

                    polygons = new List<Polygon2>();

                    foreach (var p in polygons2)
                    {
                        var poly = p.Copy();
                        polygons.Add(poly);
                    }

                    break;
            }

            if (polygons == null || polygons.Count < 1) return;

            foreach (var polygon in polygons)
            {
                polygon.Normalize();

                polygonPoints.Add(Pair2D.GetList(polygon.points));
                outlinePoints.Add(Pair2D.GetList(PreparePolygon(polygon, size).points));
                polygonPairs.Add(DoublePair2.GetList(polygon.points));
            }
        }

        public static Polygon2 PreparePolygon(Polygon2 polygon, float size)
        {
            var newPolygon = new Polygon2D();

            var pair = new DoublePair2(Vector2.zero, Vector2.zero, Vector2.zero);
            var pairA = Vector2D.Zero();
            var pairC = Vector2D.Zero();
            var vecA = Vector2D.Zero();
            var vecC = Vector2D.Zero();

            for (var i = 0; i < polygon.points.Length; i++)
            {
                var pB = polygon.points[i];

                var indexB = i;

                var indexA = indexB - 1;
                if (indexA < 0) indexA += polygon.points.Length;

                var indexC = indexB + 1;
                if (indexC >= polygon.points.Length) indexC -= polygon.points.Length;

                pair.A = polygon.points[indexA];
                pair.B = pB;
                pair.C = polygon.points[indexC];

                var rotA = pair.B.Atan2(pair.A);
                var rotC = pair.B.Atan2(pair.C);

                pairA.x = pair.A.x;
                pairA.y = pair.A.y;
                pairA.Push(rotA - Mathf.PI / 2, -size);

                pairC.x = pair.C.x;
                pairC.y = pair.C.y;
                pairC.Push(rotC + Mathf.PI / 2, -size);

                vecA.x = pair.B.x;
                vecA.y = pair.B.y;
                vecA.Push(rotA - Mathf.PI / 2, -size);
                vecA.Push(rotA, 110f);

                vecC.x = pair.B.x;
                vecC.y = pair.B.y;
                vecC.Push(rotC + Mathf.PI / 2, -size);
                vecC.Push(rotC, 110f);

                var result = Math2D.GetPointLineIntersectLine(new Pair2D(pairA, vecA), new Pair2D(pairC, vecC));

                if (result != null) newPolygon.AddPoint(result);
            }

            return new Polygon2(newPolygon);
        }
    }
}