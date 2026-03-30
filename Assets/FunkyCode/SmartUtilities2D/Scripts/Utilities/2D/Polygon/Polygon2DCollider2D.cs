using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public class Polygon2DCollider2D
    {
        public static int defaultCircleVerticesCount = 25;

        public static List<Polygon2D> CreateFromCompositeCollider(CompositeCollider2D compositeCollider)
        {
            var list = new List<Polygon2D>();

            if (compositeCollider != null)
            {
                var pathCount = compositeCollider.pathCount;

                for (var i = 0; i < pathCount; i++)
                {
                    var pointCount = compositeCollider.GetPathPointCount(i);

                    var pointsInPath = new Vector2[pointCount];

                    compositeCollider.GetPath(i, pointsInPath);

                    var polygon = new Polygon2D();

                    for (var j = 0; j < pointsInPath.Length; j++) polygon.AddPoint(pointsInPath[j]);

                    polygon.Normalize();

                    list.Add(polygon);
                }
            }

            return list;
        }

        public static Polygon2D CreateFromEdgeCollider(EdgeCollider2D edgeCollider)
        {
            var newPolygon = new Polygon2D();

            if (edgeCollider != null)
                foreach (var p in edgeCollider.points)
                    newPolygon.AddPoint(p + edgeCollider.offset);

            //newPolygon.AddPoint (edgeCollider.points[0] + edgeCollider.offset);
            newPolygon.Normalize();

            return newPolygon;
        }

        public static Polygon2D CreateFromCircleCollider(CircleCollider2D circleCollider, int pointsCount = -1)
        {
            if (pointsCount < 1) pointsCount = defaultCircleVerticesCount;

            var newPolygon = new Polygon2D();

            var size = circleCollider.radius;
            float i = 0;

            while (i < 360)
            {
                newPolygon.AddPoint(
                    new Vector2(Mathf.Cos(i * Mathf.Deg2Rad) * size, Mathf.Sin(i * Mathf.Deg2Rad) * size) +
                    circleCollider.offset);

                i += 360f / pointsCount;
            }

            return newPolygon;
        }

        public static Polygon2D CreateFromBoxCollider(BoxCollider2D boxCollider)
        {
            var newPolygon = new Polygon2D();

            var size = new Vector2(boxCollider.size.x / 2, boxCollider.size.y / 2);

            newPolygon.AddPoint(new Vector2(-size.x, -size.y) + boxCollider.offset);
            newPolygon.AddPoint(new Vector2(-size.x, size.y) + boxCollider.offset);
            newPolygon.AddPoint(new Vector2(size.x, size.y) + boxCollider.offset);
            newPolygon.AddPoint(new Vector2(size.x, -size.y) + boxCollider.offset);

            return newPolygon;
        }

        public static Polygon2D CreateFromCapsuleCollider(CapsuleCollider2D capsuleCollider, int pointsCount = -1)
        {
            if (pointsCount < 1) pointsCount = defaultCircleVerticesCount;

            var newPolygon = new Polygon2D();

            var size = new Vector2(capsuleCollider.size.x / 2, capsuleCollider.size.y / 2);
            Vector2 point;
            float offset = 0;
            float angle = 0;
            float sizeRatio = 0;
            var step = 360f / pointsCount;

            switch (capsuleCollider.direction)
            {
                case CapsuleDirection2D.Vertical:

                    sizeRatio = capsuleCollider.transform.localScale.x / capsuleCollider.transform.localScale.y;
                    size.x *= sizeRatio;
                    angle = 0;

                    if (capsuleCollider.size.x < capsuleCollider.size.y)
                        offset = (capsuleCollider.size.y - capsuleCollider.size.x) / 2;

                    while (angle < 180)
                    {
                        point.x = Mathf.Cos(angle * Mathf.Deg2Rad) * size.x;
                        point.y = offset + Mathf.Sin(angle * Mathf.Deg2Rad) * size.x;

                        newPolygon.AddPoint(point + capsuleCollider.offset);
                        angle += step;
                    }

                    while (angle < 360)
                    {
                        point.x = Mathf.Cos(angle * Mathf.Deg2Rad) * size.x;
                        point.y = -offset + Mathf.Sin(angle * Mathf.Deg2Rad) * size.x;

                        newPolygon.AddPoint(point + capsuleCollider.offset);
                        angle += step;
                    }

                    break;

                case CapsuleDirection2D.Horizontal:

                    sizeRatio = capsuleCollider.transform.localScale.y / capsuleCollider.transform.localScale.x;
                    size.x *= sizeRatio;
                    angle = -90;

                    if (capsuleCollider.size.y < capsuleCollider.size.x)
                        offset = (capsuleCollider.size.x - capsuleCollider.size.y) / 2;

                    while (angle < 90)
                    {
                        point.x = offset + Mathf.Cos(angle * Mathf.Deg2Rad) * size.y;
                        point.y = Mathf.Sin(angle * Mathf.Deg2Rad) * size.y;

                        newPolygon.AddPoint(point + capsuleCollider.offset);
                        angle += step;
                    }

                    while (angle < 270)
                    {
                        point.x = -offset + Mathf.Cos(angle * Mathf.Deg2Rad) * size.y;
                        point.y = Mathf.Sin(angle * Mathf.Deg2Rad) * size.y;

                        newPolygon.AddPoint(point + capsuleCollider.offset);
                        angle += step;
                    }

                    break;
            }

            return newPolygon;
        }
    }
}