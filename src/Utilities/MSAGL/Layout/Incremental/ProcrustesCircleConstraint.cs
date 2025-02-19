using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core;

namespace Microsoft.Msagl.Layout.Incremental {
    /// <summary>
    /// 
    /// </summary>
    public class ProcrustesCircleConstraint : IConstraint {
        Node[] V;
        int n;
        Point[] X;
        Point[] Y;
        /// <summary>
        /// Create a circle constraint with variable radius
        /// </summary>
        /// <param name="nodes"></param>
        public ProcrustesCircleConstraint(IEnumerable<Node> nodes) {
            V = nodes.ToArray();
            n = V.Length;
            X = new Point[n];
            Y = new Point[n];
            double angle = 2.0 * Math.PI / (double)n;
            double r = 10*(double)n; // we make our circle a reasonable size to aid numerical precision
            for (int i = 0; i < n; ++i) {
                double theta = angle * (double)i;
                Y[i] = new Point(r * Math.Cos(theta), r * Math.Sin(theta));
            }
        }
        /// <summary>
        /// A procrustes constraint with arbitrary target shape
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="targetConfiguration"></param>
        public ProcrustesCircleConstraint(IEnumerable<Node> nodes, IEnumerable<Point> targetConfiguration)
        {
            ValidateArg.IsNotNull(nodes, "nodes");
            ValidateArg.IsNotNull(targetConfiguration, "targetConfiguration");
            V = nodes.ToArray();
            n = V.Length;
            X = new Point[n];
            Y = new Point[n];
            int i = 0;
            Point yc = new Point();
            foreach (var p in targetConfiguration) {
                Y[i++] = p;
                yc += p;
            }
            yc /= n;
            for (i = 0; i < n; ++i)
               Y[i] -= yc;
        }
        #region IConstraint Members

        /// <summary>
        /// matrix product: A'B
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>2X2 matrix</returns>
        private static Point[] matrixProduct(Point[] A, Point[] B) {
            Point[] R = new Point[2];
            double x0 = 0, y0 = 0, x1 = 0, y1 = 0;
            for (int i = 0; i < A.Length; ++i)
            {
                x0 += A[i].X * B[i].X;
                y0 += A[i].X * B[i].Y;
                x1 += A[i].Y * B[i].X;
                y1 += A[i].Y * B[i].Y;
            }
            R[0] = new Point(x0, y0);
            R[1] = new Point(x1, y1);
            return R;
        }
        /// <summary>
        /// matrix product of two 2x2 matrices with no transpose, i.e.: AB
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns>2X2 matrix</returns>
        private static Point[] matrixProductNoTranspose(Point[] A, Point[] B) {
            Point[] R = new Point[2];
            R[0] = new Point(A[0].X*B[0].X+A[0].Y*B[1].X, A[0].X*B[0].Y + A[0].Y*B[1].Y);
            R[1] = new Point(A[1].X*B[0].X+A[1].Y*B[1].X, A[1].X*B[0].Y + A[1].Y*B[1].Y);
            return R;
        }
        private static Point MatrixTimesVector(Point[] A, Point v) {
            double a = A[0].X, b = A[0].Y, c = A[1].X, d = A[1].Y;
            return new Point(a * v.X + b * v.Y, c * v.X + d * v.Y);
        }
        private static Point eigenSystem2(Point[] B, out Point[] Q) {
            double[][] b = new double[B.Length][];
            for (int i = 0; i < B.Length; i++) {
                b[i] = new double[2];
                b[i][0] = B[i].X;
                b[i][1] = B[i].Y;
            }
            double lambda1;
            double[] q1;
            double lambda2;
            double[] q2;
            MultidimensionalScaling.SpectralDecomposition(b, out q1, out lambda1, out q2, out lambda2,300,1e-8);
            Q = new Point[] { new Point(q1[0], q1[1]), new Point(q2[0], q2[1]) };
            return new Point(lambda1, lambda2);
        }

