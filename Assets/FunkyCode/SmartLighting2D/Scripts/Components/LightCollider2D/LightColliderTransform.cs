using UnityEngine;

namespace FunkyCode
{
    public class LightColliderTransform
    {
        private bool flipX;
        private bool flipY;

        private LightCollider2D lightCollider;

        private Vector3 position3D = Vector3.zero;
        public float shadowHeight = 0;
        public float shadowTranslucency;

        private LightColliderShape shape;
        private Vector2 size = Vector2.one;

        public LightColliderTransform()
        {
            Position = Vector2.zero;
            Rotation = 0;
            Scale = Vector3.zero;
        }

        public bool UpdateNeeded { get; set; } = true;

        public Vector2 Position { private set; get; }
        public Vector2 Scale { private set; get; }
        public float Rotation { private set; get; }

        public void SetShape(LightColliderShape shape, LightCollider2D lightCollider)
        {
            this.shape = shape;

            this.lightCollider = lightCollider;
        }

        public void Reset()
        {
            Position = Vector2.zero;
            Rotation = 0;
            Scale = Vector3.zero;
        }

        private void UpdateTransform(Transform transform)
        {
            var newPosition3D = transform.position;
            var position2D = LightingPosition.GetPosition2D(transform.position);

            Vector2 scale2D = transform.lossyScale;
            var rotation2D = LightingPosition.GetRotation2D(transform);

            if (Scale != scale2D)
            {
                Scale = scale2D;

                UpdateNeeded = true;
            }

            if (Rotation != rotation2D)
            {
                Rotation = rotation2D;

                UpdateNeeded = true;
            }

            if (position3D != newPosition3D)
            {
                position3D = newPosition3D;

                UpdateNeeded = true;
            }

            if (Position != position2D)
            {
                Position = position2D;

                UpdateNeeded = true;
            }
        }

        public void Update(bool force)
        {
            var transform = shape.transform;
            if (!transform) return;

            if (transform.hasChanged || force)
            {
                transform.hasChanged = false;

                UpdateTransform(transform);
            }

            if (shadowTranslucency != lightCollider.shadowTranslucency)
            {
                shadowTranslucency = lightCollider.shadowTranslucency;

                UpdateNeeded = true;
            }

            var checkShapeSprite = shape.maskType == LightCollider2D.MaskType.SpritePhysicsShape ||
                                   shape.shadowType == LightCollider2D.ShadowType.SpritePhysicsShape;
            var checkMaskSprite = shape.maskType == LightCollider2D.MaskType.Sprite ||
                                  shape.maskType == LightCollider2D.MaskType.BumpedSprite;

            if (checkShapeSprite || checkMaskSprite)
            {
                var spriteRenderer = shape.spriteShape.GetSpriteRenderer();
                if (spriteRenderer)
                {
                    if (spriteRenderer.size != size)
                    {
                        size = spriteRenderer.size;

                        UpdateNeeded = true;
                    }

                    if (spriteRenderer.flipX != flipX || spriteRenderer.flipY != flipY)
                    {
                        flipX = spriteRenderer.flipX;
                        flipY = spriteRenderer.flipY;

                        shape.ResetWorld();

                        UpdateNeeded = true;
                    }

                    if (shape.spriteShape.GetOriginalSprite() != spriteRenderer.sprite)
                    {
                        shape.ResetLocal();

                        UpdateNeeded = true;
                    }
                }
            }

            var checkShapeMesh = shape.maskType == LightCollider2D.MaskType.MeshRenderer ||
                                 shape.shadowType == LightCollider2D.ShadowType.MeshRenderer;

            if (checkShapeMesh)
            {
                var meshFilter = shape.meshShape.GetMeshFilter();
                if (meshFilter)
                    if (meshFilter.sharedMesh != shape.meshShape.Mesh)
                    {
                        shape.ResetLocal();

                        UpdateNeeded = true;
                    }
            }
        }
    }
}