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
    
    public partial class LoggerFilterConfiguration
    {
        public int ID { get; set; }
        public int LoggerId { get; set; }
        public short FilterType { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> LowCutoff { get; set; }
        public Nullable<int> HighCutoff { get; set; }
        public Nullable<System.DateTime> SourceConfigurationUpdateDate { get; set; }
    
        public virtual loggers loggers { get; set; }
    }
}