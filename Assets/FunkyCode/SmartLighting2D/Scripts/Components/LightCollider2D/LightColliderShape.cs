using System;
using System.Collections.Generic;
using FunkyCode.LightShape;
using FunkyCode.Utilities;
using UnityEngine;

namespace FunkyCode
{
    [Serializable]
    public class LightColliderShape
    {
        public LightCollider2D.ShadowType shadowType = LightCollider2D.ShadowType.SpritePhysicsShape;
        public LightCollider2D.MaskType maskType = LightCollider2D.MaskType.Sprite;
        public LightCollider2D.MaskPivot maskPivot = LightCollider2D.MaskPivot.TransformCenter;
        public Transform transform;

        public Collider2DShape collider2DShape = new();

        public Collider3DShape collider3DShape = new();
        public CompositeCollider2DShape compositeShape = new();

        public MeshRendererShape meshShape = new();
        public SkinnedMeshRendererShape skinnedMeshShape = new();
        public SpritePhysicsShape spritePhysicsShape = new();

        public SpriteShape spriteShape = new();

        public LightColliderTransform transform2D = new();

        public Base GetShadowShape()
        {
            switch (shadowType)
            {
                case LightCollider2D.ShadowType.SpritePhysicsShape:
                    return spritePhysicsShape;

                case LightCollider2D.ShadowType.Collider2D:
                    return collider2DShape;

                case LightCollider2D.ShadowType.Collider3D:
                    return collider3DShape;

                case LightCollider2D.ShadowType.CompositeCollider2D:
                    return compositeShape;

                case LightCollider2D.ShadowType.MeshRenderer:
                    return meshShape;

                case LightCollider2D.ShadowType.SkinnedMeshRenderer:
                    return skinnedMeshShape;
            }

            return null;
        }

        public Base GetMaskShape()
        {
            switch (maskType)
            {
                case LightCollider2D.MaskType.Sprite:
                    return spriteShape;

                case LightCollider2D.MaskType.BumpedSprite:
                    return spriteShape;

                case LightCollider2D.MaskType.SpritePhysicsShape:
                    return spritePhysicsShape;

                case LightCollider2D.MaskType.CompositeCollider2D:
                    return compositeShape;

                case LightCollider2D.MaskType.Collider2D:
                    return collider2DShape;

                case LightCollider2D.MaskType.Collider3D:
                    return collider3DShape;

                case LightCollider2D.MaskType.MeshRenderer:
                    return meshShape;

                case LightCollider2D.MaskType.BumpedMeshRenderer:
                    return meshShape;

                case LightCollider2D.MaskType.SkinnedMeshRenderer:
                    return skinnedMeshShape;
            }

            return null;
        }

        public void SetTransform(LightCollider2D lightCollider2D)
        {
            transform = lightCollider2D.transform;

            transform2D.SetShape(this, lightCollider2D);

            spriteShape.SetTransform(transform);
            spritePhysicsShape.SetTransform(transform);

            collider2DShape.SetTransform(transform);
            compositeShape.SetTransform(transform);

            meshShape.SetTransform(transform);
            skinnedMeshShape.SetTransform(transform);

            collider3DShape.SetTransform(transform);
        }

        public void ResetLocal()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null)
            {
                shadowShape.ResetLocal();
                shadowShape.ResetWorld();
            }

            var maskShape = GetMaskShape();

            if (maskShape != null)
            {
                maskShape.ResetLocal();
                maskShape.ResetWorld();
            }
        }

        public void ResetWorld()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null) shadowShape.ResetWorld();

            var maskShape = GetMaskShape();

            if (maskShape != null) maskShape.ResetWorld();
        }

        public bool RectOverlap(Rect rect)
        {
            var shadowShape = GetShadowShape();
            var maskShape = GetMaskShape();

            if (shadowShape != null)
            {
                var result = shadowShape.GetWorldRect().Overlaps(rect);

                if (result) return true;
            }

            if (maskShape != null)
            {
                var result = maskShape.GetWorldRect().Overlaps(rect);

                if (result) return true;
            }

            return false;
        }

        public Rect GetWorldRect()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null) return shadowShape.GetWorldRect();

            var maskShape = GetMaskShape();

            if (maskShape != null) return maskShape.GetWorldRect();

            return new Rect();
        }

        public int GetSortingOrder()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null)
            {
                var shadowOrder = shadowShape.GetSortingOrder();

                if (shadowOrder != 0) return shadowOrder;
            }

            var maskShape = GetMaskShape();

            if (maskShape != null)
            {
                var maskOrder = maskShape.GetSortingOrder();

                if (maskOrder != 0) return maskOrder;
            }

            return 0;
        }

        public int GetSortingLayer()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null)
            {
                var shadowLayer = shadowShape.GetSortingLayer();

                if (shadowLayer != 0) return shadowLayer;
            }

            var maskShape = GetMaskShape();

            if (maskShape != null)
            {
                var maskLayer = maskShape.GetSortingLayer();

                if (maskLayer != 0) return maskLayer;
            }

            return 0;
        }

        public Rect GetIsoWorldRect()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null) return shadowShape.GetIsoWorldRect();

            var maskShape = GetMaskShape();

            if (maskShape != null) return maskShape.GetIsoWorldRect();

            return new Rect();
        }

        public List<MeshObject> GetMeshes()
        {
            var maskShape = GetMaskShape();

            if (maskShape != null) return maskShape.GetMeshes();

            return null;
        }

        public List<Polygon2> GetPolygonsLocal()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null) return shadowShape.GetPolygonsLocal();

            return null;
        }

        public List<Polygon2> GetPolygonsWorld()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null) return shadowShape.GetPolygonsWorld();

            return null;
        }

        public Vector2 GetPivotPoint()
        {
            var shadowShape = GetShadowShape();

            if (shadowShape != null)
                switch (maskPivot)
                {
                    case LightCollider2D.MaskPivot.TransformCenter:
                        return shadowShape.GetPivotPoint_TransformCenter();

                    case LightCollider2D.MaskPivot.ShapeCenter:
                        return shadowShape.GetPivotPoint_ShapeCenter();

                    case LightCollider2D.MaskPivot.LowestY:
                        return shadowShape.GetPivotPoint_LowestY();
                }

            var maskShape = GetMaskShape();

            if (maskShape != null)
                switch (maskPivot)
                {
                    case LightCollider2D.MaskPivot.TransformCenter:
                        return maskShape.GetPivotPoint_TransformCenter();

                    case LightCollider2D.MaskPivot.ShapeCenter:
                        return maskShape.GetPivotPoint_ShapeCenter();

                    case LightCollider2D.MaskPivot.LowestY:
                        return maskShape.GetPivotPoint_LowestY();
                }

            return Vector2.zero;
        }

        public bool IsEdgeCollider()
        {
            switch (shadowType)
            {
                case LightCollider2D.ShadowType.Collider2D:
                    return collider2DShape.edgeCollider2D;
            }

            return false;
        }
    }
}