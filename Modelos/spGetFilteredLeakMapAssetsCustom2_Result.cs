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
    
    public partial class spGetFilteredLeakMapAssetsCustom2_Result
    {
        public int sid { get; set; }
        public Nullable<int> LoggerID { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public Nullable<int> OwnerAccount { get; set; }
        public Nullable<bool> AlarmState { get; set; }
        public Nullable<bool> FlowLogger { get; set; }
        public string LatEast { get; set; }
        public Nullable<bool> LeakState { get; set; }
        public string LongNorth { get; set; }
        public Nullable<bool> PressureLogger { get; set; }
        public Nullable<int> quietdays { get; set; }
        public Nullable<int> QuietMins { get; set; }
        public string SiteID { get; set; }
        public Nullable<System.DateTime> LastLeakDate { get; set; }
        public Nullable<double> Noise { get; set; }
        public Nullable<double> Spread { get; set; }
        public Nullable<int> lid { get; set; }
        public Nullable<System.DateTime> LastCallIn { get; set; }
        public Nullable<double> MastLatitude { get; set; }
        public Nullable<double> MastLongitude { get; set; }
        public Nullable<int> LoggerMode { get; set; }
        public Nullable<int> accid { get; set; }
        public Nullable<int> accquietdays { get; set; }
    }
}
