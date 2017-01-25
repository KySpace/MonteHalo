using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MonteHalo
{
    namespace PhysicalModels
    {
        using GeoMath;
        /// <summary>
        /// Orientation of a hexagonal prism crystal.
        /// </summary>
        public enum OrientationRequirement { Random, Plate, Parry, Lowitz, Column };
        /// <summary>
        /// Environment of the current beam, inside or outside of the crystal.
        /// </summary>
        public enum BeamEnvironment { Inside, Outside };
        /// <summary>
        /// Polarization of the beam, choosing s component or p component.
        /// </summary>
        public enum PolarizationComponent { s, p };

        /// <summary>
        /// Base class for any kind of crystal.
        /// </summary>
        public abstract partial class Crystal
        {
            /// <summary>
            /// Refraction index.
            /// </summary>
            private double refrexIndex;
            public double RefrexIndex
            {
                get
                {
                    return this.refrexIndex;
                }
                set
                {
                    if (value > 0)
                        refrexIndex = value;
                    //Incomplete exception handling
                    else refrexIndex = double.NaN;
                }
            }


            public abstract Surface Surface(int num);
        }
        /// <summary>
        /// A hexagonal prism crystal.
        /// </summary>
        public partial class HexaPrism : Crystal
        {
            public double Scale { get; protected set; }

            //These are the sides of a hexagonal prism crystal.
            public Surface Surface1 { get; set; }
            public Surface Surface2 { get; set; }
            public Surface Surface3 { get; set; }
            public Surface Surface4 { get; set; }
            public Surface Surface5 { get; set; }
            public Surface Surface6 { get; set; }
            public Surface Surface7 { get; set; }
            public Surface Surface8 { get; set; }
            /// <summary>
            /// To return a specific side surface.
            /// </summary>
            /// <param name="num">
            /// Number of the side.
            /// 1 for bottom, 2 for lid, 3 to 8 are laterals.
            /// 3 is at the x positive direction.
            /// Number grows in CCW order looking from above.
            /// </param>
            /// <returns>The desired surface</returns>
            public override Surface Surface(int num)
            {
                switch (num)
                {
                    case 1: return Surface1;
                    case 2: return Surface2;
                    case 3: return Surface3;
                    case 4: return Surface4;
                    case 5: return Surface5;
                    case 6: return Surface6;
                    case 7: return Surface7;
                    case 8: return Surface8;
                    //Incomplete exception handling
                    default: return null;
                }
            }
            //Vertexes of the prism
            private CartCoor A1 { get; set; }
            private CartCoor B1 { get; set; }
            private CartCoor C1 { get; set; }
            private CartCoor D1 { get; set; }
            private CartCoor E1 { get; set; }
            private CartCoor F1 { get; set; }
            private CartCoor A2 { get; set; }
            private CartCoor B2 { get; set; }
            private CartCoor C2 { get; set; }
            private CartCoor D2 { get; set; }
            private CartCoor E2 { get; set; }
            private CartCoor F2 { get; set; }

            /// <summary>
            /// To construct a hexagonal prism.
            /// </summary>
            /// <param name="height">Height of the prism</param>
            /// <param name="sizeMax">A smallest triangle that contains the bottom or top surface</param>
            /// <param name="rad1">Distance of the origin point to face 4</param>
            /// <param name="rad2">Distance of the origin point to face 6</param>
            /// <param name="rad3">Distance of the origin point to face 8</param>
            public HexaPrism(double height, double sizeMax, double rad1, double rad2, double rad3)
            {
                this.RefrexIndex = 1.31;
                //To make rad2 smaller than the other two, for it would be more convenient to create a more effecient sampling.
                if (rad1 <= rad2) Utilities.Swap(ref rad1, ref rad2);
                if (rad3 <= rad2) Utilities.Swap(ref rad2, ref rad3);
                //Test if the initials meet the requirement for building a crystal
                //TO BE CONTINUED

                //Building the crystal
                //Here we are using degree as the unit for clarity.
                //Note that all the normal vectors should be pointed inward for standardization.
                Surface1 = new Surface(new CartCoor(0, 0, -height / 2), new UnitVector(0, 0, AngleUnit.Degree), 1);
                Surface2 = new Surface(new CartCoor(0, 0, height / 2), new UnitVector(0, 180, AngleUnit.Degree), 2);
                //Surface 3 is the surface in the x positice direction.
                //SizeMax is the maximum radius of the minimum triangle that covers the bottom surface, 
                //which has a side(on x positive direction) that is parallel to y axis
                //The number of the side surfaces are marked CCW seen from above
                Surface3 = new Surface(new CartCoor(sizeMax / 2, 0, 0), new UnitVector(180, 90, AngleUnit.Degree), 3);
                Surface4 = new Surface(new CartCoor(rad1 / 2, rad1 / 2 * Math.Sqrt(3), 0), new UnitVector(240, 90, AngleUnit.Degree), 4);
                Surface5 = new Surface(new CartCoor(-sizeMax / 4, sizeMax / 4 * Math.Sqrt(3), 0), new UnitVector(300, 90, AngleUnit.Degree), 5);
                Surface6 = new Surface(new CartCoor(-rad2, 0, 0), new UnitVector(0, 90, AngleUnit.Degree), 6);
                Surface7 = new Surface(new CartCoor(-sizeMax / 4, -sizeMax / 4 * Math.Sqrt(3), 0), new UnitVector(60, 90, AngleUnit.Degree), 7);
                Surface8 = new Surface(new CartCoor(rad3 / 2, -rad3 / 2 * Math.Sqrt(3), 0), new UnitVector(120, 90, AngleUnit.Degree), 8);

                //Center points of each lateral edges of the prism
                A1 = (Surface3 * Surface4) * Surface1;
                B1 = (Surface4 * Surface5) * Surface1;
                C1 = (Surface5 * Surface6) * Surface1;
                D1 = (Surface6 * Surface7) * Surface1;
                E1 = (Surface7 * Surface8) * Surface1;
                F1 = (Surface8 * Surface3) * Surface1;
                A2 = (Surface3 * Surface4) * Surface2;
                B2 = (Surface4 * Surface5) * Surface2;
                C2 = (Surface5 * Surface6) * Surface2;
                D2 = (Surface6 * Surface7) * Surface2;
                E2 = (Surface7 * Surface8) * Surface2;
                F2 = (Surface8 * Surface3) * Surface2;
                //Calculation of the surface areas
                Surface1.SurfaceArea = 3.0 / 4 * Math.Sqrt(3) * sizeMax * sizeMax
                    - Math.Sqrt(3) / 3 * (sizeMax - rad1) * (sizeMax - rad1)
                    - Math.Sqrt(3) / 3 * (sizeMax - rad2) * (sizeMax - rad2)
                    - Math.Sqrt(3) / 3 * (sizeMax - rad3) * (sizeMax - rad3);
                Surface2.SurfaceArea = Surface1.SurfaceArea;
                Surface3.SurfaceArea = height * ((A1 - F1).Modulus());
                Surface4.SurfaceArea = height * ((B1 - A1).Modulus());
                Surface5.SurfaceArea = height * ((C1 - B1).Modulus());
                Surface6.SurfaceArea = height * ((D1 - C1).Modulus());
                Surface7.SurfaceArea = height * ((E1 - D1).Modulus());
                Surface8.SurfaceArea = height * ((F1 - E1).Modulus());

                this.Scale = Math.Sqrt(Math.Pow(sizeMax, 2) + Math.Pow(height / 2, 2));
                //To examine if the crystal is legally constructed.
                //Incomplete exception handling
                if (A1.Y - F1.Y < 0 || B1.Y - C1.Y < 0 || D1.Y - E1.Y < 0)
                    ;
            }
        }
        //public class Pyramidal : Crystal { }
        /// <summary>
        /// Representing a side surface of a crystal.
        /// </summary>
        public class Surface : Plane
        {                              
            /// <summary>
            /// Number of the surface on the crystal.
            /// </summary>                                         
            public int SurfaceNum { get; set; }
            /// <summary>
            /// Area of the surface.
            /// </summary>
            public double SurfaceArea { get; set; }
            /// <summary>
            /// To find the intersection point of this surface and a given light beam.
            /// </summary>
            /// <param name="beam"></param>
            /// <param name="intersection"></param>
            /// <returns></returns>
            public double FindIntersect(ref Beam beam, out CartCoor intersection)
            {
                double CosAngle = beam.Direct & this.Normal;
                //Test whether the line and the plane are parallel, i.e., 
                //if line and normal vector are perpendicular.
                if (CosAngle == 0)
                {
                    intersection = null;
                    return double.PositiveInfinity;
                }
                else
                {
                    //distance between the point on the beam and the intersection
                    double distance = -((beam.Point - this.Point) & this.Normal.ToVect) / CosAngle;
                    intersection = new CartCoor(beam.Point + (distance * beam.Direct));
                    return Math.Abs(distance);
                }
            }
            /// <summary>
            /// To construct the surface with given point, normal vector and surface number.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="normal"></param>
            /// <param name="num"></param>
            public Surface(CartCoor point, UnitVector normal, int num)
            {
                SurfaceNum = num;
                Point = point;
                Normal = normal;
                //Surface area should be assigned later
                SurfaceArea = double.PositiveInfinity;
            }
        }
        public partial class Beam : Line
        {
            
            public UnitVector Pol { get; set; }
            
            //To indicate whether the beam is inside or outside of the crystal
            public BeamEnvironment Env { get; set; }
            //To count the refraction and reflection time of this process
            public int ReflectionCount { get; set; }
            public int RefractionCount { get; set; }
            //Initialization of the beam at the very beginning
            public Beam(CartCoor point, UnitVector direction)
            {
                this.Point = point;
                this.Direct = direction;
                this.Pol = direction.GenPerp();
                this.Env = BeamEnvironment.Outside;
                this.ReflectionCount = RefractionCount = 0;
            }
            
        }
    }
    namespace GeoMath
    {
        public enum AngleUnit { Radian, Degree };


        // Angles, Directions, and various sets of coordinates in different coordinate systems.
        // Including their mutual convertions

        /// <summary>
        /// Representing an angle which can be accessed through various of ways.
        /// The only fields of the class is sin and cos values, which is also the
        /// most frquent access.
        /// </summary>
        public class Angle
        {
            /// <summary>
            /// Sin and Cos value should have a square sum of 1.
            /// This is the allowed error whenever this relationship is being checked.
            /// </summary>
            private static double error = 0.000000001;
            /// <summary>
            /// Cosine value of the angle.
            /// </summary>
            private double cos;
            /// <summary>
            /// Sine value of the angle.
            /// </summary>
            private double sin;
            /// <summary>
            /// Cosine value of the angle.
            /// It can only be set privately.
            /// </summary>
            public double Cos
            {
                get
                {
                    return this.cos;
                }
                private set
                {
                    //Incomplete exception handling
                    if (value > 1) { this.sin = this.cos = double.NaN; }
                    else this.cos = value;
                }
            }
            /// <summary>
            /// Sine value of the angle.
            /// It can only be set privately.
            /// </summary>
            public double Sin
            {
                get
                {
                    return this.sin;
                }
                private set
                {
                    //Incomplete exception handling
                    if (value > 1) { this.sin = this.cos = double.NaN; }
                    else this.sin = value;
                }
            }
            /// <summary>
            /// Tangent accessor of the angle.
            /// </summary>
            public double Tan { get { return this.Sin / this.Cos; } }
            /// <summary>
            /// Cotangent accessor of the angle.
            /// </summary>
            public double Cot { get { return this.Cos / this.Sin; } }

            /// <summary>
            /// To convert the current angle to an acute angle.
            /// </summary>
            public void ToAcute()
            {
                if (this.Cos < 0)
                    this.Cos = -this.Cos;
            }
            /// <summary>
            /// The radian accessor. Avoid using this when cosine or sine is needed.
            /// Range from -Pi to Pi.
            /// </summary>
            public double Radian
            {
                get { return this.sin > 0 ? Math.Acos(this.cos) : -Math.Acos(this.cos); }
                private set { this.sin = Math.Sin(value); this.cos = Math.Cos(value); }
            }
            /// <summary>
            /// The degree accessor. Avoid using this when cosine or sine is needed.
            /// Range from -180 to 180.
            /// </summary>
            public double Degree
            {
                get { return this.Radian / Math.PI * 180; }
                private set { this.Radian = value * Math.PI / 180; }
            }
            /// <summary>
            /// Constructor by angle and unit.
            /// </summary>
            /// <param name="value">value</param>
            /// <param name="unit">unit</param>
            public Angle(double value, AngleUnit unit)
            {

                if (unit == AngleUnit.Radian)
                {
                    this.Radian = value;
                }
                else if (unit == AngleUnit.Degree)
                {
                    this.Degree = value;
                }
                //Incomplete error handling!
                else this.cos = this.sin = double.NaN;
            }
            /// <summary>
            /// Constructor by value in radians .
            /// </summary>
            /// <param name="value">radian value</param>
            public Angle(double value)
            {
                this.Radian = value;
            }
            /// <summary>
            /// To create a new instance by copying the original.
            /// </summary>
            /// <param name="angle"></param>
            public Angle(Angle angle)
            {
                this.Cos = angle.Cos;
                this.Sin = angle.Sin;
            }
            /// <summary>
            /// Constructor by sin and cos value.
            /// </summary>
            /// <param name="cos"></param>
            /// <param name="sin"></param>
            public Angle(double cos, double sin)
            {
                //Incomplete exception handling
                if (Math.Abs(cos * cos + sin * sin - 1) > Angle.error)
                    ;
                this.Cos = cos;
                this.Sin = sin;
            }
            /// <summary>
            /// Construct an angle with only cosine value, and whether it is larger than zero
            /// </summary>
            /// <param name="cos"></param>
            /// <param name="smallerThanPi">whether it is larger than 0</param>
            public Angle(double cos, bool region)
            {
                this.Cos = cos;
                this.Sin = region ?
                    Math.Sqrt(1 - cos * cos) :
                    -Math.Sqrt(1 - cos * cos);
            }

            /// <summary>
            /// To find the angle that has a sum of 0 with current angle.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static Angle operator -(Angle value)
            {
                return new Angle(value.Cos, -value.Sin);
            }
        }
        ///// <summary>
        ///// Direction class, representing a direction in the current frame
        ///// </summary>
        //public class Direction
        //{
        //    /// <summary>
        //    /// Longitude of the direction.
        //    /// </summary>
        //    private Angle phi;
        //    /// <summary>
        //    /// Longitude property.
        //    /// </summary>            
        //    public Angle Phi
        //    {
        //        //Getter, examines the range of the value.
        //        get
        //        {
        //            //Incomplete error handling!
        //            if (this.phi.Radian >= 2 * Math.PI || this.phi.Radian < 0)
        //                return null;
        //            else return this.phi;
        //        }
        //        //Setter, confine the value within [0,2Pi)
        //        set
        //        {
        //            this.phi = value;
        //            while (this.phi.Radian >= 2 * Math.PI)
        //            {
        //                this.phi.Radian -= 2 * Math.PI;
        //            }
        //            while (this.phi.Radian < 0)
        //            {
        //                this.phi.Radian += 2 * Math.PI;
        //            }
        //        }
        //    }
        //    /// <summary>
        //    /// Latitude of the direction.
        //    /// </summary>
        //    private Angle theta;
        //    /// <summary>
        //    /// Latitude property. Always assign to phi before theta.
        //    /// </summary>            
        //    public Angle Theta
        //    {
        //        //Getter, examines the range of the value.
        //        get
        //        {
        //            //Incomplete error handling!
        //            if (this.theta.Radian > Math.PI || this.theta.Radian < 0)
        //                return null;
        //            else return this.theta;
        //        }
        //        //Setter, confine the value within [0,Pi]
        //        set
        //        {
        //            this.theta = value;
        //            //Confine the value within [0,2Pi)
        //            while (this.theta.Radian >= 2 * Math.PI)
        //            {
        //                this.theta.Radian -= 2 * Math.PI;
        //            }
        //            while (this.theta.Radian < 0)
        //            {
        //                this.theta.Radian += 2 * Math.PI;
        //            }
        //            //Confine the value within [0,Pi]
        //            //Convert phi to its opposite direction if theta is not within [0,Pi]
        //            if (this.theta.Radian > Math.PI)
        //            {
        //                this.theta.Radian = 2 * Math.PI - this.theta.Radian;
        //                this.Phi.Radian = this.Phi.Radian + Math.PI;
        //            }
        //        }
        //    }

        //    /// <summary>
        //    /// To construct by longitude and latitude angles values,
        //    /// supporting unit selection.
        //    /// </summary>
        //    /// <param name="phi">longitude value</param>
        //    /// <param name="theta">latitude value</param>
        //    /// <param name="unit">unit of angle</param>
        //    public Direction(double phi, double theta, AngleUnit unit)
        //    {
        //        //Pay attention to the order here, phi must be initialized first,
        //        //for its value should be reconsidered according to theta
        //        this.Phi = new Angle(phi, unit);
        //        this.Theta = new Angle(theta, unit);
        //    }
        //    /// <summary>
        //    /// To construct by longitude and latitude angles type objects.
        //    /// </summary>
        //    /// <param name="phi">longitude angle</param>
        //    /// <param name="theta">latitude angle</param>
        //    public Direction(Angle phi, Angle theta)
        //    {
        //        //Pay attention to the order here, phi must be initialized first,
        //        //for its value should be reconsidered according to theta
        //        this.Phi = phi;
        //        this.Theta = theta;
        //    }
        //    /// <summary>
        //    /// Default constructor.
        //    /// </summary>
        //    public Direction() { }
        //    /// <summary>
        //    /// Instancing a new direction by copying the given one.
        //    /// </summary>
        //    /// <param name="dir"></param>
        //    public Direction(Direction direction)
        //    {
        //        this.Phi = direction.Phi;
        //        this.Theta = direction.Theta;
        //    }

        //    //Conversion between coordinate systems.
        //    /// <summary>
        //    /// To yield this direction(global coordinate) in local coordinate which has Euler angles
        //    /// alpha beta and gamma.
        //    /// </summary>
        //    /// <param name="alpha">Euler angle alpha</param>
        //    /// <param name="beta">Euler angle beta</param>
        //    /// <param name="gamma">Euler angle gamma</param>
        //    /// <returns>direction in local coordinate</returns>
        //    public Direction Convert(Angle alpha, Angle beta, Angle gamma)
        //    {
        //        if (this == null) return null;
        //        else
        //        {
        //            double Phi1 = this.Phi.Radian - alpha.Radian;
        //            double Theta1 = this.Theta.Radian;
        //            double Phi2;
        //            double Theta2 = Math.Acos(-Math.Sin(Phi1) * Math.Sin(Theta1) * Math.Sin(beta.Radian)
        //                + Math.Cos(Theta1) * Math.Cos(beta.Radian));
        //            if (Math.Sin(Theta2) == 0)
        //                Phi2 = 0;
        //            else if (Math.Sin(Phi1) * Math.Sin(Theta1) * Math.Cos(beta.Radian) + Math.Cos(Theta1) * Math.Sin(beta.Radian) >= 0)
        //                Phi2 = Math.Acos(Math.Cos(Phi1) * Math.Sin(Theta1) / Math.Sin(Theta2));
        //            else
        //                Phi2 = 2 * Math.PI - Math.Acos(Math.Cos(Phi1) * Math.Sin(Theta1) / Math.Sin(Theta2));
        //            return new Direction(new Angle(Phi2 - gamma.Radian), new Angle(Theta2));
        //        }
        //    }
        //    /// <summary>
        //    /// To yield this direction(local coordinate) in global coordinate which has Euler angles
        //    /// alpha beta and gamma.
        //    /// </summary>
        //    /// <param name="alpha">Euler angle alpha</param>
        //    /// <param name="beta">Euler angle beta</param>
        //    /// <param name="gamma">Euler angle gamma</param>
        //    /// <returns>direction in global coordinate</returns>
        //    public Direction Revert(Angle alpha, Angle beta, Angle gamma)
        //    {
        //        if (this == null) return null;
        //        else return this.Convert(new Angle(-gamma.Radian), new Angle(-beta.Radian), new Angle(-alpha.Radian));
        //    }
        //    /// <summary>
        //    /// To find the opposite direction of current direction.
        //    /// </summary>
        //    /// <returns>The opposite direction of this direction</returns>
        //    public Direction Invert()
        //    {
        //        //This is a quick method for avoiding extra codes, the correction for angles have been automatically
        //        //done by Theta setter
        //        return new Direction(Phi.Radian, Theta.Radian + Math.PI, AngleUnit.Radian);
        //    }
        //    /// <summary>
        //    /// To construct a vector based on its direction and module.
        //    /// </summary>
        //    /// <param name="mod">the desired module of the vector</param>
        //    /// <returns>the created vector</returns>
        //    public Vector ToVect(double mod)
        //    {
        //        SpherCoor SphereCoor = new SpherCoor(mod, this);
        //        return SphereCoor.ToVect();
        //    }
        //    /// <summary>
        //    /// To generate a random direction with probability uniformly distributed 
        //    /// on all directions
        //    /// </summary>
        //    /// <returns></returns>
        //    public Direction GenRandomDir()
        //    {
        //        return new Direction(RandomNum.Random(0, 360), RandomNum.Random(0, 180), AngleUnit.Degree);
        //    }
        //    /// <summary>
        //    /// To generate a polarization that is perpendicular to current direction.
        //    /// </summary>
        //    /// <returns>polarization generated</returns>
        //    /// <remarks>
        //    /// The polarization should be randomly generated with equal possibility towards 
        //    /// all perpendicular directions
        //    /// </remarks>
        //    public Polarization GenPerpPol()
        //    {
        //        Vector Result;
        //        do
        //        {
        //            Vector Auxiliary = RandomNum.Cube(new CartCoor(-1, -1, -1), new CartCoor(1, 1, 1)) as Vector;
        //            Result = Auxiliary * this.ToVect(1);
        //        }
        //        while (Result.Modulus() > 0);
        //        return new Polarization(Result.ToSphere().Angles);
        //    }
        //    /// <summary>
        //    /// To find the direction that is perpendicular to the given directions.
        //    /// </summary>
        //    /// <param name="left"></param>
        //    /// <param name="right"></param>
        //    /// <returns></returns>
        //    public static Direction operator *(Direction left, Direction right)
        //    {
        //        return (left.ToVect(1) * right.ToVect(1)).Direct();
        //    }
        //    /// <summary>
        //    /// To find the angle between to directions.
        //    /// </summary>
        //    /// <param name="left"></param>
        //    /// <param name="right"></param>
        //    /// <returns></returns>
        //    public static Angle operator -(Direction left, Direction right)
        //    {
        //        double Angle = Math.Acos((left.ToVect(1) & right.ToVect(1)));
        //        if (Angle >= Math.PI / 2)
        //            Angle = Math.PI - Angle;
        //        return new Angle(Angle, AngleUnit.Radian);
        //    }

        //    /// <summary>
        //    /// To convert the direction to string.
        //    /// </summary>
        //    /// <returns></returns>
        //    public override string ToString()
        //    {
        //        return string.Format("{0:F5}\t\t{1:F5}\r\n", Phi.Radian.ToString(), Theta.Degree.ToString());
        //    }

        //}
        ///// <summary>
        ///// This class represents a polarization direction.
        ///// </summary>
        ///// <remarks>
        ///// This really is only another saying of Direction.
        ///// </remarks>
        //public class Polarization : Direction
        //{
        //    public Polarization(Direction direction) : base(direction) { }
        //    public Polarization(Angle phi, Angle theta) : base(phi, theta) { }
        //    public Polarization(double phi, double theta, AngleUnit unit) : base(phi, theta, unit) { }
        //    public Polarization(Polarization polarization) : base(polarization as Direction) { }
        //}
        /// <summary>
        /// Representing a polar coordinate of a point in plane,
        /// or the polar coordinate of the point's projection on x-y plane
        /// </summary>
        public class PolarCoor
        {
            /// <summary>
            /// Distance to the origin point of the coordinate system.
            /// </summary>
            protected double radius;
            /// <summary>
            /// Radius property, larger than zero.
            /// </summary>
            public double Radius
            {
                get { return this.radius; }
                set
                {
                    //Incomplete exception handling
                    if (value < 0)
                        this.radius = 0;
                    else radius = value;
                }
            }
            /// <summary>
            /// Distance to z axis property.
            /// </summary>
            public double Rho
            {
                get
                {
                    return this.Radius * this.angles.CosPhi;
                }
            }
            /// <summary>
            /// Longitude and Latitude angles stored in a Direction field.
            /// </summary>
            protected UnitVector angles;
            /// <summary>
            /// Longitude angle of the point, whether in 2D or 3D space.
            /// Do not set this property in derived class!
            /// </summary>
            public Angle Phi
            {
                get
                {
                    if (angles.CosTheta == 0)
                        return angles.Phi;
                    //Incomplete exception handling
                    else return null;
                }
                set
                {
                    this.angles = new UnitVector(new Angle(value.Cos, value.Sin), new Angle(0, true));
                }
            }

            /// <summary>
            /// To construct by radius and phi, supporting unit selection.
            /// </summary>
            /// <param name="radius">distance to the origin</param>
            /// <param name="phi">angle value towards x axis</param>
            /// <param name="unit">unit of the angle</param>
            public PolarCoor(double radius, double phi, AngleUnit unit)
            {
                phi = unit == AngleUnit.Radian ? phi : phi * Math.PI / 180;
                this.Radius = radius;
                this.angles = new UnitVector(phi, Math.PI, AngleUnit.Radian);
            }
            /// <summary>
            /// To construct by radius and phi.
            /// </summary>
            /// <param name="radius"></param>
            /// <param name="phi"></param>
            public PolarCoor(double radius, Angle phi)
            {
                this.Radius = radius;
                this.Phi = phi;
            }
            /// <summary>
            /// Default constructor.
            /// </summary>
            public PolarCoor() { }

            /// <summary>
            /// To convert Polar coordinates to Cartesian coordinate
            /// </summary>
            /// <returns>the cartesian plane coordinate</returns>
            public PlaneCoor ToPlane()
            {
                return new PlaneCoor(this.Radius * this.Phi.Cos, this.Phi.Sin);
            }

        }
        /// <summary>
        /// Spherical coordinate class
        /// </summary>
        public class SpherCoor : PolarCoor
        {
            /// <summary>
            /// Full access to the parent angles field.
            /// </summary>
            public UnitVector Angles { get { return this.angles; } set { this.angles = value; } }


            /// <summary>
            /// To construct by radius and angle values, supporting unit selecting.
            /// </summary>
            /// <param name="radius">distance to the origin</param>
            /// <param name="phi">distance to the origin value</param>
            /// <param name="theta">lattitude angle value</param>
            /// <param name="unit">unit of the angles</param>
            public SpherCoor(double radius, double phi, double theta, AngleUnit unit)
            {
                this.Radius = radius;
                this.Angles = new UnitVector(phi, theta, unit);
            }
            /// <summary>
            /// To construct by radius and angles
            /// </summary>
            /// <param name="radius">distance to the origin</param>
            /// <param name="phi">distance to the origin</param>
            /// <param name="theta">lattitude angle</param>
            public SpherCoor(double radius, Angle phi, Angle theta)
            {
                this.Radius = radius;
                this.Angles = new UnitVector(phi, theta);
            }
            /// <summary>
            /// To construct by radius and angles direction as a whole
            /// </summary>
            /// <param name="radius">the distance to origin</param>
            /// <param name="angles">angles</param>
            public SpherCoor(double radius, UnitVector angles)
            {
                this.Radius = radius;
                this.Angles = angles;
            }
            /// <summary>
            /// Default constructor.
            /// </summary>
            public SpherCoor() { }

            /// <summary>
            /// To convert the spherical coordinates to Cartesian coordinates.
            /// </summary>
            /// <returns>the cartesian coordinate</returns>
            public CartCoor ToCart()
            {
                return this.Radius * this.Angles as CartCoor;
            }

        }
        /// <summary>
        /// Plane coordinate class, representing a cartesian plane coordinate
        /// </summary>
        public class PlaneCoor
        {
            public double X { get; set; }
            public double Y { get; set; }

            /// <summary>
            /// To construct by x and y values
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            public PlaneCoor(double x, double y)
            {
                X = x;
                Y = y;
            }

            /// <summary>
            /// The overriden plus operator.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns>Sum of two plane points</returns>
            public static PlaneCoor operator +(PlaneCoor left, PlaneCoor right)
            {
                return new PlaneCoor(left.X + right.X, left.Y + right.Y);
            }
            /// <summary>
            /// The overriden minus operator.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns>Difference between two points</returns>
            public static PlaneCoor operator -(PlaneCoor left, PlaneCoor right)
            {
                return new PlaneCoor(left.X - right.X, left.Y - right.Y);
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            public PlaneCoor() { }

            public PlaneCoor(PlaneCoor planeCoor)
            {
                this.X = planeCoor.X;
                this.Y = planeCoor.Y;
            }

            /// <summary>
            /// To convert the Cartesian coordinates to polar coordinate
            /// </summary>
            /// <returns>the polar coordinate</returns>
            public PolarCoor ToPolar()
            {

                double Radius = Math.Sqrt(this.X * this.X + this.Y * this.Y);

                return new PolarCoor(Radius, new Angle(this.X / Radius, this.Y >= 0));

            }
        }

        /// <summary>
        /// Cartesian coordinate class, representing a cartesian space coordinate
        /// </summary>
        public class CartCoor : PlaneCoor
        {
            public double Z { get; set; }

            /// <summary>
            /// To construct by x, y, and z values.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            public CartCoor(double x, double y, double z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            /// <summary>
            /// Default constructor
            /// </summary>
            public CartCoor()
            {

            }
            /// <summary>
            /// To construct by copying.
            /// </summary>
            /// <param name="point">copied point</param>
            public CartCoor(CartCoor point)
            {
                this.X = point.X;
                this.Y = point.Y;
                this.Z = point.Z;
            }


            /// <summary>
            /// To convert Cartesian coordinates to spherical coordinates.
            /// </summary>
            /// <returns>the spherical coordinate</returns>
            public SpherCoor ToSphere()
            {
                double Radius = Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
                return new SpherCoor(Radius, this.Direct());
            }
            /// <summary>
            /// To find the angles of this vector or point
            /// </summary>
            /// <returns></returns>
            public UnitVector Direct()
            {
                return new UnitVector(this);
            }


            //To overload operators, + for sum, - for difference.
            /// <summary>
            /// To find the difference vector between two points.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns>the difference vector between two points</returns>
            public static Vector operator -(CartCoor left, CartCoor right)
            {
                return new Vector(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
            }
            /// <summary>
            /// To find the subtraction of a vector from a point.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="vect"></param>
            /// <returns>the point a vector from the original point</returns>
            public static CartCoor operator -(CartCoor point, Vector vect)
            {
                return new CartCoor(point.X - vect.X, point.Y - vect.Y, point.Z - vect.Z);
            }
            /// <summary>
            /// To find the superposition of a vector over a point.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="vect"></param>
            /// <returns>the point a vector to the original point</returns>
            public static CartCoor operator +(CartCoor point, Vector vect)
            {
                return new CartCoor(point.X + vect.X, point.Y + vect.Y, point.Z + vect.Z);
            }
            /// <summary>
            /// To find the superposition of a vector over a point.
            /// </summary>
            /// <param name="point"></param>
            /// <param name="vect"></param>
            /// <returns>the point a vector to the original point</returns>
            public static CartCoor operator +(Vector vect, CartCoor point)
            {
                return new CartCoor(point.X + vect.X, point.Y + vect.Y, point.Z + vect.Z);
            }
            ////Special Usages
            ///// <summary>
            ///// Scalar multiplication.
            ///// </summary>
            ///// <param name="num"></param>
            ///// <param name="point"></param>
            ///// <returns></returns>
            //public static CartCoor operator *(double num, CartCoor point)
            //{
            //    return new CartCoor(num * point.X, num * point.Y, num * point.Z);
            //}
            //public static CartCoor operator +(CartCoor left, CartCoor right)
            //{
            //    return new CartCoor(
            //        left.X + right.X,
            //        left.Y + right.Y,
            //        left.Z + right.Z);
            //}
        }
        /// <summary>
        /// Vector class, which is systematically the same as cartesian coordinate,
        /// but used to implement only vectors.
        /// </summary>
        public class Vector : CartCoor
        {
            /// <summary>
            /// To construct by component values.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="z"></param>
            public Vector(double x, double y, double z) : base(x, y, z) { }

            /// <summary>
            /// To construct by copying.
            /// </summary>
            /// <param name="vector">copied vector</param>
            public Vector(Vector vector) : base(vector as CartCoor) { }

            /// <summary>
            /// The modulus of a vector.
            /// </summary>
            /// <returns>the modulus of the vector</returns>
            public double Modulus()
            {
                return Math.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            }
            /// <summary>
            /// To yield the normalized vector.
            /// </summary>
            /// <returns></returns>
            public Vector Normalize()
            {
                return (1 / this.Modulus()) * this;
            }

            //To overload the operators, + for sum, & for dot product, and * for cross product
            /// <summary>
            /// Sum of vectors.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static Vector operator +(Vector left, Vector right)
            {
                return new Vector(
                    left.X + right.X,
                    left.Y + right.Y,
                    left.Z + right.Z);
            }
            /// <summary>
            /// Subtraction of vectors.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static Vector operator -(Vector left, Vector right)
            {
                return new Vector(
                    left.X - right.X,
                    left.Y - right.Y,
                    left.Z - right.Z
                    );
            }
            /// <summary>
            /// Negative vector
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static Vector operator -(Vector value)
            {
                return new Vector(-value.X, -value.Y, -value.Z);
            }
            /// <summary>
            /// Dot product.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static double operator &(Vector left, Vector right)
            {
                return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
            }
            /// <summary>
            /// Cross product.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns></returns>
            public static Vector operator *(Vector left, Vector right)
            {
                return new Vector(
                    left.Y * right.Z - left.Z * right.Y,
                    left.Z * right.X - left.X * right.Z,
                    left.X * right.Y - left.Y * right.X
                    );
            }
            /// <summary>
            /// Scalar multiplication.
            /// </summary>
            /// <param name="num"></param>
            /// <param name="vect"></param>
            /// <returns></returns>
            public static Vector operator *(double num, Vector vect)
            {
                return new Vector(num * vect.X, num * vect.Y, num * vect.Z);
            }

            public Vector() : base() { }

        }

        /// <summary>
        /// Representing a direction in a 3D space.
        /// </summary>
        public class UnitVector
        {
            private double x;
            private double y;
            private double z;
            public double CosTheta { get { return this.z; } }
            public double SinTheta { get { return Math.Sqrt(this.x * this.x + this.y * this.y); } }
            public double CosPhi { get { return this.x / this.SinTheta; } }
            public double SinPhi { get { return this.y / this.SinTheta; } }
            public Vector ToVect { get { return new Vector(this.x, this.y, this.z); } }
            public SharpDX.Vector3 ToVect3 { get { return new SharpDX.Vector3((float)this.x, (float)this.y, (float)this.z); } }
            public Angle Theta { get { return new Angle(this.CosTheta, this.SinTheta); } }
            public Angle Phi { get { return new Angle(this.CosPhi, this.SinPhi); } }
            public void Renormalize()
            {
                double Modulus = Math.Sqrt(x * x + y * y + z * z);
                //Incomplete exception handling
                if (Modulus == 0)
                {
                    this.x = this.y = this.z = double.NaN;
                }
                this.x /= Modulus;
                this.y /= Modulus;
                this.z /= Modulus;
            }
            public void Invert()
            {
                this.x *= -1;
                this.y *= -1;
                this.z *= -1;
            }

            public UnitVector Convert(Angle alpha, Angle beta, Angle gamma)
            {
                //Incomplete exception handling
                if (this == null) return null;

                //phi1, theta1
                double CosPhi1 = this.CosPhi * alpha.Cos + this.SinPhi * alpha.Sin;
                double SinPhi1 = this.SinPhi * alpha.Cos - this.CosPhi * alpha.Sin;
                double CosThe1 = this.CosTheta;
                double SinThe1 = this.SinTheta;
                //phi2, theta2
                double CosPhi2, SinPhi2;
                double CosThe2 = -SinPhi1 * SinThe1 * beta.Sin + CosThe1 * beta.Cos;
                double SinThe2 = Math.Sqrt(1 - CosThe2 * CosThe2);
                if (SinThe2 == 0)
                {
                    CosPhi2 = 1;
                    SinPhi2 = 0;
                }
                else
                {
                    CosPhi2 = CosPhi1 * SinThe1 / SinThe2;
                    if (SinPhi1 * SinThe1 * beta.Cos + CosThe1 * beta.Sin >= 0)
                        SinPhi2 = Math.Sqrt(1 - CosPhi2 * CosPhi2);
                    else SinPhi2 = -Math.Sqrt(1 - CosPhi2 * CosPhi2);
                }

                //phi3, theta3
                double CosPhi3 = CosPhi2 * gamma.Cos + SinPhi2 * gamma.Sin;
                double SinPhi3 = SinPhi2 * gamma.Cos - CosPhi2 * gamma.Sin;
                double CosThe3 = CosThe2;
                double SinThe3 = SinThe2;
                return new UnitVector(new Angle(CosPhi3, SinPhi3), new Angle(CosThe3, SinThe3));
            }
            public UnitVector Revert(Angle alpha, Angle beta, Angle gamma)
            {
                return this.Convert(-gamma, -beta, -alpha);
            }
            public UnitVector GenPerp()
            {
                UnitVector Auxiliary = RandomNum.Direction();
                return Auxiliary ^ this;
            }
            public static double operator &(UnitVector left, UnitVector right)
            {
                return left.x * right.x + left.y * right.y + left.z * right.z;
            }
            public static double operator &(Vector left, UnitVector right)
            {
                return left.X * right.x + left.Y * right.y + left.Z * right.z;
            }
            public static double operator &(UnitVector left, Vector right)
            {
                return left.x * right.X + left.y * right.Y + left.z * right.Z;
            }
            public static Vector operator *(UnitVector left, UnitVector right)
            {
                return new Vector(
                    left.y * right.z - left.z * right.y,
                    left.z * right.x - left.x * right.z,
                    left.x * right.y - left.y * right.x
                    );
            }
            public static UnitVector operator ^(UnitVector left, UnitVector right)
            {
                return new UnitVector(
                    left.y * right.z - left.z * right.y,
                    left.z * right.x - left.x * right.z,
                    left.x * right.y - left.y * right.x
                    );
            }
            public static Angle operator -(UnitVector left, UnitVector right)
            {
                return new Angle(left & right, true);
            }
            public static UnitVector operator -(UnitVector value)
            {
                return new UnitVector(
                    -value.x,
                    -value.y,
                    -value.z
                    );
            }
            public static Vector operator *(double lambda, UnitVector unitVector)
            {
                return new Vector(
                    lambda * unitVector.x,
                    lambda * unitVector.y,
                    lambda * unitVector.z
                    );
            }
            private UnitVector(double x, double y, double z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.Renormalize();
            }
            public UnitVector(Angle phi, Angle theta)
            {
                this.x = phi.Cos * theta.Sin;
                this.y = phi.Sin * theta.Sin;
                this.z = theta.Cos;
            }
            public UnitVector(double phi, double theta, AngleUnit unit)
            {
                if (unit == AngleUnit.Degree) { phi *= Math.PI / 180; theta *= Math.PI / 180; }
                this.x = Math.Cos(phi) * Math.Sin(theta);
                this.y = Math.Sin(phi) * Math.Sin(theta);
                this.z = Math.Cos(theta);
            }
            public UnitVector(CartCoor point)
            {
                this.x = point.X;
                this.y = point.Y;
                this.z = point.Z;
                this.Renormalize();
            }
            public UnitVector() { }
        }


        /// <summary>
        /// The line class, representing a line in the 3D space.
        /// </summary>
        public class Line
        {

            /// <summary>
            /// Direction of the line.
            /// </summary>
            public UnitVector Direct { get; set; }



            /// <summary>
            /// A point on this line.
            /// </summary>
            public CartCoor Point { get; set; }

            ///// <summary>
            ///// To find the intersection between two lines. 
            ///// DON'T USE IT NOW! THIS IS YET TO BE COMPLETED!
            ///// </summary>
            ///// <param name="left"></param>
            ///// <param name="right"></param>
            ///// <returns>the intersection of the two lines, null if there is none</returns>
            //public static CartCoor operator *(Line left, Line right)
            //{
            //    //TO BE CONTINUED, NOT NECESSARY
            //    if (((left.Point - right.Point) & (left.UnitVector * right.UnitVector)) == 0)
            //        ;
            //    return null;
            //}
            /// <summary>
            /// To return the intersection between the line and the plane.
            /// </summary>
            /// <param name="line"></param>
            /// <param name="plane"></param>
            /// <returns>the intersection between the line and the plane face, null if parallel</returns>
            public static CartCoor operator *(Line line, Plane plane)
            {
                double CosAngle = (line.Direct & plane.Normal);
                if (CosAngle == 0)
                    //Incomplete exception handling
                    return null;
                else return line.Point + (((plane.Point - line.Point) & 1 * plane.Normal) / CosAngle) * line.Direct;
            }
            /// <summary>
            /// Default constructor.
            /// </summary>
            public Line() { }
            /// <summary>
            /// To construct by point and direction
            /// </summary>
            /// <param name="point">a point the line passes through</param>
            /// <param name="direction">the direction of the line</param>
            public Line(CartCoor point, UnitVector direction)
            {
                this.Point = point;
                this.Direct = direction;
            }
            /// <summary>
            /// To construct by copying.
            /// </summary>
            /// <param name="line">copied</param>
            public Line(Line line)
            {
                this.Point = line.Point;
                this.Direct = line.Direct;
            }
        }
        /// <summary>
        /// The plane class, representing a plane in the 3D space.
        /// </summary>
        public class Plane
        {

            /// <summary>
            /// Normal direction property of the plane.
            /// </summary>
            public UnitVector Normal { get; set; }


            /// <summary>
            /// One point that is one the plane.
            /// </summary>
            public CartCoor Point { get; set; }
            /// <summary>
            /// To find the intersection point of two planes.
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            /// <returns>the intersection point of two planes</returns>
            public static Line operator *(Plane left, Plane right)
            {
                double Sin = (left.Normal * right.Normal).Modulus();
                double SinSquared = Sin * Sin;
                if (SinSquared == 0)
                    //Incomplete exception handling
                    return null;

                Vector UnitVector = (left.Normal * right.Normal);
                CartCoor Point = left.Point
                    + (1 / SinSquared)
                    * ((left.Point - right.Point) & right.Normal.ToVect)
                    * (left.Normal.ToVect * (left.Normal * right.Normal));
                return new Line(Point, UnitVector.Direct());
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            public Plane() { }

            /// <summary>
            /// To construct by normal direction and point.
            /// </summary>
            /// <param name="point">a point on the surface</param>
            /// <param name="normal">the normal direction</param>
            public Plane(CartCoor point, UnitVector normal)
            {
                this.Point = point;
                this.Normal = normal;
            }
            /// <summary>
            /// To construct by copying.
            /// </summary>
            /// <param name="plane">copied plane</param>
            public Plane(Plane plane)
            {
                this.Point = plane.Point;
                this.Normal = plane.Normal;
            }
        }

        /// <summary>
        /// Random number class, a static class dealing with random numbers.
        /// </summary>
        public static class RandomNum
        {
            static Random random = new Random();
            /// <summary>
            /// To generate a random within the range min to max, 
            /// with uniform probability inside the range
            /// </summary>
            /// <param name="min"></param>
            /// <param name="max"></param>
            /// <returns>the random number</returns>
            public static double Random(double min, double max)
            {
                return RandomNum.random.NextDouble() * Math.Abs(max - min) + min;
            }
            /// <summary>
            /// To generate a pair of numbers randomly generated in a square,
            /// with uniform probability inside the square.
            /// The order of min and max is insignificant.
            /// </summary>
            /// <param name="xMin"></param>
            /// <param name="xMax"></param>
            /// <param name="yMin"></param>
            /// <param name="yMax"></param>
            /// <returns>the generated plane coordinate</returns>
            public static PlaneCoor Rect(double xMin, double xMax, double yMin, double yMax)
            {
                double x = RandomNum.random.NextDouble() * (xMax - xMin) + xMin;
                double y = RandomNum.random.NextDouble() * (yMax - yMin) + yMin;
                return new PlaneCoor(x, y);
            }
            /// <summary>
            /// To generate a vector of numbers randomly generated in a cube,
            /// with uniform probability inside the square.
            /// The order of min and max is insignificant.
            /// </summary>
            /// <param name="xMin"></param>
            /// <param name="xMax"></param>
            /// <param name="yMin"></param>
            /// <param name="yMax"></param>
            /// <param name="zMin"></param>
            /// <param name="zMax"></param>
            /// <returns>the generated Cartesian coordinate</returns>
            public static CartCoor Cube(double xMin, double xMax, double yMin, double yMax, double zMin, double zMax)
            {
                double x = RandomNum.random.NextDouble() * (xMax - xMin) + xMin;
                double y = RandomNum.random.NextDouble() * (yMax - yMin) + yMin;
                double z = RandomNum.random.NextDouble() * (zMax - zMin) + zMin;
                return new CartCoor(x, y, z);
            }
            /// <summary>
            /// To generate a pair of numbers randomly generated in a square,
            /// with uniform probability inside the square.
            /// The order of min and max is insignificant.
            /// </summary>
            /// <param name="min">a vertex of the desired square</param>
            /// <param name="max">an opposite vertex of the desired square</param>
            /// <returns></returns>
            public static PlaneCoor Rect(PlaneCoor min, PlaneCoor max)
            {
                return RandomNum.Rect(min.X, max.X, min.Y, max.Y);
            }
            /// <summary>
            /// To generate a pair of numbers randomly generated in a cube,
            /// with uniform probability inside the square.
            /// The order of min and max is insignificant.
            /// </summary>
            /// <param name="min">a vertex of the desired cube</param>
            /// <param name="max">the opposite vertex of the desired cube</param>
            /// <returns></returns>
            public static CartCoor Cube(CartCoor min, CartCoor max)
            {
                return RandomNum.Cube(min.X, max.X, min.Y, max.Y, min.Z, max.Z);
            }
            /// <summary>
            /// To generate a random point falling in the rectangle determined by the four
            /// vertices.
            /// The vertices should be listed in continual order.
            /// Validation needed for vertices on the same plane.
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <param name="c"></param>
            /// <param name="d"></param>
            /// <returns></returns>
            public static CartCoor Rect(CartCoor a, CartCoor b, CartCoor c, CartCoor d)
            {
                //Validation for all the points on the same plane needed.

                //Find two base vectors.
                Vector AB = b - a;
                Vector AD = d - a;
                return a + RandomNum.Random(0, 1) * AB + RandomNum.Random(0, 1) * AD;
            }

            public static double Triangular(double center, double delta)
            {
                double Candidate = RandomNum.Random(0, delta);
                double Auxiliary = RandomNum.Random(0, delta);
                if (Auxiliary + Candidate < delta) return Candidate + center;
                else return Candidate - delta + center;
            }
            public static UnitVector Direction()
            {
                CartCoor Auxiliary;
                do
                {
                    Auxiliary =
                        RandomNum.Cube(new CartCoor(-1, -1, -1), new CartCoor(1, 1, 1));
                } while (Auxiliary.X * Auxiliary.X + Auxiliary.Y * Auxiliary.Y + Auxiliary.Z * Auxiliary.Z >= 1);
                return new UnitVector(Auxiliary);
            }
        }
        public class Utilities
        {

            /// <summary>
            /// To swao the two objects
            /// </summary>
            /// <typeparam name="T">type of the objects</typeparam>
            /// <param name="x">one object</param>
            /// <param name="y">another object</param>
            public static void Swap<T>(ref T x, ref T y)
            {
                T temp;
                temp = x;
                x = y;
                y = temp;
            }
        }
        

    }
}
