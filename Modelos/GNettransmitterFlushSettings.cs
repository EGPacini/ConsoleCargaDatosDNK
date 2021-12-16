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
    
    public partial class GNettransmitterFlushSettings
    {
        public int Id { get; set; }
        public Nullable<int> TransmitterId { get; set; }
        public Nullable<System.DateTime> UpdateTime { get; set; }
        public Nullable<int> MinimumHotValveOpenTime { get; set; }
        public Nullable<int> MinimumColdValveOpenTime { get; set; }
        public Nullable<int> MaximumHotValveOpenTime { get; set; }
        public Nullable<int> MaximumColdValveOpenTime { get; set; }
        public Nullable<int> MaxStagnationTime { get; set; }
        public Nullable<int> DaysOfweek { get; set; }
        public Nullable<int> FlushStart { get; set; }
        public Nullable<int> FlushEnd { get; set; }
        public Nullable<double> HotTargetTemp { get; set; }
        public Nullable<double> ColdTargetTemp { get; set; }
        public Nullable<int> GroupFlushControl { get; set; }
        public Nullable<int> GroupFlushRepetition { get; set; }
        public Nullable<int> GroupFlushTimeOffset { get; set; }
        public Nullable<bool> GroupFlushHotEnabled { get; set; }
        public Nullable<bool> GroupFlushColdEnabled { get; set; }
        public Nullable<int> GroupFlushDuration { get; set; }
        public Nullable<System.DateTime> GroupFlushStartDate { get; set; }
        public Nullable<bool> IsScheduledFlush { get; set; }
    }
}