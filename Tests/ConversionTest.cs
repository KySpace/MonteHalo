using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteHalo.Tests
{
    public class ConversionTest
    {
        public static void Test1(Graphics.BasicDomePlot plot)
        {
            GeoMath.UnitVector Init, Rest;
            GeoMath.CartCoor Point = new GeoMath.CartCoor();
            SharpDX.Vector3 Vector = new SharpDX.Vector3();
            GeoMath.Angle Alpha = new GeoMath.Angle(20, GeoMath.AngleUnit.Degree);
            GeoMath.Angle Beta = new GeoMath.Angle(50, GeoMath.AngleUnit.Degree);
            GeoMath.Angle Gamma = new GeoMath.Angle(70, GeoMath.AngleUnit.Degree);

            for (int i = 0; i < 10000; i++)
            {
                Init = GeoMath.RandomNum.Direction();
                Point = Init.Convert(Alpha, Beta, Gamma).Revert(Alpha, Beta, Gamma).ToVect;
                Vector = new SharpDX.Vector3((float)Point.X, (float)Point.Y, (float)Point.Z);
                plot.UpdateVertexBuffer(new Graphics.ScatterVertex(Vector, SharpDX.Color4.White));
            }
        }
        public static void Test2(Graphics.BasicDomePlot plot)
        {
            GeoMath.UnitVector Init, Rest;
            GeoMath.CartCoor Point = new GeoMath.CartCoor();
            SharpDX.Vector3 Vector = new SharpDX.Vector3();
            GeoMath.Angle Alpha = new GeoMath.Angle(20, GeoMath.AngleUnit.Degree);
            GeoMath.Angle Beta = new GeoMath.Angle(50, GeoMath.AngleUnit.Degree);
            GeoMath.Angle Gamma = new GeoMath.Angle(70, GeoMath.AngleUnit.Degree);

            for (int i = 0; i < 10000; i++)
            {
                Init = GeoMath.RandomNum.Direction();
                Point = Init.Convert(Alpha, Beta, Gamma).ToVect;
                Vector = new SharpDX.Vector3((float)Point.X, (float)Point.Y, (float)Point.Z);
                plot.UpdateVertexBuffer(new Graphics.ScatterVertex(Vector, SharpDX.Color4.White));
            }
        }
        public static void Test3(Graphics.BasicDomePlot plot)
        {
            GeoMath.UnitVector Init, Rest;
            GeoMath.CartCoor Point = new GeoMath.CartCoor();
            SharpDX.Vector3 Vector = new SharpDX.Vector3();
            GeoMath.Angle Alpha = new GeoMath.Angle(20, GeoMath.AngleUnit.Degree);
            GeoMath.Angle Beta = new GeoMath.Angle(90, GeoMath.AngleUnit.Degree);
            GeoMath.Angle Gamma = new GeoMath.Angle(30, GeoMath.AngleUnit.Degree);

            for (int i = 0; i < 10000; i++)
            {
                Init = new GeoMath.UnitVector(0, 90, GeoMath.AngleUnit.Degree);
                Rest = Init.Convert(Alpha, Beta, Gamma);
                Rest = Rest.Revert(Alpha, Beta, Gamma);
                Vector = new SharpDX.Vector3((float)Point.X, (float)Point.Y, (float)Point.Z);
                plot.UpdateVertexBuffer(new Graphics.ScatterVertex(Vector, SharpDX.Color4.White));
            }
        }
    }
}
