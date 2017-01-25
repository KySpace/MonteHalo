using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteHalo.Tests
{
    namespace AlgorithmTest
    {
        using GeoMath;
        using Graphics;
        public static class PolySampling
        {
            public static CartCoor Polysampling(List<CartCoor> points)
            {
                double RandomSum = 0;
                double RandomDouble = 0;
                CartCoor Result = new CartCoor(0, 0, 0);
                foreach(var point in points)
                {
                    RandomSum += RandomDouble = RandomNum.Random(0, 1);
                    //Result += RandomDouble * point;                    
                }
                //return (1 / RandomSum) * Result;
                return Result;
            }

            public static void Test1(Graphics.TestPointPlot plot)
            {
                CartCoor A = new CartCoor(-5, -5, 0);
                CartCoor B = new CartCoor(5, -5, 0);
                CartCoor C = new CartCoor(-5, 5, 0);
                CartCoor D = new CartCoor(5, 5, 0);
                List<CartCoor> points = new List<CartCoor> { A, B, C, D };
                for (int i = 0; i < 100000; i++)
                {
                    plot.UpdateVertexBuffer(
                        ScatterVertex.ToVertex(PolySampling.Polysampling(points))
                        );
                }
            }
        }
        public static class RandomDirection
        {
            public static void Test1(Graphics.BasicDomePlot plot)
            {
                GeoMath.UnitVector Init;
                GeoMath.CartCoor Point = new GeoMath.CartCoor();
                SharpDX.Vector3 Vector = new SharpDX.Vector3();
                GeoMath.Angle Alpha = new GeoMath.Angle(20, GeoMath.AngleUnit.Degree);
                GeoMath.Angle Beta = new GeoMath.Angle(50, GeoMath.AngleUnit.Degree);
                GeoMath.Angle Gamma = new GeoMath.Angle(70, GeoMath.AngleUnit.Degree);

                for (int i = 0; i < 10000; i++)
                {
                    Init = GeoMath.RandomNum.Direction();
                    Point = Init.ToVect;
                    Vector = new SharpDX.Vector3((float)Point.X, (float)Point.Y, (float)Point.Z);
                    plot.UpdateVertexBuffer(new Graphics.ScatterVertex(Vector, SharpDX.Color4.White));
                }
            }
        }
        public static class RandomGenerators
        {
            public static void TriangularRandom(Graphics.BasicDomePlot plot)
            {
                int[] Bars = new int[100];
                double Random;
                for (int i = 0; i < 100000; i++)
                {
                    Random = RandomNum.Triangular(50, 50);
                    Bars[(int)Math.Floor(Random)]++;
                }
                for (int i = 0; i < 100; i++)
                {
                    plot.UpdateVertexBuffer(
                        new ScatterVertex(new SharpDX.Vector3(100, i, Bars[i]),
                         SharpDX.Color4.White)
                         );
                }
            }
            
        }
    }
}
