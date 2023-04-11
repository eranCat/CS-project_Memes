using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CS_project
{
    class MemeAPI
    {
        private static MemeAPI instance = new MemeAPI();
        private const string USERNAME = "eran9999";//static by default
        private const string API_PWD = "yf9k@rZ34LRDv7r";//static by default
        private static readonly HttpClient client = new HttpClient();

        private GeneratedMeme currentMeme;

        private List<Meme> memes;
        public List<Meme> Memes { get => memes; }

        private MemeAPI()
        {
            memes = new List<Meme>();
        }

        public static MemeAPI Instance
        {
            get => instance;
        }

        public GeneratedMeme CurrentMeme { get => currentMeme; }

        public async Task CreateMemeAsync(GeneratedMeme m)
        {
            const string apiUrl = "https://api.imgflip.com/caption_image";
            List<Dictionary<string, string>> boxes = new List<Dictionary<string, string>>();
            boxes.Add(new Dictionary<string, string> { { "text", m.Text1 } });
            boxes.Add(new Dictionary<string, string> { { "text", m.Text2 } });
            boxes.Add(new Dictionary<string, string> { { "text", m.Text3 } });
            boxes.Add(new Dictionary<string, string> { { "text", m.Text4 } });


            Dictionary<string, string> formData = new Dictionary<string, string> {
                {"template_id", m.Id },
                {"username", USERNAME },
                {"password", API_PWD },
                //{"boxes[0][text]", m.Text1},
                //{"boxes[1][text]", m.Text2},
                {"text0", m.Text1 },
                {"text1", m.Text2 },
            };

          
            // Serialize the request body to form URL-encoded string
            string formUrlEncodedRequestBody = new FormUrlEncodedContent(formData).ReadAsStringAsync().Result;

            StringContent content = new StringContent(formUrlEncodedRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
           
            // Send the POST request
            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as string
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);

                JObject json = JObject.Parse(responseContent);
                if (json.ContainsKey("success"))
                {
                    if ((bool)json["success"])
                    {
                        string pageUrl = (string)json["data"]["page_url"];
                        string url = (string)json["data"]["url"];
                        m.ImgUrl = url;
                        this.currentMeme = m;
                        Console.WriteLine("Created meme at:" + pageUrl);
                    }
                    else
                    {
                        string errMsg = (string)json["error_message"];
                        Console.WriteLine(errMsg);
                        throw new Exception(errMsg);
                    }
                }

                
            }
            else
            {
                Console.WriteLine("Failed to make POST request. Status code: " + response.StatusCode);
            }

        }

        public async Task LoadPouplarMemes()
        {
            const string url = "https://api.imgflip.com/get_memes";
            Console.WriteLine("Load memes - get");

            var jsonStr = await client.GetStringAsync(url);

            Console.WriteLine(jsonStr);
            JObject json = JObject.Parse(jsonStr);

            if ((bool)json["success"])
            {
                //convert json[data][memes] array to C# obj
                JObject data = (JObject)json["data"];
                JArray memesJsonArr = (JArray)data["memes"];

                memes = memesJsonArr.ToObject<List<Meme>>();

                Console.WriteLine("Memes Loaded:" + memes);
            }
            else
            {
                //handle the error 
                string errMsg = (string)json["error_message"];
                Console.WriteLine(errMsg);

                throw new Exception(errMsg);
            }

        }

        // Helper method to convert dictionary to query string
        private string ToQueryString(Dictionary<string, object> dictionary)
        {
            List<string> keyValues = new List<string>();

            foreach (var kvp in dictionary)
            {
                string key = Uri.EscapeDataString(kvp.Key);
                string value = Uri.EscapeDataString(kvp.Value.ToString());
                keyValues.Add($"{key}={value}");
            }

            return string.Join("&", keyValues);
        }

        public Meme getRandomMeme()
        {
            if (memes.Count == 0)
            {
                return null;
            }

            Random rnd = new Random(DateTime.Now.Millisecond);
            int rndIndex = rnd.Next(memes.Count);

            return memes[rndIndex];
        }

        public Dictionary<string,string> getMappedMemes()
        {
            return memes.ToDictionary( m => m.Id,m=> m.Name);
        }
        public static void clearResources()
        {
            client.Dispose();
        }
    }
}
