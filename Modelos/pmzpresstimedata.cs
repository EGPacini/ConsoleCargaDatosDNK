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
    
    public partial class pmzpresstimedata
    {
        public int ID { get; set; }
        public Nullable<int> PmzId { get; set; }
        public Nullable<int> TimeType { get; set; }
        public Nullable<System.TimeSpan> Time { get; set; }
        public Nullable<double> Pressure { get; set; }
        public Nullable<int> DayType { get; set; }
    }
}
