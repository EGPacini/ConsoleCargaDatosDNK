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
    
    public partial class BehaviorHidraulic
    {
        public int id { get; set; }
        public string siteIDDatagate { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public Nullable<double> value { get; set; }
        public Nullable<int> channelnum { get; set; }
        public string channeltype { get; set; }
    }
}