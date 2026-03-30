using UnityEngine;

namespace FunkyCode
{
    public class LightTransform
    {
        private Color color = Color.white;
        private bool flipX;
        private bool flipY;
        private float normalDepth = 1;

        private float normalIntensity = 1;


        private float outerAngle = 15;

        public Vector2 position = Vector2.zero;
        public float rotation;
        private float size;
        private float spotAngleInner = 360;
        private float spotAngleOuter = 360;

        private Sprite sprite;

        public Rect WorldRect;

        public bool UpdateNeeded { get; private set; } = true;

        public void ForceUpdate()
        {
            UpdateNeeded = true;
        }

        public void ClearUpdate()
        {
            UpdateNeeded = false;
        }

        public void Update(Light2D light)
        {
            if (light.gameObject == null) return;

            if (light.transform == null) return;

            var transform = light.transform;

            var position2D = LightingPosition.GetPosition2D(transform.position);

            float rotation2D = 0;

            switch (light.applyRotation)
            {
                case Light2D.Rotation.Local:

                    rotation2D = transform.localRotation.eulerAngles.z;

                    break;

                case Light2D.Rotation.World:

                    rotation2D = transform.rotation.eulerAngles.z;

                    break;
            }

            if (position != position2D)
            {
                position = position2D;

                UpdateNeeded = true;
            }

            if (rotation != rotation2D)
            {
                rotation = rotation2D;

                UpdateNeeded = true;
            }

            if (size != light.size)
            {
                size = light.size;

                UpdateNeeded = true;
            }

            if (sprite != light.sprite)
            {
                sprite = light.sprite;

                UpdateNeeded = true;
            }

            if (flipX != light.spriteFlipX)
            {
                flipX = light.spriteFlipX;

                UpdateNeeded = true;
            }

            if (flipY != light.spriteFlipY)
            {
                flipY = light.spriteFlipY;

                UpdateNeeded = true;
            }

            if (spotAngleInner != light.spotAngleInner)
            {
                spotAngleInner = light.spotAngleInner;

                UpdateNeeded = true;
            }

            if (spotAngleOuter != light.spotAngleOuter)
            {
                spotAngleOuter = light.spotAngleOuter;

                UpdateNeeded = true;
            }

            if (outerAngle != light.outerAngle)
            {
                outerAngle = light.outerAngle;

                UpdateNeeded = true;
            }

            if (normalIntensity != light.bumpMap.intensity)
            {
                normalIntensity = light.bumpMap.intensity;

                UpdateNeeded = true;
            }

            if (normalDepth != light.bumpMap.depth)
            {
                normalDepth = light.bumpMap.depth;

                UpdateNeeded = true;
            }

            if (UpdateNeeded) WorldRect = new Rect(position.x - size, position.y - size, size * 2, size * 2);

            // no need to update for color and alpha
            if (color != light.color) color = light.color;
        }
    }
}