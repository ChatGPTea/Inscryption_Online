using System;
using System.Collections;
using System.Collections.Generic;

namespace FunkyCode.Utilities.Polygon2DTriangulation
{
    public enum TriangulationAlgorithm
    {
        DTSweep
    }

    public enum Orientation
    {
        CW,
        CCW,
        Collinear
    }

    public enum TriangulationMode
    {
        Unconstrained,
        Constrained,
        Polygon
    }

    public interface ITriangulatable
    {
        IList<DelaunayTriangle> Triangles { get; }
        TriangulationMode TriangulationMode { get; }
        string FileName { get; set; }
        bool DisplayFlipX { get; set; }
        bool DisplayFlipY { get; set; }
        float DisplayRotate { get; set; }
        double Precision { get; set; }
        double MinX { get; }
        double MaxX { get; }
        double MinY { get; }
        double MaxY { get; }
        Rect2D Bounds { get; }

        void Prepare(TriangulationContext tcx);
        void AddTriangle(DelaunayTriangle t);
        void AddTriangles(IEnumerable<DelaunayTriangle> list);
        void ClearTriangles();
    }

    public class Edge
    {
        protected Point2D mP;
        protected Point2D mQ;

        public Edge()
        {
            mP = null;
            mQ = null;
        }

        public Edge(Point2D edgeStart, Point2D edgeEnd)
        {
            mP = edgeStart;
            mQ = edgeEnd;
        }

        public Point2D EdgeStart
        {
            get => mP;
            set => mP = value;
        }

        public Point2D EdgeEnd
        {
            get => mQ;
            set => mQ = value;
        }
    }

    public class TriangulationConstraint : Edge
    {
        public TriangulationConstraint(TriangulationPoint p1, TriangulationPoint p2)
        {
            mP = p1;
            mQ = p2;
            if (p1.Y > p2.Y)
            {
                mQ = p1;
                mP = p2;
            }
            else if (p1.Y == p2.Y)
            {
                if (p1.X > p2.X)
                {
                    mQ = p1;
                    mP = p2;
                }
                else if (p1.X == p2.X)
                {
                }
            }

            CalculateContraintCode();
        }

        public TriangulationPoint P
        {
            get => mP as TriangulationPoint;
            set
            {
                if (value != null && mP != value)
                {
                    mP = value;
                    CalculateContraintCode();
                }
            }
        }

        public TriangulationPoint Q
        {
            get => mQ as TriangulationPoint;
            set
            {
                if (value != null && mQ != value)
                {
                    mQ = value;
                    CalculateContraintCode();
                }
            }
        }

        public uint ConstraintCode { get; private set; }

        public override string ToString()
        {
            return "[P=" + P + ", Q=" + Q + " : {" + ConstraintCode + "}]";
        }

        public void CalculateContraintCode()
        {
            ConstraintCode = CalculateContraintCode(P, Q);
        }


        public static uint CalculateContraintCode(TriangulationPoint p, TriangulationPoint q)
        {
            if (p == null || p == null)
                throw new ArgumentNullException();

            var constraintCode = MathUtil.Jenkins32Hash(BitConverter.GetBytes(p.VertexCode), 0);
            constraintCode = MathUtil.Jenkins32Hash(BitConverter.GetBytes(q.VertexCode), constraintCode);

            return constraintCode;
        }
    }

    public abstract class TriangulationContext
    {
        public readonly List<TriangulationPoint> Points = new(200);

        public readonly List<DelaunayTriangle> Triangles = new();
        public TriangulationDebugContext DebugContext { get; protected set; }
        public TriangulationMode TriangulationMode { get; protected set; }
        public ITriangulatable Triangulatable { get; private set; }

        public int StepCount { get; private set; }

        public abstract TriangulationAlgorithm Algorithm { get; }

        public virtual bool IsDebugEnabled { get; protected set; }
        public DTSweepDebugContext DTDebugContext => DebugContext as DTSweepDebugContext;

        public void Done()
        {
            StepCount++;
        }

        public virtual void PrepareTriangulation(ITriangulatable t)
        {
            Triangulatable = t;
            TriangulationMode = t.TriangulationMode;
            t.Prepare(this);
        }

