using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple2DFEM
{
    class Triangular2DFEM
    {
        private int NodeNum = 0;   // 節点数
        public List<TriangularElement> TriElems   // 要素の集合
        {
            get; 
            private set;
        }

        public DenseVector DispVector   // 変位の境界条件
        {
            get;
            private set;
        }

        public DenseVector ForceVector   // 荷重の境界条件
        {
            get;
            private set;
        }

        public List<bool> Rest   // 拘束の境界条件
        {
            get;
            private set;
        }

        public Triangular2DFEM()
        {

        }

        public Triangular2DFEM(int nodenum, List<TriangularElement> trielems)
        {
            NodeNum = nodenum;
            TriElems = trielems;
        }

        // Kマトリックスを作成する
        private DenseMatrix makeKMatrix()
        {
            // 例外処理
            if(NodeNum <= 0 || TriElems == null || Rest.Count != NodeNum * 2)
            {
                return null;
            }

            DenseMatrix kMatrix = DenseMatrix.Create(NodeNum * 2, NodeNum * 2, 0.0);

            // 各要素のKeマトリックスを計算し、Kマトリックスに統合する
            for (int i = 0; i < TriElems.Count; i++)
            {
                Console.WriteLine("要素" + (i + 1).ToString());
                DenseMatrix keMatrix = TriElems[i].makeKeMatrix();

                for (int r = 0; r < 6; r++)
                {
                    int rt = (TriElems[i].Nodes[r / 2].No - 1) * 2 + r % 2;
                    for (int c = 0; c < 6; c++)
                    {
                        int ct = (TriElems[i].Nodes[c / 2].No - 1) * 2 + c % 2;
                        kMatrix[rt, ct] += keMatrix[r, c];
                    }
                }
            }

            Console.WriteLine("Kマトリックス");
            Console.WriteLine(kMatrix);

            // 境界条件を考慮して修正する
            ForceVector = ForceVector - kMatrix * DispVector;
            for (int i = 0; i < Rest.Count; i++)
            {
                if(Rest[i] == true)
                {
                    for (int j = 0; j < kMatrix.ColumnCount; j++)
                    {
                        kMatrix[i, j] = 0.0;
                    }
                    for (int k = 0; k < kMatrix.RowCount; k++)
                    {
                        kMatrix[k, i] = 0.0;
                    }
                    kMatrix[i, i] = 1.0;

                    ForceVector[i] = DispVector[i];
                }
            }

            Console.WriteLine("Kマトリックス(境界条件考慮)");
            Console.WriteLine(kMatrix);
            Console.WriteLine("荷重ベクトル(境界条件考慮)");
            Console.WriteLine(ForceVector);

            return kMatrix;
        }

        // 境界条件を設定する
        public void setBoundaryCondition(DenseVector dispvector, DenseVector forcevector, List<bool> rest)
        {
            DispVector = dispvector;
            ForceVector = forcevector;
            Rest = rest;
        }

        // 有限要素法を実行する
        public void Analysis()
        {
            DenseMatrix kMatrix = makeKMatrix();
            
            // 変位を計算する
            DispVector = (DenseVector)(kMatrix.Inverse().Multiply(ForceVector));
            Console.WriteLine("変位ベクトル");
            Console.WriteLine(DispVector);

            // 各要素の応力を計算する
            DenseVector dispElemVector = DenseVector.Create(6, 0.0);
            for (int i = 0; i < TriElems.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    dispElemVector[2 * j]     = DispVector[2 * (TriElems[i].Nodes[j].No - 1)];
                    dispElemVector[2 * j + 1] = DispVector[2 * (TriElems[i].Nodes[j].No - 1) + 1];
                }

                Console.WriteLine("要素" + (i + 1).ToString());
                TriElems[i].makeStrainVector(dispElemVector);
                TriElems[i].makeStressVector();
                //TriElems[i].makeEvaluate();
            }
        }
    }
}
