using System;
using System.Numerics;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ArrayFire;
using ArrayFire.Interop;
using Array = ArrayFire.Array;

// If using Visual Studio 2015 you can uncomment the following lines and type Sin() instead of Arith.Sin(), Seq() instead of Util.Seq(), and so on.
// using static ArrayFire.Arith;
// using static ArrayFire.Util;

namespace CSharpTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Device.SetBackend(Backend.CPU);
            Device.PrintInfo();
            Console.WriteLine("w\th\td\tms (avg on 10 runs)\tms (min on 10 runs)\tms (max on 10 runs)");
            var staps = new[] {10, 32, 64, 100, 128, 200, 256, 300};
            for (int i = 0; i < staps.Length; i++) {
                var stats = new List<double>();
                var width  = staps[i];
                var height = staps[i];
                var depth  = staps[i];
                Random r = new Random();
                for (int j = 0; j < 10; j++) {
                    var src = new Complex[width,height,depth];
                    for (int i1 = 0; i1 < src.GetLength(0); i1++) {
                        for (int i2 = 0; i2 < src.GetLength(1); i2++) {
                            for (int i3 = 0; i3 < src.GetLength(2); i3++) {
                                src[i1,i2,i3] = new Complex(r.NextDouble(), r.NextDouble());
                                //src[i1,i2,i3] = (float)r.NextDouble();
                            }
                        }
                    }
                    var arr1     = Data.RandUniform<float>(width, height, depth);

                    
                    var arr2     = Data.RandUniform<float>(width, height, depth);
                    var carr = Data.CreateArray(src);
                    var carr1 = Data.CreateComplexArray(arr1, arr2);
                    var carr2 = Data.CreateComplexArray(arr1, arr2);
                    var carr3 = Data.CreateComplexArray(arr1, arr2);
                    var carr4 = Data.CreateComplexArray(arr1, arr2);
                    var carr5 = Data.CreateComplexArray(arr1, arr2);
                    
                    var outArr0 =Data.GetData3D<Complex>(carr);
                    var outArr2 =Data.GetData3D<float>(arr1);
                    
                    //Console.WriteLine(outArr0.GetLength(0) + " " + outArr0.GetLength(1) + " " + outArr0.GetLength(2));
                    //Console.WriteLine(outArr0[width/2,height/2,depth/2]);
                    
                    var from     = DateTime.UtcNow;
                    //var ocarr = Data.CreateComplexArray(arr1, arr2);
                    AFSignal.af_fft3(out  carr._ptr, carr._ptr, 1, width, height, depth);
                    AFData.af_shift(out carr._ptr, carr._ptr, width / 2, height / 2, depth / 2, 0);
                    AFSignal.af_ifft3(out carr._ptr, carr._ptr, 1, width, height, depth);
                    AFData.af_shift(out carr._ptr, carr._ptr, (width+1) / 2, (height+1) / 2, (depth+1) / 2, 0);
                    
                    AFSignal.af_fft3(out  carr._ptr, carr._ptr, 1, width, height, depth);
                    AFData.af_shift(out carr._ptr, carr._ptr, width / 2, height / 2, depth / 2, 0);
                    AFSignal.af_ifft3(out carr._ptr, carr._ptr, 1, width, height, depth);
                    AFData.af_shift(out carr._ptr, carr._ptr, (width+1) / 2, (height+1) / 2, (depth+1) / 2, 0);
                    
                    var outArr =Data.GetData3D<Complex>(carr);
                    //Console.WriteLine(outArr[width/2, height/2, depth/2]);
                    outArr[0,0,0] += 1;
;                    var to = DateTime.UtcNow;
                    File.WriteAllText("out", outArr.ToString() + to.ToString());
                    stats.Add(( to-from).TotalMilliseconds);
                }

                //Util.Print(result, "fftd ocarr");
                Console.WriteLine(width + "\t" + height + "\t" + depth + "\t" + stats.Average() + "\t" + stats.Min() + "\t" + stats.Max());
            }


        }
    }
}
