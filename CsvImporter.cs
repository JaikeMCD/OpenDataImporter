using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using CsvHelper;

namespace Mcd.OpenData
{
    public class CsvImporter
    {
        public List<dynamic> Records { get; set; }

        public static CsvImporter OpenCsv(string path)
        {
            CsvImporter importer = new CsvImporter();
            importer.Open(path);
            return importer;
        }

        public void Open(string path)
        {
            using (var reader = File.OpenText(path))
            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = header => header.Replace(" ", "_");
                Records = csv.GetRecords<dynamic>().ToList();
            }
        }
    }
}
