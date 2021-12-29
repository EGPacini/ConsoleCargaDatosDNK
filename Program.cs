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
            Console.WriteLine("5) Lowest Values");
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
                        CommsHistory();
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
                                     select new
                                     {
                                         lr.LoggerID
                                     }).Distinct().ToList();

                foreach (var l in historynumber)
                {
                    var number = (from lo in auxdb2.loggers
                                  where lo.ID == l.LoggerID
                                  select lo).FirstOrDefault();

                    var allCommsQuery = (from mess in auxdb2.messages
                                         where mess.smsnumber == number.LoggerSMSNumber && mess.rxtime > lastComm
                                         select mess.rxtime
                                        ).Distinct().ToList();

                    Console.WriteLine("Site: " + sitesquery[i].SiteID + " ultima comm: " + lastComm);

                    if (allCommsQuery.Count() > 0)
                    {
                        DateTime lastDate = DateTime.Today.AddDays(-1);
                        DateTime initialDate = Convert.ToDateTime(lastComm).AddDays(1);

                        while (initialDate <= lastDate)
                        {
                            var thisDayComms = allCommsQuery.Where(x => x.Value.Date == initialDate.Date).ToList();

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
                                Console.WriteLine("El numero {0} se ha comunicado {1} veces en el dia {2} \n", number.LoggerSMSNumber, auxCommCounter, initialDate.ToString("dd-MM-yyy"));
                                ContratoMantenimientoEntities auxdb = new ContratoMantenimientoEntities();
                                HistorialComunicaciones hc = new HistorialComunicaciones
                                {
                                    SiteID = SiteID,
                                    Fecha = initialDate,
                                    NumeroComms = auxCommCounter,
                                    SMSNumber = number.LoggerSMSNumber
                                };
                                auxdb.HistorialComunicaciones.Add(hc);
                                auxdb.SaveChanges();
                            }
                            else
                            {
                                Console.WriteLine("El numero {0} se ha comunicado 0 veces en el dia {1} \n", number.LoggerSMSNumber, initialDate.ToString("dd-MM-yyy"));
                                ContratoMantenimientoEntities auxdb = new ContratoMantenimientoEntities();
                                HistorialComunicaciones hc = new HistorialComunicaciones
                                {
                                    SiteID = SiteID,
                                    Fecha = initialDate,
                                    NumeroComms = 0,
                                    SMSNumber = number.LoggerSMSNumber
                                };
                                auxdb.HistorialComunicaciones.Add(hc);
                                auxdb.SaveChanges();
                            }
                            initialDate = initialDate.AddDays(1);
                        }
                    }
                }
                Console.WriteLine("Proccess finished.");
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

                DateTime Date = new DateTime(2021, 1, 4, 00,00,00);
                DateTime DateEnd = new DateTime(2021, 12, 26, 23,59,00);
 
                double numberOfWeeks = ((DateEnd - Date).TotalDays) / 7;

                for (int y = 1; y <= numberOfWeeks; y++)
                {
                    DateEnd = Date.AddDays(7);
                    Console.WriteLine("Device: {0}, Site: {1}, Rango : {2} - {3}  Week° {4}", device, SiteID, Date, DateEnd, y);

                    //Rango de recoleccion - Semana a semana
                    var RowRange = (from bh in dbAux.BehaviorHidraulic
                                    where bh.siteIDDatagate == SiteID
                                    && bh.datetime >= Date
                                    && bh.datetime <= DateEnd
                                    select bh).ToList();

                    //Resultados de canales en rango
                    var CH1 = RowRange.FindAll(x => x.channelnum == 1).ToList();
                    var CH2 = RowRange.FindAll(x => x.channelnum == 2).ToList();
                    var CH3 = RowRange.FindAll(x => x.channelnum == 3).ToList();

                    //Resultados Ordenados - / +
                    var valuesListCh1 = CH1.ConvertAll(x => x.value).OrderBy(x => x.Value);
                    var valuesListCh2 = CH2.ConvertAll(x => x.value).OrderBy(x => x.Value);
                    var valuesListCh3 = CH3.ConvertAll(x => x.value).OrderBy(x => x.Value);

                    //Promedio de los resultados
                    double AverageCH1 = 0;
                    double AverageCH2 = 0;
                    double AverageCH3 = 0;

                    if(valuesListCh1.Any() == true)
                    {
                        AverageCH1 = Convert.ToDouble(valuesListCh1.Average());
                    }

                    if (valuesListCh2.Any() == true)
                    {
                        AverageCH2 = Convert.ToDouble(valuesListCh2.Average());
                    }

                    if (valuesListCh3.Any() == true)
                    {
                        AverageCH3 = Convert.ToDouble(valuesListCh3.Average());
                    }
                    
                    //Mediana de resultados 
                    double MedianCH1 = 0;
                    double MedianCH2 = 0;
                    double MedianCH3 = 0;
                    double realMedianCH1 = 0;
                    double realMedianCH2 = 0;
                    double realMedianCH3 = 0;

                    if (valuesListCh1.Any() == true)
                    {
                        MedianCH1 = valuesListCh1.Median();
                        realMedianCH1 = Convert.ToDouble(string.Format("{0:F2}", MedianCH1));
                    }

                    if (valuesListCh2.Any() == true)
                    {
                        MedianCH2 = valuesListCh2.Median();
                        realMedianCH2 = Convert.ToDouble(string.Format("{0:F2}", MedianCH2));
                    }

                    if (valuesListCh3.Any() == true)
                    {
                        MedianCH3 = valuesListCh3.Median();
                        realMedianCH3 = Convert.ToDouble(string.Format("{0:F2}", MedianCH3));
                    }


                    //Resultados que superan los rangos validos
                    var OverRangeCH1 = CH1.FindAll(x => x.value > 100).Count();
                    var UnderRangeCH1 = CH1.FindAll(x => x.value < 0).Count();

                    var OverRangeCH2 = CH2.FindAll(x => x.value > 100).Count();
                    var UnderRangeCH2 = CH2.FindAll(x => x.value < 0).Count();

                    var OverRangeCH3 = CH3.FindAll(x => x.value > 100).Count();
                    var UnderRangeCH3 = CH3.FindAll(x => x.value < 0).Count();

                    Debug.WriteLine("Site en evaluación: " + SiteID + " -- " + c);
                    Debug.WriteLine("");

                    void channel1()
                    {
                        var MaxValueCH1 = CH1.Max(x => x.value);
                        string RoundedMaxValueCH1 = string.Format("{0:F2}", MaxValueCH1);

                        var MinValueCH1 = CH1.Min(x => x.value);
                        string RoundedMinValueCH1 = string.Format("{0:F2}", MinValueCH1);

                        var MatchesMinCH1 = (from k in RowRange where k.value == MinValueCH1 select k).ToList();

                        var MatchesMaxCH1 = (from k in RowRange where k.value == MaxValueCH1 select k).ToList();

                        var MeasuresCount = (from ct in RowRange where ct.channelnum == 1 select ct).Count();

                        var MaxValMatchesInRangeCH1 = (from k in RowRange where k.channelnum == 1 && k.value == MaxValueCH1 select k).Count();

                        var MinValMatchesInRangeCH1 = (from k in RowRange where k.channelnum == 1 && k.value == MinValueCH1 select k).Count();

                        var FirstMinDateCH1 = (from ct in MatchesMinCH1 select ct.datetime).Min();

                        DateTime ctm = Convert.ToDateTime(FirstMinDateCH1);
                        DateTime newdate = new DateTime(2021, 10, 1);
                        int ca = 0;
                        if (FirstMinDateCH1 != null)
                        {
                            newdate = new DateTime(ctm.Year, ctm.Month, ctm.Day);
                            ca = (myCal.GetWeekOfYear(newdate.AddDays(-7), myCWR, myFirstDOW));
                        }

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
                        dbAux2.SaveChanges();
                    }
                    void channel2()
                    {
                        var MaxValueCH2 = CH2.Max(x => x.value);
                        string RoundedMaxValueCH2 = string.Format("{0:F2}", MaxValueCH2);
                        var MinValueCH2 = CH2.Min(x => x.value);
                        string RoundedMinValueCH2 = string.Format("{0:F2}", MinValueCH2);
                        var MatchesMinCH2 = (from k in RowRange where k.value == MinValueCH2 select k).ToList();
                        var MatchesMaxCH2 = (from k in RowRange where k.value == MaxValueCH2 select k).ToList();
                        var MeasuresCount = (from ct in RowRange where ct.channelnum == 2 select ct).Count();
                        var MaxValMatchesInRangeCH2 = (from k in RowRange where k.channelnum == 2 && k.value == MaxValueCH2 select k).Count();
                        var MinValMatchesInRangeCH2 = (from k in RowRange where k.channelnum == 2 && k.value == MinValueCH2 select k).Count();
                        var FirstMinDateCH2 = (from ct in MatchesMinCH2 select ct.datetime).Min();
                        var FirstMaxDateCH2 = (from ct in MatchesMaxCH2 select ct.datetime).Min();
                        DateTime ctm = Convert.ToDateTime(FirstMinDateCH2);
                        DateTime newdate = new DateTime(2021, 10, 1);
                        int ca = 0;
                        if (FirstMinDateCH2 != null)
                        {
                            newdate = new DateTime(ctm.Year, ctm.Month, ctm.Day);
                            ca = (myCal.GetWeekOfYear(newdate.AddDays(-7), myCWR, myFirstDOW));
                        }
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
                        dbAux2.SaveChanges();
                    }
                    void channel3()
                    {
                        var MaxValueCH3 = CH3.Max(x => x.value);
                        string RoundedMaxValueCH3 = string.Format("{0:F2}", MaxValueCH3);
                        var MinValueCH3 = CH3.Min(x => x.value);
                        string RoundedMinValueCH3 = string.Format("{0:F2}", MinValueCH3);
                        var MatchesMinCH3 = (from k in RowRange where k.value == MinValueCH3 select k).ToList();
                        var MatchesMaxCH3 = (from k in RowRange where k.value == MaxValueCH3 select k).ToList();
                        var MeasuresCount = (from ct in RowRange where ct.channelnum == 3 select ct).Count();
                        var MaxValMatchesInRangeCH3 = (from k in RowRange where k.channelnum == 3 && k.value == MaxValueCH3 select k).Count();
                        var MinValMatchesInRangeCH3 = (from k in RowRange where k.channelnum == 3 && k.value == MinValueCH3 select k).Count();
                        var FirstMinDateCH3 = (from ct in MatchesMinCH3 select ct.datetime).Min();
                        var FirstMaxDateCH3 = (from ct in MatchesMaxCH3 select ct.datetime).Min();
                        DateTime ctm = Convert.ToDateTime(FirstMinDateCH3);
                        DateTime newdate = new DateTime(2021, 10, 1);
                        int ca = 0;
                        if (FirstMinDateCH3 != null)
                        {
                            newdate = new DateTime(ctm.Year, ctm.Month, ctm.Day);
                            ca = (myCal.GetWeekOfYear(newdate.AddDays(-7), myCWR, myFirstDOW));
                        }
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
                        dbAux2.SaveChanges();
                    }

                    if (device == "1P")
                    {
                        channel1();
                    }
                    else if (device == "2P")
                    {
                        channel1();
                        channel3();
                    }
                    else if (device == "1P1Q")
                    {
                        channel1();
                        channel2();
                    }
                    else if (device == "2P1Q")
                    {
                        channel1();
                        channel2();
                        channel3();
                    }

                    Date = Date.AddDays(7);
                    DateEnd = DateEnd.AddDays(7);
                }

            });           
        }
        public static void DatosIntrumentacion()
        {
            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            TechnicalServiceEntities dbst = new TechnicalServiceEntities();

            var query = (from s in localdb2.sites
                         join l in localdb2.loggers on s.LoggerID equals l.ID
                         join a in localdb2.accounts on s.OwnerAccount equals a.ID
                         where a.ID == 5 || a.ID == 6 || a.ID == 10
                         select new { l.LoggerSMSNumber, s.SiteID }).ToList();

            Parallel.For(0, query.Count, new ParallelOptions { MaxDegreeOfParallelism = 20 }, i =>
            {
                LocalHWMEntities auxdb = new LocalHWMEntities();

                var SiteNumber = query[i].LoggerSMSNumber;
                var SiteID = query[i].SiteID;
                var LastCallIn = (from d in auxdb.messages where d.smsnumber == SiteNumber select d.rxtime).Max();
                var LastCall = Convert.ToDateTime(LastCallIn);

                var latestData = InstrumentationGetter.GetDataFromAPI(SiteNumber, LastCall.AddSeconds(1));

                if (latestData.Count > 0)
                {
                    foreach (var item in latestData)
                    {
                        ContratoMantenimientoEntities auxdb2 = new ContratoMantenimientoEntities();
                        BehaviorInstrumentation bhd = new BehaviorInstrumentation
                        {
                            id = Convert.ToInt32(item.id),
                            siteIDDatagate = SiteID,
                            battery = item.battery,
                            csq = item.csq,
                            lastCallIn = item.callin
                        };

                        Console.WriteLine("Insertando: LastCall: {0} Number: {1} Csq: {2} Battery: {3} Site: {4}", item.callin, item.number, item.csq, item.battery, SiteID);
                        auxdb2.BehaviorInstrumentation.Add(bhd);
                        auxdb2.SaveChanges();
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
        public static void TicketReader()
        {
            TicketDownloader();
            DateTime Hoy = DateTime.Today;
            String DownloadDate = Hoy.ToString("yyyy-MM-dd").Replace("-", string.Empty);

            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            TechnicalServiceEntities dbst = new TechnicalServiceEntities();

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
                        var siteIDDatagate = i.siteIDDatagate;
                        var currentStatus = i.currentStatus;
                        var teamAssigned = i.teamAssigned;
                        var closedDateDG = i.closedDateDG;
                        var lastUpdated = i.lastUpdated;
                        var SLAPlan = i.SLAPlan;
                        var Overdue = i.Overdue;
                        var tipoEvento = i.tipoEvento;

                        Console.WriteLine("Reading Existing Ticket: {0},  Last Updated: {1}, Current Status: {2}", ticketNumber, lastUpdated, currentStatus);
                        var query = (from sm in db.Tickets 
                                     where sm.ticketNumber.ToString() == ticketNumber.ToString()
                                     select sm).FirstOrDefault();
                        reviewed++;

                        if (query == null)
                        {
                            Tickets t = new Tickets();
                            t.ticketNumber = Convert.ToInt32(ticketNumber);
                            t.createDate = Convert.ToDateTime(createDate);
                            t.siteIDDatagate = siteIDDatagate.ToString();
                            t.currentStatus = currentStatus.ToString();
                            t.teamAssigned = teamAssigned.ToString();
                            if (t.closedDateDG is DBNull){ Convert.IsDBNull(t.closedDateDG = Convert.ToDateTime(closedDateDG)); }
                            t.lastUpdated = Convert.ToDateTime(lastUpdated);
                            t.SLAPlan = SLAPlan.ToString();
                            t.Overdue = Overdue.ToString();
                            t.tipoEvento = tipoEvento.ToString();

                            db.Tickets.Add(t);
                            Console.WriteLine("Reviewed Ticket Added: Number: {0}", t.ticketNumber);
                            added++;
                        }
                        else
                        {
                            if (query.lastUpdated.ToString() != lastUpdated.ToString() || currentStatus.ToString() != query.currentStatus.ToString())
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
                    }

                    db.SaveChanges();

                    Console.WriteLine("----------------------------");
                    Console.WriteLine("Tickets reviewed:     {0}  |", reviewed);
                    Console.WriteLine("Tickets added:        {0}    |", added);
                    Console.WriteLine("Ticket history added: {0}    |", history);
                    Console.WriteLine("----------------------------");
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

            //var prevDate = helper.Max(x => x.datetime);
            int counter = 0;
            Parallel.For(0, query.Count, new ParallelOptions { MaxDegreeOfParallelism = 30 },
               i =>
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

                               Debug.WriteLine("La ultima fecha encontrada del site {0} es {1} la fecha por la cual se buscara nuevos registros es: {2}", newsiteID, auxNewestSiteDate, newDateSite);
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
                                           //g.Item2.OrderBy(x => x.DataTime);
                                           foreach (var t in g.Item2)
                                           {
                                               try
                                               {
                                                   //Console.WriteLine(t.DataTime);
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
                           var siteHelper = (from bhd in dbaux.BehaviorHidraulic
                                             where bhd.siteIDDatagate == newsiteID
                                             select bhd).ToList();

                           DateTime startingDate = DateTime.Now.AddDays(-365);
                           var olddate = DateTime.Now.AddDays(-1);
                           var today = DateTime.Now;
                           int[] start2 = { startingDate.Year, startingDate.Month, startingDate.Day, startingDate.Hour, startingDate.Minute };
                           int[] end2 = { today.Year, today.Month, today.Day, today.Hour, today.Minute };
                           var data = DataGetter.GetDataFromAPI(newId, "custom", null, start2, end2, DateTime.Now);

                           //Debug.WriteLine("La ultima fecha encontrada del site {0} es {1} la fecha por la cual se buscara nuevos registros es: {2}", newsiteID, auxNewestSiteDate, newDateSite);

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
    }
}
