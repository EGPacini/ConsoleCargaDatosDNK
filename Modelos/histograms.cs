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
    
    public partial class histograms
    {
        public int ID { get; set; }
        public byte[] BinData { get; set; }
        public Nullable<int> RecordingID { get; set; }
        public Nullable<int> NumSamples { get; set; }
        public Nullable<int> SampleTime { get; set; }
        public Nullable<bool> IsOnLogBoundary { get; set; }
    }
}
