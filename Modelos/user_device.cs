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
    
    public partial class user_device
    {
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string IP_Address { get; set; }
        public string App2FASecret { get; set; }
        public Nullable<int> Mode2FA { get; set; }
        public string Internal2FAPasscode { get; set; }
        public Nullable<bool> Lock { get; set; }
        public Nullable<System.DateTime> LastLogin { get; set; }
        public string Browser { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string OsInfo { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public Nullable<bool> IsNewDevice { get; set; }
        public Nullable<bool> TrustedDevice { get; set; }
        public Nullable<System.DateTime> TrustedDeviceDate { get; set; }
    
        public virtual users users { get; set; }
    }
}