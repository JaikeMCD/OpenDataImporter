using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Mcd.OpenData.Utilities
{
    class Source
    {
        public static int List(ListOptions opts)
        {
            using (var directory = new Data.DirectoryContext())
            {
                var sources = from s in directory.OpenDataSources
                              select s;
                            
                foreach (var s in sources)
                    Console.WriteLine("{0}: {1} - {2}", s.ID, s.Name, s.Description);
            }

            return 0;
        }

        public static int Info(InfoOptions opts)
        {
            using (var directory = new Data.DirectoryContext())
            {
                var source = directory.OpenDataSources.First(s => s.ID == opts.ID);
                var json = JsonConvert.SerializeObject(source, Formatting.Indented);

                Console.WriteLine(json);
            }

            return 0;
        }
    }
}
