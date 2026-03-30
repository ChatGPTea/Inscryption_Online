using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FunkyCode.Utilities
{
    public static class Math2D
    {
        private const double tor = 1E-10;


        // LEGACY OLD CODE


        private static readonly Pair2D pointInPoly_Pair2D = Pair2D.Zero();

        /*
            //Compute the dot product AB . BC
        float DotProduct(float2 pointA, float2 pointB, float2 pointC)
        {
            float2 AB, BC;

            AB.x = pointB.x - pointA.x;
            AB.y = pointB.y - pointA.y;
            BC.x = pointC.x - pointB.x;
            BC.y = pointC.y - pointB.y;

            return AB.x * BC.x + AB.y * BC.y;
        }

        //Compute the cross product AB x AC
        float CrossProduct(float2 pointA, float2 pointB, float2 pointC)
        {
            float2 AB, AC;

            AB.x = pointB.x - pointA.x;
            AB.y = pointB.y - pointA.y;
            AC.x = pointC.x - pointA.x;
            AC.y = pointC.y - pointA.y;

            return AB.x * AC.y - AB.y * AC.x;
        }

        float LineToPointDistance2D(float2 pointA, float2 pointB, float2 pointC, bool isSegment)
        {
            float dist = CrossProduct(pointA, pointB, pointC) / distance(pointA, pointB);
            if (isSegment)
            {
                float dot1 = DotProduct(pointA, pointB, pointC);
                if (dot1 > 0) {
                    return distance(pointB, pointC);
                }

                float dot2 = DotProduct(pointB, pointA, pointC);
                if (dot2 > 0) {
                    return distance(pointA, pointC);
                }

            }

            return abs(dist);
        }

        */
        public static float Range(float value, float min, float max)
        {
            value = value > max ? max : value;

            value = value < min ? min : value;

            return value;
        }

        public static float NormalizeRotation(float rotation)
        {
            return (rotation + 720) % 360;
        }

        public static Vector2 ClosestPointOnLine(Vector2 vA, Vector2 vB)
        {
            var vx = vB.x - vA.x;
            var vy = vB.y - vA.y;

            var distance = Mathf.Sqrt(vx * vx + vy * vy);
            vx = vx / distance;
            vy = vy / distance;

            var t = vx * -vA.x + vy * -vA.y;

            if (t <= 0) return vA;

            var xs = vA.x - vB.x;
            var ys = vA.y - vB.y;

            var dist = t * t >= xs * xs + ys * ys;

            if (dist) return vB;

            vA.x += vx * t;
            vA.y += vy * t;

            return vA;
        }

        public static Vector2 ClosestPointOnLine(Vector2 vPoint, Vector2 vA, Vector2 vB)
        {
            var vx = vB.x - vA.x;
            var vy = vB.y - vA.y;

            var distance = Mathf.Sqrt(vx * vx + vy * vy);
            vx = vx / distance;
            vy = vy / distance;

            var v1x = vPoint.x - vA.x;
            var v1y = vPoint.y - vA.y;

            // Vector2.Dot(v, v1);
            var t = vx * v1x + vy * v1y;

            if (t <= 0) return vA;

            var xs = vA.x - vB.x;
            var ys = vA.y - vB.y;

            var dist = t * t >= xs * xs + ys * ys;

            // t >= Vector2.Distance(vA, vB)
            if (dist) return vB;

            vA.x += vx * t;
            vA.y += vy * t;

            return vA;
        }

        public static Vector3 GetPitchYawRollRad(Quaternion rotation)
        {
            var roll = Mathf.Atan2(2 * rotation.y * rotation.w - 2 * rotation.x * rotation.z,
                1 - 2 * rotation.y * rotation.y - 2 * rotation.z * rotation.z);
            var pitch = Mathf.Atan2(2 * rotation.x * rotation.w - 2 * rotation.y * rotation.z,
                1 - 2 * rotation.x * rotation.x - 2 * rotation.z * rotation.z);
            var yaw = Mathf.Asin(2 * rotation.x * rotation.y + 2 * rotation.z * rotation.w);

            return new Vector3(pitch, roll, yaw);
        }

        public static Vector3 GetPitchYawRollDeg(Quaternion rotation)
        {
            var radResult = GetPitchYawRollRad(rotation);
            return new Vector3(radResult.x * Mathf.Rad2Deg, radResult.y * Mathf.Rad2Deg, radResult.z * Mathf.Rad2Deg);
        }

        public static Rect GetBounds(List<Vector2D> pointsList)
        {
            double rMinX = 1e+10f;
            double rMinY = 1e+10f;
            double rMaxX = -1e+10f;
            double rMaxY = -1e+10f;

            foreach (var id in pointsList)
            {
                rMinX = Math.Min(rMinX, id.x);
                rMinY = Math.Min(rMinY, id.y);
                rMaxX = Math.Max(rMaxX, id.x);
                rMaxY = Math.Max(rMaxY, id.y);
            }

            return new Rect((float)rMinX, (float)rMinY, (float)Math.Abs(rMinX - rMaxX), (float)Math.Abs(rMinY - rMaxY));
        }

        public static Rect GetBounds(Pair2D pair)
        {
            double rMinX = 1e+10f;
            double rMinY = 1e+10f;
            double rMaxX = -1e+10f;
            double rMaxY = -1e+10f;

            var id = pair.A;
            rMinX = Math.Min(rMinX, id.x);
            rMinY = Math.Min(rMinY, id.y);
            rMaxX = Math.Max(rMaxX, id.x);
            rMaxY = Math.Max(rMaxY, id.y);

            id = pair.B;
            rMinX = Math.Min(rMinX, id.x);
            rMinY = Math.Min(rMinY, id.y);
            rMaxX = Math.Max(rMaxX, id.x);
            rMaxY = Math.Max(rMaxY, id.y);

            return new Rect((float)rMinX, (float)rMinY, (float)Math.Abs(rMinX - rMaxX), (float)Math.Abs(rMinY - rMaxY));
        }


        public static bool PolyInPoly(Polygon2D polyA, Polygon2D polyB)
        {
            foreach (var p in Pair2D.GetList(polyB.pointsList))
                if (!PointInPoly(p.A, polyA))
                    return false;

            if (PolyIntersectPoly(polyA, polyB)) return false;

            return true;
        }

        // Is it not finished?
        public static bool PolyCollidePoly(Polygon2D polyA, Polygon2D polyB)
        {
            if (PolyIntersectPoly(polyA, polyB)) return true;

            if (PolyInPoly(polyA, polyB)) return true;

            if (PolyInPoly(polyB, polyA)) return true;

            return false;
        }

        public static bool PolyIntersectPoly(Polygon2D polyA, Polygon2D polyB)
        {
            foreach (var a in Pair2D.GetList(polyA.pointsList))
            foreach (var b in Pair2D.GetList(polyB.pointsList))
                if (LineIntersectLine(a, b))
                    return true;

            return false;
        }

        public static bool SliceIntersectPoly(List<Vector2D> slice, Polygon2D poly)
        {
            var pairA = new Pair2D(null, null);
            foreach (var pointA in slice)
            {
                pairA.B = pointA;

                if (pairA.A != null && pairA.B != null)
                {
                    var pairB = new Pair2D(new Vector2D(poly.pointsList.Last()), null);
                    foreach (var pointB in poly.pointsList)
                    {
                        pairB.B = pointB;

                        if (LineIntersectLine(pairA, pairB)) return true;

                        pairB.A = pointB;
                    }
                }

                pairA.A = pointA;
            }

            return false;
        }

        public static bool SliceIntersectSlice(List<Vector2D> sliceA, List<Vector2D> sliceB)
        {
            var pairA = new Pair2D(null, null);
            foreach (var pointA in sliceA)
            {
                pairA.B = pointA;

                if (pairA.A != null && pairA.B != null)
                {
                    var pairB = new Pair2D(null, null);
                    foreach (var pointB in sliceB)
                    {
                        pairB.B = pointB;

                        if (pairB.A != null && pairB.B != null)
                            if (LineIntersectLine(pairA, pairB))
                                return true;

                        pairB.A = pointB;
                    }
                }

                pairA.A = pointA;
            }

            return false;
        }

        public static bool LineIntersectPoly(Pair2D line, Polygon2D poly)
        {
            var pair = new Pair2D(new Vector2D(poly.pointsList.Last()), new Vector2D(Vector2.zero));
            foreach (var point in poly.pointsList)
            {
                pair.B = point;

                if (LineIntersectLine(line, pair)) return true;

                pair.A = point;
            }

            return false;
        }

        public static bool LineIntersectLine(Pair2D lineA, Pair2D lineB)
        {
            if (GetPointLineIntersectLine(lineA, lineB) != null) return true;

            return false;
        }

        public static bool PolygonIntersectItself(List<Vector2> points)
        {
            var pairAA = Vector2.zero;
            var pairAB = Vector2.zero;
            var pairBA = Vector2.zero;
            var pairBB = Vector2.zero;

            for (var i = 0; i < points.Count; i++)
            {
                pairAA = points[i % points.Count];

                pairAB = points[(i + 1) % points.Count];

                for (var x = 0; x < points.Count; x++)
                {
                    pairBA = points[x % points.Count];

                    pairBB = points[(x + 1) % points.Count];

                    if (GetPointLineIntersectLine3(pairAA, pairAB, pairBA, pairBB) != null)
                        if (pairAA != pairBA && pairAB != pairBB && pairAA != pairBB && pairAB != pairBA)
                            return true;
                }
            }

            return false;
        }

        public static bool SliceIntersectItself(List<Vector2D> slice)
        {
            var pairA = new Pair2D(null, null);
            foreach (var va in slice)
            {
                pairA.B = va;

                if (pairA.A != null && pairA.B != null)
                {
                    var pairB = new Pair2D(null, null);
                    foreach (var vb in slice)
                    {
                        pairB.B = vb;

                        if (pairB.A != null && pairB.B != null)
                            if (GetPointLineIntersectLine(pairA, pairB) != null)
                                if (pairA.A != pairB.A && pairA.B != pairB.B && pairA.A != pairB.B &&
                                    pairA.B != pairB.A)
                                    return true;

                        pairB.A = vb;
                    }
                }

                pairA.A = va;
            }

            return false;
        }

        public static Vector2D GetPointLineIntersectLine(Pair2D lineA, Pair2D lineB)
        {
            double ay_cy, ax_cx, px, py;
            var dx_cx = lineB.B.x - lineB.A.x;
            var dy_cy = lineB.B.y - lineB.A.y;
            var bx_ax = lineA.B.x - lineA.A.x;
            var by_ay = lineA.B.y - lineA.A.y;
            var de = bx_ax * dy_cy - by_ay * dx_cx;
            var tor = 1E-10;

            if (Math.Abs(de) < 0.0001d) return null;

            if (de > -tor && de < tor) return null;

            ax_cx = lineA.A.x - lineB.A.x;
            ay_cy = lineA.A.y - lineB.A.y;

            var r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
            var s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

            if (r < 0 || r > 1 || s < 0 || s > 1) return null;

            px = lineA.A.x + r * bx_ax;
            py = lineA.A.y + r * by_ay;

            return new Vector2D(px, py);
        }

        public static Vector2? GetPointLineIntersectLine2(Pair2D lineA, Pair2D lineB)
        {
            double ay_cy, ax_cx;
            var dx_cx = lineB.B.x - lineB.A.x;
            var dy_cy = lineB.B.y - lineB.A.y;
            var bx_ax = lineA.B.x - lineA.A.x;
            var by_ay = lineA.B.y - lineA.A.y;
            var de = bx_ax * dy_cy - by_ay * dx_cx;
            var tor = 1E-10;

            if (Math.Abs(de) < 0.0001d) return null;

            if (de > -tor && de < tor) return null;

            ax_cx = lineA.A.x - lineB.A.x;
            ay_cy = lineA.A.y - lineB.A.y;

            var r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
            var s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

            if (r < 0 || r > 1 || s < 0 || s > 1) return null;

            var px = (float)(lineA.A.x + r * bx_ax);
            var py = (float)(lineA.A.y + r * by_ay);

            return new Vector2(px, py);
        }

        public static Vector2? GetPointLineIntersectLine3(Vector2 a_a, Vector2 a_b, Vector2 b_a, Vector2 b_b)
        {
            double dx_cx = b_b.x - b_a.x;
            double dy_cy = b_b.y - b_a.y;
            double bx_ax = a_b.x - a_a.x;
            double by_ay = a_b.y - a_a.y;
            var de = bx_ax * dy_cy - by_ay * dx_cx;

            if (Math.Abs(de) < 0.0001d) return null;

            if (de > -tor && de < tor) return null;

            double ax_cx = a_a.x - b_a.x;
            double ay_cy = a_a.y - b_a.y;

            var r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
            var s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

            if (r < 0 || r > 1 || s < 0 || s > 1) return null;

            return new Vector2((float)(a_a.x + r * bx_ax), (float)(a_a.y + r * by_ay));
        }

        public static bool GetPointLineIntersectLine4(Vector2 a_a, Vector2 a_b, Vector2 b_a, Vector2 b_b)
        {
            double dx_cx = b_b.x - b_a.x;
            double dy_cy = b_b.y - b_a.y;
            double bx_ax = a_b.x - a_a.x;
            double by_ay = a_b.y - a_a.y;
            var de = bx_ax * dy_cy - by_ay * dx_cx;

            if (Math.Abs(de) < 0.0001d) return false;

            if (de > -tor && de < tor) return false;

            double ax_cx = a_a.x - b_a.x;
            double ay_cy = a_a.y - b_a.y;

            var r = (ay_cy * dx_cx - ax_cx * dy_cy) / de;
            var s = (ay_cy * bx_ax - ax_cx * by_ay) / de;

            if (r < 0 || r > 1 || s < 0 || s > 1) return false;

            return true;
        }


        public static bool PointInPoly(Vector2 point, Polygon2 poly)
        {
            var count = poly.points.Length;
            if (count < 3) return false;

            var total = 0;
            var diff = 0;

            var A = poly.points[count - 1];
            Vector2 B;

            for (var i = 0; i < count; i++)
            {
                B = poly.points[i];

                var vertARes = 0;
                var vertBRes = 0;

                if (A.x < point.x)
                    vertARes = A.y < point.y ? 1 : 4;
                else
                    vertARes = A.y < point.y ? 2 : 3;

                if (B.x < point.x)
                    vertBRes = B.y < point.y ? 1 : 4;
                else
                    vertBRes = B.y < point.y ? 2 : 3;

                diff = vertARes - vertBRes;

                switch (diff)
                {
                    case -2:
                    case 2:
                        if (B.x - (B.y - point.y) * (A.x - B.x) / (A.y - B.y) < point.x)
                            diff = -diff;

                        break;

                    case 3:
                        diff = -1;
                        break;

                    case -3:
                        diff = 1;
                        break;
                }

                total += diff;

                A = B;
            }

            return total == 4 || total == -4;
        }


        // Getting List is Slower
        public static List<Vector2D> GetListLineIntersectPoly(Pair2D line, Polygon2D poly)
        {
            var result = new List<Vector2D>();

            var pair = new Pair2D(new Vector2D(poly.pointsList.Last()), null);
            foreach (var point in poly.pointsList)
            {
                pair.B = point;

                var intersection = GetPointLineIntersectLine(line, pair);
                if (intersection != null) result.Add(intersection);

                pair.A = point;
            }

            return result;
        }

        public static List<Vector2D> GetListLineIntersectSlice(Pair2D pair, List<Vector2D> slice)
        {
            var resultList = new List<Vector2D>();

            var id = new Pair2D(null, null);
            foreach (var point in slice)
            {
                id.B = point;

                if (id.A != null && id.B != null)
                {
                    var result = GetPointLineIntersectLine(id, pair);
                    if (result != null) resultList.Add(result);
                }

                id.A = point;
            }

            return resultList;
        }

        public static bool PolygonIntersectCircle(Polygon2D poly, Vector2D circle, float radius)
        {
            foreach (var id in Pair2D.GetList(poly.pointsList))
                if (LineIntersectCircle(id, circle, radius))
                    return true;

            return false;
        }

        public static bool LineIntersectCircle(Pair2D line, Vector2D circle, float radius)
        {
            var sx = line.B.x - line.A.x;
            var sy = line.B.y - line.A.y;

            var q = ((circle.x - line.A.x) * (line.B.x - line.A.x) + (circle.y - line.A.y) * (line.B.y - line.A.y)) /
                    (sx * sx + sy * sy);

            if (q < 0.0f)
                q = 0.0f;
            else if (q > 1.0) q = 1.0f;

            var dx = circle.x - ((1.0f - q) * line.A.x + q * line.B.x);
            var dy = circle.y - ((1.0f - q) * line.A.y + q * line.B.y);

            if (dx * dx + dy * dy < radius * radius) return true;

            return false;
        }


        public static float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p)
        {
            return (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);
        }

        public static bool PointInPoly(Vector2D point, Polygon2D poly)
        {
            var count = poly.pointsList.Count;
            if (count < 3) return false;

            var total = 0;
            var diff = 0;

            var id = pointInPoly_Pair2D;
            id.A = poly.pointsList[count - 1];

            Vector2D p;

            for (var i = 0; i < count; i++)
            {
                p = poly.pointsList[i];

                id.B = p;

                diff = GetQuad(point, id.A) - GetQuad(point, id.B);

                switch (diff)
                {
                    case -2:
                    case 2:
                        if (id.B.x - (id.B.y - point.y) * (id.A.x - id.B.x) / (id.A.y - id.B.y) < point.x)
                            diff = -diff;

                        break;

                    case 3:
                        diff = -1;
                        break;

                    case -3:
                        diff = 1;
                        break;
                }

                total += diff;

                id.A = id.B;
            }

            return Math.Abs(total) == 4;
        }

        private static int GetQuad(Vector2D axis, Vector2D vert)
        {
            if (vert.x < axis.x)
            {
                if (vert.y < axis.y) return 1;
                return 4;
            }

            if (vert.y < axis.y) return 2;
            return 3;
        }
    }
}