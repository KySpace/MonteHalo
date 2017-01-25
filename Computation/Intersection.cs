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
        public partial class Beam
        {
            public PolarizationComponent GenComp(Surface face)
            {
                //Incomplete exception handling
                if (face == null) return PolarizationComponent.s;


                Vector dirS = (this.Direct * face.Normal);
                Vector dirP = (this.Direct.ToVect * dirS);
                //(abolished)Here it should be reconsidered whether it is a better way to use vectors instead
                //(abolished)of directions in the computation to avoid frequent convertion
                //To use vector instead of direction should save lot of works
                //Idealy, dirS, dirP should all be unit vectors
                double componentS = this.Pol & dirS;
                //To generate a random number between [0,1)
                double randSP = RandomNum.Random(0, 1);
                //To decide whether it is S or P component
                if(randSP <= componentS*componentS)
                {
                    //Component S
                    this.Pol = dirS.Direct();
                    return PolarizationComponent.s;
                }
                else
                {
                    //Component P
                    this.Pol = dirP.Direct();
                    return PolarizationComponent.p;
                }
            }
            //To find the reflection of a beam by the given surface
            //Remember to calculate the intersection beforehand.
            public void Reflection(ref Surface face, CartCoor intersection)
            {
                //Beam reflected = new Beam(this);
                try
                {
                    this.ReflectionCount++;
                    //Point and direction
                    this.Point = intersection;
                    this.Direct = (this.Direct.ToVect -
                        (2 * (face.Normal & this.Direct) * face.Normal)).Direct();
                    //Polarization
                    this.Pol = (this.Pol.ToVect -
                        (2 * (face.Normal & this.Pol) * face.Normal)).Direct();
                }
                catch (DivideByZeroException e)
                {
                    //To be continued
                }
                //To return
                //return reflected;
            }
            //To find the refraction of a beam by the given surface
            //Remember to calculate the refraction angle and the intersection beforehand.
            //Index ratio is the ratio of n2 over n1
            public void Refraction(ref Surface face, Angle inci, Angle refr, CartCoor intersection)
            {
                //Beam refracted = new Beam(this);
                try
                {
                    this.RefractionCount++;
                    this.Env = (this.Env == BeamEnvironment.Inside ? BeamEnvironment.Outside : BeamEnvironment.Inside);
                    //Points and direction
                    this.Point = intersection;
                    this.Direct = (this.Direct.ToVect + (
                        ((inci.Tan / refr.Tan - 1)
                        * (face.Normal & this.Direct))
                        * face.Normal)).Direct();
                    //Polarization
                    this.Pol = (this.Pol.ToVect + (
                        ((inci.Cot / refr.Cot - 1)
                        * (face.Normal & this.Pol))
                        * face.Normal
                        )
                        ).Direct();
                }
                catch (DivideByZeroException e)
                {
                    //To be continued
                }
                //To return
                //return refracted;
            }

            /// <summary>
            /// To determine what happens when the beam encounters a surface.
            /// To yield the refracted or reflected beam.
            /// Remember to calculate the intersection beforehand.  
            /// </summary>
            /// <param name="face"></param>
            /// <param name="intersection"></param>
            /// <param name="indexRatio"></param>
            /// <returns></returns>
            public void Interaction(ref Surface face, CartCoor intersection, double indexRatio)
            {
                //Incomplete exception handling
                if (face == null || intersection == null || indexRatio == 0)
                    return;
                //To find the incidence and refraction angle, if there should be any
                Angle InciAngle = (this.Direct - face.Normal); InciAngle.ToAcute();
                double SinRefr = InciAngle.Sin / indexRatio;
                Angle RefrAngle = new Angle(Math.Sqrt(1 - SinRefr * SinRefr), true);
                //To adjust polarization
                PolarizationComponent PolarComp = GenComp(face);
                //To calculate the power flow
                double PowerFlowRefl = 0;
                double SqrtDenominator;
                double Numerator = 4 * InciAngle.Sin * InciAngle.Cos * RefrAngle.Sin * RefrAngle.Cos;
                if (InciAngle.Sin == 0 || RefrAngle.Sin == 0)
                    PowerFlowRefl = Math.Pow((indexRatio - 1) / (indexRatio + 1), 2);
                else if (PolarComp == PolarizationComponent.s)
                {
                    SqrtDenominator = InciAngle.Sin * RefrAngle.Cos + InciAngle.Cos * RefrAngle.Sin;
                    PowerFlowRefl = Numerator / (SqrtDenominator * SqrtDenominator);                        
                }
                else if (PolarComp == PolarizationComponent.p)
                {
                    SqrtDenominator = InciAngle.Sin * InciAngle.Cos + RefrAngle.Sin * RefrAngle.Cos;
                    PowerFlowRefl = Numerator / (SqrtDenominator * SqrtDenominator);
                }
                //Incomplete exception handling
                else PowerFlowRefl = double.NaN;
                //Iteraction
                //Beam Result;
                //Total reflection
                if (SinRefr > 1)
                    //Result = 
                    this.Reflection(ref face, intersection);
                else
                {
                    //Other occasions, using MC method
                    double rand = RandomNum.Random(0, 1);
                    //Refraction
                    if (rand <= PowerFlowRefl)
                        //Result = 
                        this.Refraction(ref face, InciAngle, RefrAngle, intersection);
                    //Reflecion
                    else //Result = 
                        this.Reflection(ref face, intersection);
                    //return Result;
                }
            }
        }
    }
}
