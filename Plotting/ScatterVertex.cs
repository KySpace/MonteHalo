using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteHalo.Graphics
{
    using SharpDX.Mathematics;
    public struct ScatterVertex
    {
        SharpDX.Vector3 Position;
        SharpDX.Color4 Color;
        public ScatterVertex(SharpDX.Vector3 _position, SharpDX.Color4 _color)
        {
            Position = _position;
            Color = _color;
        }
        ///// <summary>
        ///// This is the static method that convert a direction quantity from the computational part to the vertex
        ///// that can be read into a buffer
        ///// </summary>
        ///// <param name="_dir">Direction to be converted</param>
        ///// <param name="_wavelength">wavelength of the light ray</param>
        ///// <returns>The vertex</returns>
        //public static ScatterVertex ToVertex(MonteHalo.Quantities.Direction _dir, double _wavelength)
        //{
        //    return new ScatterVertex(
        //        new SharpDX.Vector3(
        //            (float)(Math.Cos(_dir.Phi.Radian) * Math.Sin(_dir.Theta.Radian)),
        //            (float)(Math.Sin(_dir.Phi.Radian) * Math.Sin(_dir.Theta.Radian)),
        //            (float)(Math.Cos(_dir.Theta.Radian))
        //            ),
        //        ScatterVertex.ToColor(_wavelength)
        //        );
        //}
        /// <summary>
        /// This is the static method that convert the wavelength to corresponding color in float-4
        /// </summary>
        /// <param name="_wavelength">The wavelength to be converted</param>
        /// <returns>A float-4 color</returns>
        public static SharpDX.Color4 ToColor(double _wavelength)
        {
            return new SharpDX.Color4(new float[] { 1.0f, 1.0f, 1.0f, 1.0f });//To be continued
        }
        public static ScatterVertex ToVertex(MonteHalo.GeoMath.CartCoor _coor)
        {
            return new ScatterVertex(new SharpDX.Vector3(
                (float)_coor.X,
                (float)_coor.Y,
                (float)_coor.Z),
                ToColor(0)
                );
        }
    }
    public struct ScatterVertex2
    {
        SharpDX.Vector4 Position;
        SharpDX.Color4 Color;
        public ScatterVertex2(SharpDX.Vector3 _position, SharpDX.Color4 _color)
        {
            Position = new SharpDX.Vector4(_position, 1.0f);
            Color = _color;
        }
        //public static ScatterVertex2 ToVertex(MonteHalo.Quantities.Direction _dir, double _wavelength)
        //{
        //    return new ScatterVertex2(
        //        new SharpDX.Vector3(
        //            (float)(Math.Cos(_dir.Phi.Radian) * Math.Sin(_dir.Theta.Radian)),
        //            (float)(Math.Sin(_dir.Phi.Radian) * Math.Sin(_dir.Theta.Radian)),
        //            (float)(Math.Sin(_dir.Phi.Radian))                    
        //            ),
        //        ScatterVertex.ToColor(_wavelength)
        //        );
        //}
        public static SharpDX.Color4 ToColor(double _wavelength)
        {
            return new SharpDX.Color4();//To be continued
        }
    }
}
