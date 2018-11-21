using Aqua_Control;
using AquaMan.Extensions;
using AquaMan.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace AquaMan.Services
{
    /// <summary>
    /// Implementation of the data layer
    /// </summary>
    public class FormDataService : IFormDataService
    {
        #region Constants
        private const string TANK_SPEC_FILE = "TankSpecs.xml";
        private const string FEEDING_TIMES_FILE = "FeedingTimes.xml";

        private const string FEEDING_TIME_START_ELM = "FeedingTimes";
        private const string FEEDING_TIME_PINCH_START_ELM = "Pinches";
        private const string FEEDING_TIME = "Time";
        private const string FEEDING_TIME_1 = "Time1";
        private const string FEEDING_TIME_2 = "Time2";
        private const string FEEDING_TIME_3 = "Time3";
        private const string FEEDING_TIME_4 = "Time4";
        private const string FEEDING_TIME_5 = "Time5";

        private const string TANK_SPECS_START_ELM = "TankSpecs";
        private const string TANK_SPECS_WATER_CHANGE = "WaterChangeTime";
        private const string TANK_SPECS_DEPTH = "Depth";
        private const string TANK_SPECS_HEIGHT = "Height";
        private const string TANK_SPECS_WIDTH = "Width";
        #endregion

        #region Methods
        /// <summary>
        /// Gets the feeding times
        /// </summary>
        /// <returns></returns>
        public FeedingTimes GetFeedingTimes()
        {
            FeedingTimes feedingTimes;
            try
            {
                string feedingTimesXml = FEEDING_TIMES_FILE;

                feedingTimesXml = GetXmlFromFile(feedingTimesXml);
                feedingTimes = DeSerializeFeedingTimes(feedingTimesXml);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);

                feedingTimes = new FeedingTimes();
            }

            return feedingTimes;
        }

        /// <summary>
        /// Deserializes the <see cref="FeedingTimes"/>
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public FeedingTimes DeSerializeFeedingTimes(string xmlData)
        {
            FeedingTimes feedingTimes = new FeedingTimes();

            if (!string.IsNullOrEmpty(xmlData))
            {
                XmlDocument document = new XmlDocument();

                document.SecureLoadXml(xmlData);
                XmlNode rootNode = document.SelectSingleNode(FEEDING_TIME_START_ELM);

                XmlNode pinchNode = rootNode.SelectSingleNode(FEEDING_TIME_PINCH_START_ELM);
                int pinches = 1;
                int.TryParse(pinchNode.InnerXml, out pinches);
                feedingTimes.Pinches = pinches;

                for (int x = 1; x <= rootNode.ChildNodes.Count - 1; x++)
                {
                    XmlNode test = rootNode.SelectSingleNode($"{FEEDING_TIME}{x}");
                    feedingTimes.Feedings.Add(GetDateTime(rootNode.SelectSingleNode($"{FEEDING_TIME}{x}").FirstChild?.Value));
                }
            }

            return feedingTimes;
        }

        private DateTime? GetDateTime(string xmlDateTime)
        {
            DateTime? dateTime;
            if (string.IsNullOrEmpty(xmlDateTime))
            {
                dateTime = null;
            }
            else
            {
                dateTime = DateTime.Parse(xmlDateTime);
            }
            return dateTime;
        }
        /// <summary>
        /// Sets the feeding times
        /// </summary>
        /// <returns></returns>
        public void SetFeedingTimes(FeedingTimes feedingTimes)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(FEEDING_TIMES_FILE, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement(FEEDING_TIME_START_ELM);
                xmlWriter.WriteElementString(FEEDING_TIME_PINCH_START_ELM, feedingTimes.Pinches.ToString());
                for (int x = 0; x < feedingTimes.Feedings.Count; x++)
                {
                    xmlWriter.WriteElementString($"{FEEDING_TIME}{x + 1}", feedingTimes.Feedings[x].ToString());
                }
                
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
        }

        /// <summary>
        /// Gets the tank size specs, <see cref="TankSpecs"/>
        /// </summary>
        public TankSpecs GetTankSpecs()
        {
            TankSpecs tankSpecs;
            try
            {
                string tankSpecsXml = TANK_SPEC_FILE;

                tankSpecsXml = GetXmlFromFile(tankSpecsXml);
                tankSpecs = DeSerializeTankSpecs(tankSpecsXml);
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);

                tankSpecs = new TankSpecs();
            }
            return tankSpecs;
        }

        /// <summary>
        /// Deserializes the <see cref="TankSpecs"/>
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        public TankSpecs DeSerializeTankSpecs(string xmlData)
        {
            TankSpecs tankSpecs = new TankSpecs();

            if (!string.IsNullOrEmpty(xmlData))
            {
                XmlDocument document = new XmlDocument();

                document.SecureLoadXml(xmlData);

                XmlNode rootNode = document.SelectSingleNode(TANK_SPECS_START_ELM);
                tankSpecs.WaterChangeTime = GetDateTime(rootNode.SelectSingleNode($"{TANK_SPECS_WATER_CHANGE}").FirstChild.Value).Value;
                tankSpecs.Depth = double.Parse(rootNode.SelectSingleNode(TANK_SPECS_DEPTH).FirstChild.Value);
                tankSpecs.Height = double.Parse(rootNode.SelectSingleNode(TANK_SPECS_HEIGHT).FirstChild.Value);
                tankSpecs.Width = double.Parse(rootNode.SelectSingleNode(TANK_SPECS_WIDTH).FirstChild.Value);
            }

            return tankSpecs;
        }

        /// <summary>
        /// Sets the tank size specs, <see cref="TankSpecs"/>
        /// </summary>
        /// <param name="tankSpecs"></param>
        public void SetTankSpecs(TankSpecs tankSpecs)
        {
            using (XmlTextWriter xmlWriter = new XmlTextWriter(TANK_SPEC_FILE, Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement(TANK_SPECS_START_ELM);

                xmlWriter.WriteElementString(TANK_SPECS_WATER_CHANGE, tankSpecs.WaterChangeTime.ToString());
                xmlWriter.WriteElementString(TANK_SPECS_HEIGHT, tankSpecs.Height.ToString());
                xmlWriter.WriteElementString(TANK_SPECS_WIDTH, tankSpecs.Width.ToString());
                xmlWriter.WriteElementString(TANK_SPECS_DEPTH, tankSpecs.Depth.ToString());

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Close();
            }
        }

        private static string GetXmlFromFile(string xmlFile)
        {
            string xmlString = string.Empty;

            using (XmlReader reader = XmlTextReader.Create(xmlFile))
            {
                while (reader.Read())

                {
                    if (reader.IsStartElement())
                    {
                        xmlString = reader.ReadOuterXml();
                        break;
                    }
                }
            }

            return xmlString;
        }
        #endregion
    }
}
