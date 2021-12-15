using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoIt;
using ConsoleCargaDatosDNK.Classes;
using ConsoleCargaDatosDNK.Modelos;
using Microsoft.VisualBasic.FileIO;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace ConsoleCargaDatosDNK
{
    class Program
    {
        static void Main(string[] args)
        {
            var arg = args.FirstOrDefault();

            TicketReader();

            //switch (arg)
            //{
            //    case "Comms":
            //        CommsHistory();
            //        break;

            //    case "ST":
            //        ServicioTecnico();
            //        break;

            //    case "Hidraulics":
            //        DatosHidraulics();
            //        break;

            //    case "Instrumentacion":
            //        DatosIntrumentacion();
            //        break;
            //}
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
        public static void DatosHidraulics()
        {
            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            TechnicalServiceEntities dbst = new TechnicalServiceEntities();

            DateTime oldate3 = DateTime.Now.AddDays(-2); 

            var query = (from s in localdb2.sites
                         join l in localdb2.loggers
                          on s.LoggerID equals l.ID
                         join a in localdb2.accounts
                          on s.OwnerAccount equals a.ID
                         where a.ID == 5 || a.ID == 6 || a.ID == 10
                         select new { l.LoggerSMSNumber, s.ID, s.SiteID }).ToList();

            var helper = (from bhd in db.BehaviorHidraulic
                          where bhd.datetime >= oldate3
                          select bhd).ToList();

            var prevDate = helper.Max(x => x.datetime);
            Console.WriteLine(helper.Count());
            int counter = 0;
            Parallel.For(0, query.Count, new ParallelOptions { MaxDegreeOfParallelism = 20 }, i =>
               {
                   counter++;
                   int newId = query[i].ID;
                   string newsiteID = query[i].SiteID;
                   Debug.Write(
                       "-------------------\n\n" +
                       "Ahora trabajando en el site numero " + counter + "\n" +
                       "Query ID: " + query[i].ID + "\n" +
                       "Query SiteID: " + newsiteID + "\n\n" +
                       "--------------------\n\n\n");
                   if (helper.Count != 0)
                   {
                       try
                       {
                           ContratoMantenimientoEntities dbaux = new ContratoMantenimientoEntities();
                           var siteHelper = (from bhd in dbaux.BehaviorHidraulic
                                             where bhd.siteIDDatagate == newsiteID
                                             select bhd).ToList();

                           var newestSiteDate = siteHelper.Max(x => x.datetime);

                           DateTime auxNewestSiteDate = Convert.ToDateTime(newestSiteDate);

                           DateTime newDateSite = auxNewestSiteDate.AddMinutes(15);

                           var olddate = DateTime.Now.AddDays(-1);
                           var today = DateTime.Now;
                           int[] start2 = { auxNewestSiteDate.Year, auxNewestSiteDate.Month, auxNewestSiteDate.Day, auxNewestSiteDate.Hour, auxNewestSiteDate.Minute };
                           int[] end2 = { today.Year, today.Month, today.Day, today.Hour, today.Minute };
                           var data = DataGetter.GetDataFromAPI(newId, "custom", null, start2, end2, DateTime.Now);
                           Console.WriteLine("La ultima fecha encontrada del site {0} es {1} la fecha por la cual se buscara nuevos registros es: {2}", newsiteID, auxNewestSiteDate, newDateSite);
                           if (data.Count() > 0)
                           {
                               Console.WriteLine("El site {0} tiene {1} registros", newsiteID, data.Count());
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
                           else
                           {
                               Console.WriteLine("El site {0} no contiene nuevos registros ", newsiteID);
                           }
                       }
                       catch (Exception err)
                       {
                           Console.WriteLine("Se ha producido un error en el site {0}", query[i].ID);
                           Console.WriteLine(err);
                       }
                   }
                   else
                   {
                       try
                       {
                           ContratoMantenimientoEntities dbaux = new ContratoMantenimientoEntities();
                           var siteHelper = (from bhd in dbaux.BehaviorHidraulic
                                             where bhd.siteIDDatagate == newsiteID
                                             select bhd).ToList();

                           var newestSiteDate = siteHelper.Max(x => x.datetime);
                           DateTime auxNewestSiteDate = Convert.ToDateTime(newestSiteDate);
                           DateTime newDateSite = auxNewestSiteDate.AddMinutes(15);
                           var olddate = DateTime.Now.AddDays(-1);
                           var today = DateTime.Now;
                           int[] start2 = { auxNewestSiteDate.Year, auxNewestSiteDate.Month, auxNewestSiteDate.Day, auxNewestSiteDate.Hour, auxNewestSiteDate.Minute };
                           int[] end2 = { today.Year, today.Month, today.Day, today.Hour, today.Minute };
                           var data = DataGetter.GetDataFromAPI(newId, "custom", null, start2, end2, DateTime.Now);

                           Console.WriteLine("La ultima fecha encontrada del site {0} es {1} la fecha por la cual se buscara nuevos registros es: {2}", newsiteID, auxNewestSiteDate, newDateSite);

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
        public static void DatosIntrumentacion()
        {
            ContratoMantenimientoEntities db = new ContratoMantenimientoEntities();
            LocalHWMEntities localdb2 = new LocalHWMEntities();
            TechnicalServiceEntities dbst = new TechnicalServiceEntities();

            var query = (from s in localdb2.sites join l in localdb2.loggers on s.LoggerID equals l.ID
                         join a in localdb2.accounts on s.OwnerAccount equals a.ID where a.ID == 5 || a.ID == 6 || a.ID == 10
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

                        Console.WriteLine("Insertando: Number: {0} Site: {1} Battery: {2} CSQ: {3} LastCall: {4}",
                                                          item.number, SiteID, item.battery, item.csq, item.callin);

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

            var username = wd.FindElement(By.Name("userid"));
            Assert.That(username.Displayed, Is.True);
            username.SendKeys("jbarrios");

            wd.FindElement(By.Id("pass")).SendKeys("barrios");
            wd.FindElement(By.Name("submit")).Submit();
            Thread.Sleep(7000);

            var export = wd.FindElement(By.XPath("//a[@id='queue-export']"));
            export.Click();
            Thread.Sleep(3000);

            var nextExport = wd.FindElement(By.XPath("//*[@id='popup']//input[@value='Export']"));
            nextExport.Click();
            Thread.Sleep(9000);

            AutoItX.Send("{DOWN}");
            Thread.Sleep(2000);
            AutoItX.Send("{ENTER}");

            wd.Close();
        }

        public static void TicketReader()
        {
            TicketDownloader();
            List<String[]> fileContent = new List<string[]>();

            DateTime Hoy = DateTime.Today;
            String fechahoy = Hoy.ToString("yyyy-MM-dd").Replace("-", string.Empty);
            var file = "C:\\Users\\DNK Water\\Downloads\\Abiertos Tickets - " + fechahoy + ".csv";
            //file = Path.ChangeExtension(file, ".xlsx");

            using (FileStream reader = File.OpenRead(file))
            using(TextFieldParser parser = new TextFieldParser(reader))
            {
                parser.Delimiters = new[] { ";" };
                parser.HasFieldsEnclosedInQuotes = false;
                while (!parser.EndOfData)
                {
                    string[] line = parser.ReadFields();
                    for(int i = 0; i < 10; i++)
                    {

                    }
                    
                    fileContent.Add(line);

                    foreach(var i in line)
                    {
                        Console.WriteLine(i);
                    }
                    
                }
                Console.ReadLine();
            }
        }       
    }
}
