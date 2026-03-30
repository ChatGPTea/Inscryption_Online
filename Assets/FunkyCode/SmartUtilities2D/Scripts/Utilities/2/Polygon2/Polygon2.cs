using System;
using System.Collections.Generic;
using FunkyCode.LightingSettings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FunkyCode.Utilities
{
    public class Polygon2
    {
        public Vector2[] points;

        public Polygon2(List<Vector2> pointList)
        {
            points = pointList.ToArray();
        }

        public Polygon2(int size)
        {
            points = new Vector2[size];
        }

        public Polygon2(Polygon2D polygon)
        {
            points = new Vector2[polygon.pointsList.Count];

            for (var id = 0; id < polygon.pointsList.Count; id++) points[id] = polygon.pointsList[id].ToVector2();
        }

        public Polygon2(Vector2[] array)
        {
            points = array;
        }

        public Rect GetRect()
        {
            var rect = new Rect();

            float minX = 100000;
            float minY = 100000;
            float maxX = -100000;
            float maxY = -100000;

            var pointsCount = points.Length;

            for (var i = 0; i < pointsCount; i++)
            {
                var id = points[i];

                minX = id.x < minX ? id.x : minX;
                maxX = id.x > maxX ? id.x : maxX;

                minY = id.y < minY ? id.y : minY;
                maxY = id.y > maxY ? id.y : maxY;
            }

            rect.x = minX;
            rect.y = minY;
            rect.width = maxX - minX;
            rect.height = maxY - minY;

            return rect;
        }

        public Polygon2 Copy()
        {
            var array = new Vector2[points.Length];

            Array.Copy(points, array, points.Length);

            return new Polygon2(array);
        }

        public Polygon2 ToWorldSpace(Transform transform)
        {
            var newPolygon = Copy();

            newPolygon.ToWorldSpaceSelfUNIVERSAL(transform);

            return newPolygon;
        }

        public void ToScaleSelf(Vector2 scale, Vector2? center = null)
        {
            if (center == null) center = Vector2.zero;

            float dist, rot;
            Vector2 point;

            for (var id = 0; id < points.Length; id++)
            {
                point = points[id];

                dist = Vector2.Distance(point, center.Value);
                rot = point.Atan2(center.Value); //??

                point.x = center.Value.x + Mathf.Cos(rot) * dist * scale.x;
                point.y = center.Value.y + Mathf.Sin(rot) * dist * scale.y;

                points[id] = point;
            }
        }

        public void ToRotationSelf(float rotation, Vector2? center = null)
        {
            if (center == null) center = Vector2.zero;

            float dist, rot;
            Vector2 point;

            for (var id = 0; id < points.Length; id++)
            {
                point = points[id];

                dist = Vector2.Distance(point, center.Value);
                rot = point.Atan2(center.Value) + rotation; //??

                point.x = center.Value.x + Mathf.Cos(rot) * dist;
                point.y = center.Value.y + Mathf.Sin(rot) * dist;

                points[id] = point;
            }
        }

        public void ToOffsetSelf(Vector2 pos)
        {
            for (var id = 0; id < points.Length; id++) points[id] += pos;
        }

        public bool IsClockwise()
        {
            if (points.Length < 1) return true;

            double sum = 0;

            var A = points[points.Length - 1];
            Vector2 B;

            for (var i = 0; i < points.Length; i++)
            {
                B = points[i];

                sum += (B.x - A.x) * (B.y + A.y);

                A = B;
            }

            return sum > 0;
        }

        public void Normalize()
        {
            if (!IsClockwise()) Array.Reverse(points);
        }


        ///// Constructors - Polygon Creating //////

        public static Polygon2 CreateRect(Vector2 size)
        {
            size = size / 2;

            var polygon = new Polygon2(4);

            polygon.points[0] = new Vector2(-size.x, -size.y);
            polygon.points[1] = new Vector2(size.x, -size.y);
            polygon.points[2] = new Vector2(size.x, size.y);
            polygon.points[3] = new Vector2(-size.x, size.y);

            polygon.Normalize();

            return polygon;
        }

        public static Polygon2 CreateIsometric(Vector2 size)
        {
            size = size / 2;

            var polygon = new Polygon2(4);

            polygon.points[0] = new Vector2(-size.x, size.y);
            polygon.points[1] = new Vector2(0, 0);
            polygon.points[2] = new Vector2(size.x, size.y);
            polygon.points[3] = new Vector2(0, size.y * 2);

            polygon.Normalize();

            return polygon;
        }

        public static Polygon2 CreateHexagon(Vector2 size)
        {
            size = size / 2;

            var polygon = new Polygon2(6);


            polygon.points[0] = new Vector2(-size.x, size.y);
            polygon.points[1] = new Vector2(-size.x, -size.y);
            polygon.points[2] = new Vector2(0, -size.y * 2);
            polygon.points[3] = new Vector2(size.x, -size.y);
            polygon.points[4] = new Vector2(size.x, size.y);
            polygon.points[5] = new Vector2(0, size.y * 2);

            polygon.Normalize();

            return polygon;
        }

        public Mesh CreateMesh(GameObject gameObject, Vector2 UVScale, Vector2 UVOffset,
            PolygonTriangulator2.Triangulation triangulation = PolygonTriangulator2.Triangulation.Advanced)
        {
            if (gameObject.GetComponent<MeshRenderer>() == null) gameObject.AddComponent<MeshRenderer>();

            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null) filter = gameObject.AddComponent<MeshFilter>();

            filter.sharedMesh = PolygonTriangulator2.Triangulate(this, UVScale, UVOffset, triangulation);
            if (filter.sharedMesh == null) Object.Destroy(gameObject);

            return filter.sharedMesh;
        }

        public Mesh CreateMesh(Vector2 UVScale, Vector2 UVOffset,
            PolygonTriangulator2.Triangulation triangulation = PolygonTriangulator2.Triangulation.Advanced)
        {
            return PolygonTriangulator2.Triangulate(this, UVScale, UVOffset, triangulation);
        }


        public void ToWorldSpaceSelfUNIVERSAL(Transform transform)
        {
            switch (Lighting2D.CoreAxis)
            {
                case CoreAxis.XY:

                    ToWorldSpaceSelfXY(transform);

                    break;

                case CoreAxis.XYFLIPPED:

                    ToWorldSpaceSelfFlipped(transform);

                    break;

                case CoreAxis.XZFLIPPED:

                    ToWorldSpaceSelfXZFlipped(transform);

                    break;

                case CoreAxis.XZ:

                    ToWorldSpaceSelfXZ(transform);

                    break;
            }
        }

        //public void ToWorldSpaceSelf(Transform transform) {
        //	for(int id = 0; id < points.Length; id++) {
        //		points[id] = transform.TransformPoint (points[id]);
        //	}
        //}

        public void ToWorldSpaceSelfXY(Transform transform)
        {
            var count = points.Length;

            Vector2 scale = transform.lossyScale;
            Vector2 position = transform.position;
            var rotation = transform.eulerAngles.z;

            var angle = rotation * Mathf.Deg2Rad;
            var cos = Mathf.Cos(angle);
            var sin = Mathf.Sin(angle);

            for (var id = 0; id < count; id++)
            {
                var a = points[id];

                var x = a.x * scale.x;
                var y = a.y * scale.y;

                a.x = x * cos - y * sin + position.x;
                a.y = x * sin + y * cos + position.y;

                points[id] = a;
            }
        }

        public void ToWorldSpaceSelfFlipped(Transform transform)
        {
            var count = points.Length;

            for (var id = 0; id < count; id++) points[id] = points[id].TransformToWorldXYFlipped(transform);
        }

        public void ToWorldSpaceSelfXZ(Transform transform)
        {
            var count = points.Length;

            for (var id = 0; id < count; id++) points[id] = points[id].TransformToWorldXZ(transform);
        }

        public void ToWorldSpaceSelfXZFlipped(Transform transform)
        {
            var count = points.Length;

            for (var id = 0; id < count; id++) points[id] = points[id].TransformToWorldXZFlipped(transform);
        }

        public bool PointInPoly(Vector2 point)
        {
            return Math2D.PointInPoly(point, this);
        }
    }
}