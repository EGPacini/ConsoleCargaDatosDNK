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
    
    public partial class alarmconditions
    {
        public int ID { get; set; }
        public Nullable<int> SiteID { get; set; }
        public Nullable<int> channel { get; set; }
        public Nullable<int> AlarmType { get; set; }
        public Nullable<double> HighAlarmLevel { get; set; }
        public Nullable<double> LowAlarmLevel { get; set; }
        public Nullable<int> PersistenceX { get; set; }
        public Nullable<int> PersistenceY { get; set; }
    }
}