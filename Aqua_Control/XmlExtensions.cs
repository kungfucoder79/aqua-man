using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Aqua_Control
{
    public static class XmlExtensions
    {

        public static void SecureLoadXml(this XmlDocument doc, string strXml)
        {
            using (StringReader strReader = new StringReader(strXml))
            {
                using (XmlReader xmlReader = XmlReader.Create(strReader, getXmlReaderSettings()))
                {
                    doc.Load(xmlReader);
                }
            }
        }

        private static XmlReaderSettings getXmlReaderSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            return settings;
        }

    }
}
