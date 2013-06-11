using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml.Linq;

namespace HealthTopicsWeb.Controllers
{
    public class HealthTopicsController : ApiController
    {
        /// <summary>
        /// Returns the Health Topic Information. 
        /// </summary>
        /// <param name="healthTopicName"></param>
        /// <returns></returns>
        public string Get(string id)
        {
            string webUrl = string.Empty;
            webUrl = "http://wsearch.nlm.nih.gov/ws/query?db=healthTopics&term=" + id;

            var response = MakeRequest(webUrl);

            if (response != null)
            {
                if (response.ToString().Contains("spellingCorrection"))
                    return string.Format("Spelling Correction: {0}", ProcessSpellingCorrection(response));
                else
                    return ProcessResponse(response);
            }

            return string.Empty;
        }

        private string ProcessSpellingCorrection(XDocument healthTopicsResponse)
        {
            return healthTopicsResponse.Descendants("spellingCorrection").First().Value;
        }

        private string ProcessResponse(XDocument healthTopicsResponse)
        {
            if (healthTopicsResponse == null) return string.Empty;

            string formattedResponse = "";
            var fullSummaryNodes = (from node in healthTopicsResponse.Descendants("content")
                                    where node.Attribute("name").Value == "FullSummary"
                                    select node);
            foreach (var node in fullSummaryNodes)
            {
                formattedResponse += node.Value;
                formattedResponse += "<br/>";
            }
            return formattedResponse;
        }

        private static XDocument MakeRequest(string requestUrl)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                var xmlDoc = XDocument.Load(response.GetResponseStream());
                return (xmlDoc);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
