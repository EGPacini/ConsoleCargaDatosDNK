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
    
    public partial class UserUnitLookup
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UnitLookupId { get; set; }
        public string UnitLookupType { get; set; }
    
        public virtual unitslookup unitslookup { get; set; }
        public virtual users users { get; set; }
    }
}
