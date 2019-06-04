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
        // show the first location of the plane
        public ActionResult Index(string ip, int port)
        {
            ViewBag.ip = ip;
            ViewBag.port = port;
            CommandSender commandSender = CommandSender.Instance;
            // connect
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
        // display the plane path every 4 seconds
        public ActionResult displayFourTimes(string ip, int port, int time)
        {
            ViewBag.ip = ip;
            ViewBag.port = port.ToString();
            ViewBag.time = time;
            Session["time"] = time;
            CommandSender commandSender = CommandSender.Instance;
            // connect
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

            /////////////////
            //Random rnd = new Random();
            //strLon = (float.Parse(strLon) + rnd.Next(50)).ToString();
            //strLat = (float.Parse(strLat) + rnd.Next(50)).ToString();
            ///////////////////

            // concat lon and lat to one string
            string lonAndLatStr = strLon + " " + strLat;

            return LlToXml(lonAndLatStr);
        }

        private string LlToXml(string toXml)
        {
            // split the parameters
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
        // the default home page
        public ActionResult Default()
        {
            return View();
        }


        [HttpGet]
        // display the path and save it
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
            // connect
            commandSender.connectToServer(ip, port);
            Debug.WriteLine("MapController - displayAndSaveFunc");
            // get the saved file path to check whether to delete it
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            // if the file exists delete it
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
            //string valuesStr = strLon + " " + strLat + " " + strThrottle + " " + strRudder;

            /////////////////
            //Random rnd = new Random();
            //strLon = (float.Parse(strLon) + rnd.Next(50)).ToString();
            //strLat = (float.Parse(strLat) + rnd.Next(50)).ToString();
            ///////////////////

            string fileName = (string) Session["fileName"];
            // get a file path to save the file in
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            
            // save the values
            using (StreamWriter streamWriter = System.IO.File.AppendText(filePath))
            {
                streamWriter.WriteLine(strLon + ',' + strLat + ',' + strThrottle + ',' + strRudder);
            }

            string valuesStr = strLon + " " + strLat + " " + strThrottle + " " + strRudder;
            return valuesToXml(valuesStr);
        }

        private string valuesToXml(string toXml)
        {
            // split the string elements
            string[] locations = toXml.Split(' ');

            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Values");

            /////////////////
            //Random rnd = new Random();
            //locations[0] = (float.Parse(locations[0]) + rnd.Next(50)).ToString();
            //locations[1] = (float.Parse(locations[1]) + rnd.Next(50)).ToString();
            ///////////////////

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
        // load the plane path from the file
        public ActionResult loadAndDisplay(string fileName,int time)
        {
            ViewBag.fileName = fileName;
            ViewBag.time = time;
            Session["time"] = time;
            Session["fileName"] = fileName;

            Debug.WriteLine("MapController - loadAndDisplay");
            // get the file path
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            // read all the file lines
            string[] lines = System.IO.File.ReadAllLines(path);
            Session["fileLines"] = lines;


            return View("loadAndDisplay");
        }

        [HttpPost]
        // get lon and lat from the saved array of file-lines, one line at every function call
        public string GetLonAndLatFromFile()
        {
            string[] lines = (string[]) Session["fileLines"];
            string strLon;
            string strLat;
            string lonAndLatStr;
            // if the arary is not empty
            if (lines.Length > 0)
            {
                // get line
                string line = lines[0];
                // parse the line
                string[] vals = line.Split(',');
                // delete this used line
                lines = lines.Skip(1).ToArray();
                Session["fileLines"] = lines;
                strLon = vals[0];
                strLat = vals[1];
            }
            // if the array is empty
            else
            {
                strLon = "stop";
                strLat = "stop";
            }
            lonAndLatStr = strLon + " " + strLat;
            return LlToXml(lonAndLatStr);
        }

        [HttpGet]
        // a function to check which function to use
        public ActionResult checkFunc(string str, int number)
        {
            try
            {
                // if the argument is an ip address retun Index function 
                IPAddress.Parse(str);
                return Index(str, number);
            }
            catch
            {
                // if the argument is not an ip address, (so it's a file name) return loadAndDisplay function
                return loadAndDisplay(str, number);
            }
        }

        [HttpPost]
        // disconnect function
        public void disconnect()
        {
            CommandSender commandSender = CommandSender.Instance;
            commandSender.close();
        }
    }
}