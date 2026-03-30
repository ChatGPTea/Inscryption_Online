using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public class Polygon2DListCollider3D : Polygon2DCollider3D
    {
        public static List<Polygon2D> CreateFromGameObject(GameObject gameObject)
        {
            var result = new List<Polygon2D>();

            foreach (var collider in gameObject.GetComponents<Collider>())
            {
                var type = collider.GetType();

                if (type == typeof(BoxCollider))
                {
                    var boxCollider = (BoxCollider)collider;

                    result.Add(CreateFromBoxCollider(boxCollider));
                }

                if (type == typeof(SphereCollider))
                {
                    var sphereCollider = (SphereCollider)collider;

                    result.Add(CreateFromSphereCollider(sphereCollider));
                }

                if (type == typeof(CapsuleCollider))
                {
                    var capsuleCollider = (CapsuleCollider)collider;

                    result.Add(CreateFromCapsuleCollider(capsuleCollider));
                }

                if (type == typeof(MeshCollider))
                {
                    var meshCollider = (MeshCollider)collider;

                    var polygons = CreateFromMeshCollider(meshCollider);

                    foreach (var polygon in polygons) result.Add(polygon);
                }
            }

            foreach (var poly in result) poly.Normalize();

            return result;
        }
    }
}