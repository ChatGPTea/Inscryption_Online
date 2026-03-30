using UnityEngine;
using UnityEngine.Tilemaps;

namespace FunkyCode
{
    public class TilemapEvents
    {
        private static bool initialized;

        public static void Initialize()
        {
            if (initialized) return;

            initialized = true;

#if UNITY_EDITOR

#if UNITY_2019_4_OR_NEWER
            Tilemap.tilemapTileChanged -= Events;
            Tilemap.tilemapTileChanged += Events;
#endif

#endif
        }

#if UNITY_EDITOR
#if UNITY_2019_4_OR_NEWER
        public static void Events(Tilemap tilemap, Tilemap.SyncTile[] s)
        {
            if (Application.isPlaying) return;

            foreach (var tilemap2D in LightTilemapCollider2D.List) tilemap2D.Initialize();

            Light2D.ForceUpdateAll();
        }
#endif
#endif
    }
}