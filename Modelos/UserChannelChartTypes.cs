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
    
    public partial class UserChannelChartTypes
    {
        public int UserId { get; set; }
        public string ChannelType { get; set; }
        public string ChartType { get; set; }
    
        public virtual users users { get; set; }
    }
}