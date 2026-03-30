using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public static class Polygon2Helper
    {
        public static Pair2 GetAxis(Polygon2 polygon, float rotation)
        {
            var pair = new Pair2(Vector2.zero, Vector2.zero);

            if (polygon == null) return pair;

            float minX = 100000;
            float maxX = -100000;

            var pointsCount = polygon.points.Length;

            var center = polygon.GetRect().center;

            for (var i = 0; i < pointsCount; i++)
            {
                var id = polygon.points[i];

                var tid = id - center;

                var angle2 = Mathf.Atan2(tid.y, tid.x) + rotation + Mathf.PI / 2;
                var dist2 = Mathf.Sqrt(tid.x * tid.x + tid.y * tid.y);

                tid.x = Mathf.Cos(angle2) * dist2;
                tid.y = Mathf.Sin(angle2) * dist2;

                if (minX > tid.x)
                {
                    minX = tid.x;

                    pair.A = id;
                }

                if (maxX < tid.x)
                {
                    maxX = tid.x;

                    pair.B = id;
                }
            }

            return pair;
        }

        public static Rect GetRect(List<Polygon2> polygons)
        {
            var rect = new Rect();

            if (polygons == null) return rect;

            if (polygons.Count > 0)
            {
                float minX = 100000;
                float minY = 100000;
                float maxX = -100000;
                float maxY = -100000;

                for (var pid = 0; pid < polygons.Count; pid++)
                {
                    var poly = polygons[pid];

                    var pointsCount = poly.points.Length;

                    for (var i = 0; i < pointsCount; i++)
                    {
                        var id = poly.points[i];

                        minX = id.x < minX ? id.x : minX;
                        maxX = id.x > maxX ? id.x : maxX;

                        minY = id.y < minY ? id.y : minY;
                        maxY = id.y > maxY ? id.y : maxY;
                    }
                }

                rect.x = minX;
                rect.y = minY;
                rect.width = maxX - minX;
                rect.height = maxY - minY;
            }

            return rect;
        }

        public static Rect GetDayRect(List<Polygon2> polygons, float height)
        {
            var rect = new Rect();

            if (polygons == null) return rect;

            if (polygons.Count > 0)
            {
                float minX = 100000;
                float minY = 100000;
                float maxX = -100000;
                float maxY = -100000;

                var direction = -Lighting2D.DayLightingSettings.direction * Mathf.Deg2Rad;
                var shadowDistance = height * Lighting2D.DayLightingSettings.height;

                foreach (var poly in polygons)
                {
                    var pointsCount = poly.points.Length;

                    for (var i = 0; i < pointsCount; i++)
                    {
                        var id = poly.points[i];

                        minX = Mathf.Min(minX, id.x);
                        minY = Mathf.Min(minY, id.y);
                        maxX = Mathf.Max(maxX, id.x);
                        maxY = Mathf.Max(maxY, id.y);

                        var x = Mathf.Cos(direction) * shadowDistance;
                        var y = Mathf.Sin(direction) * shadowDistance;


                        minX = Mathf.Min(minX, id.x + x);
                        minY = Mathf.Min(minY, id.y + y);
                        maxX = Mathf.Max(maxX, id.x + x);
                        maxY = Mathf.Max(maxY, id.y + y);
                    }
                }

                rect.x = minX;
                rect.y = minY;
                rect.width = maxX - minX;
                rect.height = maxY - minY;
            }

            return rect;
        }

        public static Rect GetIsoRect(List<Polygon2> polygons)
        {
            var rect = new Rect();

            if (polygons == null) return rect;

            if (polygons.Count > 0)
            {
                float minX = 100000;
                float minY = 100000;
                float maxX = -100000;
                float maxY = -100000;

                foreach (var poly in polygons)
                {
                    var pointsCount = poly.points.Length;

                    for (var i = 0; i < pointsCount; i++)
                    {
                        var id = poly.points[i];

                        var x = id.y + id.x / 2;
                        var y = id.y - id.x / 2;

                        minX = Mathf.Min(minX, x);
                        minY = Mathf.Min(minY, y);
                        maxX = Mathf.Max(maxX, x);
                        maxY = Mathf.Max(maxY, y);
                    }
                }

                rect.x = minX;
                rect.y = minY;
                rect.width = maxX - minX;
                rect.height = maxY - minY;
            }

            return rect;
        }
    }
}