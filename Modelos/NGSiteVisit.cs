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
    
    public partial class NGSiteVisit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NGSiteVisit()
        {
            this.NGSiteVisitStockItems = new HashSet<NGSiteVisitStockItems>();
        }
    
        public string NGMReference { get; set; }
        public string JobType { get; set; }
        public string MeterPointReference { get; set; }
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public System.DateTime DateRequestReceived { get; set; }
        public Nullable<System.DateTime> ExpectedPlanDate { get; set; }
        public Nullable<System.DateTime> TargetDate { get; set; }
        public string JobOutcome { get; set; }
        public Nullable<System.DateTime> DateOfOutcome { get; set; }
        public string OutcomeReason { get; set; }
        public int NgStatus { get; set; }
        public string TransactionStatus { get; set; }
        public string MeterSerialNumber { get; set; }
        public string MeterManufacturer { get; set; }
        public string MeterModel { get; set; }
        public string MeterYearOfManufacture { get; set; }
        public string ReadFrequency { get; set; }
        public Nullable<bool> IsAlarmRequired { get; set; }
        public string AlarmHigh { get; set; }
        public string AlarmLow { get; set; }
        public string AlarmPeriod { get; set; }
        public string AlarmNotification { get; set; }
        public string AnnualQuantity { get; set; }
        public string SiteContact { get; set; }
        public string SiteContactNumber { get; set; }
        public string AccessInstructions { get; set; }
        public string SiteAddress { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public Nullable<bool> IsDMLoggerOnSite { get; set; }
        public Nullable<bool> IsConverterFitted { get; set; }
        public Nullable<int> NumberOfDials { get; set; }
        public Nullable<int> MeterReadingFactor { get; set; }
        public Nullable<double> CalibrationPulseSignificance { get; set; }
        public string ImperialMetric { get; set; }
        public string MeterReading { get; set; }
        public string ArrivalMeterReading { get; set; }
        public string DepartMeterReading { get; set; }
        public Nullable<System.DateTime> TimeOfMainMeterRead { get; set; }
        public string ReadFlag { get; set; }
        public Nullable<int> MeterConsumption { get; set; }
        public Nullable<int> CorrectedConsumption { get; set; }
        public string ConverterSerialNumber { get; set; }
        public Nullable<int> ConverterNumberOfDials { get; set; }
        public string ConverterReading { get; set; }
        public string ArrivalConverterReading { get; set; }
        public string ArrivalConverterUncorrectedReading { get; set; }
        public string DepartConverterReading { get; set; }
        public string AMRModelCode { get; set; }
        public string AMRSerialNumber { get; set; }
        public string AMRYearOfManufacture { get; set; }
        public string AMRMeterReading { get; set; }
        public string AMRConverterReading { get; set; }
        public string ArrivalAMRMeterReading { get; set; }
        public string DepartAMRMeterReading { get; set; }
        public string ArrivalAMRConverterReading { get; set; }
        public string DepartAMRConverterReading { get; set; }
        public Nullable<bool> IsOCRFitted { get; set; }
        public string OCRSerialNumber { get; set; }
        public string GPS { get; set; }
        public string MeterInspection { get; set; }
        public Nullable<bool> IsBatteryChange { get; set; }
        public string Comments { get; set; }
        public string RequestNGMReference { get; set; }
        public bool IsExported { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public Nullable<System.DateTime> DateAssigned { get; set; }
        public Nullable<int> AerialPosition { get; set; }
        public Nullable<int> AerialUsed { get; set; }
        public string UncorrectedReading { get; set; }
        public Nullable<int> FinalSignalStrength { get; set; }
        public Nullable<bool> IsGPRSTestPassed { get; set; }
        public string LocationDescription { get; set; }
        public string MaintenanceRequired { get; set; }
        public string MaintenanceCarriedOut { get; set; }
        public Nullable<decimal> XCoord { get; set; }
        public Nullable<decimal> YCoord { get; set; }
        public Nullable<bool> IsChannel1Used { get; set; }
        public Nullable<bool> IsChannel2Used { get; set; }
        public Nullable<bool> IsPriorSiteAccessRequired { get; set; }
        public Nullable<bool> IsTwoManVisitRequired { get; set; }
        public Nullable<bool> IsGasDetectorRequired { get; set; }
        public Nullable<bool> IsVirtualRiskAssessmentCompleted { get; set; }
        public string SMSNumber { get; set; }
        public string UncorrectedMeterSerialNumber { get; set; }
        public string UncorrectedTimeOfMeterRead { get; set; }
        public Nullable<int> UncorrectedMeterBillableDials { get; set; }
        public string SiteSpecificRequirements { get; set; }
        public string HWMSerialNumber { get; set; }
        public string HWMMeterSerialNumber { get; set; }
        public string KeysRequired { get; set; }
        public Nullable<int> ReportCompletedBy { get; set; }
        public Nullable<System.DateTime> ReportCompletedDate { get; set; }
        public Nullable<bool> IsLoggerOutsideBoundary { get; set; }
        public Nullable<System.DateTime> TimeOfConverterReading { get; set; }
        public string HWMMPRN { get; set; }
        public string DepartUncorrectedReading { get; set; }
        public bool SignedOff { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NGSiteVisitStockItems> NGSiteVisitStockItems { get; set; }
    }
}
