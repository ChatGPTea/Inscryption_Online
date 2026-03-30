using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    public static class GizmosHelper
    {
        public static void DrawRect(Vector3 position, Rect rect)
        {
            var p0 = LightingPosition.GetPosition3DWorld(new Vector2(rect.x, rect.y), position);
            var p1 = LightingPosition.GetPosition3DWorld(new Vector2(rect.x + rect.width, rect.y), position);
            var p2 = LightingPosition.GetPosition3DWorld(new Vector2(rect.x + rect.width, rect.y + rect.height),
                position);
            var p3 = LightingPosition.GetPosition3DWorld(new Vector2(rect.x, rect.y + rect.height), position);

            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);
        }

        public static Vector3 IsoConvert(Vector3 vector)
        {
            var org = Vector3.zero;
            org.z = vector.z;

            org.x = vector.x - vector.y;
            org.y = vector.x / 2 + vector.y / 2;

            return org;
        }

        public static void DrawIsoRect(Vector3 position, Rect rect)
        {
            var p0 = LightingPosition.GetPosition3DWorld(new Vector2(rect.x, rect.y), position);
            var p1 = LightingPosition.GetPosition3DWorld(new Vector2(rect.x + rect.width, rect.y), position);
            var p2 = LightingPosition.GetPosition3DWorld(new Vector2(rect.x + rect.width, rect.y + rect.height),
                position);
            var p3 = LightingPosition.GetPosition3DWorld(new Vector2(rect.x, rect.y + rect.height), position);

            p0 = IsoConvert(p0);
            p1 = IsoConvert(p1);
            p2 = IsoConvert(p2);
            p3 = IsoConvert(p3);


            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);
        }

        public static void DrawCircle(Vector3 position, float rotation, float angle, float size)
        {
            var center = position;
            var step = 10;

            var start = -(int)(angle / 2);
            var end = (int)(angle / 2);

            for (var i = start; i < end; i += step)
            {
                var rot = i + 90 + rotation;

                var rotA = rot * Mathf.Deg2Rad;
                var rotB = (rot + step) * Mathf.Deg2Rad;

                var pointA = LightingPosition.GetPosition3D(new Vector2(Mathf.Cos(rotA) * size, Mathf.Sin(rotA) * size),
                    center);
                var pointB = LightingPosition.GetPosition3D(new Vector2(Mathf.Cos(rotB) * size, Mathf.Sin(rotB) * size),
                    center);

                Gizmos.DrawLine(pointA, pointB);

                if (angle < 360 && angle > 0)
                {
                    if (i == start) Gizmos.DrawLine(pointA, center);

                    if (i + step > end) Gizmos.DrawLine(pointB, center);
                }
            }
        }

        public static void DrawPolygons(List<Polygon2> polygons, Vector3 position)
        {
            if (polygons == null) return;

            foreach (var polygon in polygons) DrawPolygon(polygon, position);
        }

        public static void DrawPolygon(Polygon2 polygon, Vector3 position)
        {
            if (polygon == null) return;

            var a = Vector3.zero;
            var b = Vector3.zero;

            for (var i = 0; i < polygon.points.Length; i++)
            {
                var p0 = polygon.points[i];
                var p1 = polygon.points[(i + 1) % polygon.points.Length];

                a = LightingPosition.GetPosition3DWorld(p0, position);
                b = LightingPosition.GetPosition3DWorld(p1, position);

                Gizmos.DrawLine(a, b);
            }
        }
    }
}