        /// <summary>
        /// Compute singular value decomposition of a 2X2 matrix X=PSQ'
        /// </summary>
        /// <param name="X">input 2x2 matrix</param>
        /// <param name="P">left singular vectors</param>
        /// <param name="Q">right singular vectors (eigenvectors of X'X)</param>
        /// <returns>Singular values (Sqrt of eigenvalues of X'X)</returns>
        private static Point SingularValueDecomposition(Point[] X, out Point[] P, out Point[] Q) {
            Point[] XX = matrixProduct(X, X);
            Point l = eigenSystem2(XX, out Q);
            /*
            Point[] Q0;
            Point l0 = eigenSystem3(XX, out Q0);

            Q0 = new Point[] { Q0[1], Q0[0] };
            Q0 = Transpose(Q0);
            Q0 = new Point[] { -Q0[0], -Q0[1] };
            double tmp = l0.X;
            l0.X = l0.Y;
            l0.Y = tmp;
            if (!cmpEigenSystem(Q0, Q, l0, l)) {
                System.Console.Write("XX=");
                printMatrix(XX);
                System.Console.Write("Q0=");
                printMatrix(Q0);
                System.Console.WriteLine("l0=" + l0 + ";");
                System.Console.Write("Q=");
                printMatrix(Q);
                System.Console.WriteLine("l=" + l + ";");
            }
            l = l0;
            Q = Q0;
            System.Console.Write("Q=");
            printMatrix(Q);
            System.Console.WriteLine("l=" + l + ";");
             */
            Point s = new Point(Math.Sqrt(l.X), Math.Sqrt(l.Y));
            P = matrixProductNoTranspose(X, Q);
            P = matrixProductNoTranspose(P,new Point[]{new Point(1.0/s.X,0), new Point(0,1.0/s.Y)});
            return s;
        }
#if OLDPROJECT
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Project() {
            Point barycenter = Barycenter();
            Point[] X = new Point[ni];
            Point[] Y = new Point[ni];
            double angle = 2.0 * Math.PI / nd;
            double r = 0;
            int i = 0;
            foreach(var v in nodes) {
                X[i] = v.Center - barycenter;
                r += X[i].LengthSquared;
                ++i;
            }
            r = Math.Sqrt(r / nd);
            i = 0;
            foreach (var v in nodes) {
                double theta = angle * (double)i;
                Y[i] = new Point(r * Math.Cos(theta), r * Math.Sin(theta));
                ++i;
            }
            Point[] C = matrixProduct(Y, X);
            Point[] P, Q;
            SingularValueDecomposition(C, out P, out Q);
            Point[] T = matrixProductNoTranspose(Q, Transpose(P));
            /*
            Point[] XY = matrixProduct(X, Y);
            Point[] XYT = matrixProductNoTranspose(XY, T);
            Point[] YY = matrixProduct(Y, Y);
            double s = XYT[0].X + XYT[1].Y;
                   s /= YY[0].X + YY[1].Y;
            for (int i = 0; i < nodes.Count; ++i) {
                Y[i] = Y[i] * s;
            }
             */
            i = 0;
            foreach (var v in nodes) {
                v.Center = barycenter + MatrixTimesVector(T, Y[i++]);
            }
            return 0;
        }
#endif
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double Project() {
            for (int i = 0; i < n; ++i)
               X[i] = V[i].Center;

            Point[] T;
            double s;
            Point t;
            FindTransform(X, Y, out T, out s, out t);
            //System.Console.WriteLine("X=");
            //printMatrix(X);
            //System.Console.WriteLine("Y=");
            //printMatrix(Y);
            //System.Console.WriteLine("T=");
            //printMatrix(T);
            //System.Console.WriteLine("s="+s);
            //System.Console.WriteLine("t="+t);
            Point[] TT = Transpose(T);
            double displacement = 0;
            for (int i = 0; i < n; ++i)
            {
                var v = V[i];
                v.Center = double.IsNaN(s) ? Y[i] : s * MatrixTimesVector(TT, Y[i]) + t;
                displacement += (v.Center - X[i]).Length;
            }
            return displacement;
        }
        /// <summary>
        /// output an n*2 point matrix in Mathematica format
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.Write(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.Write(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String)")]
        public static void PrintMatrix(Point[] points) {
            ValidateArg.IsNotNull(points, "points");
            System.Console.Write("{");
            for (int i = 0; i < points.Length;) {
                System.Console.Write("{{{0},{1}}}", points[i].X, points[i].Y);
                if (++i != points.Length)
                   System.Console.Write(",");
            }
            System.Console.WriteLine("};");
        }
        private static Point[] Transpose(Point[] X) {
            return new Point[] { new Point(X[0].X, X[1].X), new Point(X[0].Y, X[1].Y) };
        }
        //// <summary>
        //// 
        //// </summary>
        //public static void Test1() {
        //    Random rand = new Random();
        //    Point[] X = new Point[] { new Point(rand.NextDouble(), rand.NextDouble()), new Point(rand.NextDouble(), rand.NextDouble()) };
        //    System.Console.Write("X="); printMatrix(X);
        //    Point[] P, Q;
        //    Point s = SingularValueDecomposition(X, out P, out Q);
        //    Point[] S = new Point[] { new Point(s.X, 0), new Point(0, s.Y) };
        //    System.Console.Write("S="); printMatrix(S);
        //    System.Console.Write("Q="); printMatrix(Q);
        //    System.Console.Write("P="); printMatrix(P);
        //    Point[] R = matrixProductNoTranspose(P, S);
        //    System.Console.Write("PS="); printMatrix(R);
        //    R = matrixProductNoTranspose(R, Transpose(Q));
        //    System.Console.Write("R="); printMatrix(R);
        //    Debug.Assert(Math.Abs(R[0].X - X[0].X) < 0.01);
        //    Debug.Assert(Math.Abs(R[0].Y - X[0].Y) < 0.01);
        //    Debug.Assert(Math.Abs(R[1].X - X[1].X) < 0.01);
        //    Debug.Assert(Math.Abs(R[1].Y - X[1].Y) < 0.01);
        //}
        //// <summary>
        //// 
        //// </summary>
  //      public static void Test() {
  //          double[,] XX = {{-0.342439, -0.815696}, {-0.772753, -0.807363}, {-0.264356, 
  //0.847908}, {-0.524064, 0.826169}, {0.615021, -0.762655}};
  //          List<Microsoft.Msagl.Node> vs = new List<Microsoft.Msagl.Node>();
  //          for (int i = 0; i < XX.Length/2; ++i) {
  //              var v = new Microsoft.Msagl.Node();
  //              v.Center = new Point(XX[i,0], XX[i,1]);
  //              vs.Add(v);
  //          }
  //          ProcrustesCircleConstraint c = new ProcrustesCircleConstraint(vs);
  //          c.Project();
  //      }  
        ///<summary>
        ///</summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Body"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.WriteLine(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Console.Write(System.String)")]
        public static void Test() {
            double[,] XX = { { 1, 2 }, { -1, 2 }, { -1, -2 }, { 1, -2 } },
                YY = { { 0.07, 2.62 }, { 0.93, 3.12 }, { 1.93, 1.38 }, { 1.07, 0.88 } };
            int ni = 4;
            Point[] X = new Point[ni];
            Point[] Y = new Point[ni];
            for (int i = 0; i < ni; ++i) {
                X[i] = new Point(XX[i, 0], XX[i, 1]);
                Y[i] = new Point(YY[i, 0], YY[i, 1]);
            }
            Point[] XY = matrixProduct(X, Y);
            System.Console.Write("XY=");
            PrintMatrix(XY);
            Point[] P, Q;
            SingularValueDecomposition(XY, out P, out Q);
            PrintMatrix(P);
            PrintMatrix(Q);
            Point[] T;
            double s;
            Point t;
            FindTransform(X, Y, out T, out s, out t);
            PrintMatrix(T);
            System.Console.WriteLine("s={0}", s);
            System.Console.WriteLine("t=({0},{1})", t.X,t.Y);
        }
        private static void FindTransform(Point[] X, Point[] Y,
            out Point[] T, out double s, out Point t) {
            int ni = X.Length;
            Point[] C = matrixProduct(X, Y);
            Point[] P, Q;
            SingularValueDecomposition(C, out P, out Q);
            T = matrixProductNoTranspose(Q, Transpose(P));
            Point[] XYT = matrixProductNoTranspose(C, T);
            Point[] Y2 = matrixProduct(Y, Y);
            s = XYT[0].X + XYT[1].Y;
            s /= Y2[0].X + Y2[1].Y;
            t = new Point();
            for (int i = 0; i < ni; ++i)
               t += X[i] - s * MatrixTimesVector(T, Y[i]);

            t /= ni;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Level {
            get {
                return 1;
            }
        }
        /// <summary>
        /// Get the list of nodes involved in the constraint
        /// </summary>
        public IEnumerable<Node> Nodes { get { return V.AsEnumerable(); } }
        #endregion
    }
}
