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
    
    public partial class correlationPath
    {
        public int Id { get; set; }
        public Nullable<int> accountId { get; set; }
        public Nullable<int> loggerxId { get; set; }
        public Nullable<int> loggeryId { get; set; }
        public Nullable<double> leakDistance { get; set; }
        public string leakPath { get; set; }
        public string polyLinePath { get; set; }
        public Nullable<System.DateTime> updated { get; set; }
    
        public virtual accounts accounts { get; set; }
    }
}