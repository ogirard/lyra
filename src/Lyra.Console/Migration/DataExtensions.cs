using System;
using System.Web;
using System.Xml.Linq;

namespace Lyra.Console.Migration
{
    public static class DataExtensions
    {
        public static string GetValue(this XElement node, string childName)
            => node?.Element(childName)?.Value;

        public static string GetAttributeValue(this XElement node, string childName)
            => node?.Attribute(childName)?.Value;

        public static int? GetIntValue(this XElement node, string childName)
        {
            var value = node.GetValue(childName);

            if (value == null)
            {
                return null;
            }

            return int.Parse(value);
        }

        public static int? GetAttributeIntValue(this XElement node, string childName)
        {
            var value = node.GetAttributeValue(childName);

            if (value == null)
            {
                return null;
            }

            return int.Parse(value);
        }

        public static decimal? GetDecimalValue(this XElement node, string childName)
        {
            var value = node.GetValue(childName);

            if (value == null)
            {
                return null;
            }

            return decimal.Parse(value);
        }

        public static decimal? GetAttributeDecimalValue(this XElement node, string childName)
        {
            var value = node.GetAttributeValue(childName);

            if (value == null)
            {
                return null;
            }

            return decimal.Parse(value);
        }

        public static bool? GetBoolValue(this XElement node, string childName)
        {
            var value = node.GetValue(childName);
            return value == "yes";
        }

        public static bool? GetAttributeBoolValue(this XElement node, string childName)
        {
            var value = node.GetAttributeValue(childName);
            return value == "yes";
        }

        public static string CleanText(this string value)
            => HttpUtility.HtmlDecode(value)
                .Replace("\r", string.Empty)
                .Replace("\n", Environment.NewLine);
    }
}