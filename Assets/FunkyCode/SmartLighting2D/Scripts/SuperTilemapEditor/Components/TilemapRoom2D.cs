using FunkyCode.LightTilemapCollider;

#if (SUPER_TILEMAP_EDITOR)
    namespace FunkyCode.SuperTilemapEditorSupport
    {
        public class TilemapRoom2D : TilemapCollider
        {
            // public enum MaskType {None, Grid, Sprite};
            // public MaskType maskType = MaskType.Sprite;
            // No Enums?
        }
    }

#else

namespace FunkyCode.SuperTilemapEditorSupport
{
    public class TilemapRoom2D : Base
    {
        public override void Initialize()
        {
        }
    }
}

#endif