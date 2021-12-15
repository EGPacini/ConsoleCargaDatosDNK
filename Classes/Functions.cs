using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleCargaDatosDNK.Classes
{
    class Functions
    {
    }

    public class DataGetter : WebClient
    {
        /// <summary>
        /// Función que realiza la llamada a la API de HWM para obtener los datos de los canales que están con información.
        /// </summary>
        /// <param name="siteID"></param>
        /// <param name="period">custom, day, week, month, quarter, all</param>
        /// <param name="units">Debe ingresarse un string los valores de unidad por defecto de la lectura del logger y separados por una coma ","</param>
        /// <param name="parameters">|X|Y : X corresponde al tiempo de muestro, 0 usage, 1 s, 60 minutos, 3600 horas y 86400 día. Él parámetro Y corresponde al tipo de gráfico que se mostrará
        /// en datagate, si es 0 y además el canal es analógico y no caudal, mostrará como barras, caso contrario, sólo de lineas. Debe comenzar con "|" e intercalado con "|" Ej: |1|0 </param>
        /// <param name="start">[year, month, day, hours, minutes]</param>
        /// <param name="end">[year, month, day, hours, minutes]</param>
        /// <returns>Retorna un lista de tuplas (correspondiente a todos los canales del sitio) con lo siguiente: [numero del canal, [lista de datos]]></returns>
        public static List<Tuple<int, List<DataPoints>, string>> GetDataFromAPI(
            int siteID,
            string period,
            string units,
            int[] start,
            int[] end,
            DateTime now)
        {
            List<Tuple<int, List<DataPoints>, string>> allData = new List<Tuple<int, List<DataPoints>, string>>();
            string API = ConfigurationManager.AppSettings["HWMAPI"];
            if (API == "" || API == null)
            {
                //API = "https://www.dnkenlinea.com/api";
                API = "http://192.168.188.220/api";
            }
            string data = string.Empty;
            var url = string.Empty;

            var uniqueValue = now.Minute + now.Second + now.Millisecond;

            url = API + "/LoggerData.ashx?signals=" + siteID
                       + "&period=" + period +
                        "&chunits=23,26|1|0|&DemoMode=False&enablestitching=True&fyear=" + start[0] +
                        "&fmonth=" + start[1] +
                        "&fday=" + start[2] +
                        "&fhour=" + start[3] +
                        "&fmin=" + start[4] +
                        "&tyear=" + end[0] +
                        "&tmonth=" + end[1] +
                        "&tday=" + end[2] +
                        "&thour=" + end[3] +
                        "&tmin=" + end[4] +
                        "&uv=" + uniqueValue +
                        "&dt=1&int=0";

            try
            {
                using (var wc = new DataGetter())
                {
                    wc.Timeout = 300000;
                    data = wc.DownloadString(url);
                    data.Trim();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.HelpLink);
            }
            try
            {
                foreach (JObject item in JArray.Parse(data))
                {
                    var label = item.GetValue("label").ToString();
                    if (label.IndexOf("No data found") == -1)
                    {
                        List<DataPoints> dataSet = new List<DataPoints>();
                        var chNo = Convert.ToInt32(item.GetValue("chNo"));
                        var dataItem = item.GetValue("data");
                        var chType = item.GetValue("chType").ToString();

                        if (dataItem.Count() > 0)
                        {
                            foreach (var dc in dataItem)
                            {
                                DataPoints d = new DataPoints();
                                DateTime dDate = new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(Convert.ToInt64(dc[0]));
                                d.DataTime = dDate.AddMinutes(-15);
                                d.value = (float)dc[1];

                                dataSet.Add(d);
                            }
                        }
                        if (dataSet.Count > 0)
                        {
                            allData.Add(new Tuple<int, List<DataPoints>, string>(chNo, dataSet, chType));
                        }
                    }
                }
            }
            catch (Exception e) { Debug.WriteLine(e.HelpLink); }
            return allData;
        }

        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            lWebRequest.Timeout = Timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
            return lWebRequest;
        }

    }

    public class InstrumentationGetter : WebClient
    {
        public static List<Msg> GetDataFromAPI(string SMSNumber, DateTime beginDate)
        {
            List<Msg> thisData = new List<Msg>();
            var API = ConfigurationManager.AppSettings["HWMAPI"];

            if (API == null || API == "")
            {
                API = "http://192.168.188.220/api/";
            }

            string data = string.Empty;
            var url = string.Empty;
            var newDate = beginDate.ToString();
            var newDate2 = newDate.Replace("-", "/");

            url = API + "messagingapi.ashx?action=getmessages"
                       + "&number=" + SMSNumber
                       + "&username=datagate"
                       + "&password=Hwm2019Dnk"
                       + "&beginDate=" + newDate2;
            try
            {
                using (var wc = new DataGetter())
                {
                    wc.Timeout = 300000;
                    data = wc.DownloadString(url);
                    data.Trim();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.HelpLink);
            }

            try
            {
                XmlDocument doc1 = new XmlDocument();
                doc1.LoadXml(data);
                XmlElement root = doc1.DocumentElement;
                XmlNodeList nodes = root.SelectNodes("/messages/message");

                foreach (XmlNode node in nodes)
                {
                    Msg msg = new Msg
                    {
                        id = node["id"].InnerText,
                        number = node["number"].InnerText,
                        battery = Convert.ToInt32(node["batteryCondition"].InnerText),
                        csq = Convert.ToInt32(node["signalStrength"].InnerText),
                        callin = Convert.ToDateTime(node["dateCredited"].InnerText)
                    };

                    thisData.Add(msg);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.HelpLink);
            }

            return thisData;
        }

        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest lWebRequest = base.GetWebRequest(uri);
            lWebRequest.Timeout = Timeout;
            ((HttpWebRequest)lWebRequest).ReadWriteTimeout = Timeout;
            return lWebRequest;
        }

    }
}
