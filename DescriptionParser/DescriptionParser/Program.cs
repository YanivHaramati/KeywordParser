using System;
using System.Linq;
using System.Net;
using System.Xml;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace DescriptionParser
{
    
    public class Program
    {
        const string QueryListingDescriptions = @"some link to an xml result page";

        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            RequestData(QueryListingDescriptions, result =>
            {
                var xml = new XmlDocument();
                xml.LoadXml(result);
                var doc = BsonDocument.Parse(JsonConvert.SerializeXmlNode(xml));
                
                // assume what we want is in x.y.z path and we're expecting an array.
                doc["x"].AsBsonDocument["y"].AsBsonDocument["z"].AsBsonArray.ToList().ForEach(d =>
                {
                    Console.WriteLine("\nProcessing listing:\n");
                    ProcessListing(d.AsBsonDocument);
                });
                Console.ReadKey();
            });
        }

        private static void ProcessListing(BsonDocument listing)
        {
            // assuming what we want is in node x and then y is our listing_description node, which has a text node.
            var description = listing["x"].AsBsonArray.FirstOrDefault(d => d.AsBsonDocument["y"].AsString == "listing_description");
            if (description == null) return;            
            Relevance.TagText(description["text"].AsString);
        }

        private static void RequestData(string uri, Action<string> action)
        {
            using (var client = new WebClient())
            {
                var data = client.DownloadString(new Uri(uri));
                action(data);
            }
        }

    }
}
