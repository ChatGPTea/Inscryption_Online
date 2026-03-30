using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.LightShape
{
    public class Collider3DShape : Base
    {
        public bool edgeCollider2D = false;

        public override List<MeshObject> GetMeshes()
        {
            if (Meshes == null)
            {
                var polygons = GetPolygonsLocal();

                if (polygons == null) return null;

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

        public override List<Polygon2> GetPolygonsLocal()
        {
            if (LocalPolygons != null) return LocalPolygons;

            if (transform == null) return LocalPolygons;

            LocalPolygons = Polygon2ListCollider3D.CreateFromGameObject(transform.gameObject);

            return LocalPolygons;
        }

        public override List<Polygon2> GetPolygonsWorld()
        {
            if (WorldPolygons != null) return WorldPolygons;

            WorldPolygons = new List<Polygon2>();

            if (GetPolygonsLocal() != null)
                foreach (var poly in GetPolygonsLocal())
                    WorldPolygons.Add(poly.ToWorldSpace(transform));

            return WorldPolygons;
        }
    }
}