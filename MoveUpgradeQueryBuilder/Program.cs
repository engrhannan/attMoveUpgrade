using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace MoveUpgradeQueryBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ContainerObject> lstcontObject = new List<ContainerObject>();
            List<string> lstString = new List<string> { "8662900","8658006","8660419","8663612"};
            //foreach (string item in lstString)
            //{
            //   // System.Net.WebRequest webRequest = System.Net.WebRequest.Create("https://gateway.dgsapi.com/tracing/xml.ashx?c=PIPE.BusinessProcesses.ConversationLog.Dashboard&file=2016-06-08_15%5C160608155420DGPR10%5CATT-SubmitOrder-IORProvide-d2f6b459-6cd5-45a5-a899-b3c8799d98bc%5CRequest.txt");
            //    System.Net.WebClient wb = new System.Net.WebClient();
            //    string xml = wb.DownloadString("https://gateway.dgsapi.com/tracing/xml.ashx?c=PIPE.BusinessProcesses.ConversationLog.Dashboard&file=2016-06-08_15%5C160608155420DGPR10%5CATT-SubmitOrder-IORProvide-d2f6b459-6cd5-45a5-a899-b3c8799d98bc%5CRequest.txt");
            //    //using (var response = webRequest.GetResponse())
            //    //using (var content = response.GetResponseStream())
            //    //using (var reader = new StreamReader(content))
            //    //{
            //    //    var strContent = reader.ReadToEnd();
            //    //}
            //}


            foreach (string file in Directory.EnumerateFiles(@"C:\Users\mhatiq\Desktop\MoveInformation", "*.xml"))
            {
                 string content = File.ReadAllText(file);
                //System.Net.WebClient wb = new System.Net.WebClient();
                //string url = "https://gateway.dgsapi.com/tracing/xml.ashx?c=PIPE.BusinessProcesses.ConversationLog.Dashboard&file=2016-06-08_15%5C160608155420DGPR10%5CATT-SubmitOrder-IORProvide-d2f6b459-6cd5-45a5-a899-b3c8799d98bc%5CRequest.txt";
                //string xmlString = wb.DownloadString(url);
               var  xml = XDocument.Load(file);
                ContainerObject contObject = new ContainerObject();
                //XDocument xml;
                //using (StringReader s = new StringReader(xmlString))
                //{
                //    xml = XDocument.Load(s);
                //}
                contObject.LeadId = xml.Root.Descendants("Leadid").FirstOrDefault().Value;
              //  contObject.LeadId = item;
                contObject.MoveCity = xml.Root.Descendants("City").FirstOrDefault().Value;
                contObject.MovingAddress = xml.Root.Descendants("AddressLine1").FirstOrDefault().Value;
                if(xml.Root.Descendants("FiberSchedulingInfo").FirstOrDefault()!=null)
                contObject.MovingDate = xml.Root.Descendants("FiberSchedulingInfo").FirstOrDefault().Attribute("actualDisconnectDate").Value;
                else
                    contObject.MovingDate = xml.Root.Descendants("ActualSchedule").FirstOrDefault().Attribute("actualDisconnectDate").Value;
                contObject.MovingState = xml.Root.Descendants("State").FirstOrDefault().Value;
                contObject.MovingZip = xml.Root.Descendants("ZipCode").FirstOrDefault().Value;
                if (xml.Root.Descendants("ApartmentUnit").FirstOrDefault() != null)
                    contObject.MovingSuite = xml.Root.Descendants("ApartmentUnit").FirstOrDefault().Value + ' ' + xml.Root.Descendants("ApartmentUnitNum").FirstOrDefault().Value;
                lstcontObject.Add(contObject);


            }
            string queryTemp = "\n--------------------------------------------------------------\nUse DGS;\ngo \nbegin tran";
            foreach (ContainerObject query in lstcontObject)
            {
                if (string.IsNullOrEmpty(query.MovingSuite))
                    queryTemp += string.Format("\nupdate dgscrm.Lead\nset \nismoving = 1 ,\nMovingAddress = '{0}' , \nMoveCity = '{1}' , \nMoveState = '{2}' ,  \nMovingZip = '{3}' , \nMovingDate = '{4}' \nwhere leadid in ( '{5}' ); \n\n--1 rows affected", query.MovingAddress, query.MoveCity, query.MovingState, query.MovingZip,query.MovingDate, query.LeadId);

                else
                    queryTemp += string.Format("\nupdate dgscrm.Lead\nset \nismoving = 1 , \nMovingAddress = '{0}' , \nMoveCity = '{1}' ,\nMoveState = '{2}' ,  \nMovingZip = '{3}' ,  \nMovingDate = '{4}',\nMovingSuite = '{5}' \nwhere leadid in ( '{6}' ); \n\n--1 rows affected", query.MovingAddress, query.MoveCity, query.MovingState, query.MovingZip,query.MovingDate, query.MovingSuite,query.LeadId);

            }
            queryTemp += "\nCommit \n----------------------------------------------------------";
            System.IO.StreamWriter writer = new System.IO.StreamWriter(@"C:\Users\mhatiq\Desktop\output.txt"); //open the file for writing.
            writer.Write(queryTemp); //write the current date to the file. change this with your date or something.
            writer.Close(); //remember to close the file again.
            writer.Dispose();
        }
    }
}
