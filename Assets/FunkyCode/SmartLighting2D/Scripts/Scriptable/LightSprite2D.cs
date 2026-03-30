using System;
using System.Collections.Generic;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode.Scriptable
{
    [Serializable]
    public class LightSprite2D
    {
        public static List<LightSprite2D> List = new();

        public LightSpriteShape lightSpriteShape = new();

        [SerializeField] private Vector2 position = Vector2.zero;
        [SerializeField] private Vector2 scale = Vector2.one;
        [SerializeField] private int lightLayer;
        [SerializeField] private Sprite sprite;
        [SerializeField] private Color color = new(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private float rotation;

        public SpriteMeshObject spriteMeshObject = new();

        public LightSprite2D()
        {
            List.Add(this);
        }

        public int LightLayer
        {
            set => lightLayer = value;
            get => lightLayer;
        }

        public Sprite Sprite
        {
            set => sprite = value;
            get => sprite;
        }

        public Vector2 Position
        {
            set => position = value;
            get => position;
        }

        public Vector2 Scale
        {
            set => scale = value;
            get => scale;
        }

        public Color Color
        {
            set => color = value;
            get => color;
        }

        public float Rotation
        {
            set => rotation = value;
            get => rotation;
        }

        public bool InCamera(Camera camera)
        {
            var cameraRect = CameraTransform.GetWorldRect(camera);

            lightSpriteShape.Update(this);

            if (cameraRect.Overlaps(lightSpriteShape.GetWorldRect())) return true;

            return false;
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                if (!List.Contains(this)) List.Add(this);
            }
            else
            {
                List.Remove(this);
            }
        }
    }

    [Serializable]
    public class LightSpriteTransform
    {
        public Vector2 scale = new(1, 1);
        public float rotation;
        public Vector2 position = new(0, 0);
    }

    [Serializable]
    public class LightSpriteShape
    {
        public bool update;

        private Polygon2 polygon;

        private Vector2 position = Vector2.zero;
        private float rotation;
        private Vector2 scale = Vector2.one;

        private Sprite sprite;

        private Polygon2 worldPolygon;
        private Rect worldrect;

        public void Update(LightSprite2D light)
        {
            var position2D = light.Position;
            var rotation2D = light.Rotation;
            var scale2D = light.Scale;

            sprite = light.Sprite;

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

            var position = this.position;
            var scale = this.scale;
            var rotation = this.rotation;

            var virtualSprite = new VirtualSpriteRenderer();
            virtualSprite.sprite = sprite;

            var spriteTransform = new SpriteTransform(virtualSprite, position, scale, rotation);

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