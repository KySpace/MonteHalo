using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MonteHalo
{
    using PhysicalModels;
    using GeoMath;
    namespace Tests
    {
        public static class TempTest
        {
            private static void GenOrientationTest(out Angle alpha, out Angle beta, out Angle gamma)
            {
                alpha = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                beta = new Angle(RandomNum.Random(0, 180), AngleUnit.Degree);
                gamma = new Angle(RandomNum.Random(0, 360), AngleUnit.Degree);
                return;
            }
            ///// <summary>
            ///// This test method use file output
            ///// </summary>
            //async public static void test1()
            //{
            //    Angle alpha, beta, gamma;
            //    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            //    Windows.Storage.StorageFile sampleFile = await storageFolder.CreateFileAsync
            //        ("trial.txt", Windows.Storage.CreationCollisionOption.ReplaceExisting);
            //    System.Diagnostics.Debug.WriteLine("File has been saved in: ",Windows.Storage.ApplicationData.Current.LocalFolder.Path);
            //    Direction sol = new Direction(0, 90, AngleUnit.Degree);
            //    Direction output = new Quantities.Direction();
            //    HexaPrism crystal = new HexaPrism(2, 5, 4, 4, 4);
            //    var stream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            //    using (var outputStream = stream.GetOutputStreamAt(0))
            //    {
            //        using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
            //        {
            //            for (int counter = 1; counter <= 10000; counter++)
            //            {
            //                GenOrientationTest(out alpha, out beta, out gamma);
            //                output = crystal.SingleScattering(sol, alpha, beta, gamma);
            //                dataWriter.WriteString(output.ToString());
            //            }
            //            await dataWriter.StoreAsync();
            //            await outputStream.FlushAsync();
            //        }
            //    }
            //    stream.Dispose();
            //    System.Diagnostics.Debug.WriteLine("Done");               
            //}
            //public static void test2(Graphics.TestPointPlot plot)
            //{
            //    Angle alpha, beta, gamma;
            //    Direction sol = new Direction(0, 90, AngleUnit.Degree);
            //    Direction output = new Quantities.Direction();
            //    HexaPrism crystal = new HexaPrism(2, 5, 4, 4, 4);


            //    for (int counter = 1; counter <= 3000000; counter++)
            //    {
            //        GenOrientationTest(out alpha, out beta, out gamma);
            //        output = crystal.SingleScattering(sol, alpha, beta, gamma);
            //        //output = sol.Convert(alpha, beta, gamma);                    
            //        //output = new Direction(alpha, beta);                    
            //        plot.UpdateVertexBuffer(Graphics.ScatterVertex.ToVertex(output, 0));

            //    }
            //}
            //public static void testInitPoint(Graphics.TestPointPlot _plot)
            //{
            //    Beam beam;
            //    CartCoor point;
            //    Surface targetFace;
            //    HexaPrism crystal = new HexaPrism(2, 4, 2, 2, 2);
            //    for (int i = 0; i < 100000; i++)
            //    {
            //        beam = new Beam(null, new Direction(23, 65, AngleUnit.Degree));
            //        point = crystal.FindNextPoint(ref beam, out targetFace);
            //        _plot.UpdateVertexBuffer(Graphics.ScatterVertex.ToVertex(point));
            //    }
            //}
            public static void test3(Graphics.TestPointPlot plot)
            {
                Angle alpha, beta, gamma;
                UnitVector sol = new UnitVector(0, 45, AngleUnit.Degree);
                UnitVector output = new GeoMath.UnitVector();
                HexaPrism crystal = new HexaPrism(2, 5, 4, 4, 4);


                for (int counter = 1; counter <= 3000000; counter++)
                {
                    GenOrientationTest(out alpha, out beta, out gamma);
                    output = crystal.SingleScattering(sol, alpha, beta, gamma);
                    //output = sol.Convert(alpha, beta, gamma);                    
                    //output = new Direction(alpha, beta);                    
                    plot.UpdateVertexBuffer(Graphics.ScatterVertex.ToVertex(200 * output as CartCoor));

                }
            }
        }
    }
}
