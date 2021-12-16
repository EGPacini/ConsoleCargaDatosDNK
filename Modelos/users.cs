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
    
    public partial class users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public users()
        {
            this.InstallationReport = new HashSet<InstallationReport>();
            this.leakconfirmation = new HashSet<leakconfirmation>();
            this.Maintenance = new HashSet<Maintenance>();
            this.recentsites = new HashSet<recentsites>();
            this.SiteLoggerUserActions = new HashSet<SiteLoggerUserActions>();
            this.SMSFailures = new HashSet<SMSFailures>();
            this.user_device = new HashSet<user_device>();
            this.UserChannelChartTypes = new HashSet<UserChannelChartTypes>();
            this.UserNotificationAcknowledgement = new HashSet<UserNotificationAcknowledgement>();
            this.userpasswords = new HashSet<userpasswords>();
            this.UserUnitLookup = new HashSet<UserUnitLookup>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public Nullable<System.DateTime> PasswordUpdated { get; set; }
        public Nullable<bool> Locked { get; set; }
        public Nullable<int> AccessFailedCount { get; set; }
        public Nullable<System.Guid> SessionId { get; set; }
        public Nullable<System.DateTime> SessionUpdated { get; set; }
        public Nullable<System.Guid> ForgotId { get; set; }
        public Nullable<System.DateTime> ForgotUpdated { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int ParentAccount { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public Nullable<bool> LoggedIn { get; set; }
        public string Photo { get; set; }
        public Nullable<int> Role { get; set; }
        public Nullable<bool> RecieveNotifications { get; set; }
        public Nullable<int> AlarmForwardingMethod { get; set; }
        public Nullable<int> EmailNotificationMethod { get; set; }
        public Nullable<int> SMSNotificationMethod { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> MaxAlarmAgeMins { get; set; }
        public string IPAddress { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<int> Language { get; set; }
        public Nullable<bool> RecieveSystemAlarms { get; set; }
        public Nullable<bool> RecieveAlarmClears { get; set; }
        public Nullable<int> DisplayAccMode { get; set; }
        public Nullable<bool> FlippedAccDisplay { get; set; }
        public Nullable<bool> Deactive { get; set; }
        public Nullable<System.Guid> EmailVerifiedId { get; set; }
        public Nullable<System.DateTime> EmailVerified { get; set; }
        public Nullable<int> FlowAlarmUnit { get; set; }
        public Nullable<bool> MapShowLevelAndSpread { get; set; }
        public Nullable<bool> MapShowSiteId { get; set; }
        public Nullable<bool> MapOnlyShowActiveSites { get; set; }
        public Nullable<int> MapShowCorrelationLines { get; set; }
        public Nullable<bool> MapShowHighConfidenceOnly { get; set; }
        public Nullable<bool> MapShowGISMapData { get; set; }
        public Nullable<bool> MapShowMapWithoutGrouping { get; set; }
        public Nullable<bool> EnableReportForwarding { get; set; }
        public Nullable<bool> MapShowGISAssetData { get; set; }
        public Nullable<bool> MapShowGISAssetIdData { get; set; }
        public Nullable<bool> MapShowGISDmaData { get; set; }
        public Nullable<bool> MapShowCsq { get; set; }
        public bool MapShowCorrelationsSubAccounts { get; set; }
        public Nullable<bool> MapShowUseXY { get; set; }
        public string App2FASecret { get; set; }
        public string Internal2FAPasscode { get; set; }
        public string MobilePhoneCode { get; set; }
        public Nullable<bool> MobileSMSVerified { get; set; }
        public Nullable<bool> SSOExemption { get; set; }
        public Nullable<bool> EnabledPredictiveAlarms { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InstallationReport> InstallationReport { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<leakconfirmation> leakconfirmation { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Maintenance> Maintenance { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<recentsites> recentsites { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SiteLoggerUserActions> SiteLoggerUserActions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SMSFailures> SMSFailures { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<user_device> user_device { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserChannelChartTypes> UserChannelChartTypes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserNotificationAcknowledgement> UserNotificationAcknowledgement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userpasswords> userpasswords { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserUnitLookup> UserUnitLookup { get; set; }
    }
}