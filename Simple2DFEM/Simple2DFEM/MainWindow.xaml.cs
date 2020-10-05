using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Simple2DFEM
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private Triangular2DFEM fem;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += loadedEvent;
        }

        private void loadedEvent(object sender, RoutedEventArgs e)
        {
            DrawXYArrow();
        }

        private void Example1Clicked(object sender, RoutedEventArgs e)
        {
            // 初期化
            ClearClicked(null, null);

            double thickness = 1.0;
            double young = 210000;
            double poisson = 0.3;

            // 要素を作成する
            List<TriangularElement> elems = new List<TriangularElement>();
            {
                // 要素1
                {
                    Node[] nodes = new Node[3];
                    nodes[0].No = 1;
                    nodes[0].Point = new System.Windows.Vector(0.0, 0.0);
                    nodes[1].No = 2;
                    nodes[1].Point = new System.Windows.Vector(1.0, 0.0);
                    nodes[2].No = 5;
                    nodes[2].Point = new System.Windows.Vector(1.0, 1.0);
                    elems.Add(new TriangularElement(nodes, thickness, young, poisson));
                }
                // 要素2
                {
                    Node[] nodes = new Node[3];
                    nodes[0].No = 2;
                    nodes[0].Point = new System.Windows.Vector(1.0, 0.0);
                    nodes[1].No = 3;
                    nodes[1].Point = new System.Windows.Vector(2.0, 0.0);
                    nodes[2].No = 4;
                    nodes[2].Point = new System.Windows.Vector(2.0, 1.0);
                    elems.Add(new TriangularElement(nodes, thickness, young, poisson));
                }
                // 要素3
                {
                    Node[] nodes = new Node[3];
                    nodes[0].No = 2;
                    nodes[0].Point = new System.Windows.Vector(1.0, 0.0);
                    nodes[1].No = 4;
                    nodes[1].Point = new System.Windows.Vector(2.0, 1.0);
                    nodes[2].No = 5;
                    nodes[2].Point = new System.Windows.Vector(1.0, 1.0);
                    elems.Add(new TriangularElement(nodes, thickness, young, poisson));
                }
                // 要素4
                {
                    Node[] nodes = new Node[3];
                    nodes[0].No = 1;
                    nodes[0].Point = new System.Windows.Vector(0.0, 0.0);
                    nodes[1].No = 5;
                    nodes[1].Point = new System.Windows.Vector(1.0, 1.0);
                    nodes[2].No = 6;
                    nodes[2].Point = new System.Windows.Vector(0.0, 1.0);
                    elems.Add(new TriangularElement(nodes, thickness, young, poisson));
                }
            }

            // 境界条件を設定する
            // 変位
            List<double> disp = new List<double>();
            for (int i = 0; i < 12; i++)
            {
                disp.Add(0);
            }
            DenseVector DispVector = DenseVector.OfArray(disp.ToArray());
            // 荷重
            List<double> force = new List<double>();
            for (int i = 0; i < 12; i++)
            {
                force.Add(0);
            }
            force[7] = -10000;
            DenseVector ForceVector = DenseVector.OfArray(force.ToArray());
            // 拘束
            List<bool> Rest = new List<bool>();
            {
                for (int i = 0; i < 12; i++)
                {
                    Rest.Add(false);
                }
                Rest[0] = true;
                Rest[1] = true;
                Rest[10] = true;
            }
            fem = new Triangular2DFEM(6, elems);
            fem.setBoundaryCondition(DispVector, ForceVector, Rest);

            // モデルを描画する
            for (int i = 0; i < elems.Count; i++)
            {
                DrawElement(elems[i], Brushes.LightGreen, Brushes.Blue, 1.0);
            }

        }

        private void Example2Clicked(object sender, RoutedEventArgs e)
        {
            // 初期化
            ClearClicked(null, null);

            double thickness = 0.01;
            double young = 210000;
            double poisson = 0.0;

            // 要素を作成する
            List<TriangularElement> elems = new List<TriangularElement>();
            {
                // 節点を作成する
                Node[] nodes = new Node[55];
                {
                    for (int i = 0; i < 11; i++)
                    {
                        nodes[i].No = i + 1;
                        nodes[i].Point = new System.Windows.Vector(5.0 * i, 0.0);
                    }
                    for (int i = 11; i < 22; i++)
                    {
                        nodes[i].No = i + 1;
                        nodes[i].Point = new System.Windows.Vector(5.0 * (i - 11), 0.5);
                    }
                    for (int i = 22; i < 33; i++)
                    {
                        nodes[i].No = i + 1;
                        nodes[i].Point = new System.Windows.Vector(5.0 * (i - 22), 1.0);
                    }
                    for (int i = 33; i < 44; i++)
                    {
                        nodes[i].No = i + 1;
                        nodes[i].Point = new System.Windows.Vector(5.0 * (i - 33), 1.5);
                    }
                    for (int i = 44; i < 55; i++)
                    {
                        nodes[i].No = i + 1;
                        nodes[i].Point = new System.Windows.Vector(5.0 *(i - 44), 2.0);
                    }
                }

                // 要素を作成する
                // 1段目
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Node[] elemNodes = new Node[3];
                        elemNodes[0] = nodes[i];
                        elemNodes[1] = nodes[i + 1];
                        elemNodes[2] = nodes[i + 12];
                        elems.Add(new TriangularElement(elemNodes, thickness, young, poisson));
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        Node[] elemNodes = new Node[3];
                        elemNodes[0] = nodes[i];
                        elemNodes[1] = nodes[i + 12];
                        elemNodes[2] = nodes[i + 11];
                        elems.Add(new TriangularElement(elemNodes, thickness, young, poisson));
                    }
                }
                // 2段目
                {
                    for (int i =11; i < 21; i++)
                    {
                        Node[] elemNodes = new Node[3];
                        elemNodes[0] = nodes[i];
                        elemNodes[1] = nodes[i + 1];
                        elemNodes[2] = nodes[i + 12];
                        elems.Add(new TriangularElement(elemNodes, thickness, young, poisson));
                    }
                    for (int i = 11; i < 21; i++)
                    {
                        Node[] elemNodes = new Node[3];
                        elemNodes[0] = nodes[i];
                        elemNodes[1] = nodes[i + 12];
                        elemNodes[2] = nodes[i + 11];
                        elems.Add(new TriangularElement(elemNodes, thickness, young, poisson));
                    }
                }
                // 3段目
                {
                    for (int i = 22; i < 32; i++)
                    {
                        Node[] elemNodes = new Node[3];
                        elemNodes[0] = nodes[i];
                        elemNodes[1] = nodes[i + 1];
                        elemNodes[2] = nodes[i + 12];
                        elems.Add(new TriangularElement(elemNodes, thickness, young, poisson));
                    }
                    for (int i = 22; i < 32; i++)
                    {
                        Node[] elemNodes = new Node[3];
                        elemNodes[0] = nodes[i];
                        elemNodes[1] = nodes[i + 12];
                        elemNodes[2] = nodes[i + 11];
                        elems.Add(new TriangularElement(elemNodes, thickness, young, poisson));
                    }
                }
                // 4段目
                {
                    for (int i = 33; i < 43; i++)
                    {
                        Node[] elemNodes = new Node[3];
                        elemNodes[0] = nodes[i];
                        elemNodes[1] = nodes[i + 1];
                        elemNodes[2] = nodes[i + 12];
                        elems.Add(new TriangularElement(elemNodes, thickness, young, poisson));
                    }
                    for (int i = 33; i < 43; i++)
                    {
                        Node[] elemNodes = new Node[3];
                        elemNodes[0] = nodes[i];
                        elemNodes[1] = nodes[i + 12];
                        elemNodes[2] = nodes[i + 11];
                        elems.Add(new TriangularElement(elemNodes, thickness, young, poisson));
                    }
                }

                // 境界条件を設定する
                // 変位
                List<double> disp = new List<double>();
                for (int i = 0; i < nodes.Length * 2; i++)
                {
                    disp.Add(0);
                }
                DenseVector DispVector = DenseVector.OfArray(disp.ToArray());
                // 荷重
                List<double> force = new List<double>();
                for (int i = 0; i < nodes.Length * 2; i++)
                {
                    force.Add(0);
                }
                force[65] = -10;
                DenseVector ForceVector = DenseVector.OfArray(force.ToArray());
                // 拘束
                List<bool> Rest = new List<bool>();
                {
                    for (int i = 0; i < nodes.Length * 2; i++)
                    {
                        Rest.Add(false);
                    }
                    // 節点1
                    Rest[0] = true;
                    Rest[1] = true;
                    // 節点12
                    Rest[22] = true;
                    Rest[23] = true;
                    // 節点23
                    Rest[44] = true;
                    Rest[45] = true;
                    // 節点34
                    Rest[66] = true;
                    Rest[67] = true;
                    // 節点45
                    Rest[88] = true;
                    Rest[89] = true;
                }
                fem = new Triangular2DFEM(nodes.Length, elems);
                fem.setBoundaryCondition(DispVector, ForceVector, Rest);

                // モデルを描画する
                for (int i = 0; i < elems.Count; i++)
                {
                    DrawElement(elems[i], Brushes.LightGreen, Brushes.Blue, 1.0);
                }
            }
        }

        private void AnalysisClicked(object sender, RoutedEventArgs e)
        {
            // 例外処理
            if(fem == null)
            {
                return;
            }

            // FEM解析を実行する
            fem.Analysis();
            List<TriangularElement> elems = fem.TriElems;

            // モデルを描画する
            DenseVector dispElemVector = DenseVector.Create(6, 0.0);
            DenseVector dispVector = fem.DispVector;
            for (int i = 0; i < elems.Count; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    dispElemVector[2 * j] = dispVector[2 * (elems[i].Nodes[j].No - 1)];
                    dispElemVector[2 * j + 1] = dispVector[2 * (elems[i].Nodes[j].No - 1) + 1];
                }

                // 変形後の要素を計算する
                TriangularElement elem = elems[i].ShallowCopy();   // 変形前の要素をコピーする
                for (int j = 0; j < 3; j++)
                {
                    elem.Nodes[j].Point.X += dispElemVector[2 * j];
                    elem.Nodes[j].Point.Y += dispElemVector[2 * j + 1];
                }

                // 変形後の要素を描画する
                DrawElement(elem, Brushes.LightCoral, Brushes.Red, 0.5);
            }
        }

        private void ClearClicked(object sender, RoutedEventArgs e)
        {
            fem = null;
            this.Canvas.Children.Clear();
        }

        private void DrawXYArrow()
        {
            // X軸を描画する
            Line xArrow = new Line();
            xArrow.Stroke = Brushes.Red;
            xArrow.StrokeThickness = 4;
            xArrow.StrokeEndLineCap = PenLineCap.Triangle;
            xArrow.X1 = 0;
            xArrow.Y1 = 0;
            xArrow.X2 = 50;
            xArrow.Y2 = 0;
            this.Canvas.Children.Add(xArrow);

            // Y軸を描画する
            Line yArrow = new Line();
            yArrow.Stroke = Brushes.Blue;
            yArrow.StrokeThickness = 4;
            yArrow.StrokeEndLineCap = PenLineCap.Triangle;
            yArrow.X1 = 0;
            yArrow.Y1 = 0;
            yArrow.X2 = 0;
            yArrow.Y2 = 50;
            this.Canvas.Children.Add(yArrow);
        }
        
        private void DrawElement(TriangularElement elem, Brush elemcolor, Brush nodecolor, Double opacity)
        {
            // 要素を描画する
            Polygon polygon = new Polygon();
            polygon.Fill = elemcolor;
            polygon.Stroke = Brushes.Black;
            polygon.StrokeThickness = 1;
            polygon.Opacity = opacity;
            PointCollection Points = new PointCollection();
            for (int i = 0; i < 3; i++)
            {
                Point p = (Point)(elem.Nodes[i].Point * 15);
                p.X += 50;
                p.Y += 500;
                Points.Add(p);
            
            }
            polygon.Points = Points;
            this.Canvas.Children.Add(polygon);

            // 節点を描画する
            for (int i = 0; i < 3; i++)
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Fill = nodecolor;
                ellipse.Width = 4;
                ellipse.Height = 4;
                Canvas.SetLeft(ellipse, elem.Nodes[i].Point.X * 15 - ellipse.Width * 0.5 + 50);
                Canvas.SetTop(ellipse, elem.Nodes[i].Point.Y * 15 - ellipse.Height * 0.5 + 500);

                this.Canvas.Children.Add(ellipse);
            }
        }


    }
}
