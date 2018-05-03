using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Mcd.OpenData.Utilities
{
    public class ImportScript
    {
        public class Args
        {
            public dynamic src;
            public OpenDataRecord dst;
        }

        public string ScriptSource { get; set; }

        protected ScriptRunner<int> runner;

        public static int Update(UpdateScriptOptions opts)
        {
            try
            {
                Console.WriteLine("Reading script {0}", opts.ScriptPath);

                var scriptSource = File.ReadAllText(opts.ScriptPath);

                //Console.WriteLine(scriptSource);

                using (var directory = new Data.DirectoryContext())
                {
                    var source = directory.OpenDataSources.First(s => s.ID == opts.ID);

                    Console.WriteLine("Saving script to OpenDataSource: '{0}'", source.Name);

                    source.ImportScript = scriptSource;

                    return directory.SaveChanges();
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found");
                return 1;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Invalid OpenDataSource ({0})", opts.ID);
                return 2;
            }
        }

        public static ImportScript Read(string path)
        {
            string scriptSource = File.ReadAllText(path);
            return new ImportScript(scriptSource);
        }

        public ImportScript(string scriptSource)
        {
            ScriptSource = scriptSource;
        }

        public void CompileScriptRunner()
        {
            var refs = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ImportUtils).GetTypeInfo().Assembly.Location)
            };

            ScriptOptions options = ScriptOptions.Default.AddReferences(refs);

            var script = CSharpScript.Create<int>(
                ScriptSource,
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

    public static class ImportUtils
    {
        public static string TrimAndJoin(string[] strings, string delimiter = " ")
        {
            var trimmed = strings.Select(s => s.Trim())
                                 .Where(s => !string.IsNullOrEmpty(s))
                                 .ToArray();

            return string.Join(delimiter, trimmed);
        }
    }
}
