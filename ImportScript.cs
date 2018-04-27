using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

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

        public IEnumerable<OpenDataRecord> ConvertRecords(IEnumerable<dynamic> records)
        {
            return records.Select<dynamic, OpenDataRecord>(r => ConvertRecord(r));
        }

        public OpenDataRecord ConvertRecord(dynamic record)
        {
            var args = new Args { src = record, dst = new OpenDataRecord() };
            var r = runner(args).GetAwaiter().GetResult();
            return args.dst;
        }
    }
}
