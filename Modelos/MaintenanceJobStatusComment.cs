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
    
    public partial class MaintenanceJobStatusComment
    {
        public int ID { get; set; }
        public Nullable<int> MaintenanceJobId { get; set; }
        public Nullable<int> Status { get; set; }
        public string Comment { get; set; }
        public Nullable<int> CommentBy { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
    }
}
