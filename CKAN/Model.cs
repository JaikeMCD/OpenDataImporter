using System;

namespace Mcd.OpenData.CKAN
{
    public class Resource
    {
        public class Result
        {
            public Guid package_id { get; set; }
            public Guid id { get; set; }
            public Guid revision_id { get; set; }
            public Uri url { get; set; }
            public string format { get; set; }
            public DateTime last_modified { get; set; }
            public bool datastore_active { get; set; }
        }

        public bool success { get; set; }
        public Result result { get; set; }
    }
}
