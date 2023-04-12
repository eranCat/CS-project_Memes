using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CS_project
{
    class MemeAPI
    {
        private static MemeAPI instance = new MemeAPI();
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

        public GeneratedMeme CurrentMeme { get => currentMeme; set => currentMeme = value; }

        public async Task CreateMemeAsync(GeneratedMeme m)
        {
            Dictionary<string, string> formData = new Dictionary<string, string> {
                {"template_id", m.Id },
                {"username", Resource1.API_User },
                {"password", Resource1.API_Password},
                //{"boxes[0][text]", m.Text1},
                //{"boxes[1][text]", m.Text2},
                {"text0", m.Text1 },
                {"text1", m.Text2 },
            };

          
            // Serialize the request body to form URL-encoded string
            string formUrlEncodedRequestBody = new FormUrlEncodedContent(formData).ReadAsStringAsync().Result;

            StringContent content = new StringContent(formUrlEncodedRequestBody, Encoding.UTF8, "application/x-www-form-urlencoded");
           
            // Send the POST request
            HttpResponseMessage response = await client.PostAsync(Resource1.EndPointCreate, content);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response content as string
                string responseContent = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(responseContent);

                JObject json = JObject.Parse(responseContent);
                if (json.ContainsKey("success"))
                {
                    if ((bool)json["success"])
                    {
                        string pageUrl = (string)json["data"]["page_url"];
                        string url = (string)json["data"]["url"];
                        m.ImgUrl = url;
                        this.currentMeme = m;
                        Debug.WriteLine("Created meme at:" + pageUrl);
                    }
                    else
                    {
                        string errMsg = (string)json["error_message"];
                        Debug.WriteLine(errMsg);
                        throw new Exception(errMsg);
                    }
                }

                
            }
            else
            {
                Debug.WriteLine("Failed to make POST request. Status code: " + response.StatusCode);
            }

        }

        public async Task LoadPouplarMemes()
        {
            Debug.WriteLine("Load memes - get");

            var jsonStr = await client.GetStringAsync(Resource1.EndPointGetPopular);

            Debug.WriteLine(jsonStr);
            JObject json = JObject.Parse(jsonStr);

            if ((bool)json["success"])
            {
                //convert json[data][memes] array to C# obj
                JObject data = (JObject)json["data"];
                JArray memesJsonArr = (JArray)data["memes"];

                memes = memesJsonArr.ToObject<List<Meme>>();

                Debug.WriteLine("Memes Loaded:" + memes);
            }
            else
            {
                //handle the error 
                string errMsg = (string)json["error_message"];
                Debug.WriteLine(errMsg);

                throw new Exception(errMsg);
            }

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
