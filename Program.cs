using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoIt;
using ConsoleCargaDatosDNK.Classes;
using ConsoleCargaDatosDNK.Modelos;
using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using MathNet.Numerics.Statistics;
using System.Collections.Generic;
using static ConsoleCargaDatosDNK.Modelos.Tickets;
using System.Text;

namespace ConsoleCargaDatosDNK
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose method to run:");
            Console.WriteLine("1) Hidraulics");
            Console.WriteLine("2) Instrumentation");
            Console.WriteLine("3) Ticket Reader");
            Console.WriteLine("4) Communication History");
            Console.WriteLine("5) Data Evaluation");
            Console.Write("\r\nSelect an option: ");
            
                switch (Console.ReadLine())
                {
                    case "1":
                        HidraulicData();
                        break;

                    case "2":
                        DatosIntrumentacion();
                        break;

                    case "3":
                        TicketReader();
                        break;

                    case "4":
                        TestComms();
                        break;

                    case "5":
                        ValuesEvaluation();
                        break;
                }         
        }

        public static void CommsHistory()
        {
            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            TechnicalServiceEntities dbst = new TechnicalServiceEntities();

            var sitesquery = (from s in localdb2.sites
                              join l in localdb2.loggers
                              on s.LoggerID equals l.ID
                              join a in localdb2.accounts
                              on s.OwnerAccount equals a.ID
                              where a.ID == 5 || a.ID == 6 || a.ID == 10
                              select new { l.LoggerSMSNumber, s.SiteID, s.ID }).ToList();


            Parallel.For(0, sitesquery.Count, new ParallelOptions { MaxDegreeOfParallelism = 20 }, i =>
            {
                LocalHWMEntities auxdb2 = new LocalHWMEntities();
                ContratoMantenimientoEntities auxdb3 = new ContratoMantenimientoEntities();

                var sms = sitesquery[i].LoggerSMSNumber;

                var lastComm = (from hc in auxdb3.HistorialComunicaciones where hc.SMSNumber == sms select hc.Fecha).Max();

                var lastCommRange = (from hc in auxdb3.HistorialComunicaciones where hc.Fecha > lastComm select hc).ToList();

                var sitesqueryID = sitesquery[i].ID;
                var SiteID = sitesquery[i].SiteID;
                Debug.WriteLine("Trabajando en el site: " + SiteID);

                var historynumber = (from lr in auxdb2.loggerrecordings
                                     where lr.Site_ID == sitesqueryID
                                     select new { lr.LoggerID }).Distinct().ToList();

                foreach (var l in historynumber)
                {
                    var number = (from lo in auxdb2.loggers
                                  where lo.ID == l.LoggerID
                                  select lo).FirstOrDefault();

                    var PerNumberRange = (from m in auxdb2.messages
                                          where m.smsnumber == number.LoggerSMSNumber
                                          select new 
                                          {
                                              Start = (from mg in auxdb2.messages where m.smsnumber == number.LoggerSMSNumber select m.rxtime).Min(),
                                              End =   (from mg in auxdb2.messages where m.smsnumber == number.LoggerSMSNumber select m.rxtime).Max()
                                          }).FirstOrDefault();

                    DateTime startDate = Convert.ToDateTime(PerNumberRange.Start);
                    DateTime endDate = Convert.ToDateTime(PerNumberRange.End);
                    TimeSpan go = new TimeSpan(0,0,0);
                    TimeSpan stop = new TimeSpan(0, 0, 0);

                    DateTime rangeStart = startDate.Add(go);
                    DateTime rangeEnd = startDate.Add(stop);

                    var allCommsQuery = (from mess in auxdb2.messages
                                         where mess.smsnumber == number.LoggerSMSNumber 
                                         && mess.rxtime > rangeStart
                                         && mess.rxtime < rangeEnd
                                         select mess.rxtime
                                        ).Distinct().ToList();

                    if (allCommsQuery.Count() > 0)
                    {
                        while (startDate <= endDate)
                        {
                            var ActualDate = Convert.ToDateTime(startDate.ToString("dd-MM-yyyy"));
                            var thisDayComms = allCommsQuery.Where(x => x.Value.Date == startDate.Date).ToList();
                            int commsCounter = thisDayComms.Count();

                            if (commsCounter > 0)
                            {
                                DateTime? prevCommDate = null;
                                int auxCommCounter = 0;
                                foreach (var thisComm in thisDayComms)
                                {
                                    if (prevCommDate == null)
                                    {
                                        prevCommDate = thisComm;
                                        auxCommCounter++;
                                    }
                                    else
                                    {
                                        Console.WriteLine(prevCommDate);
                                        Console.WriteLine(thisComm);
                                        double TimeDiff = (thisComm - prevCommDate).Value.TotalMinutes;

                                        if (TimeDiff > 60)
                                        {
                                            Console.WriteLine("Diff de " + TimeDiff);
                                            auxCommCounter++;
                                        }
                                        prevCommDate = thisComm;
                                    }
                                }
                                Console.WriteLine("El numero {0} se ha comunicado {1} veces en el dia {2}, y en total: {3} \n", number.LoggerSMSNumber, auxCommCounter, startDate.ToString("dd-MM-yyy"), commsCounter);
                                ContratoMantenimientoEntities auxdb = new ContratoMantenimientoEntities();
                                HistorialComunicaciones hc = new HistorialComunicaciones
                                {
                                    SiteID = SiteID,
                                    Fecha = ActualDate,
                                    NumeroComms = auxCommCounter,
                                    SMSNumber = number.LoggerSMSNumber,
                                    TotalComms = commsCounter
                                };
                                auxdb.HistorialComunicaciones.Add(hc);
                                auxdb.SaveChanges();
                            }
                            else
                            {
                                Console.WriteLine("El numero {0} se ha comunicado 0 veces en el dia {1}, y en total: {2} \n", number.LoggerSMSNumber, startDate.ToString("dd-MM-yyy"), commsCounter);
                                ContratoMantenimientoEntities auxdb = new ContratoMantenimientoEntities();
                                HistorialComunicaciones hc = new HistorialComunicaciones
                                {
                                    SiteID = SiteID,
                                    Fecha = ActualDate,
                                    NumeroComms = 0,
                                    SMSNumber = number.LoggerSMSNumber,
                                    TotalComms = commsCounter
                                };
                                auxdb.HistorialComunicaciones.Add(hc);
                                auxdb.SaveChanges();
                            }
                            startDate = startDate.AddDays(1);
                        }
                    }
                }               
            });
        }
        public static void ServicioTecnico()
        {
            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            TechnicalServiceEntities dbst = new TechnicalServiceEntities();

            var auditList = dbst.Audit.ToList();
            var suppliesList = (from t in auditList where t.ActionNotes.ToLower().Contains("supplie mobile added") select t).ToList();

            foreach (var item in suppliesList)
            {
                string line = item.ActionNotes;

                DateTime dateLine = item.AuditDate;
                string deli = "/n";

                string[] valores = Regex.Split(line, deli);

                InsumosST newAudit = new InsumosST();
                int indexador2 = 0;

                foreach (string val in valores)
                {
                    string[] values = Regex.Split(val, ":|\\.");
                    int indexador = 0;
                    foreach (string v in values)
                    {
                        string newV = v.TrimStart();

                        if (indexador == 1)
                        {
                            switch (indexador2)
                            {
                                case 0: break;

                                case 1:
                                    newAudit.SupplyID = Convert.ToInt32(newV);
                                    break;
                                case 2:
                                    newAudit.Supply = newV;
                                    break;
                                case 3:
                                    newAudit.Descripcion = newV;
                                    break;
                                case 4:
                                    newAudit.CurrentStock = Convert.ToInt32(newV);
                                    break;
                                case 5:
                                    newAudit.MinStock = Convert.ToInt32(newV);
                                    break;
                                case 6:
                                    newAudit.SquadID = Convert.ToInt32(newV);
                                    break;
                                case 7:
                                    newAudit.AuditDate = dateLine;
                                    break;
                            }
                            indexador2++;
                        }
                        indexador++;
                    }
                };
                Console.WriteLine("\n------------------------------------------------");
                Console.WriteLine(newAudit.SupplyID);
                Console.WriteLine(newAudit.Supply);
                Console.WriteLine(newAudit.Descripcion);
                Console.WriteLine(newAudit.CurrentStock);
                Console.WriteLine(newAudit.MinStock);
                Console.WriteLine(newAudit.SquadID);
                Console.WriteLine(newAudit.AuditDate);
                Console.WriteLine("------------------------------------------------");
                db.InsumosST.Add(newAudit);
                db.SaveChanges();
            }

        }
        public static void ValuesEvaluation()
        {
            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            TechnicalServiceEntities dbst = new TechnicalServiceEntities();

            var sitesquery = (from s in localdb2.sites
                              join l in localdb2.loggers
                              on s.LoggerID equals l.ID
                              join a in localdb2.accounts
                              on s.OwnerAccount equals a.ID
                              where a.ID == 5 || a.ID == 6 || a.ID == 10
                              select new { l.LoggerSMSNumber, s.SiteID, s.ID }).ToList();

            CultureInfo myCI = new CultureInfo("es-CL");
            Calendar myCal = myCI.Calendar;
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            int c = 0;

            Parallel.For(0, sitesquery.Count, new ParallelOptions { MaxDegreeOfParallelism = 30 }, i =>
            {
                var SiteID = sitesquery[i].SiteID;

                ContratoMantenimientoEntities dbAux = new ContratoMantenimientoEntities();

                var device = (from n in dbAux.SitesMtto
                              where n.siteIDDatagate == SiteID
                              select n.MeasuresDevice).FirstOrDefault();

                c++;

                DateTime Date = new DateTime(2021, 1, 3, 00,00,00);
                DateTime DateEnd = new DateTime(2022, 1, 9, 23,59,00);
 
                double numberOfWeeks = ((DateEnd - Date).TotalDays) / 7;

                for (int y = 1; y <= numberOfWeeks; y++)
                {
                    DateEnd = Date.AddDays(7);
                    DateEnd = DateEnd.AddMinutes(-1);

                    var RowRange = (from bh in dbAux.BehaviorHidraulic
                                    where bh.siteIDDatagate == SiteID
                                    && bh.datetime >= Date
                                    && bh.datetime < DateEnd
                                    select bh).ToList();

                    if (Date.Month == DateEnd.Month)
                    {

                        Debug.WriteLine("Site en evaluación: " + SiteID + " -- " + c);
                        Debug.WriteLine("");

                        if (device == "1P")
                        {
                            Channel1Aux(RowRange, SiteID, y, device);
                        }
                        else if (device == "2P")
                        {
                            Channel1Aux(RowRange, SiteID, y, device);
                            Channel3Aux(RowRange, SiteID, y, device);
                        }
                        else if (device == "1P1Q")
                        {
                            Channel1Aux(RowRange, SiteID, y, device);
                            Channel2Aux(RowRange, SiteID, y, device);
                        }
                        else if (device == "2P1Q")
                        {
                            Channel1Aux(RowRange, SiteID, y, device);
                            Channel2Aux(RowRange, SiteID, y, device);
                            Channel3Aux(RowRange, SiteID, y, device);
                        }

                        Date = Date.AddDays(7);
                        DateEnd = DateEnd.AddDays(7);
                    }
                    else
                    {
                        if(RowRange.Any() == true)
                        {
                            List<BehaviorHidraulic> list1 = new List<BehaviorHidraulic>();
                            List<BehaviorHidraulic> list2 = new List<BehaviorHidraulic>();
                            var firstElement = RowRange.First();

                            DateTime prevIpDate = Convert.ToDateTime(firstElement.datetime);
                            DateTime defaultIpDate = Convert.ToDateTime(firstElement.datetime);
                            int t = 0;

                            foreach (var ip in RowRange)
                            {
                                DateTime thisIpDate = Convert.ToDateTime(ip.datetime);

                                if (t == 0)
                                {
                                    list1.Add(ip);
                                    prevIpDate = thisIpDate;
                                }
                                else
                                {
                                    var ThisFecha = Convert.ToDateTime(ip.datetime);

                                    if (thisIpDate.Month == prevIpDate.Month)
                                    {
                                        list1.Add(ip);
                                        prevIpDate = thisIpDate;
                                    }
                                    else
                                    {
                                        list2.Add(ip);
                                        prevIpDate = defaultIpDate;
                                    }
                                }
                                t++;
                            }

                            if (device == "1P")
                            {
                                Channel1Aux(list1, SiteID, y, device);
                                Channel1Aux(list2, SiteID, y, device);
                            }
                            else if (device == "2P")
                            {
                                Channel1Aux(list1, SiteID, y, device);
                                Channel3Aux(list1, SiteID, y, device);
                                Channel1Aux(list2, SiteID, y, device);
                                Channel3Aux(list2, SiteID, y, device);
                            }
                            else if (device == "1P1Q")
                            {
                                Channel1Aux(list1, SiteID, y, device);
                                Channel2Aux(list1, SiteID, y, device);
                                Channel1Aux(list2, SiteID, y, device);
                                Channel2Aux(list2, SiteID, y, device);
                            }
                            else if (device == "2P1Q")
                            {
                                Channel1Aux(list1, SiteID, y, device);
                                Channel2Aux(list1, SiteID, y, device);
                                Channel3Aux(list1, SiteID, y, device);
                                Channel1Aux(list2, SiteID, y, device);
                                Channel2Aux(list2, SiteID, y, device);
                                Channel3Aux(list2, SiteID, y, device);
                            }
                            Date = Date.AddDays(7);
                            DateEnd = DateEnd.AddDays(7);
                        }
                    }
                }
            });           
        }
        public static void Channel1Aux(List<BehaviorHidraulic> hidraulicData, string SiteID, int y, string device)
        {
            var CH1 = hidraulicData.FindAll(x => x.channelnum == 1).ToList();
            var valuesListCh1 = CH1.ConvertAll(x => x.value).OrderBy(x => x.Value);
            double MedianCH1 = 0;
            double realMedianCH1 = 0;
            double AverageCH1 = 0;

            var OverRangeCH1 = CH1.FindAll(x => x.value > 100).Count();
            var UnderRangeCH1 = CH1.FindAll(x => x.value < 0).Count();

            if (valuesListCh1.Any() == true)
            {
                AverageCH1 = Convert.ToDouble(valuesListCh1.Average());
            }
            if (valuesListCh1.Any() == true)
            {
                MedianCH1 = valuesListCh1.Median();
                realMedianCH1 = Convert.ToDouble(string.Format("{0:F2}", MedianCH1));
            }


            var MaxValueCH1 = CH1.Max(x => x.value);
            string RoundedMaxValueCH1 = string.Format("{0:F2}", MaxValueCH1);
            var MinValueCH1 = CH1.Min(x => x.value);
            string RoundedMinValueCH1 = string.Format("{0:F2}", MinValueCH1);
            var MatchesMinCH1 = (from k in hidraulicData where k.value == MinValueCH1 select k).ToList();
            var MatchesMaxCH1 = (from k in hidraulicData where k.value == MaxValueCH1 select k).ToList();
            var MeasuresCount = (from ct in hidraulicData where ct.channelnum == 1 select ct).Count();
            var MaxValMatchesInRangeCH1 = (from k in hidraulicData where k.channelnum == 1 && k.value == MaxValueCH1 select k).Count();
            var MinValMatchesInRangeCH1 = (from k in hidraulicData where k.channelnum == 1 && k.value == MinValueCH1 select k).Count();
            var FirstMinDateCH1 = (from ct in MatchesMinCH1 select ct.datetime).Min();


            DateTime ctm = Convert.ToDateTime(FirstMinDateCH1);
            DateTime newdate = new DateTime(2021, 10, 1);
            var FirstMaxDateCH1 = (from ct in MatchesMaxCH1 select ct.datetime).Min();

            var month = "";

            if (FirstMinDateCH1 == null)
            {
                month = "SIN DATOS";
            }
            else
            {
                month = Convert.ToDateTime(FirstMinDateCH1).ToString("MMMM").ToUpper();
            }

            Indicator ind = new Indicator
            {
                SiteID = SiteID,
                Week = y,
                Month = month,
                Channel = "1",
                Minimum = Convert.ToDouble(MinValueCH1),
                Maximum = Convert.ToDouble(MaxValueCH1),
                Average = AverageCH1,
                Median = Convert.ToDouble(realMedianCH1),
                FirstMinDate = FirstMinDateCH1,
                FirstMaxDate = FirstMaxDateCH1,
                MeasuresCount = MeasuresCount,
                MinCount = MinValMatchesInRangeCH1,
                MaxCount = MaxValMatchesInRangeCH1,
                OORMeasures = OverRangeCH1 + UnderRangeCH1,
                Device = device
            };
            ContratoMantenimientoEntities dbAux2 = new ContratoMantenimientoEntities();
            dbAux2.Indicator.Add(ind);
            Console.WriteLine("Site: {0}, Month: {1}, Channel: {2}, Week: {3}, Device: {4}", ind.SiteID,ind.Month,ind.Channel,ind.Week,ind.Device);
            dbAux2.SaveChanges();
        }
        public static void Channel2Aux(List<BehaviorHidraulic> hidraulicData, string SiteID, int y, string device)
        {
            var CH2 = hidraulicData.FindAll(x => x.channelnum == 2).ToList();
            var valuesListCh2 = CH2.ConvertAll(x => x.value).OrderBy(x => x.Value);
            double MedianCH2 = 0;
            double realMedianCH2 = 0;
            double AverageCH2 = 0;

            var OverRangeCH2 = CH2.FindAll(x => x.value > 100).Count();
            var UnderRangeCH2 = CH2.FindAll(x => x.value < 0).Count();

            if (valuesListCh2.Any() == true)
            {
                AverageCH2 = Convert.ToDouble(valuesListCh2.Average());
            }
            if (valuesListCh2.Any() == true)
            {
                MedianCH2 = valuesListCh2.Median();
                realMedianCH2 = Convert.ToDouble(string.Format("{0:F2}", MedianCH2));
            }

            var MaxValueCH2 = CH2.Max(x => x.value);
            string RoundedMaxValueCH2 = string.Format("{0:F2}", MaxValueCH2);
            var MinValueCH2 = CH2.Min(x => x.value);
            string RoundedMinValueCH2 = string.Format("{0:F2}", MinValueCH2);
            var MatchesMinCH2 = (from k in hidraulicData where k.value == MinValueCH2 select k).ToList();
            var MatchesMaxCH2 = (from k in hidraulicData where k.value == MaxValueCH2 select k).ToList();
            var MeasuresCount = (from ct in hidraulicData where ct.channelnum == 2 select ct).Count();
            var MaxValMatchesInRangeCH2 = (from k in hidraulicData where k.channelnum == 2 && k.value == MaxValueCH2 select k).Count();
            var MinValMatchesInRangeCH2 = (from k in hidraulicData where k.channelnum == 2 && k.value == MinValueCH2 select k).Count();
            var FirstMinDateCH2 = (from ct in MatchesMinCH2 select ct.datetime).Min();
            var FirstMaxDateCH2 = (from ct in MatchesMaxCH2 select ct.datetime).Min();
            DateTime ctm = Convert.ToDateTime(FirstMinDateCH2);
            DateTime newdate = new DateTime(2021, 10, 1);
            var month = "";
            if (FirstMinDateCH2 == null)
            {
                month = "SIN DATOS";
            }
            else
            {
                month = Convert.ToDateTime(FirstMinDateCH2).ToString("MMMM").ToUpper();
            }

            Indicator ind = new Indicator
            {
                SiteID = SiteID,
                Week = y,
                Month = month,
                Channel = "2",
                Minimum = Convert.ToDouble(MinValueCH2),
                Maximum = Convert.ToDouble(MaxValueCH2),
                Average = AverageCH2,
                Median = Convert.ToDouble(realMedianCH2),
                FirstMinDate = FirstMinDateCH2,
                FirstMaxDate = FirstMaxDateCH2,
                MeasuresCount = MeasuresCount,
                MinCount = MinValMatchesInRangeCH2,
                MaxCount = MaxValMatchesInRangeCH2,
                OORMeasures = OverRangeCH2 + UnderRangeCH2,
                Device = device
            };
            ContratoMantenimientoEntities dbAux2 = new ContratoMantenimientoEntities();
            dbAux2.Indicator.Add(ind);
            Console.WriteLine("Site: {0}, Month: {1}, Channel: {2}, Week: {3}, Device: {4}", ind.SiteID, ind.Month, ind.Channel, ind.Week, ind.Device);
            dbAux2.SaveChanges();
        }
        public static void Channel3Aux(List<BehaviorHidraulic> hidraulicData, string SiteID, int y, string device)
        {
            var CH3 = hidraulicData.FindAll(x => x.channelnum == 3).ToList();
            var valuesListCh3 = CH3.ConvertAll(x => x.value).OrderBy(x => x.Value);
            double MedianCH3 = 0;
            double realMedianCH3 = 0;
            double AverageCH3 = 0;

            var OverRangeCH3 = CH3.FindAll(x => x.value > 100).Count();
            var UnderRangeCH3 = CH3.FindAll(x => x.value < 0).Count();

            if (valuesListCh3.Any() == true)
            {
                AverageCH3 = Convert.ToDouble(valuesListCh3.Average());
            }
            if (valuesListCh3.Any() == true)
            {
                MedianCH3 = valuesListCh3.Median();
                realMedianCH3 = Convert.ToDouble(string.Format("{0:F2}", MedianCH3));
            }

            var MaxValueCH3 = CH3.Max(x => x.value);
            string RoundedMaxValueCH3 = string.Format("{0:F2}", MaxValueCH3);
            var MinValueCH3 = CH3.Min(x => x.value);
            string RoundedMinValueCH3 = string.Format("{0:F2}", MinValueCH3);
            var MatchesMinCH3 = (from k in hidraulicData where k.value == MinValueCH3 select k).ToList();
            var MatchesMaxCH3 = (from k in hidraulicData where k.value == MaxValueCH3 select k).ToList();
            var MeasuresCount = (from ct in hidraulicData where ct.channelnum == 3 select ct).Count();
            var MaxValMatchesInRangeCH3 = (from k in hidraulicData where k.channelnum == 3 && k.value == MaxValueCH3 select k).Count();
            var MinValMatchesInRangeCH3 = (from k in hidraulicData where k.channelnum == 3 && k.value == MinValueCH3 select k).Count();
            var FirstMinDateCH3 = (from ct in MatchesMinCH3 select ct.datetime).Min();
            var FirstMaxDateCH3 = (from ct in MatchesMaxCH3 select ct.datetime).Min();
            DateTime ctm = Convert.ToDateTime(FirstMinDateCH3);
            DateTime newdate = new DateTime(2021, 10, 1);
            var month = "";
            if (FirstMinDateCH3 == null)
            {
                month = "SIN DATOS";
            }
            else
            {
                month = Convert.ToDateTime(FirstMinDateCH3).ToString("MMMM").ToUpper();
            }

            Indicator ind = new Indicator
            {
                SiteID = SiteID,
                Week = y,
                Month = month,
                Channel = "3",
                Minimum = Convert.ToDouble(MinValueCH3),
                Maximum = Convert.ToDouble(MaxValueCH3),
                Average = AverageCH3,
                Median = Convert.ToDouble(realMedianCH3),
                FirstMinDate = FirstMinDateCH3,
                FirstMaxDate = FirstMaxDateCH3,
                MeasuresCount = MeasuresCount,
                MinCount = MinValMatchesInRangeCH3,
                MaxCount = MaxValMatchesInRangeCH3,
                OORMeasures = OverRangeCH3 + UnderRangeCH3,
                Device = device
            };
            ContratoMantenimientoEntities dbAux2 = new ContratoMantenimientoEntities();
            dbAux2.Indicator.Add(ind);
            Console.WriteLine("Site: {0}, Month: {1}, Channel: {2}, Week: {3}, Device: {4}", ind.SiteID, ind.Month, ind.Channel, ind.Week, ind.Device);
            dbAux2.SaveChanges();
        }
        public static void DatosIntrumentacion()
        {
            HidraulicTestEntities db = new HidraulicTestEntities();
            hwmdbEntities db2 = new hwmdbEntities();

            var query = (from s in db2.sites
                         join l in db2.loggers on s.LoggerID equals l.ID
                         join a in db2.accounts on s.OwnerAccount equals a.ID
                         where a.ID == 5 || a.ID == 6 || a.ID == 10
                         select new { l.LoggerSMSNumber, s.SiteID }).ToList();

            Parallel.For(0, query.Count, new ParallelOptions { MaxDegreeOfParallelism = 25 }, i =>
            {
                HidraulicTestEntities aux = new HidraulicTestEntities();

                var SiteNumber = query[i].LoggerSMSNumber;
                var SiteID = query[i].SiteID;

                var lastInsert = (from b in aux.BehaviorInstrumentation where b.siteIDDatagate == SiteID select b.lastCallIn).Max();

                if(lastInsert != null)
                {
                    var ytd = DateTime.Today.AddDays(-1);                   
                    var shortytd = ytd.ToShortDateString();
                    DateTime format = DateTime.ParseExact(shortytd, "dd/MM/yy", CultureInfo.InvariantCulture);

                    TimeSpan midnight = new TimeSpan(23, 59, 59);
                    var Yesterday = Convert.ToDateTime(format).Add(midnight);

                    var latestData = InstrumentationGetter.GetDataFromAPI(SiteNumber, (DateTime) lastInsert, Yesterday);

                    var q = from item in latestData
                            group item by item.callin.ToString("MM/dd/yyyy HH:mm") into ItemGroup
                            select new
                            {
                                id = ItemGroup.First().id,
                                sms = ItemGroup.First().number,
                                callin = ItemGroup.OrderBy(x => x.callin).First().callin,
                                Site = SiteID,
                                battery = ItemGroup.First().battery,
                                csq = ItemGroup.First().csq
                            };

                    HidraulicTestEntities auxdb2 = new HidraulicTestEntities();

                    if (latestData.Count > 0)
                    {
                        foreach (var t in q)
                        {
                            List<Msg> GroupedByMinute = latestData.FindAll(x => x.callin.ToString("dd/MM/yyyy HH:mm") == t.callin.ToString("dd/MM/yyyy HH:mm")).ToList();

                            List<double> batteryLogs = new List<double>();
                            List<double> csqLogs = new List<double>();

                            foreach (var row in GroupedByMinute)
                            {
                                batteryLogs.Add(row.battery);
                                csqLogs.Add(row.csq);
                            }

                            var batteryAvg = batteryLogs.Average();
                            var csqAvg = csqLogs.Average();

                            var minBattery = batteryLogs.Min();
                            var minCsq = csqLogs.Min();

                            var maxBattery = batteryLogs.Max();
                            var maxCsq = csqLogs.Max();

                            string shortBatteryAvg = batteryAvg.ToString("N2");
                            string shortCsqAvg = csqAvg.ToString("N2");

                            BehaviorInstrumentation bhd = new BehaviorInstrumentation
                            {
                                id = Convert.ToInt32(t.id),
                                siteIDDatagate = SiteID,
                                battery = t.battery,
                                csq = t.csq,
                                lastCallIn = t.callin,
                                CsqAverage = Convert.ToDouble(shortCsqAvg),
                                BatteryAverage = Convert.ToDouble(shortBatteryAvg),
                                MinBattery = minBattery,
                                MinCsq = minCsq,
                                MaxBattery = maxBattery,
                                MaxCsq = maxCsq
                            };

                            Console.WriteLine("Insertando: Site: {0}, Fecha: {1}, Bateria: {2}, Csq: {3}, CSQ Avg: {4}, Battery Avg: {5}, Min Battery: {6}, Max Battery: {7}, Min Csq: {8}. Max Csq: {9}",
                                                                    SiteID, bhd.lastCallIn, bhd.battery, bhd.csq, bhd.CsqAverage, bhd.BatteryAverage, bhd.MinBattery, bhd.MaxBattery, bhd.MinCsq, bhd.MaxCsq);
                            auxdb2.BehaviorInstrumentation.Add(bhd);
                            auxdb2.SaveChanges();
                        }

                    }
                }             
            });
        }
        
        public static void TicketReader()
        {
            TicketDownloader();
            DateTime Hoy = DateTime.Today;
            String DownloadDate = Hoy.ToString("yyyy-MM-dd").Replace("-", string.Empty);

            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();

            var file = "C:\\Users\\DNK Water\\Downloads\\Abiertos Tickets - " + DownloadDate + ".csv";
            var config = new CsvConfiguration(CultureInfo.GetCultureInfo("es_CL")) { Delimiter = ";" };
            Console.WriteLine("Thread sleeping to ensure file is ready...");
            Thread.Sleep(3000);
            
            using (var reader = new StreamReader(file))
            {
                using (var csvReader = new CsvReader(reader, config))
                {
                    csvReader.Context.RegisterClassMap<TicketClassMap>();
                    var records = csvReader.GetRecords<Tickets>().ToList();

                    int added = 0;
                    int reviewed = 0;
                    int history = 0;

                    foreach (var i in records)
                    {
                        var ticketNumber = i.ticketNumber;
                        var createDate = i.createDate;
                        var siteIDDatagate = PCPCleaner(i.siteIDDatagate);
                        var currentStatus = i.currentStatus;
                        var teamAssigned = i.teamAssigned;
                        var closedDateDG = i.closedDateDG;
                        var lastUpdated = i.lastUpdated;
                        var SLAPlan = i.SLAPlan;
                        var Overdue = i.Overdue;
                        var tipoEvento = i.tipoEvento;

                        //Datagate site id
                        var newSiteID = AddressFinder(siteIDDatagate);

                        Console.WriteLine("Reading Existing Ticket: {0}, Last Updated: {1}, Current Status: {2}", ticketNumber, lastUpdated, currentStatus);
                        reviewed++;

                        //check for existing ticket
                        var ticketCheck = (from n in db.Tickets where n.ticketNumber.ToString() == ticketNumber.ToString() select n).FirstOrDefault();

                        if(ticketCheck != null)
                        {
                            //get latest update from current ticket
                            var currentTicket = (from n in db.Tickets where n.ticketNumber.ToString() == ticketNumber.ToString() select n.id).Max();
                            var query = (from sm in db.Tickets where sm.id == currentTicket select sm).FirstOrDefault();

                            if (query.lastUpdated.ToString() != lastUpdated.ToString() || query.currentStatus.ToString() != currentStatus.ToString())
                            {
                                Tickets nt = new Tickets();
                                nt.ticketNumber = query.ticketNumber;
                                nt.createDate = query.createDate;
                                nt.siteIDDatagate = query.siteIDDatagate;
                                nt.currentStatus = currentStatus.ToString();
                                nt.teamAssigned = query.teamAssigned;
                                if (nt.closedDateDG is DBNull) { Convert.IsDBNull(nt.closedDateDG = Convert.ToDateTime(closedDateDG)); }
                                nt.lastUpdated = Convert.ToDateTime(lastUpdated);
                                nt.SLAPlan = query.SLAPlan;
                                nt.Overdue = query.Overdue;
                                nt.tipoEvento = query.tipoEvento;

                                db.Tickets.Add(nt);
                                Console.WriteLine("New Ticket History Added: Number: {0},Last Updated: {1}", nt.ticketNumber, nt.lastUpdated);
                                history++;
                            }
                        }
                        else
                        {
                            Tickets t = new Tickets();
                            t.ticketNumber = Convert.ToInt32(ticketNumber);
                            t.createDate = Convert.ToDateTime(createDate);
                            t.siteIDDatagate = newSiteID;
                            t.currentStatus = currentStatus.ToString();
                            t.teamAssigned = teamAssigned.ToString();
                            if (t.closedDateDG is DBNull) { Convert.IsDBNull(t.closedDateDG = Convert.ToDateTime(closedDateDG)); }
                            t.lastUpdated = Convert.ToDateTime(lastUpdated);
                            t.SLAPlan = SLAPlan.ToString();
                            t.Overdue = Overdue.ToString();
                            t.tipoEvento = tipoEvento.ToString();

                            db.Tickets.Add(t);
                            Console.WriteLine("Reviewed Ticket Added: Number: {0}", t.ticketNumber);
                            added++;                         
                        }                                                           
                    }
                    
                    Console.WriteLine("----------------------------");
                    Console.WriteLine("| Tickets reviewed:     {0}  |", reviewed);
                    Console.WriteLine("| Tickets added:        {0}    |", added);
                    Console.WriteLine("| Ticket history added: {0}    |", history);
                    Console.WriteLine("----------------------------");
                    Console.ReadLine();
                    db.SaveChanges();
                }
            }
        }
        public static void HidraulicData()
        {
            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            TechnicalServiceEntities dbst = new TechnicalServiceEntities();

            string oldate2 = "2021-11-15 16:00:00.000";
            DateTime oldate3 = Convert.ToDateTime(oldate2);
            DateTime oldate4 = DateTime.Today.AddDays(-45);
            var query = (from s in localdb2.sites
                         join l in localdb2.loggers
                          on s.LoggerID equals l.ID
                         join a in localdb2.accounts
                          on s.OwnerAccount equals a.ID
                         where a.ID == 5 || a.ID == 6 || a.ID == 10
                         select new { l.LoggerSMSNumber, s.ID, s.SiteID }).ToList();

            ContratoMantenimientoEntities dbaux5 = new ContratoMantenimientoEntities();
            var helper = (from bhd in dbaux5.BehaviorHidraulic
                          where bhd.datetime >= oldate3
                          select bhd).FirstOrDefault();

            int counter = 0;
            Parallel.For(0, query.Count, new ParallelOptions { MaxDegreeOfParallelism = 30 }, i =>
            {
                counter++;
                int newId = query[i].ID;
                string newsiteID = query[i].SiteID;
                Console.Write(
                    "-------------------\n\n" +
                    "Ahora trabajando en el site numero " + counter + "\n" +
                    "Query ID: " + query[i].ID + "\n" +
                    "Query SiteID: " + newsiteID + "\n\n" +
                    "--------------------\n\n\n");
                if (helper != null)
                {
                    try
                    {
                        using (ContratoMantenimientoEntities dbaux = new ContratoMantenimientoEntities())
                        {
                            var siteHelper = (from bhd in dbaux.BehaviorHidraulic
                                                where bhd.siteIDDatagate == newsiteID
                                                select bhd).ToList();

                            var newestSiteDate = siteHelper.Max(x => x.datetime);
                            DateTime auxNewestSiteDate = Convert.ToDateTime(newestSiteDate);
                            DateTime newDateSite = auxNewestSiteDate.AddMinutes(2);
                            double diff = (DateTime.Today - newDateSite).TotalDays;
                            Debug.WriteLine("La ultima fecha encontrada del site {0} es {1} la fecha por la cual se buscara nuevos registros es: {2}",newsiteID, auxNewestSiteDate, newDateSite);
                            Debug.WriteLine(diff);
                            if (diff > 365)
                            {
                                Debug.WriteLine("Se ejecutara con fecha default");
                                DateTime defaultDate = DateTime.Now.AddDays(-365);
                                var olddate = DateTime.Now.AddDays(-1);
                                var today = DateTime.Now;
                                int[] start2 = { defaultDate.Year, defaultDate.Month, defaultDate.Day, defaultDate.Hour, defaultDate.Minute };
                                int[] end2 = { today.Year, today.Month, today.Day, today.Hour, today.Minute };
                                var data = DataGetter.GetDataFromAPI(newId, "custom", null, start2, end2, DateTime.Now);

                                Debug.WriteLine("La ultima fecha encontrada del site {0} es {1} la fecha por la cual se buscara nuevos registros es: {2}", newsiteID, auxNewestSiteDate, newDateSite);
                                if (data.Count() > 0)
                                {
                                    Debug.WriteLine("El site {0} tiene {1} registros", newsiteID, data.Count());
                                    foreach (var g in data)
                                    {
                                        foreach (var t in g.Item2)
                                        {
                                            try
                                            {
                                                BehaviorHidraulic bhd = new BehaviorHidraulic
                                                {
                                                    channelnum = Convert.ToInt32(g.Item1) + 1,
                                                    siteIDDatagate = newsiteID,
                                                    channeltype = g.Item3,
                                                    datetime = t.DataTime,
                                                    value = t.value
                                                };
                                                using (ContratoMantenimientoEntities dbAux3 = new ContratoMantenimientoEntities())
                                                {
                                                    dbAux3.BehaviorHidraulic.Add(bhd);
                                                    dbAux3.SaveChanges();
                                                }
                                            }
                                            catch (Exception err)
                                            {
                                                Debug.WriteLine("Se ha producido un error en el siteIddGate {0}", newsiteID);
                                                Debug.WriteLine(err);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("El site {0} no contiene nuevos registros ", newsiteID);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("Se ejecutara con ultima fecha del site");
                                var olddate = DateTime.Now.AddDays(-1);
                                var today = DateTime.Now;
                                int[] start2 = { newDateSite.Year, newDateSite.Month, newDateSite.Day, newDateSite.Hour, newDateSite.Minute };
                                int[] end2 = { today.Year, today.Month, today.Day, today.Hour, today.Minute };
                                var data = DataGetter.GetDataFromAPI(newId, "custom", null, start2, end2, DateTime.Now);
                                Debug.WriteLine("La ultima fecha encontrada del site {0} es {1} la fecha por la cual se buscara nuevos registros es: {2}", newsiteID, auxNewestSiteDate, newDateSite);
                                if (data.Count() > 0)
                                {
                                    Debug.WriteLine("El site {0} tiene {1} registros", newsiteID, data.Count());
                                    foreach (var g in data)
                                    {
                                        foreach (var t in g.Item2)
                                        {
                                            try
                                            {
                                                BehaviorHidraulic bhd = new BehaviorHidraulic
                                                {
                                                    channelnum = Convert.ToInt32(g.Item1) + 1,
                                                    siteIDDatagate = newsiteID,
                                                    channeltype = g.Item3,
                                                    datetime = t.DataTime,
                                                    value = t.value
                                                };
                                                using (ContratoMantenimientoEntities dbAux3 = new ContratoMantenimientoEntities())
                                                {
                                                    dbAux3.BehaviorHidraulic.Add(bhd);
                                                    dbAux3.SaveChanges();
                                                }
                                            }
                                            catch (Exception err)
                                            {
                                                Debug.WriteLine("Se ha producido un error en el siteIddGate {0}", newsiteID);
                                                Debug.WriteLine(err);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Debug.WriteLine("El site {0} no contiene nuevos registros ", newsiteID);
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.WriteLine("Se ha producido un error en el site {0}", query[i].ID);
                        Debug.WriteLine(err);
                    }
                }
                else
                {
                    try
                    {
                        Debug.WriteLine("Se ejecutara como si no tuviera datos la DB");
                        ContratoMantenimientoEntities dbaux = new ContratoMantenimientoEntities();
                        var siteHelper = (from bhd in dbaux.BehaviorHidraulic where bhd.siteIDDatagate == newsiteID select bhd).ToList();
                        DateTime startingDate = DateTime.Now.AddDays(-365);
                        var olddate = DateTime.Now.AddDays(-1);
                        var today = DateTime.Now;
                        int[] start2 = { startingDate.Year, startingDate.Month, startingDate.Day, startingDate.Hour, startingDate.Minute };
                        int[] end2 = { today.Year, today.Month, today.Day, today.Hour, today.Minute };
                        var data = DataGetter.GetDataFromAPI(newId, "custom", null, start2, end2, DateTime.Now);

                        foreach (var g in data)
                        {
                            foreach (var t in g.Item2)
                            {
                                try
                                {
                                    BehaviorHidraulic bhd = new BehaviorHidraulic
                                    {
                                        channelnum = Convert.ToInt32(g.Item1) + 1,
                                        siteIDDatagate = newsiteID,
                                        channeltype = g.Item3,
                                        datetime = t.DataTime,
                                        value = t.value
                                    };
                                    ContratoMantenimientoEntities dbAux3 = new ContratoMantenimientoEntities();
                                    dbAux3.BehaviorHidraulic.Add(bhd);
                                    dbAux3.SaveChanges();
                                }
                                catch (Exception err)
                                {
                                    Console.WriteLine("Se ha producido un error en el siteIddGate {0}", newsiteID);
                                    Console.WriteLine(err);
                                }
                            }
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine("Se ha producido un error en el site {0}", query[i].ID);
                        Console.WriteLine(err);
                    }
                }
            });
        }


  
        public static void TicketDownloader()
        {
            IWebDriver wd = new FirefoxDriver(@"C:\Users\DNK Water\Desktop\operadriver_win64");
            wd.Navigate().GoToUrl("https://www.dnk.support/scp/login.php");
            Console.WriteLine("The page is being scraped...");
            var username = wd.FindElement(By.Name("userid"));
            Assert.That(username.Displayed, Is.True);
            username.SendKeys("jbarrios");

            wd.FindElement(By.Id("pass")).SendKeys("barrios");
            wd.FindElement(By.Name("submit")).Submit();
            Thread.Sleep(7000);

            var export = wd.FindElement(By.XPath("//a[@id='queue-export']"));
            export.Click();
            Thread.Sleep(6000);

            var nextExport = wd.FindElement(By.XPath("//*[@id='popup']//input[@value='Export']"));

            nextExport.Click();
            Thread.Sleep(9000);

            AutoItX.Send("{DOWN}");
            AutoItX.Send("{ENTER}");
            Console.WriteLine("Tickets successfully downloaded.");
            wd.Close();
        }
        public static string PCPCleaner(string siteIDDatagate)
        {
            string str = siteIDDatagate.ToLower();
            StringBuilder newStr = new StringBuilder();
             newStr.Append(str);
            (newStr).Replace(" medida fuera de rango", String.Empty)
                    .Replace(" medidas fuera de rango", String.Empty)
                    .Replace("cau ", String.Empty)
                    .Replace(" sin comunicación", String.Empty)
                    .Replace("_", String.Empty)
                    .Replace("lo blanco", String.Empty)
                    .Replace("el sol-maillin", String.Empty)
                    .Replace(" p1", String.Empty)
                    .Replace(" p2", String.Empty)
                    .Replace(" p3", String.Empty)
                    .Replace("con", String.Empty)
                    .Replace("aa y aab", String.Empty)
                    .Replace("fuera de rango", String.Empty)
                    .Replace("(pcp 820)", String.Empty)
                    .Replace("pvrp ", String.Empty)
                    .Replace("cambio a datalogger multilog 2", String.Empty)
                    .Replace(" cambio a datalogger", String.Empty)
                    .Replace("pegasus plus", String.Empty)
                    .Replace("caudal", String.Empty)
                    .Replace("sin medición de", String.Empty)
                    .Replace("medida q", String.Empty)
                    .Replace("instalación datalogger", String.Empty)
                    .Replace(" con ", String.Empty)
                    .Replace("presión", String.Empty)
                    .Replace("con medición aab fuera rang", String.Empty)
                    .Replace("aa y aab fuera de rango", String.Empty)
                    .Replace(" con medida ", String.Empty)
                    .Replace(".", String.Empty)
                    .Replace("intermitencia", String.Empty)
                    .Replace(" med ", String.Empty)
                    .Replace(" medida aa", String.Empty)
                    .Replace(" medida aab", String.Empty)
                    .Replace(" presiones", String.Empty)
                    .Replace(" aab", String.Empty)
                    .Replace("camino", String.Empty)
                    .Replace(" ajuste peso pulso", String.Empty)
                    .Replace(" mediciónb fuera rang", String.Empty)
                    .Replace(" medición fuera rang", String.Empty)
                    .Replace(" ectar a logger", String.Empty)
                    .Replace(" medida", String.Empty)
                    .Replace(" b", String.Empty)
                    .Replace("b ", String.Empty)
                    .Replace("r ", String.Empty)
                    .ToString()
                    .ToUpper();

            return newStr.ToString().ToUpper().Trim();       
        }
        public static string AddressFinder(string siteIDDatagate)
        {
            LocalHWMEntities localdb2 = new LocalHWMEntities();

            var q = localdb2.sites.AsEnumerable();
            var SiteIDQuery = q.Where(x => x.Address.Contains(siteIDDatagate)).FirstOrDefault();
            if(SiteIDQuery != null)
            {
                return SiteIDQuery.SiteID.ToString();
            }
            return siteIDDatagate;
        }
        public static void AllComms()
        {
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            HidraulicTestEntities testdb = new HidraulicTestEntities();

            var commsquery = (from hc in testdb.HistorialComunicaciones
                              select hc).ToList();

            int c = 0;

            Parallel.For(0, commsquery.Count, new ParallelOptions { MaxDegreeOfParallelism = 30 }, i =>
            {
                c++;
                LocalHWMEntities auxdb2 = new LocalHWMEntities();
                HidraulicTestEntities aux3 = new HidraulicTestEntities();

                DateTime date = (DateTime)commsquery[i].Fecha;
                var smsnum = commsquery[i].SMSNumber;
                var id = commsquery[i].id;

                TimeSpan start = new TimeSpan(0, 0, 0);
                TimeSpan end = new TimeSpan(23, 59, 59);

                var formatDateStart = date.Add(start);
                var formatDateEnd = date.Add(end);

                var msgs = (from m in auxdb2.messages
                            where
                            m.rxtime >= formatDateStart &&
                            m.rxtime <= formatDateEnd &&
                            m.smsnumber == smsnum
                            select m).Count();

                using (HidraulicTestEntities auxdb = new HidraulicTestEntities())
                {
                    var query = (from h in auxdb.HistorialComunicaciones
                                 where h.id == id
                                 select h).FirstOrDefault();

                    query.TotalComms = msgs;
                    Debug.WriteLine("id: {0} iteracion: {1}", id, c);
                    auxdb.SaveChanges();
                }
            });
        }
        public static void TestComms()
        {
            HidraulicTestEntities db = new HidraulicTestEntities();
            hwmdbEntities db2 = new hwmdbEntities();

            var sitesquery = (from s in db2.sites
                              join l in db2.loggers
                              on s.LoggerID equals l.ID
                              join a in db2.accounts
                              on s.OwnerAccount equals a.ID
                              where a.ID == 5 || a.ID == 6 || a.ID == 10
                              select new { l.LoggerSMSNumber, s.SiteID, s.ID }).ToList();

            Parallel.For(0, sitesquery.Count, new ParallelOptions { MaxDegreeOfParallelism = 1 }, i =>
            {
                hwmdbEntities HWMAUX = new hwmdbEntities();

                var sms = sitesquery[i].LoggerSMSNumber;             

                var sitesqueryID = sitesquery[i].ID;
                var SiteID = sitesquery[i].SiteID;
                Debug.WriteLine("Trabajando en el site: " + SiteID);

                var historynumber = (from lr in HWMAUX.loggerrecordings
                                     where lr.Site_ID == sitesqueryID
                                     select new
                                     {
                                         lr.LoggerID
                                     }).Distinct().ToList();
            
                foreach (var l in historynumber)
                {
                    hwmdbEntities aux3 = new hwmdbEntities();

                    var RegistrationTime = (from ymca in db2.loggers where ymca.ID == l.LoggerID select ymca.NetRegistrationTime).FirstOrDefault();

                    var number = (from lo in aux3.loggers where lo.ID == l.LoggerID select lo).FirstOrDefault();

                    var ThisNumberStart = (from m in aux3.messages where m.smsnumber == number.LoggerSMSNumber select m.rxtime).Min();

                    var ThisNumberEnd = (from m in aux3.messages where m.smsnumber == number.LoggerSMSNumber select m.rxtime).Max();

                    if(ThisNumberStart != null)
                    {
                        DateTime startDate = (DateTime)ThisNumberStart.GetValueOrDefault();
                        DateTime endDate = (DateTime)ThisNumberEnd.GetValueOrDefault();

                        TimeSpan start = new TimeSpan(0, 0, 0);
                        TimeSpan end = new TimeSpan(23, 59, 59);

                        var startholder = startDate.ToShortDateString();
                        var endholder = startDate.ToShortDateString();

                        var rangeStart = Convert.ToDateTime(startholder).Add(start);
                        var rangeEnd = Convert.ToDateTime(endholder).Add(end);

                        Console.WriteLine("\n\nSite {0}, SMS: {1}, con un rango desde {2} a {3}", SiteID, number.LoggerSMSNumber, startDate, endDate);

                        while (startDate <= endDate)
                        {
                            hwmdbEntities hwmAux2 = new hwmdbEntities();
        
                            var DistinctComms = (from mess in hwmAux2.messages
                                                 where mess.smsnumber == number.LoggerSMSNumber
                                                 && mess.rxtime >= rangeStart
                                                 && mess.rxtime <= rangeEnd
                                                 select mess.rxtime).Distinct().ToList();

                            if(DistinctComms.Count != 0)
                            {
                                var AllComms = (from mess in hwmAux2.messages
                                                where mess.smsnumber == number.LoggerSMSNumber
                                                && mess.rxtime >= rangeStart
                                                && mess.rxtime <= rangeEnd
                                                select mess.rxtime).ToList();

                                var ActualDate = Convert.ToDateTime(startDate.ToShortDateString());
                                Console.WriteLine("---------------------------------");
                                Console.WriteLine("\nFecha actual: {0}", ActualDate);
                                var thisDayComms = DistinctComms.Where(x => x.Value.Date == startDate.Date).ToList();

                                if (thisDayComms.Count() > 0)
                                {
                                    DateTime? prevCommDate = null;
                                    int auxCommCounter = 0;
                                    foreach (var thisComm in thisDayComms)
                                    {
                                        if (prevCommDate == null)
                                        {
                                            prevCommDate = thisComm;
                                            auxCommCounter++;
                                        }
                                        else
                                        {
                                            double TimeDiff = (thisComm - prevCommDate).Value.TotalMinutes;

                                            if (TimeDiff > 60)
                                            {
                                                auxCommCounter++;
                                            }
                                            prevCommDate = thisComm;
                                        }
                                    }
                                    Console.WriteLine("\nEl numero {0} se ha comunicado {1}  y en total: {2} \n", number.LoggerSMSNumber, auxCommCounter, AllComms.Count);

                                    HidraulicTestEntities MTTOAUX = new HidraulicTestEntities();
                                    HistorialComunicaciones hc = new HistorialComunicaciones
                                    {
                                        SiteID = SiteID,
                                        Fecha = Convert.ToDateTime(ActualDate),
                                        NumeroComms = auxCommCounter,
                                        SMSNumber = number.LoggerSMSNumber,
                                        TotalComms = AllComms.Count()
                                    };
                                    MTTOAUX.HistorialComunicaciones.Add(hc);
                                    MTTOAUX.SaveChanges();
                                    startDate = startDate.AddDays(1);
                                    rangeStart = rangeStart.AddDays(1);
                                    rangeEnd = rangeEnd.AddDays(1);
                                }
                                else
                                {
                                    Console.WriteLine("\nEl numero {0} se ha comunicado 0 veces\n", number.LoggerSMSNumber);
                                    HidraulicTestEntities MTTOAUX = new HidraulicTestEntities();
                                    HistorialComunicaciones hc = new HistorialComunicaciones
                                    {
                                        SiteID = SiteID,
                                        Fecha = Convert.ToDateTime(ActualDate),
                                        NumeroComms = 0,
                                        SMSNumber = number.LoggerSMSNumber,
                                        TotalComms = AllComms.Count()
                                    };
                                    MTTOAUX.HistorialComunicaciones.Add(hc);
                                    MTTOAUX.SaveChanges();
                                    startDate = startDate.AddDays(1);
                                    rangeStart = rangeStart.AddDays(1);
                                    rangeEnd = rangeEnd.AddDays(1);
                                }
                            }
                            else
                            {
                                startDate = startDate.AddDays(1);
                                Console.WriteLine("Sin Comunicaciónes, avanzando... {1}", startDate);                              
                                rangeStart = rangeStart.AddDays(1);
                                rangeEnd = rangeEnd.AddDays(1);
                            }                  
                        }
                    }              
                }
            });
        }
    }
}
