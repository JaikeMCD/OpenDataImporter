using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Mcd.OpenData
{
    public class OpenDataRecord
    {
        public string Title { get; set; }
        public string Location { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        public string Field5 { get; set; }
        public string Field6 { get; set; }
        public string Field7 { get; set; }
        public string Field8 { get; set; }
        public string Field9 { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitutde { get; set; }
    }

    public class ImportScriptArgs
    {
        public dynamic src;
        public OpenDataRecord dst;
    }

    public static class ImportScript
    {
        public static async Task Run()
        {
            try
            {
                var scriptContent = "dst.Title = src.Title";

                dynamic expando = new ExpandoObject();
                expando.Title = "Wangaratta";

                var refs = new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly.Location)
                };

                ScriptOptions options = ScriptOptions.Default
                                                     .AddReferences(refs);

                var script = CSharpScript.Create(
                    scriptContent,
                    options: options,
                    globalsType: typeof(ImportScriptArgs)
                );

                script.Compile();

                var g = new ImportScriptArgs { src = expando, dst = new OpenDataRecord() };

                var r = await script.RunAsync(g);

                Console.WriteLine(r.ReturnValue);


                /*
                ScriptRunner<int> runner = script.CreateDelegate();

                for (int i = 0; i < 10; ++i)
                {
                    int r = await runner(new ImportScriptArgs { x = i, y = i });

                    Console.WriteLine("{0} -> {1}", i, r);
                }
                */
            }
            catch(Exception e)
            {
                Console.WriteLine("Import Script: {0}", e);
            }
        }
    }
}