        public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);

        public void Update(string message)
        {
        }

        public virtual void Clear()
        {
            Points.Clear();
            if (DebugContext != null)
                DebugContext.Clear();
            StepCount = 0;
        }
    }

    public abstract class TriangulationDebugContext
    {
        protected TriangulationContext _tcx;

        public TriangulationDebugContext(TriangulationContext tcx)
        {
            _tcx = tcx;
        }

        public abstract void Clear();
    }

    public class TriangulationPoint : Point2D
    {
        public static readonly double kVertexCodeDefaultPrecision = 3.0;

        protected uint mVertexCode;

        public TriangulationPoint(double x, double y) : this(x, y, kVertexCodeDefaultPrecision)
        {
        }

        public TriangulationPoint(double x, double y, double precision) : base(x, y)
        {
            mVertexCode = CreateVertexCode(x, y, precision);
        }

        public override double X
        {
            get => mX;
            set
            {
                if (value != mX)
                {
                    mX = value;
                    mVertexCode = CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);
                }
            }
        }

        public override double Y
        {
            get => mY;
            set
            {
                if (value != mY)
                {
                    mY = value;
                    mVertexCode = CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);
                }
            }
        }

        public uint VertexCode => mVertexCode;

        public List<DTSweepConstraint> Edges { get; private set; }
        public bool HasEdges => Edges != null;

        public override string ToString()
        {
            return base.ToString() + ":{" + mVertexCode + "}";
        }

        public override int GetHashCode()
        {
            return (int)mVertexCode;
        }

        public override bool Equals(object obj)
        {
            var p2 = obj as TriangulationPoint;
            if (p2 != null)
                return mVertexCode == p2.VertexCode;
            return base.Equals(obj);
        }

        public override void Set(double x, double y)
        {
            if (x != mX || y != mY)
            {
                mX = x;
                mY = y;
                mVertexCode = CreateVertexCode(mX, mY, kVertexCodeDefaultPrecision);
            }
        }

        public static uint CreateVertexCode(double x, double y, double precision)
        {
            var fx = (float)MathUtil.RoundWithPrecision(x, precision);
            var fy = (float)MathUtil.RoundWithPrecision(y, precision);
            var vc = MathUtil.Jenkins32Hash(BitConverter.GetBytes(fx), 0);
            vc = MathUtil.Jenkins32Hash(BitConverter.GetBytes(fy), vc);
            return vc;
        }

        public void AddEdge(DTSweepConstraint e)
        {
            if (Edges == null)
                Edges = new List<DTSweepConstraint>();
            Edges.Add(e);
        }

        public bool HasEdge(TriangulationPoint p)
        {
            DTSweepConstraint tmp = null;
            return GetEdge(p, out tmp);
        }

        public bool GetEdge(TriangulationPoint p, out DTSweepConstraint edge)
        {
            edge = null;
            if (Edges == null || Edges.Count < 1 || p == null || p.Equals(this))
                return false;

            foreach (var sc in Edges)
                if ((sc.P.Equals(this) && sc.Q.Equals(p)) || (sc.P.Equals(p) && sc.Q.Equals(this)))
                {
                    edge = sc;
                    return true;
                }

            return false;
        }

        public static Point2D ToPoint2D(TriangulationPoint p)
        {
            return p;
        }
    }


    public class TriangulationPointEnumerator : IEnumerator<TriangulationPoint>
    {
        protected IList<Point2D> mPoints;
        protected int position = -1;

        public TriangulationPointEnumerator(IList<Point2D> points)
        {
            mPoints = points;
        }

        public bool MoveNext()
        {
            position++;
            return position < mPoints.Count;
        }

        public void Reset()
        {
            position = -1;
        }

        void IDisposable.Dispose()
        {
        }

        object IEnumerator.Current => Current;

        public TriangulationPoint Current
        {
            get
            {
                if (position < 0 || position >= mPoints.Count)
                    return null;
                return mPoints[position] as TriangulationPoint;
            }
        }
    }

    public class TriangulationPointList : Point2DList
    {
    }
}