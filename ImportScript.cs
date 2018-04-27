using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Mcd.OpenData
{
    public class ImportScript
    {
        public class Args
        {
            public dynamic src;
            public OpenDataRecord dst;
        }

        protected ScriptRunner<int> runner;

        public static ImportScript Create(string scriptSource)
        {
            ImportScript import = new ImportScript();
            import.CompileScriptRunner(scriptSource);
            return import;
        }

        public void CompileScriptRunner(string scriptSource)
        {
            try
            {
                var refs = new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly.Location)
                };

                ScriptOptions options = ScriptOptions.Default.AddReferences(refs);

                var script = CSharpScript.Create<int>(
                    scriptSource,
                    options: options,
                    globalsType: typeof(Args)
                );

                runner = script.CreateDelegate();
            }
            catch (Exception e)
            {
                Console.WriteLine("Import Script: {0}", e);
            }
        }

        public IEnumerable<OpenDataRecord> ConvertRecords(IEnumerable<dynamic> records)
        {
            foreach (var r in records)
                yield return ConvertRecord(r);
        }

        public OpenDataRecord ConvertRecord(dynamic record)
        {
            var args = new Args { src = record, dst = new OpenDataRecord() };
            var r = runner(args).GetAwaiter().GetResult();
            return args.dst;
        }
    }
}
