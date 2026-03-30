using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode.SpriteExtension
{
    public static class PhysicsShapeManager
    {
        public static Dictionary<Sprite, PhysicsShape> dictionary = new();

        public static void Clear()
        {
            dictionary = new Dictionary<Sprite, PhysicsShape>();
        }

        public static PhysicsShape RequestCustomShape(Sprite originalSprite)
        {
            if (originalSprite == null) return null;

            PhysicsShape shape = null;

            var exist = dictionary.TryGetValue(originalSprite, out shape);

            if (exist)
            {
                if (shape == null || shape.GetSprite().texture == null)
                    shape = RequestCustomShapeAccess(originalSprite);

                return shape;
            }

            return RequestCustomShapeAccess(originalSprite);
        }

        public static PhysicsShape RequestCustomShapeAccess(Sprite originalSprite)
        {
            PhysicsShape shape = null;

            var exist = dictionary.TryGetValue(originalSprite, out shape);

            if (exist)
            {
                if (shape == null || shape.GetSprite().texture == null)
                {
                    dictionary.Remove(originalSprite);

                    shape = AddShape(originalSprite);

                    dictionary.Add(originalSprite, shape);
                }

                return shape;
            }

            shape = AddShape(originalSprite);

            dictionary.Add(originalSprite, shape);

            return shape;
        }

        private static PhysicsShape AddShape(Sprite sprite)
        {
            if (sprite == null || sprite.texture == null) return null;

            var shape = new PhysicsShape(sprite);

            return shape;
        }
    }
}