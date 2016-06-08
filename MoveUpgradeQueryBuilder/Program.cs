using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;

namespace MoveUpgradeQueryBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ContainerObject> lstcontObject = new List<ContainerObject>();

            foreach (string file in Directory.EnumerateFiles(@"C:\Users\mhatiq\Desktop\MoveInformation", "*.xml"))
            {
                // string contents = File.ReadAllText(file);
                ContainerObject contObject = new ContainerObject();
                var xml = XDocument.Load(file);
                contObject.LeadId = xml.Root.Descendants("Leadid").FirstOrDefault().Value;
                contObject.MoveCity = xml.Root.Descendants("City").FirstOrDefault().Value;
                contObject.MovingAddress = xml.Root.Descendants("AddressLine1").FirstOrDefault().Value;
                contObject.MovingDate = xml.Root.Descendants("FiberSchedulingInfo").FirstOrDefault().Attribute("actualDisconnectDate").Value;
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
