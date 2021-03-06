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
    using CsvHelper.Configuration;
    using System;
    using System.Collections.Generic;

    public partial class Tickets
    {
        public int id { get; set; }
        public Nullable<int> ticketNumber { get; set; }
        public Nullable<System.DateTime> createDate { get; set; }
        public string siteIDDatagate { get; set; }
        public string currentStatus { get; set; }
        public string teamAssigned { get; set; }
        public Nullable<System.DateTime> closedDateDG { get; set; }
        public Nullable<System.DateTime> lastUpdated { get; set; }
        public string SLAPlan { get; set; }
        public string Overdue { get; set; }
        public string tipoEvento { get; set; }
        public string PCP { get; set; }

        public class TicketClassMap : ClassMap<Tickets>
        {
            public TicketClassMap()
            {
                Map(t => t.ticketNumber).Name("Ticket Number");
                Map(t => t.createDate).Name("Date Created");
                Map(t => t.siteIDDatagate).Name("Subject");
                Map(t => t.currentStatus).Name("Current Status");
                Map(t => t.teamAssigned).Name("Team Assigned");
                Map(t => t.closedDateDG).Name("Closed Date");
                Map(t => t.lastUpdated).Name("Last Updated");
                Map(t => t.SLAPlan).Name("SLA Plan");
                Map(t => t.Overdue).Name("Overdue");
                Map(t => t.tipoEvento).Name("Tipo de Evento");
            }
        }
    }
}