using FunkyCode.LightTilemapCollider;
using UnityEngine;

namespace FunkyCode.Rendering.Light.Shadow
{
    public class TilemapCollider
    {
        public class Rectangle
        {
            public static void Draw(Light2D light, LightTilemapCollider2D id)
            {
                Vector2 position = -light.transform.position;

                switch (id.rectangle.shadowType)
                {
                    case ShadowType.CompositeCollider:

                        ShadowEngine.objectOffset = id.transform.position;

                        ShadowEngine.Draw(id.rectangle.compositeColliders, 0, 0, 0);

                        ShadowEngine.objectOffset = Vector2.zero;

                        break;
                }
            }
        }
    }
}