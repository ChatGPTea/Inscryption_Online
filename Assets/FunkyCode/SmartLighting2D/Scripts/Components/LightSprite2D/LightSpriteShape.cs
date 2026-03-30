using System;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    [Serializable]
    public class LightSpriteTransform
    {
        public bool applyRotation = true;

        public Vector2 scale = new(1, 1);
        public float rotation;
        public Vector2 position = new(0, 0);
    }

    [Serializable]
    public class LightSpriteShape
    {
        public bool update;
        private LightSpriteTransform lightSpriteTransform;

        private Polygon2 polygon;
        private Vector2 position = Vector2.zero;
        private float rotation;
        private Vector2 scale = Vector2.one;
        private Sprite sprite;
        private VirtualSpriteRenderer spriteRenderer;
        private Transform transform;

        private Polygon2 worldPolygon;
        private Rect worldrect;

        public void Set(VirtualSpriteRenderer spriteRenderer, Transform transform,
            LightSpriteTransform lightSpriteTransform)
        {
            this.spriteRenderer = spriteRenderer;
            this.lightSpriteTransform = lightSpriteTransform;
            this.transform = transform;
        }

        public void Update()
        {
            if (lightSpriteTransform == null) return;

            if (transform == null) return;

            var position2D = lightSpriteTransform.position;
            position2D += LightingPosition.GetPosition2D(transform.position);

            var rotation2D = transform.eulerAngles.z + lightSpriteTransform.rotation;

            var scale2D = lightSpriteTransform.scale;
            scale2D += LightingPosition.GetPosition2D(transform.lossyScale);

            if (position != position2D)
            {
                position = position2D;

                update = true;
            }

            if (rotation != rotation2D)
            {
                rotation = rotation2D;

                update = true;
            }

            if (scale != scale2D)
            {
                scale = scale2D;

                update = true;
            }

            if (update)
            {
                worldPolygon = null;

                update = false;
            }
        }

        public Rect GetWorldRect()
        {
            GetSpriteWorldPolygon();
            return worldrect;
        }

        public Polygon2 GetSpriteWorldPolygon()
        {
            if (worldPolygon != null) return worldPolygon;

            if (transform == null) return null;

            var position = LightingPosition.GetPosition2D(transform.position);
            position += lightSpriteTransform.position;

            var scale = LightingPosition.GetPosition2D(transform.lossyScale);
            scale *= lightSpriteTransform.scale;

            float rotation = 0;

            if (lightSpriteTransform.applyRotation) rotation += transform.eulerAngles.z + lightSpriteTransform.rotation;

            var spriteTransform = new SpriteTransform(spriteRenderer, position, scale, rotation);

            var rot = spriteTransform.rotation;
            var size = spriteTransform.scale;
            var pos = spriteTransform.position;

            rot = rot * Mathf.Deg2Rad + Mathf.PI;

            var rectAngle = Mathf.Atan2(size.y, size.x);
            var dist = Mathf.Sqrt(size.x * size.x + size.y * size.y);

            var v1 = new Vector2(pos.x + Mathf.Cos(rectAngle + rot) * dist, pos.y + Mathf.Sin(rectAngle + rot) * dist);
            var v2 = new Vector2(pos.x + Mathf.Cos(-rectAngle + rot) * dist,
                pos.y + Mathf.Sin(-rectAngle + rot) * dist);
            var v3 = new Vector2(pos.x + Mathf.Cos(rectAngle + Mathf.PI + rot) * dist,
                pos.y + Mathf.Sin(rectAngle + Mathf.PI + rot) * dist);
            var v4 = new Vector2(pos.x + Mathf.Cos(-rectAngle + Mathf.PI + rot) * dist,
                pos.y + Mathf.Sin(-rectAngle + Mathf.PI + rot) * dist);

            worldPolygon = GetPolygon();

            worldPolygon.points[0].x = v1.x;
            worldPolygon.points[0].y = v1.y;

            worldPolygon.points[1].x = v2.x;
            worldPolygon.points[1].y = v2.y;

            worldPolygon.points[2].x = v3.x;
            worldPolygon.points[2].y = v3.y;

            worldPolygon.points[3].x = v4.x;
            worldPolygon.points[3].y = v4.y;

            worldrect = worldPolygon.GetRect();

            return worldPolygon;
        }

        private Polygon2 GetPolygon()
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