using System;
using UnityEngine;

namespace FunkyCode.Rendering.Light.Sorting
{
    public class SortList
    {
        public SortObject[] List = new SortObject[1024];

        public SortList()
        {
            Count = 0;

            for (var i = 0; i < List.Length; i++)
                List[i] = new SortObject();
        }

        public int Count { private set; get; }

        public void Add(object collider, float dist)
        {
            if (Count < List.Length)
            {
                List[Count] = new SortObject(dist, collider);
                Count++;
            }
            else
            {
                Debug.LogError("Collider Depth Overhead!");
            }
        }

        public void Add(LightTilemapCollider2D tilemap, LightTile tile2D, float value)
        {
            if (Count < List.Length)
            {
                List[Count] = new SortObject(value, tile2D, tilemap);
                Count++;
            }
            else
            {
                Debug.LogError("Tile Depth Overhead!");
            }
        }

        public void Reset()
        {
            Count = 0;
        }

        public void Sort()
        {
            Array.Sort(List, 0, Count, SortObject.Sort());
        }
    }
}