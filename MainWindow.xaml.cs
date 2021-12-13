using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GrafikaKomputerowa4
{
    public partial class MainWindow : Window
    {
        private readonly DenseMatrix viewMatrix = DenseMatrix.OfArray(new float[,]
        {
            {  0.124f,  0.992f,  0.000f, -0.496f },
            { -0.122f,  0.015f,  0.992f, -0.504f },
            {  0.985f, -0.123f,  0.123f, -4.062f },
            { 0, 0, 0, 1 },
        });

        private float n = 1;
        private float f = 100;
        private float angle = 0;

        private DenseMatrix projectionMatrix;

        private DenseVector[] points;

        public MainWindow()
        {
            points = new DenseVector[16]
            {
                DenseVector.OfArray(new float[] { 1, 1, 0, 1}),
                DenseVector.OfArray(new float[] { -1, 1, 0, 1 }),

                DenseVector.OfArray(new float[] { -1, 1, 0, 1 }),
                DenseVector.OfArray(new float[] { -1, -1, 0, 1 }),

                DenseVector.OfArray(new float[] { -1, -1, 0, 1 }),
                DenseVector.OfArray(new float[] { 1, -1, 0, 1 }),

                DenseVector.OfArray(new float[] { 1, -1, 0, 1 }),
                DenseVector.OfArray(new float[] { 1, 1, 0, 1}),

                DenseVector.OfArray(new float[] { 1, 1, 0, 1}),
                DenseVector.OfArray(new float[] { 0, 0, 2, 1}),

                DenseVector.OfArray(new float[] { -1, 1, 0, 1 }),
                DenseVector.OfArray(new float[] { 0, 0, 2, 1}),

                DenseVector.OfArray(new float[] { -1, -1, 0, 1 }),
                DenseVector.OfArray(new float[] { 0, 0, 2, 1}),

                DenseVector.OfArray(new float[] { 1, -1, 0, 1 }),
                DenseVector.OfArray(new float[] { 0, 0, 2, 1}),
            };

            InitializeComponent();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += Animate;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dispatcherTimer.Start();
        }

        private void Render(DenseMatrix modelMatrix)
        {
            float width = (float)RenderCanvas.ActualWidth;
            float height = (float)RenderCanvas.ActualHeight;
            float e = 1 / (float)Math.Tan(FovSlider.Value.ToRadians() / 2);
            float a = height / width;

            projectionMatrix = DenseMatrix.OfArray(new float[,]
            {
                { e, 0, 0, 0 },
                { 0, e/a, 0, 0 },
                { 0, 0, -(f+n)/(f-n), -(2*f*n)/(f-n) },
                { 0, 0, -1, 0 },
            });

            for (var i = 0; i < points.Length; i += 2)
            {
                var p0 = points[i];
                var p1 = points[i + 1];

                var pp0 = projectionMatrix * viewMatrix * modelMatrix * p0;
                var pp1 = projectionMatrix * viewMatrix * modelMatrix * p1;

                var x0 = ((pp0[0] / pp0[3]) + 1.0f) / 2 * width;
                var y0 = ((pp0[1] / pp0[3]) + 1.0f) / 2 * height;
                y0 = height - y0;

                var x1 = ((pp1[0] / pp1[3]) + 1.0f) / 2 * width;
                var y1 = ((pp1[1] / pp1[3]) + 1.0f) / 2 * height;
                y1 = height - y1;

                var line = new Line();
                line.X1 = x0;
                line.Y1 = y0;
                line.X2 = x1;
                line.Y2 = y1;
                line.Stroke = Brushes.Black;
                RenderCanvas.Children.Add(line);
            }
        }

        private void Animate(object? sender, EventArgs e)
        {
            angle += (float)Math.PI / 40;

            DenseMatrix modelMatrix = DenseMatrix.CreateIdentity(4);
            modelMatrix[0, 0] = (float)Math.Cos(angle);
            modelMatrix[0, 1] = (float)-Math.Sin(angle);
            modelMatrix[1, 0] = (float)Math.Sin(angle);
            modelMatrix[1, 1] = (float)Math.Cos(angle);

            modelMatrix[0, 3] = 0.1f;
            modelMatrix[1, 3] = 0.2f;
            modelMatrix[2, 3] = 0.3f;

            RenderCanvas.Children.Clear();
            Render(modelMatrix);

            modelMatrix = DenseMatrix.CreateIdentity(4);
            modelMatrix[0, 0] = (float)Math.Cos(2 * angle);
            modelMatrix[0, 2] = (float)-Math.Sin(2 * angle);
            modelMatrix[2, 0] = (float)Math.Sin(2 * angle);
            modelMatrix[2, 2] = (float)Math.Cos(2 * angle);

            modelMatrix[0, 3] = -0.1f;
            modelMatrix[1, 3] = -0.2f;
            modelMatrix[2, 3] = -0.3f;

            Render(modelMatrix);
        }
    }

    public static class NumericExtensions
    {
        public static double ToRadians(this double val)
        {
            return Math.PI / 180 * val;
        }
    }
}