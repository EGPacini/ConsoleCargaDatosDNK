//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleCargaDatosDNK.Modelos
{
    using System;
    using System.Collections.Generic;
    
    public partial class LoggerLevelAndSpread
    {
        public long ID { get; set; }
        public int LoggerID { get; set; }
        public short NoiseLevel { get; set; }
        public short Spread { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime DataPointDate { get; set; }
        public Nullable<int> SiteID { get; set; }
        public Nullable<System.DateTime> SiteLastResolvedDate { get; set; }
        public Nullable<System.DateTime> ResolveSiteAfterDate { get; set; }
    
        public virtual loggers loggers { get; set; }
        public virtual sites sites { get; set; }
    }
}
