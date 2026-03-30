using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public class Vector2DList
    {
        public static List<Vector2D> ToWorldSpace(Transform transform, List<Vector2D> pointsList)
        {
            var resultList = new List<Vector2D>();
            foreach (var id in pointsList) resultList.Add(new Vector2D(transform.TransformPoint(id.ToVector2())));

            return resultList;
        }

        /// <summary>
        ///     Return a sorted list by distance to a given 2D point reference
        /// </summary>
        public static List<Vector2D> GetListSortedToPoint(List<Vector2D> pointsList, Vector2D point)
        {
            var resultList = new List<Vector2D>();
            var listCopy = new List<Vector2D>(pointsList);
            while (listCopy.Count > 0)
            {
                double dist = 1e+10f;
                Vector2D obj = null;
                foreach (var p in listCopy)
                {
                    var d = Vector2D.Distance(point, p);
                    if (d < dist)
                    {
                        obj = p;
                        dist = d;
                    }
                }

                if (obj != null)
                {
                    resultList.Add(obj);
                    listCopy.Remove(obj);
                }
            }

            return resultList;
        }

        /// <summary>
        ///     Return a list which starts with given 2D vector
        /// </summary>
        public static List<Vector2D> GetListStartingPoint(List<Vector2D> pointsList, Vector2D point)
        {
            // What if pointList does not contain point? 
            var result = new List<Vector2D>();
            var start = false;
            foreach (var p in pointsList)
                if (p == point || start)
                {
                    result.Add(p);
                    start = true;
                }

            foreach (var p in pointsList)
            {
                if (p == point)
                    start = false;
                if (start)
                    result.Add(p);
            }

            return result;
        }

        /// <summary>
        ///     Return a list which starts with first interesction with given line
        /// </summary>
        public static List<Vector2D> GetListStartingIntersectLine(List<Vector2D> pointsList, Pair2D line)
        {
            var result = new List<Vector2D>();
            var start = false;

            var pointsPairList = Pair2D.GetList(pointsList);

            foreach (var p in pointsPairList)
            {
                var r = Math2D.GetPointLineIntersectLine(p, line);
                if (start) result.Add(p.A);

                if (r != null)
                {
                    result.Add(r);
                    start = true;
                }
            }

            foreach (var p in pointsPairList)
            {
                var r = Math2D.GetPointLineIntersectLine(p, line);
                if (start) result.Add(p.A);

                if (r != null)
                {
                    result.Add(r);
                    start = false;
                }
            }

            return result;
        }

        // Might Break (Only for 2 collisions)
        /// <summary>
        ///     Return a list which starts with first interesction with given slice
        /// </summary>
        public static List<Vector2D> GetListStartingIntersectSlice(List<Vector2D> pointsList, List<Vector2D> slice)
        {
            var result = new List<Vector2D>();
            var start = false;

            var pointsPairList = Pair2D.GetList(pointsList);

            foreach (var p in pointsPairList)
            {
                var r = Math2D.GetListLineIntersectSlice(p, slice);
                if (start) result.Add(p.A);

                if (r.Count > 0)
                {
                    result.Add(r.First());
                    start = true;
                }
            }

            foreach (var p in pointsPairList)
            {
                var r = Math2D.GetListLineIntersectSlice(p, slice);
                if (start) result.Add(p.A);

                if (r.Count > 0)
                {
                    result.Add(r.First());
                    start = false;
                }
            }

            return result;
        }
    }
}