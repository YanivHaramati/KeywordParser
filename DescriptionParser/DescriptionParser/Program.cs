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
        const string QueryListingDescriptions = @"http://dvc01mad904:8080/solr/listings/select?q=*:*&rows=100";

        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            RequestData(QueryListingDescriptions, result =>
            {
                var xml = new XmlDocument();
                xml.LoadXml(result);
                var doc = BsonDocument.Parse(JsonConvert.SerializeXmlNode(xml));
                doc["response"].AsBsonDocument["result"].AsBsonDocument["doc"].AsBsonArray.ToList().ForEach(d =>
                {
                    Console.WriteLine("\nProcessing listing:\n");
                    ProcessListing(d.AsBsonDocument);
                });
                Console.ReadKey();
            });
        }

        private static void ProcessListing(BsonDocument listing)
        {
            var description = listing["str"].AsBsonArray.FirstOrDefault(d => d.AsBsonDocument["@name"].AsString == "listing__description__public");
            if (description == null) return;            
            Relevance.TagText(description["#text"].AsString);
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
