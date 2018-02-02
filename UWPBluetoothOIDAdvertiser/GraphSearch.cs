using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace UWPBluetoothOIDAdvertiser
{
    public class GraphSearch
    {
        const string graphResourceUri = "https://graph.windows.net";
        public static string graphApiVersion = "1.5";
        public static async Task<string> SearchByAlias(string alias)
        {
            JObject jResult = null;

            string graphRequest = String.Format(CultureInfo.InvariantCulture, "{0}/{1}/users?api-version={2}&$filter=mailNickname eq '{3}'", graphResourceUri, WAMAuthentication.Instance.TenantId, graphApiVersion, alias);
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, graphRequest);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", WAMAuthentication.Instance.AccessToken);
            HttpResponseMessage response = await client.SendAsync(request);

            string content = await response.Content.ReadAsStringAsync();
            jResult = JObject.Parse(content);


            if (jResult["odata.error"] != null)
            {
                throw new Exception((string)jResult["odata.error"]["message"]["value"]);
            }
            if (jResult["value"] == null)
            {
                throw new Exception("Unknown Error.");
            }
            foreach (JObject result in jResult["value"])
            {
                if (!string.IsNullOrEmpty((string)result["objectId"]))
                {
                    return (string)result["objectId"];
                }
            }

            return string.Empty;
        }
    }
}
