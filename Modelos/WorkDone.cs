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
    
    public partial class WorkDone
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Nullable<int> IdWorkValuation { get; set; }
        public Nullable<int> IdRepairGuideDevice { get; set; }
    
        public virtual RepairGuideDevice RepairGuideDevice { get; set; }
        public virtual WorkValuation WorkValuation { get; set; }
    }
}