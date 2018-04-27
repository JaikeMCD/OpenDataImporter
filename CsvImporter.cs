﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CsvHelper;

namespace Mcd.OpenData
{
    public class CsvImporter
    {
        public List<dynamic> Records { get; set; }

        public CsvImporter()
        {
        }

        public static CsvImporter Import(StreamReader reader)
        {
            var importer = new CsvImporter();

            using (var csv = new CsvReader(reader))
            {
                csv.Configuration.PrepareHeaderForMatch = header => header.Replace(" ", "_");
                importer.Records = csv.GetRecords<dynamic>().ToList();
            }

            return importer;
        }
    }
}