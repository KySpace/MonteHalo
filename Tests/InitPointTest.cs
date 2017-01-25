using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteHalo.Tests
{
    public class InitPointTest
    {
        public static void Test1(Graphics.BasicDomePlot plot)
        {
            PhysicalModels.HexaPrism Crystal = new PhysicalModels.HexaPrism(10, 50, 25, 25, 25);
            GeoMath.UnitVector Sun = new GeoMath.UnitVector(25, 75, GeoMath.AngleUnit.Degree);
            GeoMath.CartCoor Point = new GeoMath.CartCoor();
            PhysicalModels.Surface Face = null;
            SharpDX.Vector3 Vector = new SharpDX.Vector3();
            for (int i = 0; i < 10000; i++)
            {
                Point = Crystal.GenInitPoint(Sun, out Face);
                Vector = new SharpDX.Vector3((float)Point.X, (float)Point.Y, (float)Point.Z);
                plot.UpdateVertexBuffer(new Graphics.ScatterVertex(Vector, SharpDX.Color4.White));
            }
        }
    }
}
