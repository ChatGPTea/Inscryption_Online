using System;
using System.Collections.Generic;

namespace FunkyCode.Utilities.Polygon2DTriangulation
{
    public class PointSet : Point2DList, ITriangulatable, IEnumerable<TriangulationPoint>, IList<TriangulationPoint>
    {
        protected Dictionary<uint, TriangulationPoint> mPointMap = new();

        protected double mPrecision = TriangulationPoint.kVertexCodeDefaultPrecision;

        public PointSet(List<TriangulationPoint> bounds)
        {
            foreach (var p in bounds)
            {
                Add(p, -1, false);
                mBoundingBox.AddPoint(p);
            }

            mEpsilon = CalculateEpsilon();
            mWindingOrder = WindingOrderType.Unknown;
        }

        public IList<TriangulationPoint> Points
        {
            get => this;
            private set { }
        }

        IEnumerator<TriangulationPoint> IEnumerable<TriangulationPoint>.GetEnumerator()
        {
            return new TriangulationPointEnumerator(mPoints);
        }

        public new TriangulationPoint this[int index]
        {
            get => mPoints[index] as TriangulationPoint;
            set => mPoints[index] = value;
        }

        public int IndexOf(TriangulationPoint p)
        {
            return mPoints.IndexOf(p);
        }

        public virtual void Add(TriangulationPoint p)
        {
            Add(p, -1, false);
        }

        public void Insert(int idx, TriangulationPoint item)
        {
            mPoints.Insert(idx, item);
        }

        public bool Remove(TriangulationPoint p)
        {
            return mPoints.Remove(p);
        }

        public override void RemoveAt(int idx)
        {
            if (idx < 0 || idx >= Count)
                return;
            mPoints.RemoveAt(idx);
        }

        public bool Contains(TriangulationPoint p)
        {
            return mPoints.Contains(p);
        }

        public void CopyTo(TriangulationPoint[] array, int arrayIndex)
        {
            var numElementsToCopy = Math.Min(Count, array.Length - arrayIndex);
            for (var i = 0; i < numElementsToCopy; ++i)
                array[arrayIndex + i] = mPoints[i] as TriangulationPoint;
        }

        public IList<DelaunayTriangle> Triangles { get; private set; }

        public string FileName { get; set; }
        public bool DisplayFlipX { get; set; }
        public bool DisplayFlipY { get; set; }
        public float DisplayRotate { get; set; }

        public double Precision
        {
            get => mPrecision;
            set => mPrecision = value;
        }

        public double MinX => mBoundingBox.MinX;
        public double MaxX => mBoundingBox.MaxX;
        public double MinY => mBoundingBox.MinY;
        public double MaxY => mBoundingBox.MaxY;
        public Rect2D Bounds => mBoundingBox;

        public virtual TriangulationMode TriangulationMode => TriangulationMode.Unconstrained;

        public virtual void AddTriangle(DelaunayTriangle t)
        {
            Triangles.Add(t);
        }

        public void AddTriangles(IEnumerable<DelaunayTriangle> list)
        {
            foreach (var tri in list)
                AddTriangle(tri);
        }

        public void ClearTriangles()
        {
            Triangles.Clear();
        }

        public virtual void Prepare(TriangulationContext tcx)
        {
            if (Triangles == null)
                Triangles = new List<DelaunayTriangle>(Points.Count);
            else
                Triangles.Clear();
            tcx.Points.AddRange(Points);
        }

        public override void Add(Point2D p)
        {
            Add(p as TriangulationPoint, -1, false);
        }

        protected override void Add(Point2D p, int idx, bool constrainToBounds)
        {
            Add(p as TriangulationPoint, idx, constrainToBounds);
        }

        protected bool Add(TriangulationPoint p, int idx, bool constrainToBounds)
        {
            if (p == null)
                return false;

            if (constrainToBounds)
                ConstrainPointToBounds(p);

            if (mPointMap.ContainsKey(p.VertexCode))
                return true;

            mPointMap.Add(p.VertexCode, p);

            if (idx < 0)
                mPoints.Add(p);
            else
                mPoints.Insert(idx, p);

            return true;
        }

        public override void AddRange(IEnumerator<Point2D> iter, WindingOrderType windingOrder)
        {
            if (iter == null)
                return;

            iter.Reset();
            while (iter.MoveNext())
                Add(iter.Current);
        }

        public virtual bool AddRange(List<TriangulationPoint> points)
        {
            var bOK = true;
            foreach (var p in points)
                bOK = Add(p, -1, false) && bOK;

            return bOK;
        }

        public bool TryGetPoint(double x, double y, out TriangulationPoint p)
        {
            var vc = TriangulationPoint.CreateVertexCode(x, y, Precision);
            if (mPointMap.TryGetValue(vc, out p))
                return true;

            return false;
        }

        public override bool Remove(Point2D p)
        {
            return mPoints.Remove(p);
        }

