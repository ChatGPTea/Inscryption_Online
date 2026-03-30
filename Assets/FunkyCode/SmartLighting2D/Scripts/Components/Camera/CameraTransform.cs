using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    public class CameraTransform
    {
        public static List<CameraTransform> List = new();

        private Polygon2 polygon;

        private Vector2 position = Vector2.zero;
        private float rotation;
        private Vector2 scale = Vector2.one;
        private float size;
        private Transform transform;

        private Polygon2 worldPolygon;
        private Rect worldRect;

        public Camera Camera { get; private set; }

        public static void Update()
        {
            for (var id = 0; id < List.Count; id++)
            {
                var transform = List[id];

                if (transform.Camera)
                    transform.Update();
                else
                    List.Remove(transform);
            }
        }

        // Change
        public static float GetRadius(Camera camera)
        {
            var cameraRadius = camera.orthographicSize;

            if (camera.pixelWidth > camera.pixelHeight) cameraRadius *= (float)camera.pixelWidth / camera.pixelHeight;

            cameraRadius = Mathf.Sqrt(cameraRadius * cameraRadius + cameraRadius * cameraRadius);

            return cameraRadius;
        }

        public static Rect GetWorldRect(Camera camera)
        {
            var cameraTransform = GetCamera(camera);

            return cameraTransform.WorldRect();
        }

        public static CameraTransform GetCamera(Camera camera)
        {
            if (camera == null)
                Debug.LogError("Camera == Null");

            var cameraExists = List.Find(x => x.Camera == camera);
            if (cameraExists != null)
                return cameraExists;

            var cameraTransform = new CameraTransform();
            cameraTransform.Camera = camera;
            cameraTransform.transform = camera.transform;

            cameraTransform.Update(true);

            List.Add(cameraTransform);

            return cameraTransform;
        }

        public void Update(bool force = false)
        {
            if (Camera == null)
                return;

            var transform = Camera.transform;
            if (transform.hasChanged || force)
            {
                transform.hasChanged = false;

                position = LightingPosition.GetPosition2D(transform.position);
                scale = transform.lossyScale;
                rotation = LightingPosition.GetRotation2D(transform);
                size = Camera.orthographicSize;

                worldPolygon = null;
            }
        }

        public Rect WorldRect()
        {
            if (worldPolygon != null)
                return worldRect;

            return WorldRectGenerate();
        }

        private Rect WorldRectGenerate()
        {
            var cameraSizeY = Camera.orthographicSize;
            var cameraSizeX = cameraSizeY * Camera.pixelWidth / Camera.pixelHeight;

            var sizeX = cameraSizeX * 2;
            var sizeY = cameraSizeY * 2;

            var x = -sizeX / 2;
            var y = -sizeY / 2;

            worldPolygon = Polygon();

            worldPolygon.points[0].x = x;
            worldPolygon.points[0].y = y;

            worldPolygon.points[1].x = x + sizeX;
            worldPolygon.points[1].y = y;

            worldPolygon.points[2].x = x + sizeX;
            worldPolygon.points[2].y = y + sizeY;

            worldPolygon.points[3].x = x;
            worldPolygon.points[3].y = y + sizeY;

            worldPolygon.ToRotationSelf(rotation * Mathf.Deg2Rad);
            worldPolygon.ToOffsetSelf(position);

            worldRect = worldPolygon.GetRect();

            return worldRect;
        }

        private Polygon2 Polygon()
        {
            if (polygon == null)
            {
                polygon = new Polygon2(4);

                polygon.points[0] = Vector2.zero;
                polygon.points[1] = Vector2.zero;
                polygon.points[2] = Vector2.zero;
                polygon.points[3] = Vector2.zero;
            }

            return polygon;
        }
    }
}