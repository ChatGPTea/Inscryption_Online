using System.Collections.Generic;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public class DoublePair2
    {
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;

        public DoublePair2(Vector2 pointA, Vector2 pointB, Vector2 pointC)
        {
            A = pointA;
            B = pointB;
            C = pointC;
        }

        public static List<DoublePair2> GetList(Vector2[] list, bool connect = true)
        {
            var pairsList = new List<DoublePair2>();
            if (list.Length > 0)
                for (var i = 0; i < list.Length; i++)
                {
                    var pB = list[i];

                    var indexB = i;

                    var indexA = indexB - 1;
                    if (indexA < 0) indexA += list.Length;

                    var indexC = indexB + 1;
                    if (indexC >= list.Length) indexC -= list.Length;

                    pairsList.Add(new DoublePair2(list[indexA], pB, list[indexC]));
                }

            return pairsList;
        }

        public static List<DoublePair2> GetListCopy(List<Vector2> list, bool connect = true)
        {
            var pairsList = new List<DoublePair2>();
            if (list.Count > 0)
                foreach (var pB in list)
                {
                    var indexB = list.IndexOf(pB);

                    var indexA = indexB - 1;
                    if (indexA < 0) indexA += list.Count;

                    var indexC = indexB + 1;
                    if (indexC >= list.Count) indexC -= list.Count;

                    pairsList.Add(new DoublePair2(list[indexA], pB, list[indexC]));
                }

            return pairsList;
        }
    }
}