        protected bool ConstrainPointToBounds(Point2D p)
        {
            var oldX = p.X;
            var oldY = p.Y;
            p.X = Math.Max(MinX, p.X);
            p.X = Math.Min(MaxX, p.X);
            p.Y = Math.Max(MinY, p.Y);
            p.Y = Math.Min(MaxY, p.Y);

            return p.X != oldX || p.Y != oldY;
        }

        protected bool ConstrainPointToBounds(TriangulationPoint p)
        {
            var oldX = p.X;
            var oldY = p.Y;
            p.X = Math.Max(MinX, p.X);
            p.X = Math.Min(MaxX, p.X);
            p.Y = Math.Max(MinY, p.Y);
            p.Y = Math.Min(MaxY, p.Y);

            return p.X != oldX || p.Y != oldY;
        }

        public virtual bool Initialize()
        {
            return true;
        }
    }

    public class ConstrainedPointSet : PointSet
    {
        protected Dictionary<uint, TriangulationConstraint> mConstraintMap = new();
        protected List<Contour> mHoles = new();

        public ConstrainedPointSet(List<TriangulationPoint> bounds) : base(bounds)
        {
            AddBoundaryConstraints();
        }

        public ConstrainedPointSet(List<TriangulationPoint> bounds, List<TriangulationConstraint> constraints) :
            base(bounds)
        {
            AddBoundaryConstraints();
            AddConstraints(constraints);
        }

        public ConstrainedPointSet(List<TriangulationPoint> bounds, int[] indices) : base(bounds)
        {
            AddBoundaryConstraints();
            var l = new List<TriangulationConstraint>();
            for (var i = 0; i < indices.Length; i += 2)
            {
                var tc = new TriangulationConstraint(bounds[i], bounds[i + 1]);
                l.Add(tc);
            }

            AddConstraints(l);
        }

        public override TriangulationMode TriangulationMode => TriangulationMode.Constrained;

        protected void AddBoundaryConstraints()
        {
            TriangulationPoint ptLL = null;
            TriangulationPoint ptLR = null;
            TriangulationPoint ptUR = null;
            TriangulationPoint ptUL = null;
            if (!TryGetPoint(MinX, MinY, out ptLL))
            {
                ptLL = new TriangulationPoint(MinX, MinY);
                Add(ptLL);
            }

            if (!TryGetPoint(MaxX, MinY, out ptLR))
            {
                ptLR = new TriangulationPoint(MaxX, MinY);
                Add(ptLR);
            }

            if (!TryGetPoint(MaxX, MaxY, out ptUR))
            {
                ptUR = new TriangulationPoint(MaxX, MaxY);
                Add(ptUR);
            }

            if (!TryGetPoint(MinX, MaxY, out ptUL))
            {
                ptUL = new TriangulationPoint(MinX, MaxY);
                Add(ptUL);
            }

            var tcLLtoLR = new TriangulationConstraint(ptLL, ptLR);
            AddConstraint(tcLLtoLR);
            var tcLRtoUR = new TriangulationConstraint(ptLR, ptUR);
            AddConstraint(tcLRtoUR);
            var tcURtoUL = new TriangulationConstraint(ptUR, ptUL);
            AddConstraint(tcURtoUL);
            var tcULtoLL = new TriangulationConstraint(ptUL, ptLL);
            AddConstraint(tcULtoLL);
        }

        public override void Add(Point2D p)
        {
            Add(p as TriangulationPoint, -1, true);
        }

        public override void Add(TriangulationPoint p)
        {
            Add(p, -1, true);
        }

        public override bool AddRange(List<TriangulationPoint> points)
        {
            var bOK = true;
            foreach (var p in points)
                bOK = Add(p, -1, true) && bOK;

            return bOK;
        }

