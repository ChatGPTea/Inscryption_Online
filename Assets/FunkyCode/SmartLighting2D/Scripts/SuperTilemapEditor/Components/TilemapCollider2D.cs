using System;
using FunkyCode.LightTilemapCollider;

#if (SUPER_TILEMAP_EDITOR)
    namespace FunkyCode.SuperTilemapEditorSupport
    {
        [System.Serializable]
        public class TilemapCollider2D : TilemapCollider
        {
        }
    }

#else

namespace FunkyCode.SuperTilemapEditorSupport
{
    [Serializable]
    public class TilemapCollider2D : TilemapCollider
    {
    }

    public class TilemapCollider : Base
    {
        public enum MaskType
        {
            None,
            Grid,
            Sprite,
            BumpedSprite
        }

        public enum ShadowType
        {
            None,
            Grid,
            TileCollider,
            Collider
        }

        public bool eventsInit;
        public MaskType maskTypeSTE = MaskType.Sprite;

        public ShadowType shadowTypeSTE = ShadowType.Grid;
    }
}

#endif