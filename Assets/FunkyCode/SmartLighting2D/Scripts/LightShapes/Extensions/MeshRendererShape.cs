using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.LightShape
{
    public class MeshRendererShape : Base
    {
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public Mesh Mesh { get; private set; }

        public override int GetSortingLayer()
        {
            return SortingLayer.GetLayerValueFromID(GetMeshRenderer().sortingLayerID);
        }

        public override int GetSortingOrder()
        {
            return GetMeshRenderer().sortingOrder;
        }

        public override void ResetLocal()
        {
            base.ResetLocal();

            Mesh = null;
            Meshes = null;
        }

        public MeshFilter GetMeshFilter()
        {
            if (!meshFilter && transform) meshFilter = transform.GetComponent<MeshFilter>();

            return meshFilter;
        }

        public MeshRenderer GetMeshRenderer()
        {
            if (!meshRenderer && transform) meshRenderer = transform.GetComponent<MeshRenderer>();

            return meshRenderer;
        }

        public override List<MeshObject> GetMeshes()
        {
            if (Meshes == null)
            {
                var meshFilter = GetMeshFilter();
                if (meshFilter)
                {
                    Mesh = meshFilter.sharedMesh;
                    if (!Mesh.isReadable)
                        Debug.LogError(
                            "SL2D: the mesh you are using is not readable (vert " + Mesh.vertices.Length + ", tris " +
                            Mesh.triangles.Length + ", uv " + Mesh.uv.Length + ")", transform.gameObject);

                    if (Mesh)
                    {
                        Meshes = new List<MeshObject>();

                        var meshObject = MeshObject.Get(Mesh);
                        if (meshObject != null) Meshes.Add(meshObject);
                    }
                }
            }

            return Meshes;
        }

        public override List<Polygon2> GetPolygonsWorld()
        {
            if (WorldPolygons != null) return WorldPolygons;

            var meshes = GetMeshes();
            if (meshes == null)
            {
                WorldPolygons = new List<Polygon2>();

                return WorldPolygons;
            }

            if (meshes.Count < 1)
            {
                WorldPolygons = new List<Polygon2>();

                Debug.LogError("SL2D: no meshes found", transform.gameObject);
                return WorldPolygons;
            }

            var meshObject = meshes[0];
            if (meshObject == null)
            {
                WorldPolygons = new List<Polygon2>();

                return WorldPolygons;
            }

            Vector3 vecA, vecB, vecC;
            Polygon2 poly;

            if (WorldCache == null)
            {
                WorldPolygons = new List<Polygon2>();

                for (var i = 0; i < meshObject.triangles.GetLength(0); i = i + 3)
                {
                    vecA = transform.TransformPoint(meshObject.vertices[meshObject.triangles[i]]);
                    vecB = transform.TransformPoint(meshObject.vertices[meshObject.triangles[i + 1]]);
                    vecC = transform.TransformPoint(meshObject.vertices[meshObject.triangles[i + 2]]);

                    poly = new Polygon2(3);
                    poly.points[0] = vecA;
                    poly.points[1] = vecB;
                    poly.points[2] = vecC;

                    WorldPolygons.Add(poly);
                }

                WorldCache = WorldPolygons;
            }
            else
            {
                var count = 0;

                WorldPolygons = WorldCache;

                for (var i = 0; i < meshObject.triangles.GetLength(0); i = i + 3)
                {
                    vecA = transform.TransformPoint(meshObject.vertices[meshObject.triangles[i]]);
                    vecB = transform.TransformPoint(meshObject.vertices[meshObject.triangles[i + 1]]);
                    vecC = transform.TransformPoint(meshObject.vertices[meshObject.triangles[i + 2]]);

                    poly = WorldPolygons[count];
                    poly.points[0].x = vecA.x;
                    poly.points[0].y = vecA.y;
                    poly.points[1].x = vecB.x;
                    poly.points[1].y = vecB.y;
                    poly.points[2].x = vecC.x;
                    poly.points[2].y = vecC.y;

                    count += 1;
                }
            }

            return WorldPolygons;
        }
    }
}