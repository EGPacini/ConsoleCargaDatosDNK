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
    
    public partial class leakconfirmation
    {
        public int ID { get; set; }
        public Nullable<int> SiteID { get; set; }
        public Nullable<System.DateTime> LeakConfirmed { get; set; }
        public Nullable<System.DateTime> LeakDetected { get; set; }
        public string TypeOfLeak { get; set; }
        public Nullable<System.DateTime> LeakRepair { get; set; }
        public Nullable<int> LeakComfirmationAssocaitions { get; set; }
        public string LeakSize { get; set; }
        public string EstimatedGPM { get; set; }
        public string CostPer1000 { get; set; }
        public Nullable<int> EstimatedUnits { get; set; }
        public Nullable<int> MeasurementUnits { get; set; }
        public string leakLongNorth { get; set; }
        public string leakLatitudeEast { get; set; }
        public Nullable<int> LoggerMode { get; set; }
        public string WorkOrderNum { get; set; }
        public string LeakAddress { get; set; }
        public string RaisedAs { get; set; }
        public Nullable<System.DateTime> RepairedDate { get; set; }
        public Nullable<double> JobRaisedNominalESPB { get; set; }
        public Nullable<double> JobRaisedNominalMId { get; set; }
        public string RepairedAs { get; set; }
        public Nullable<double> RepairedAsNominalESPB { get; set; }
        public Nullable<double> RepairedAsNominalMId { get; set; }
        public string LeakRepairCode { get; set; }
        public string ActiveWorkOrder { get; set; }
        public Nullable<int> CorrelationResultId { get; set; }
        public Nullable<System.DateTime> ActualFollowUpDate { get; set; }
        public string LastNote { get; set; }
        public string NoiseFeedback { get; set; }
        public Nullable<System.DateTime> AlarmDate { get; set; }
        public Nullable<System.DateTime> DesktopAnalysisDate { get; set; }
        public string TechnicianName { get; set; }
        public string OperatingPressure { get; set; }
        public Nullable<int> PipeSizeID { get; set; }
        public string PipeMaterialType { get; set; }
        public string Notes { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<bool> Closed { get; set; }
        public Nullable<System.DateTime> ARDate { get; set; }
        public Nullable<System.DateTime> ICDate { get; set; }
        public string LeakFractureShape { get; set; }
        public Nullable<double> LeakWidth { get; set; }
    
        public virtual PipeSize PipeSize { get; set; }
        public virtual sites sites { get; set; }
        public virtual users users { get; set; }
    }
}
