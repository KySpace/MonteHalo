using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonteHalo
{
    using GeoMath;
    namespace PhysicalModels
    {
        public abstract partial class Crystal
        {
            /// <summary>
            /// This is the method that generate an Hexagonal crystal orientation 
            /// (represented by three Euler angles)
            /// with the given requirement
            /// THIS METHOD IS INCOMPLETE
            /// The orientation generated fits accurately with the requirement without any deviation
            /// </summary>
            /// <param name="req">Orientation requirement</param>
            /// <param name="alpha">Euler angle alpha</param>
            /// <param name="beta">Euler angle beta</param>
            /// <param name="gamma">Eular ange gamma</param>
            private void GenOrientation(OrientationRequirement req, out Angle alpha, out Angle beta, out Angle gamma)
            {
                switch(req)
                {
                    case OrientationRequirement.Column:
                        alpha = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                        beta = new Angle(RandomNum.Triangular(90, 2), AngleUnit.Degree);
                        gamma = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                        break;
                    case OrientationRequirement.Lowitz:
                        alpha = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                        beta = new Angle(RandomNum.Random(0, 180), AngleUnit.Degree);
                        gamma = new Angle(RandomNum.Triangular(90, 2), AngleUnit.Degree);
                        break;
                    case OrientationRequirement.Parry:
                        alpha = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                        beta = new Angle(RandomNum.Triangular(90, 2), AngleUnit.Degree);
                        gamma = new Angle(RandomNum.Triangular(90, 2), AngleUnit.Degree);
                        break;
                    case OrientationRequirement.Plate:
                        alpha = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                        beta = new Angle(RandomNum.Triangular(0, 1), AngleUnit.Degree);
                        gamma = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                        break;
                    case OrientationRequirement.Random:
                        UnitVector Temp = RandomNum.Direction();
                        alpha = Temp.Phi;
                        beta = Temp.Theta;
                        gamma = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                        break;
                    default:
                        alpha = beta = gamma = null;
                        break;
                }
                //alpha = new Angle(0, AngleUnit.Degree);
                //beta = new Angle(0, AngleUnit.Degree);
                //gamma = new Angle(0, AngleUnit.Degree);
                //return;
            }
            /// <summary>
            /// This is a method that provide a single photon scattering with given orientation
            /// (represented by three Euler angles) and the solar direction
            /// </summary>
            /// <param name="sol">The direction of the sun</param>
            /// <param name="alpha">Alpha of the Euler angles for crystal orientation</param>
            /// <param name="beta">Beta of the Euler angles for crystal orientation</param>
            /// <param name="gamma">Gamma of the Euler angles for crystal orientation</param>
            /// <returns>The direction of the scattered photon</returns>
            public UnitVector SingleScattering(UnitVector sol, Angle alpha, Angle beta, Angle gamma)
            {
                return LocalScattering((-sol).Convert(alpha, beta, gamma)).Revert(alpha, beta, gamma);
            }
            /// <summary>
            /// This is a method that provide a single photon scattering with given 
            /// orientation requirement and direction of the sun
            /// </summary>
            /// <param name="sol">The direction of the sun</param>
            /// <param name="req">The requirement of orientation</param>
            /// <returns></returns>
            public UnitVector SingleScattering(UnitVector sol, OrientationRequirement req)
            {
                Angle alpha = null, beta = null, gamma = null;
                GenOrientation(req, out alpha, out beta, out gamma);
                return LocalScattering((-sol).Convert(alpha, beta, gamma)).Revert(alpha, beta, gamma);
            }
            /// <summary>
            /// This method implements a single photon scattering process inside a crystal in local coordinate
            /// </summary>
            /// <param name="dir">The incoming ray direction in the local coordinate</param>
            /// <returns>the outcoming direction in the local coordinate</returns>
            private UnitVector LocalScattering(UnitVector dir)
            {
                Beam LightBeam = new Beam(null, dir);
                Surface TargetFace = null;
                CartCoor intersection;
                do
                {
                    intersection = FindNextPoint(ref LightBeam, out TargetFace);
                    //LightBeam = 
                    LightBeam.Interaction(
                        ref TargetFace, intersection,
                        (LightBeam.Env == BeamEnvironment.Inside ? 1 / this.RefrexIndex : this.RefrexIndex)
                        );
                    if (LightBeam == null)
                        break;
                } while (LightBeam.Env == BeamEnvironment.Inside);
                return LightBeam.Direct;
            }
            //The above process mainly uses the following two methods
            //--One to find the next point and the next surface
            //--(if being the first point, generate the first)
            /// <summary>
            /// To find the next point and the next surface. If it is the first point, generate the first.
            /// </summary>
            /// <param name="beam">The beam concerned</param>
            /// <param name="face">The next surface to hit</param>
            /// <returns>Next intersection point</returns>
            public CartCoor FindNextPoint(ref Beam beam, out Surface face)
            {
                //Incomplete exception handling
                if(beam == null)
                { face = null; return null; }
                //To initialize the beam
                if (beam.Env == BeamEnvironment.Outside)
                    return beam.Point = GenInitPoint(beam.Direct, out face);
                //To find the next point
                else if (beam.Env == BeamEnvironment.Inside)
                    return FindTarget(ref beam, out face);
                //Incomplete exception handling
                else
                {
                    face = null;
                    return null;
                }
            }
            //----This is for generating the initial point and selecting the surface
            /// <summary>
            /// To randomly select a surface and generate an intial point.
            /// </summary>
            /// <param name="dir">Direction of the beam</param>
            /// <param name="surface">Randomly selected surface</param>
            /// <returns>The first point on the surface</returns>
            public CartCoor GenInitPoint(UnitVector dir, out Surface surface)
            {
                CartCoor Result;
                surface = Surface(this.GenFaceChoice(dir));
                if (surface == null)
                    ;
                return  Result = this.GenPointOnFace(ref surface);
            }
            //----where it uses mainly two functions, 
            //------the one who generate the surface,
            /// <summary>
            /// Generate the choice of the first face to hit.
            /// The probability of the choice is proportional to the projection area of the face
            /// on the given direction.
            /// </summary>
            /// <param name="dir">Direction of projection, i.e. direction of the incident light</param>
            /// <returns>The face number of the chosen face</returns>
            private int GenFaceChoice(UnitVector dir)
            {
                //Incomplete exception handling
                if (dir == null) return 0;
                //To selected the possible faces by the direction generally                
                double AreaSum = 0; double AreaNew = 0;
                double CosAngle; 
                int ResultFaceNum = 0;                
                for (int candNum = 1; candNum <= 8; candNum++)
                {
                    //To discard the faces that are at the back
                    if (((CosAngle = Surface(candNum).Normal & dir)) <= 0)
                        continue;
                    else
                    {
                        AreaSum += AreaNew = Surface(candNum).SurfaceArea * CosAngle;
                        //To choose the new face over the previous one if reach the sampling condition
                        if (RandomNum.Random(0, AreaSum) < AreaNew)
                            ResultFaceNum = candNum;
                    }
                }
                //Incomplete exception handling
                if (ResultFaceNum == 0) return 0;


                return ResultFaceNum;
            }
            //------and the one that generate a point on surface
            /// <summary>
            /// To generate a random point uniformly distibuted on a surface.
            /// </summary>
            /// <param name="surface">The surface</param>
            /// <returns></returns>
            protected abstract CartCoor GenPointOnFace(ref Surface surface);
            //----And this is for generating the next point if not the first
            /// <summary>
            /// To find the next intersection point and the target surface.
            /// </summary>
            /// <param name="beam">The beam concerned</param>
            /// <param name="face">Next surface output</param>
            /// <returns>Next intersection point</returns>
            private CartCoor FindTarget(ref Beam beam, out Surface face)
            {
                //Incomplete exception handling
                if (beam == null) 
                { face = null; return null; }

                double DistanceTemp = double.PositiveInfinity;
                double DistanceMin = DistanceTemp;
                CartCoor IntersectionTemp = new CartCoor();
                CartCoor IntersectionMin = new CartCoor(IntersectionTemp);
                int MinFaceNum = 0;
                for (int candNum = 1; candNum <= 8; candNum++)
                {
                    //To discard the faces that cannot be hit
                    if (((Surface(candNum).Normal & beam.Direct)) >= 0) continue;
                    else
                    {
                        //Find the nearest one of all possible faces
                        if ((DistanceTemp = Surface(candNum).FindIntersect(ref beam, out IntersectionTemp)) < DistanceMin)
                        {
                            MinFaceNum = candNum;
                            DistanceMin = DistanceTemp;
                            IntersectionMin = IntersectionTemp;
                        }
                    }
                    //TO BE CONTINUED, EXCEPTION HANDLER                    
                }
                //Incomplete exception handling
                if (MinFaceNum == 0)
                    ;
                face = Surface(MinFaceNum);
                return IntersectionMin;
            }
            //--The other one to simulate what happens during the interaction, in "Intersection.cs"
            
        }

        public partial class HexaPrism : Crystal
        {
            /// <summary>
            /// To generate a random point uniformly distibuted on a surface.
            /// </summary>
            /// <param name="surface">The surface</param>
            /// <returns></returns>
            protected override CartCoor GenPointOnFace(ref Surface surface)
            {
                //Incomplete exception handling
                if (surface == null) return null;
                CartCoor ResultRandPoint;
                //Lid or Bottom
                if (surface.SurfaceNum == 1 || surface.SurfaceNum == 2)
                {
                    //Dealing with a hexagon                    
                    do
                    {
                        //Find a rough random point with possibility uniformally distributed 
                        //within the range of the rectangle.
                        ResultRandPoint = RandomNum.Rect(
                            new CartCoor(this.A1.X, this.E1.Y, surface.Point.Z),
                            new CartCoor(this.D1.X, this.E1.Y, surface.Point.Z),
                            new CartCoor(this.D1.X, this.B1.Y, surface.Point.Z),
                            new CartCoor(this.A1.X, this.B1.Y, surface.Point.Z)
                            );
                    }
                    while (
                    //Exclude those out of range of the hexagon.
                        ((ResultRandPoint - Surface4.Point) & Surface4.Normal) <= 0 ||
                        ((ResultRandPoint - Surface5.Point) & Surface5.Normal) <= 0 ||
                        ((ResultRandPoint - Surface7.Point) & Surface7.Normal) <= 0 ||
                        ((ResultRandPoint - Surface8.Point) & Surface8.Normal) <= 0
                    );

                }
                else
                {
                    //Dealing with a rectangle.                                    
                    switch (surface.SurfaceNum)
                    {
                        case 3: ResultRandPoint = RandomNum.Rect(this.F2, this.F1, this.A1, this.A2); break;
                        case 4: ResultRandPoint = RandomNum.Rect(this.A2, this.A1, this.B1, this.B2); break;
                        case 5: ResultRandPoint = RandomNum.Rect(this.B2, this.B1, this.C1, this.C2); break;
                        case 6: ResultRandPoint = RandomNum.Rect(this.C2, this.C1, this.D1, this.D2); break;
                        case 7: ResultRandPoint = RandomNum.Rect(this.D2, this.D1, this.E1, this.E2); break;
                        case 8: ResultRandPoint = RandomNum.Rect(this.E2, this.E1, this.F1, this.F2); break;
                        //Incomplete exception handling
                        default: ResultRandPoint = null; break;
                    }
                }
                return ResultRandPoint;
            }
        }
    }
}
