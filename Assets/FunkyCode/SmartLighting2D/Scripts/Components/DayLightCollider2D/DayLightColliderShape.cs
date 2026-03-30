using System;
using System.Collections.Generic;
using FunkyCode.LightShape;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    [Serializable]
    public class DayLightColliderShape
    {
        public DayLightCollider2D.ShadowType shadowType = DayLightCollider2D.ShadowType.SpritePhysicsShape;

        public DayLightCollider2D.MaskType maskType = DayLightCollider2D.MaskType.Sprite;

        public Transform transform;

        public float height = 1;
        public float thickness = 1;

        public bool isStatic;
        public Collider2DShape colliderShape = new();
        public SpritePhysicsShape spritePhysicsShape = new();

        public SpriteShape spriteShape = new();

        public DayLightingColliderTransform transform2D = new();

        public void SetTransform(Transform t)
        {
            transform = t;

            transform2D.SetShape(this);

            spriteShape.SetTransform(t);
            spritePhysicsShape.SetTransform(t);

            colliderShape.SetTransform(t);
        }

        public void ResetLocal()
        {
            spriteShape.ResetLocal();
            spritePhysicsShape.ResetLocal();

            colliderShape.ResetLocal();
        }

        public void ResetWorld()
        {
            spritePhysicsShape.ResetWorld();

            colliderShape.ResetWorld();
        }

        public List<MeshObject> GetMeshes()
        {
            switch (shadowType)
            {
                case DayLightCollider2D.ShadowType.FillCollider2D:

                    return colliderShape.GetMeshes();

                case DayLightCollider2D.ShadowType.FillSpritePhysicsShape:

                    return spritePhysicsShape.GetMeshes();
            }

            return null;
        }

        public List<Polygon2> GetPolygonsLocal()
        {
            switch (shadowType)
            {
                case DayLightCollider2D.ShadowType.SpritePhysicsShape:
                case DayLightCollider2D.ShadowType.FillSpritePhysicsShape:

                    return spritePhysicsShape.GetPolygonsLocal();

                case DayLightCollider2D.ShadowType.Collider2D:
                case DayLightCollider2D.ShadowType.FillCollider2D:

                    return colliderShape.GetPolygonsLocal();
            }

            return null;
        }

        public List<Polygon2> GetPolygonsWorld()
        {
            switch (shadowType)
            {
                case DayLightCollider2D.ShadowType.SpritePhysicsShape:
                case DayLightCollider2D.ShadowType.SpriteProjectionShape:
                case DayLightCollider2D.ShadowType.FillSpritePhysicsShape:

                    return spritePhysicsShape.GetPolygonsWorld();

                case DayLightCollider2D.ShadowType.Collider2D:
                case DayLightCollider2D.ShadowType.SpriteProjectionCollider:
                case DayLightCollider2D.ShadowType.FillCollider2D:

                    return colliderShape.GetPolygonsWorld();
            }

            return null;
        }

        public Rect GetShadowBounds()
        {
            var polygons = GetPolygonsWorld();

            if (polygons != null) return Polygon2Helper.GetDayRect(polygons, height);

            return new Rect();
        }
    }
}