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
    
    public partial class GenerateLeakListReportByAccountId_Result
    {
        public int ID { get; set; }
        public string SiteID { get; set; }
        public string Address { get; set; }
        public string SMSNumber { get; set; }
        public Nullable<System.DateTime> LeakDetected { get; set; }
        public System.DateTime LeakConfirmed { get; set; }
        public string TypeOfLeak { get; set; }
        public System.DateTime LeakRepairDate { get; set; }
        public string LeakSize { get; set; }
        public string EstimatedLeakVolume { get; set; }
        public string Units { get; set; }
        public string CostPer1000 { get; set; }
        public string CalculatedTotalCostOfLeak { get; set; }
        public string CalculatedAnnualized { get; set; }
        public string SitesThatHeardTheLeak { get; set; }
    }
}