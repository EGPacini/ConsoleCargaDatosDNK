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
    
    public partial class datapoints
    {
        public int ID { get; set; }
        public Nullable<int> SiteID { get; set; }
        public Nullable<System.DateTime> DataTime { get; set; }
        public Nullable<int> ChannelNumber { get; set; }
        public Nullable<double> value { get; set; }
    
        public virtual sites sites { get; set; }
    }
}
