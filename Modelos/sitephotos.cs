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
    
    public partial class sitephotos
    {
        public int Id { get; set; }
        public Nullable<int> SiteId { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> UploadDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> InstallReportID { get; set; }
        public Nullable<int> CompletionReportID { get; set; }
        public Nullable<int> Size { get; set; }
        public Nullable<int> MaintenanceID { get; set; }
    }
}
