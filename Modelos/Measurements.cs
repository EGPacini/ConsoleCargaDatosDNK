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
    
    public partial class Measurements
    {
        public long SurveyID { get; set; }
        public System.DateTime DataTime { get; set; }
        public long Min { get; set; }
        public long Max { get; set; }
        public long Mean { get; set; }
        public long HpFrequency { get; set; }
        public long LpFrequency { get; set; }
        public long Gain { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; }
        public bool HasRecording { get; set; }
        public string Comment { get; set; }
        public long ID { get; set; }
    }
}