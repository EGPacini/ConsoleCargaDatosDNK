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
    
    public partial class UsedSupplies
    {
        public int ID { get; set; }
        public int UsedQuantity { get; set; }
        public Nullable<int> IdWorkValuation { get; set; }
        public Nullable<int> IdSupplies { get; set; }
    
        public virtual Supplies Supplies { get; set; }
        public virtual WorkValuation WorkValuation { get; set; }
    }
}
