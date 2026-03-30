using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.LightShape
{
    public class CompositeCollider2DShape : Base
    {
        private CompositeCollider2D compositeCollider;

        public CompositeCollider2D GetCompositeCollider()
        {
            if (compositeCollider == null) compositeCollider = transform.GetComponent<CompositeCollider2D>();

            return compositeCollider;
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

            if (WorldCache != null)
            {
                WorldPolygons = WorldCache;

                Polygon2 poly;
                Vector2 point;
                var list = GetPolygonsLocal();

                for (var i = 0; i < list.Count; i++)
                {
                    poly = list[i];
                    for (var p = 0; p < poly.points.Length; p++)
                    {
                        point = poly.points[p];

                        WorldPolygons[i].points[p].x = point.x;
                        WorldPolygons[i].points[p].y = point.y;
                    }

                    WorldPolygons[i].ToWorldSpaceSelfUNIVERSAL(transform);
                }
            }
            else
            {
                WorldPolygons = new List<Polygon2>();

                foreach (var poly in GetPolygonsLocal()) WorldPolygons.Add(poly.ToWorldSpace(transform));

                WorldCache = WorldPolygons;
            }

            return WorldPolygons;
        }

        public override List<Polygon2> GetPolygonsLocal()
        {
            if (LocalPolygons != null) return LocalPolygons;

            var compositeCollider = GetCompositeCollider();

            LocalPolygons = Polygon2Collider2D.CreateFromCompositeCollider(compositeCollider);

            if (LocalPolygons.Count <= 0)
                Debug.LogWarning("SmartLighting2D: LightingCollider2D object is missing CompositeCollider2D Component",
                    transform.gameObject);

            return LocalPolygons;
        }
    }
}