        public bool AddHole(List<TriangulationPoint> points, string name)
        {
            if (points == null)
                return false;

            var pts = new List<Contour>();
            var listIdx = 0;
            {
                var c = new Contour(this, points, WindingOrderType.Unknown);
                pts.Add(c);

                if (mPoints.Count > 1)
                {
                    var numPoints = pts[listIdx].Count;
                    for (var i = 0; i < numPoints; ++i)
                        ConstrainPointToBounds(pts[listIdx][i]);
                }
            }

            while (listIdx < pts.Count)
            {
                pts[listIdx].RemoveDuplicateNeighborPoints();
                pts[listIdx].WindingOrder = WindingOrderType.Default;

                var bListOK = true;
                var err = pts[listIdx].CheckPolygon();
                while (bListOK && err != PolygonError.None)
                {
                    if ((err & PolygonError.NotEnoughVertices) == PolygonError.NotEnoughVertices)
                    {
                        bListOK = false;
                        continue;
                    }

                    if ((err & PolygonError.NotSimple) == PolygonError.NotSimple)
                    {
                        var l = PolygonUtil.SplitComplexPolygon(pts[listIdx], pts[listIdx].Epsilon);
                        pts.RemoveAt(listIdx);
                        foreach (var newList in l)
                        {
                            var c = new Contour(this);
                            c.AddRange(newList);
                            pts.Add(c);
                        }

                        err = pts[listIdx].CheckPolygon();
                        continue;
                    }

                    if ((err & PolygonError.Degenerate) == PolygonError.Degenerate)
                    {
                        pts[listIdx].Simplify(Epsilon);
                        err = pts[listIdx].CheckPolygon();
                        continue;
                    }

                    if ((err & PolygonError.AreaTooSmall) == PolygonError.AreaTooSmall ||
                        (err & PolygonError.SidesTooCloseToParallel) == PolygonError.SidesTooCloseToParallel ||
                        (err & PolygonError.TooThin) == PolygonError.TooThin ||
                        (err & PolygonError.Unknown) == PolygonError.Unknown) bListOK = false;
                }

                if (!bListOK && pts[listIdx].Count != 2)
                    pts.RemoveAt(listIdx);
                else
                    ++listIdx;
            }

            var bOK = true;
            listIdx = 0;
            while (listIdx < pts.Count)
            {
                var numPoints = pts[listIdx].Count;
                if (numPoints < 2)
                {
                    ++listIdx;
                    bOK = false;
                    continue;
                }

                if (numPoints == 2)
                {
                    var constraintCode =
                        TriangulationConstraint.CalculateContraintCode(pts[listIdx][0], pts[listIdx][1]);
                    TriangulationConstraint tc = null;
                    if (!mConstraintMap.TryGetValue(constraintCode, out tc))
                    {
                        tc = new TriangulationConstraint(pts[listIdx][0], pts[listIdx][1]);
                        AddConstraint(tc);
                    }
                }
                else
                {
                    var ph = new Contour(this, pts[listIdx], WindingOrderType.Unknown);
                    ph.WindingOrder = WindingOrderType.Default;
                    ph.Name = name + ":" + listIdx;
                    mHoles.Add(ph);
                }

                ++listIdx;
            }

            return bOK;
        }

        public bool AddConstraints(List<TriangulationConstraint> constraints)
        {
            if (constraints == null || constraints.Count < 1)
                return false;

            var bOK = true;
            foreach (var tc in constraints)
            {
                if (ConstrainPointToBounds(tc.P) || ConstrainPointToBounds(tc.Q))
                    tc.CalculateContraintCode();

                TriangulationConstraint tcTmp = null;
                if (!mConstraintMap.TryGetValue(tc.ConstraintCode, out tcTmp))
                {
                    tcTmp = tc;
                    bOK = AddConstraint(tcTmp) && bOK;
                }
            }

            return bOK;
        }


        public bool AddConstraint(TriangulationConstraint tc)
        {
            if (tc == null || tc.P == null || tc.Q == null)
                return false;

            if (mConstraintMap.ContainsKey(tc.ConstraintCode))
                return true;

            TriangulationPoint p;
            if (TryGetPoint(tc.P.X, tc.P.Y, out p))
                tc.P = p;
            else
                Add(tc.P);

            if (TryGetPoint(tc.Q.X, tc.Q.Y, out p))
                tc.Q = p;
            else
                Add(tc.Q);

            mConstraintMap.Add(tc.ConstraintCode, tc);

            return true;
        }

        public bool TryGetConstraint(uint constraintCode, out TriangulationConstraint tc)
        {
            return mConstraintMap.TryGetValue(constraintCode, out tc);
        }

        public int GetNumConstraints()
        {
            return mConstraintMap.Count;
        }

        public Dictionary<uint, TriangulationConstraint>.Enumerator GetConstraintEnumerator()
        {
            return mConstraintMap.GetEnumerator();
        }

        public int GetNumHoles()
        {
            var numHoles = 0;
            foreach (var c in mHoles)
                numHoles += c.GetNumHoles(false);

            return numHoles;
        }

        public Contour GetHole(int idx)
        {
            if (idx < 0 || idx >= mHoles.Count)
                return null;

            return mHoles[idx];
        }

        public int GetActualHoles(out List<Contour> holes)
        {
            holes = new List<Contour>();
            foreach (var c in mHoles)
                c.GetActualHoles(false, ref holes);

            return holes.Count;
        }

        protected void InitializeHoles()
        {
            Contour.InitializeHoles(mHoles, this, this);
            foreach (var c in mHoles)
                c.InitializeHoles(this);
        }

        public override bool Initialize()
        {
            InitializeHoles();
            return base.Initialize();
        }

        public override void Prepare(TriangulationContext tcx)
        {
            if (!Initialize())
                return;

            base.Prepare(tcx);

            var it = mConstraintMap.GetEnumerator();
            while (it.MoveNext())
            {
                var tc = it.Current.Value;
                tcx.NewConstraint(tc.P, tc.Q);
            }
        }

        public override void AddTriangle(DelaunayTriangle t)
        {
            Triangles.Add(t);
        }
    }
}