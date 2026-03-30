using System;
using System.Collections.Generic;
using FunkyCode.Utilities.Polygon2DTriangulation;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public static class TriangulationWrapper
    {
        private static List<PolygonPoint> ConvertPoints(List<Vector2> points, Dictionary<uint, Vector2> codeToPosition)
        {
            var count = points.Count;
            var result = new List<PolygonPoint>(count);
            var pos = Vector2.zero;
            PolygonPoint pp = null;
            for (var i = 0; i < count; i++)
            {
                pos = points[i];
                pp = new PolygonPoint(pos.x, -pos.y);
                codeToPosition[pp.VertexCode] = pos;
                result.Add(pp);
            }

            return result;
        }

        public static Mesh CreateMesh(Polygon polygon)
        {
            if (polygon.holes.Count == 0 && (polygon.outside.Count == 3 ||
                                             (polygon.outside.Count == 4 && polygon.outside[3] == polygon.outside[0])))
                return CreateTriangle(polygon);

            var codeToPosition = new Dictionary<uint, Vector2>();

            var poly = new Polygon2DTriangulation.Polygon(ConvertPoints(polygon.outside, codeToPosition));

            foreach (var hole in polygon.holes)
                poly.AddHole(new Polygon2DTriangulation.Polygon(ConvertPoints(hole, codeToPosition)));

            try
            {
                var tcx = new DTSweepContext();
                tcx.PrepareTriangulation(poly);
                DTSweep.Triangulate(tcx);
                tcx = null;
            }
            catch (Exception e)
            {
                throw e;
            }

            var codeToIndex = new Dictionary<uint, int>();
            var vertexList = new List<Vector2>();

            foreach (var t in poly.Triangles)
            foreach (var p in t.Points)
            {
                if (codeToIndex.ContainsKey(p.VertexCode))
                    continue;

                codeToIndex[p.VertexCode] = vertexList.Count;

                Vector2 pos;
                if (!codeToPosition.TryGetValue(p.VertexCode, out pos))
                    pos = new Vector2(p.Xf, -p.Yf);

                vertexList.Add(pos);
            }

            var indices = new int[poly.Triangles.Count * 3];
            {
                var i = 0;
                foreach (var t in poly.Triangles)
                {
                    indices[i++] = codeToIndex[t.Points[0].VertexCode];
                    indices[i++] = codeToIndex[t.Points[1].VertexCode];
                    indices[i++] = codeToIndex[t.Points[2].VertexCode];
                }
            }

            Vector2[] uv = null;
            if (polygon.outsideUVs != null)
            {
                uv = new Vector2[vertexList.Count];
                for (var i = 0; i < vertexList.Count; i++)
                    uv[i] = polygon.ClosestUV(vertexList[i]);
            }

            return CreateMesh(vertexList.ToArray(), indices, uv);
        }

        public static Mesh CreateTriangle(Polygon polygon)
        {
            var vertices = new Vector2[3] { polygon.outside[0], polygon.outside[1], polygon.outside[2] };
            var indices = new int[3] { 0, 1, 2 };

            Vector2[] uv = null;
            if (polygon.outsideUVs != null)
            {
                uv = new Vector2[3];
                for (var i = 0; i < 3; i++)
                    uv[i] = polygon.ClosestUV(vertices[i]);
            }

            return CreateMesh(vertices, indices, uv);
        }

        public static Mesh CreateMesh(Vector2[] vertices, int[] indices, Vector2[] uv)
        {
            var msh = new Mesh();

            var v = new Vector3[vertices.Length];
            for (var i = 0; i < v.Length; i++)
                v[i] = vertices[i];

            msh.vertices = v;
            msh.triangles = indices;
            msh.uv = uv;
            msh.RecalculateNormals();
            msh.RecalculateBounds();
            return msh;
        }

        public class Polygon
        {
            public List<List<Vector2>> holes;
            public List<List<Vector2>> holesUVs;

            public List<Vector2> outside;
            public List<Vector2> outsideUVs;

            public Polygon()
            {
                outside = new List<Vector2>();
                holes = new List<List<Vector2>>();

                outsideUVs = new List<Vector2>();
                holesUVs = new List<List<Vector2>>();
            }

            public Vector2 ClosestUV(Vector2 pos)
            {
                var bestUV = outsideUVs[0];
                var bestDSqr = (outside[0] - pos).sqrMagnitude;

                for (var i = 1; i < outsideUVs.Count; i++)
                {
                    var dsqr = (outside[i] - pos).sqrMagnitude;
                    if (dsqr < bestDSqr)
                    {
                        bestDSqr = dsqr;
                        bestUV = outsideUVs[i];
                    }
                }

                List<Vector2> hole = null;
                List<Vector2> holeUVs = null;

                for (var h = 0; h < holes.Count; h++)
                {
                    hole = holes[h];
                    holeUVs = holesUVs[h];
                    for (var i = 0; i < holeUVs.Count; i++)
                    {
                        var dsqr = (hole[i] - pos).sqrMagnitude;
                        if (dsqr < bestDSqr)
                        {
                            bestDSqr = dsqr;
                            bestUV = holeUVs[i];
                        }
                    }
                }

                return bestUV;
            }
        }
    }
}