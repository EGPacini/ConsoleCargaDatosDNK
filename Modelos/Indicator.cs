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
    
    public partial class Indicator
    {
        public int Id { get; set; }
        public string SiteID { get; set; }
        public string Month { get; set; }
        public Nullable<int> Week { get; set; }
        public string Channel { get; set; }
        public Nullable<double> Minimum { get; set; }
        public Nullable<double> Maximum { get; set; }
        public Nullable<double> Average { get; set; }
        public Nullable<double> Median { get; set; }
        public Nullable<System.DateTime> FirstMinDate { get; set; }
        public Nullable<System.DateTime> FirstMaxDate { get; set; }
        public Nullable<int> MeasuresCount { get; set; }
        public Nullable<int> MinCount { get; set; }
        public Nullable<int> MaxCount { get; set; }
    }
}