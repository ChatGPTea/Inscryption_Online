using System.Collections.Generic;

namespace FunkyCode.Rendering.Lightmap.Sorting
{
    public struct SortObject : IComparer<SortObject>
    {
        public float Distance;

        public object LightObject;

        public SortObject(float distance, object lightObject)
        {
            Distance = distance;
            LightObject = lightObject;
        }

        public int Compare(SortObject a, SortObject b)
        {
            if (a.Distance > b.Distance)
                return 1;

            return a.Distance < b.Distance ? -1 : 0;
        }

        public static IComparer<SortObject> Sort()
        {
            return new SortObject();
        }
    }
}