using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Exercise3.Models;


namespace Exercise3.Controllers
{
    public class MapController : Controller
    {
        // GET: Map
        public ActionResult Index(string ip, int port)
        {
            ViewBag.ip = ip;
            ViewBag.port = port;
            CommandSender commandSender = CommandSender.Instance;
            commandSender.connectToServer(ip, port);
            string strLon = commandSender.sendAndGetData("get /position/longitude-deg");
            string strLat = commandSender.sendAndGetData("get /position/latitude-deg");
            float lon = float.Parse(strLon);
            float lat = float.Parse(strLat);
            ViewBag.lon = lon;
            ViewBag.lat = lat;
            Console.WriteLine(lon);
            Console.WriteLine(lat);

            return View("Index");
        }
        
        [HttpGet]
        public ActionResult displayFourTimes(string ip, int port, int time)
        {
            ViewBag.ip = ip;
            ViewBag.port = port.ToString();
            ViewBag.time = time;
            Session["time"] = time;
            CommandSender commandSender = CommandSender.Instance;
            commandSender.connectToServer(ip, port);
            Debug.WriteLine("MapController - displayFourTimes");

            return View();
        }

        [HttpPost]
        public string GetLonAndLat()
        {
            Debug.WriteLine("yyyyyyyyyyyyyyyyyyyyyyyyyyyy");
            CommandSender commandSender = CommandSender.Instance;
            string strLon = commandSender.sendAndGetData("get /position/longitude-deg");
            string strLat = commandSender.sendAndGetData("get /position/latitude-deg");
            string lonAndLatStr = strLon + " " + strLat;

            return LlToXml(lonAndLatStr);
        }

        private string LlToXml(string toXml)
        {
            string[] locations = toXml.Split(' ');

            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Positions");

            /////////////////
            //if (locations[0] != "stop")
            //{
            //    Random rnd = new Random();
            //    locations[0] = (float.Parse(locations[0]) + rnd.Next(50)).ToString();
            //    locations[1] = (float.Parse(locations[1]) + rnd.Next(50)).ToString();

            //}
            ///////////////////

            writer.WriteStartElement("Position");
            writer.WriteElementString("Lon", locations[0]);
            writer.WriteElementString("Lat", locations[1]);

            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

        [HttpGet]
        public ActionResult Default()
        {
            return View();
        }


        [HttpGet]
        public ActionResult displayAndSaveFunc(string ip, int port, int time, int range, string fileName)
        {
            ViewBag.ip = ip;
            ViewBag.port = port.ToString();
            ViewBag.time = time;
            ViewBag.range = range;
            ViewBag.fileName = fileName;
            Session["time"] = time;
            Session["range"] = range;
            Session["fileName"] = fileName;
            CommandSender commandSender = CommandSender.Instance;
            commandSender.connectToServer(ip, port);
            Debug.WriteLine("MapController - displayAndSaveFunc");
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            return View();
        }


        [HttpPost]
        public string GetValues()
        {
            Debug.WriteLine("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqq");
            CommandSender commandSender = CommandSender.Instance;
            string strLon = commandSender.sendAndGetData("get /position/longitude-deg");
            string strLat = commandSender.sendAndGetData("get /position/latitude-deg");
            string strThrottle = commandSender.sendAndGetData("get /controls/engines/current-engine/throttle");
            string strRudder = commandSender.sendAndGetData("get /controls/flight/rudder");
            string valuesStr = strLon + " " + strLat + " " + strThrottle + " " + strRudder;

            ///////////////
            Random rnd = new Random();
            strLon = (float.Parse(strLon) + rnd.Next(50)).ToString();
            strLat = (float.Parse(strLat) + rnd.Next(50)).ToString();
            /////////////////

            string fileName = (string) Session["fileName"];
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            
            using (StreamWriter streamWriter = System.IO.File.AppendText(filePath))
            {
                streamWriter.WriteLine(strLon + ',' + strLat + ',' + strThrottle + ',' + strRudder);
            }

                return valuesToXml(valuesStr);
        }

        private string valuesToXml(string toXml)
        {
            string[] locations = toXml.Split(' ');

            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Values");

            ///////////////
            Random rnd = new Random();
            locations[0] = (float.Parse(locations[0]) + rnd.Next(50)).ToString();
            locations[1] = (float.Parse(locations[1]) + rnd.Next(50)).ToString();
            /////////////////

            writer.WriteElementString("Lon", locations[0]);
            writer.WriteElementString("Lat", locations[1]);
            writer.WriteElementString("Throttle", locations[2]);
            writer.WriteElementString("Rudder", locations[3]);


            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }

        [HttpGet]
        public ActionResult loadAndDisplay(string fileName,int time)
        {
            ViewBag.fileName = fileName;
            ViewBag.time = time;
            Session["time"] = time;
            Session["fileName"] = fileName;

            Debug.WriteLine("MapController - loadAndDisplay");

            string path = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            //string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            string[] lines = System.IO.File.ReadAllLines(path);
            Session["fileLines"] = lines;


            return View("loadAndDisplay");
        }

        [HttpPost]
        public string GetLonAndLatFromFile()
        {
            string[] lines = (string[]) Session["fileLines"];
            string strLon;
            string strLat;
            string lonAndLatStr;
            if (lines.Length > 0)
            {
                string line = lines[0];
                string[] vals = line.Split(',');
                lines = lines.Skip(1).ToArray();
                Session["fileLines"] = lines;
                strLon = vals[0];
                strLat = vals[1];
            }
            else
            {
                strLon = "stop";
                strLat = "stop";
            }
            lonAndLatStr = strLon + " " + strLat;
            return LlToXml(lonAndLatStr);
        }

        [HttpGet]
        public ActionResult checkFunc(string str, int number)
        {
            try
            {
                IPAddress.Parse(str);
                return Index(str, number);
            }
            catch
            {
                return loadAndDisplay(str, number);
            }
        }

        [HttpPost]
        public void disconnect()
        {
            CommandSender commandSender = CommandSender.Instance;
            commandSender.close();
        }
    }
}