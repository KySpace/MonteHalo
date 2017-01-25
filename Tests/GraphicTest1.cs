using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonteHalo.Graphics;
using SharpDX;

namespace MonteHalo
{
    namespace Tests
    {
        public class GraphicTest1
        {            
            public static void test1(Graphics.TestPointPlot _plot)
            {
                System.Random rand = new Random();
                int i;
                for (i = 0; i < 1000000; i++)
                {
                    _plot.UpdateVertexBuffer(new ScatterVertex(
                        new Vector3(5*(float)rand.NextDouble(), 5*(float)rand.NextDouble(), 5*(float)rand.NextDouble()),
                        rand.NextColor()
                        )
                        );                        
                }                
            }
            public static void test2(Graphics.TestPointPlot _plot)
            {
                System.Random rand = new Random();
                ScatterVertex[] array = new ScatterVertex[1000005];
                int i;
                for (i = 0; i < 1000000; i++)
                {
                    array[i] = (new ScatterVertex(
                        new Vector3(5 * (float)rand.NextDouble(), 5 * (float)rand.NextDouble(), 5 * (float)rand.NextDouble()),
                        rand.NextColor()
                        )
                        );                        
                }
                _plot.UpdateVertexBuffer(ref array);
            }            
        }
    }
}
