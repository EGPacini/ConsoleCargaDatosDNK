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
    
    public partial class spGetAllAssociatedSites_Result
    {
        public Nullable<int> Id { get; set; }
        public string SiteId { get; set; }
        public string LoggerType { get; set; }
        public string LoggerSoftware { get; set; }
        public string LoggerTypeFriendlyName { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<int> SiteQuietDays { get; set; }
        public Nullable<int> AccountQuietDays { get; set; }
        public Nullable<System.DateTime> LastCallIn { get; set; }
        public string OwnerAccount { get; set; }
        public string SmsNumber { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<int> AccountId { get; set; }
        public Nullable<int> OwnerAccountId { get; set; }
        public string Address2 { get; set; }
        public string LoggerGSMNumber { get; set; }
        public Nullable<double> BatteryLevel { get; set; }
        public Nullable<int> SignalLevel { get; set; }
    }
}
