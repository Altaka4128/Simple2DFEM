using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Simple2DFEM
{
    public struct Node 
    {
        public System.Windows.Vector Point;
        public int No;

    }

    public class TriangularElement
    {
        public Node[] Nodes
        {
            get;
            private set;
        }
        private double Thickness;
        private double Young;
        private double Poisson;
        private DenseMatrix KeMatrix;
        public DenseVector StrainVector
        {
            get;
            private set;
        }
        public DenseVector StressVector
        {
            get;
            private set;
        }

        public TriangularElement()
        {
        }

        public TriangularElement(
            Node[] nodes,
            double thickness,
            double young,
            double poisson)
        {

            if(nodes.Length != 3)
            {
                return;
            }

            Nodes = nodes;
            Thickness = thickness;
            Young = young;
            Poisson = poisson;

        }

        // Dマトリックスを計算する
        private DenseMatrix makeDMatirx()
        {
            // 例外処理
            if (Young == 0.0)
            {
                return null;
            }

            double coef = Young / ((1 - 2 * Poisson) * (1 + Poisson));
            double[,] dmatrixArray = new double[3,3];
            dmatrixArray[0, 0] = 1 - Poisson;
            dmatrixArray[0, 1] = Poisson;
            dmatrixArray[0, 2] = 0;
            dmatrixArray[1, 0] = Poisson;
            dmatrixArray[1, 1] = 1 - Poisson;
            dmatrixArray[1, 2] = 0;
            dmatrixArray[2, 0] = 0;
            dmatrixArray[2, 1] = 0;
            dmatrixArray[2, 2] = (1 - 2 * Poisson) / 2;

            return coef * DenseMatrix.OfArray(dmatrixArray);
        }

        // Bマトリックスを計算する
        private DenseMatrix makeBMatirx()
        {
            // 例外処理
            if(Thickness <= 0)
            {
                return null;
            }

            double AreaMult2 = Nodes[0].Point.X * Nodes[1].Point.Y - Nodes[0].Point.X * Nodes[2].Point.Y +
                               Nodes[1].Point.X * Nodes[2].Point.Y - Nodes[1].Point.X * Nodes[0].Point.Y +
                               Nodes[2].Point.X * Nodes[0].Point.Y - Nodes[2].Point.X * Nodes[1].Point.Y;
            double coef = 1 / AreaMult2;
            double[,] bmatrixArray = new double[3, 6];
            bmatrixArray[0, 0] = Nodes[1].Point.Y - Nodes[2].Point.Y;
            bmatrixArray[0, 1] = 0;
            bmatrixArray[0, 2] = Nodes[2].Point.Y - Nodes[0].Point.Y;
            bmatrixArray[0, 3] = 0;
            bmatrixArray[0, 4] = Nodes[0].Point.Y - Nodes[1].Point.Y;
            bmatrixArray[0, 5] = 0;
            bmatrixArray[1, 0] = 0;
            bmatrixArray[1, 1] = Nodes[2].Point.X - Nodes[1].Point.X;
            bmatrixArray[1, 2] = 0;
            bmatrixArray[1, 3] = Nodes[0].Point.X - Nodes[2].Point.X;
            bmatrixArray[1, 4] = 0;
            bmatrixArray[1, 5] = Nodes[1].Point.X - Nodes[0].Point.X;
            bmatrixArray[2, 0] = Nodes[2].Point.X - Nodes[1].Point.X;
            bmatrixArray[2, 1] = Nodes[1].Point.Y - Nodes[2].Point.Y;
            bmatrixArray[2, 2] = Nodes[0].Point.X - Nodes[2].Point.X;
            bmatrixArray[2, 3] = Nodes[2].Point.Y - Nodes[0].Point.Y;
            bmatrixArray[2, 4] = Nodes[1].Point.X - Nodes[0].Point.X;
            bmatrixArray[2, 5] = Nodes[0].Point.Y - Nodes[1].Point.Y;

            return coef * DenseMatrix.OfArray(bmatrixArray);
        }

        // Keマトリックスを計算する
        public DenseMatrix makeKeMatrix()
        {
            // Dマトリックスを計算する
            DenseMatrix DMatrix = makeDMatirx();
            Console.WriteLine("Dマトリックス");
            Console.WriteLine(DMatrix);

            // Bマトリックスを計算する
            DenseMatrix BMatrix = makeBMatirx();
            Console.WriteLine("Bマトリックス");
            Console.WriteLine(BMatrix);

            // 例外処理
            if (BMatrix == null || DMatrix == null)
            {
                return null;
            }

            double Area = 0.5 * (Nodes[0].Point.X * Nodes[1].Point.Y - Nodes[0].Point.X * Nodes[2].Point.Y +
                                 Nodes[1].Point.X * Nodes[2].Point.Y - Nodes[1].Point.X * Nodes[0].Point.Y +
                                 Nodes[2].Point.X * Nodes[0].Point.Y - Nodes[2].Point.X * Nodes[1].Point.Y);
            double coef = Thickness * Area;
            var keMatrix = coef * BMatrix.Transpose() * DMatrix.Transpose() * BMatrix;
            DenseMatrix KeMatrix = DenseMatrix.OfColumnArrays(keMatrix.ToColumnArrays());
            Console.WriteLine("Keマトリックス");
            Console.WriteLine(KeMatrix);

            return KeMatrix;
        }

        // ひずみベクトルを計算する
        public void makeStrainVector(DenseVector dispvector)
        {
            DenseMatrix bMatrix = makeBMatirx();
            StrainVector = (DenseVector)bMatrix.Multiply(dispvector);
            Console.WriteLine("ひずみベクトル");
            Console.WriteLine(StrainVector);
        }

        // 応力ベクトルを計算する
        public void makeStressVector()
        {
            if(StrainVector == null)
            {
                return;
            }
            
            DenseMatrix dMatrix = makeDMatirx();
            StressVector = (DenseVector)dMatrix.Multiply(StrainVector);
            Console.WriteLine("応力ベクトル");
            Console.WriteLine(StressVector);
        }

        // 要素の各応力値を計算する
        public void makeEvaluate()
        {
            //例外処理
            if(StressVector == null)
            {
                return;
            }

            // 主応力を計算する
            double[,] stressTensorArray = new double[2,2];
            stressTensorArray[0, 0] = StressVector[0];
            stressTensorArray[1, 1] = StressVector[1];
            stressTensorArray[0, 1] = StressVector[2];
            stressTensorArray[1, 0] = stressTensorArray[0, 1];
            DenseMatrix stressTensor = DenseMatrix.OfArray(stressTensorArray);
            var evd = stressTensor.Evd();
            var evdValue = evd.EigenValues;   // 固有値
            //Console.WriteLine("固有値");
            //Console.WriteLine(evdValue);
            //double maxStress = new double();
            //double minStress = new double();
        }

        public TriangularElement ShallowCopy()
        {
            return (TriangularElement)MemberwiseClone();
        }
    }
}
