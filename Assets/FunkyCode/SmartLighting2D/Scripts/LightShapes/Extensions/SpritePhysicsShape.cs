using System.Collections.Generic;
using FunkyCode.SpriteExtension;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.LightShape
{
    public class SpritePhysicsShape : Base
    {
        public PhysicsShape physicsShape;
        private Sprite sprite;

        private SpriteRenderer spriteRenderer;

        public override void ResetLocal()
        {
            LocalPolygonsCache = LocalPolygons;

            base.ResetLocal();

            physicsShape = null;

            sprite = null;
        }

        public Sprite GetOriginalSprite()
        {
            if (sprite == null)
            {
                GetSpriteRenderer();

                if (spriteRenderer != null) sprite = spriteRenderer.sprite;
            }

            return sprite;
        }

        public SpriteRenderer GetSpriteRenderer()
        {
            if (spriteRenderer != null) return spriteRenderer;

            if (transform == null) return spriteRenderer;

            if (spriteRenderer == null) spriteRenderer = transform.GetComponent<SpriteRenderer>();

            return spriteRenderer;
        }

        public PhysicsShape GetPhysicsShape()
        {
            if (physicsShape == null)
            {
                var sprite = GetOriginalSprite();

                if (sprite != null) physicsShape = PhysicsShapeManager.RequestCustomShape(sprite);
            }

            return physicsShape;
        }

        public override List<MeshObject> GetMeshes()
        {
            if (Meshes == null)
            {
                var polygons = GetPolygonsLocal();

                if (polygons.Count > 0)
                {
                    Meshes = new List<MeshObject>();

                    foreach (var poly in polygons)
                    {
                        if (poly.points.Length < 3) continue;

                        var mesh = PolygonTriangulator2.Triangulate(poly, Vector2.zero, Vector2.zero,
                            PolygonTriangulator2.Triangulation.Advanced);

                        if (mesh)
                        {
                            var meshObject = MeshObject.Get(mesh);

                            if (meshObject != null) Meshes.Add(meshObject);
                        }
                    }
                }
            }

            return Meshes;
        }

        public override List<Polygon2> GetPolygonsWorld()
        {
            if (WorldPolygons != null) return WorldPolygons;

            var scale = new Vector2();

            var localPolygons = GetPolygonsLocal();

            if (WorldCache != null)
            {
                WorldPolygons = WorldCache;

                Polygon2 poly;
                Polygon2 wPoly;

                var spriteRenderer = GetSpriteRenderer();

                for (var i = 0; i < localPolygons.Count; i++)
                {
                    poly = localPolygons[i];
                    wPoly = WorldPolygons[i];

                    var invert = false;

                    for (var p = 0; p < poly.points.Length; p++) wPoly.points[p] = poly.points[p];

                    if (spriteRenderer != null)
                        if (spriteRenderer.flipX || spriteRenderer.flipY)
                        {
                            scale.x = 1;
                            scale.y = 1;

                            if (spriteRenderer.flipX)
                            {
                                scale.x = -1;

                                invert = !invert;
                            }

                            if (spriteRenderer.flipY)
                            {
                                scale.y = -1;

                                invert = !invert;
                            }

                            wPoly.ToScaleSelf(scale);
                        }

                    wPoly.ToWorldSpaceSelfUNIVERSAL(transform);

                    if (invert) wPoly.Normalize();
                }
            }
            else
            {
                Polygon2 polygon;

                WorldPolygons = new List<Polygon2>();

                var spriteRenderer = GetSpriteRenderer();

                foreach (var poly in localPolygons)
                {
                    polygon = poly.Copy();

                    var invert = false;

                    if (spriteRenderer != null)
                        if (spriteRenderer.flipX || spriteRenderer.flipY)
                        {
                            scale.x = 1;
                            scale.y = 1;

                            if (spriteRenderer.flipX)
                            {
                                scale.x = -1;

                                invert = !invert;
                            }

                            if (spriteRenderer.flipY)
                            {
                                scale.y = -1;

                                invert = !invert;
                            }

                            polygon.ToScaleSelf(scale);
                        }

                    polygon.ToWorldSpaceSelfUNIVERSAL(transform);

                    if (invert) polygon.Normalize();

                    WorldPolygons.Add(polygon);
                }

                WorldCache = WorldPolygons;
            }

            return WorldPolygons;
        }

        public override List<Polygon2> GetPolygonsLocal()
        {
            if (LocalPolygons != null) return LocalPolygons;

            physicsShape = GetPhysicsShape();

            if (physicsShape != null) LocalPolygons = physicsShape.Get();

            if (LocalPolygons == null) LocalPolygons = new List<Polygon2>();

            return LocalPolygons;
        }
    }
}