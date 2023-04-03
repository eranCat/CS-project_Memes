using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CS_project
{
    class MemeAPI
    {
        private static MemeAPI instance = new MemeAPI();

        private MemeAPI() {
            memes = new List<Meme>();
        }

        public static MemeAPI Instance
        {
            get => instance;
        }
        
        private List<Meme> memes;
        public List<Meme> Memes { get => memes; }


        public bool LoadPouplarMemes()
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var endpoint = new Uri("https://api.imgflip.com/get_memes");
                var result = client.GetAsync(endpoint).Result;
                var jsonStr = result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(jsonStr);
                JObject json = JObject.Parse(jsonStr);

                if ((bool)json["success"])
                {
                    //convert json[data][memes] array to C# obj
                    JObject data = (JObject)json["data"];
                    JArray memesJsonArr = (JArray)data["memes"];

                    this.memes = memesJsonArr.ToObject<List<Meme>>();

                    //this.memes = ser.Deserialize<List<Meme>>(strMemes);
                    Console.WriteLine(memes);
                    //memes.AddRange((Array<Meme>)json["data"]["meme"]);

                    return true;
                }
                else
                {
                    //handle the error 
                    string errMsg = (string)json["error_message"];
                    Console.WriteLine(errMsg);
                    throw new Exception(errMsg);
                    //return false;
                }
            }
            return false;
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
    }

}
