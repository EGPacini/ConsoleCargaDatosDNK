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
    
    public partial class statuspegasus
    {
        public int ID { get; set; }
        public Nullable<bool> control { get; set; }
        public Nullable<int> control_type { get; set; }
        public Nullable<double> target_pressure { get; set; }
        public Nullable<int> sol_up_ms { get; set; }
        public Nullable<int> sol_down_ms { get; set; }
        public Nullable<int> deadband_dm { get; set; }
        public Nullable<int> sec_calc_flow { get; set; }
        public Nullable<int> loggerID { get; set; }
        public Nullable<System.DateTime> lastdatereceived { get; set; }
        public Nullable<int> gain { get; set; }
    }
}