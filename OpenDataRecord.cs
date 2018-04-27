using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class OpenDataOutput
    {
        public List<OpenDataRecord> OpenData { get; set; }

        public OpenDataOutput(List<OpenDataRecord> records)
        {
            OpenData = records;
        }

        public void WriteJson(string path)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
