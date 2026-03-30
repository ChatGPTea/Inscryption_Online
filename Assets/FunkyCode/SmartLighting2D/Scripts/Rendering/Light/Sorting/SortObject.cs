using System.Collections.Generic;

namespace FunkyCode.Rendering.Light.Sorting
{
    public struct SortObject : IComparer<SortObject>
    {
        public float Value; // value

        public object LightObject;

        public LightTilemapCollider2D Tilemap;

        public SortObject(float value, object lightObject, LightTilemapCollider2D tilemap = null)
        {
            Value = value;
            LightObject = lightObject;
            Tilemap = tilemap;
        }

        public int Compare(SortObject a, SortObject b)
        {
            if (a.Value > b.Value)
                return 1;

            return a.Value < b.Value ? -1 : 0;
        }

        private static readonly IComparer<SortObject> comparer = new SortObject();

        public static IComparer<SortObject> Sort()
        {
            return comparer;
        }
    }
}