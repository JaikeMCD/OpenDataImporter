//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mcd.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class OpenDataSource
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BaseAddress { get; set; }
        public System.Guid ResourceId { get; set; }
        public Nullable<System.Guid> RevisionId { get; set; }
        public Nullable<System.DateTime> LastRevision { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
        public string ImportScriptPath { get; set; }
        public string ImportScript { get; set; }
    }
}
