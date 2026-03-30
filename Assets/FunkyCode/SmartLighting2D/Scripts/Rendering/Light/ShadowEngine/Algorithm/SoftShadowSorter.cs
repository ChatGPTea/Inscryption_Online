using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    public static class SoftShadowSorter
    {
        public static Vector2 center;

        public static Vector2 minPoint;
        public static Vector2 maxPoint;

        public static void Set(Polygon2 polygon, Light2D light)
        {
            var lightPosition = -light.transform2D.position;

            center.x = 0;
            center.y = 0;

            var pointsCount = polygon.points.Length;

            // polygon center could be optimized

            for (var i = 0; i < pointsCount; i++)
            {
                var p = polygon.points[i];

                center.x += p.x + lightPosition.x;
                center.y += p.y + lightPosition.y;
            }

            center.x /= pointsCount;
            center.y /= pointsCount;

            var centerDirection = Mathf.Atan2(center.x, center.y) * Mathf.Rad2Deg;

            centerDirection = (centerDirection + 720) % 360 + 180;

            float min = 10000;
            float max = -10000;

            for (var id = 0; id < polygon.points.Length; id++)
            {
                var p = polygon.points[id];

                var dir = Mathf.Atan2(p.x + lightPosition.x, p.y + lightPosition.y) * Mathf.Rad2Deg;

                dir = (dir + 720 - centerDirection) % 360;

                var direction = dir;

                if (direction < min)
                {
                    min = direction;
                    minPoint = p;
                }

                if (direction > max)
                {
                    max = direction;
                    maxPoint = p;
                }
            }
        }
    }
}