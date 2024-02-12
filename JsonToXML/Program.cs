using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

namespace JsonToXML
{
    public class JsonToXMLConverter
    {
        public static string ConvertJsonToXML(string json)
        {
            using (JsonDocument document = JsonDocument.Parse(json))
            {
                var rootElement = document.RootElement;
                var XMLDoc = new XmlDocument();
                XMLDoc.LoadXml("<root></root>");
                ConvertJsonToXMLElement(rootElement, XMLDoc.DocumentElement);
                return XMLDoc.OuterXml;
            }
        }

        private static void ConvertJsonToXMLElement(JsonElement jsonElement, XmlElement xmlElement)
        {
            foreach (var item in jsonElement.EnumerateObject())
            {
                if (item.Value.ValueKind == JsonValueKind.Object)
                {
                    var newXMLElement = xmlElement.OwnerDocument.CreateElement(item.Name);
                    xmlElement.AppendChild(newXMLElement);
                    ConvertJsonToXMLElement(item.Value, newXMLElement);
                }
                else if(item.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in item.Value.EnumerateArray())
                    {
                        var newXMLElement = xmlElement.OwnerDocument.CreateElement(item.Name);
                        xmlElement.AppendChild(newXMLElement);
                        ConvertJsonToXMLElement(element, newXMLElement);
                    }
                }
                else
                {
                    var newXMLElement = xmlElement.OwnerDocument.CreateElement(item.Name);
                    newXMLElement.InnerText = item.Value.ToString();
                    xmlElement.AppendChild(newXMLElement);
                }
            }
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            string json = "{\"firstName\":\"Tom\",\"lastName\":\"Jackson\",\"gender\":\"male\"}";
            string xml = JsonToXMLConverter.ConvertJsonToXML(json);

            Console.WriteLine(xml);
        }
    }
}