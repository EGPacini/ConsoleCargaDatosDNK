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
    
    public partial class PredictiveAlarmSetting
    {
        public int id_aset { get; set; }
        public Nullable<bool> enable_alarm { get; set; }
        public Nullable<System.DateTime> date_predictive { get; set; }
        public Nullable<int> sample_type { get; set; }
        public Nullable<int> site_id { get; set; }
        public Nullable<int> channelNumber { get; set; }
        public Nullable<int> suggested_values { get; set; }
    }
}
