using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace KeyStrokeAutomator
{
	public static class Utils
	{
		public static EntryCollection GetEntries()
		{
			XmlDocument xDoc = new XmlDocument();
			xDoc.LoadXml(File.ReadAllText("Config.xml"));
			XmlNodeReader xNodeReader = new XmlNodeReader(xDoc.DocumentElement);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(EntryCollection));
			var employeeData = xmlSerializer.Deserialize(xNodeReader);
			EntryCollection deserializedEmployee = (EntryCollection) employeeData;
			return deserializedEmployee;
		}
	}
}
