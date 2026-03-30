using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public class Polygon2DListCollider2D : Polygon2DCollider2D
    {
        // Get List Of Polygons from Collider (Usually Used Before Creating Slicer2D Object)
        public static List<Polygon2D> CreateFromPolygonColliderToWorldSpace(PolygonCollider2D collider)
        {
            var result = new List<Polygon2D>();

            if (collider != null && collider.pathCount > 0)
            {
                var newPolygon = new Polygon2D();

                foreach (var p in collider.GetPath(0)) newPolygon.AddPoint(p + collider.offset);

                newPolygon = newPolygon.ToWorldSpace(collider.transform);

                result.Add(newPolygon);

                for (var i = 1; i < collider.pathCount; i++)
                {
                    var hole = new Polygon2D();
                    foreach (var p in collider.GetPath(i)) hole.AddPoint(p + collider.offset);

                    hole = hole.ToWorldSpace(collider.transform);

                    if (newPolygon.PolyInPoly(hole))
                        newPolygon.AddHole(hole);
                    else
                        result.Add(hole);
                }
            }

            return result;
        }

        public static List<Polygon2D> CreateFromPolygonColliderToLocalSpace(PolygonCollider2D collider)
        {
            var result = new List<Polygon2D>();

            if (collider != null && collider.pathCount > 0)
            {
                var newPolygon = new Polygon2D();

                foreach (var p in collider.GetPath(0)) newPolygon.AddPoint(p + collider.offset);

                result.Add(newPolygon);

                for (var i = 1; i < collider.pathCount; i++)
                {
                    var hole = new Polygon2D();
                    foreach (var p in collider.GetPath(i)) hole.AddPoint(p + collider.offset);

                    if (newPolygon.PolyInPoly(hole))
                        newPolygon.AddHole(hole);
                    else
                        result.Add(hole);
                }
            }

            return result;
        }

        // Slower CreateFromCollider
        public static List<Polygon2D> CreateFromGameObject(GameObject gameObject)
        {
            var result = new List<Polygon2D>();

            foreach (var c in gameObject.GetComponents<Collider2D>())
            {
                var type = c.GetType();

                if (type == typeof(BoxCollider2D))
                {
                    var boxCollider2D = (BoxCollider2D)c;


                    result.Add(CreateFromBoxCollider(boxCollider2D));
                }

                if (type == typeof(CircleCollider2D))
                {
                    var circleCollider2D = (CircleCollider2D)c;

                    result.Add(CreateFromCircleCollider(circleCollider2D));
                }

                if (type == typeof(CapsuleCollider2D))
                {
                    var capsuleCollider2D = (CapsuleCollider2D)c;

                    result.Add(CreateFromCapsuleCollider(capsuleCollider2D));
                }

                if (type == typeof(EdgeCollider2D))
                {
                    var edgeCollider2D = (EdgeCollider2D)c;

                    result.Add(CreateFromEdgeCollider(edgeCollider2D));
                }

                if (type == typeof(PolygonCollider2D))
                {
                    var polygonCollider2D = (PolygonCollider2D)c;

                    var polygonColliders = CreateFromPolygonColliderToLocalSpace(polygonCollider2D);

                    foreach (var poly in polygonColliders) result.Add(poly);
                }
            }

            foreach (var poly in result) poly.Normalize();

            return result;
        }
    }
}