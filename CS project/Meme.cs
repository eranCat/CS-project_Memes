using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CS_project
{
    class Meme
    {
        string id;
        string name;
        string text0;
        string text1;
        string url;

        public Meme()
        {
        }

        public Meme(JObject data)
        {
            Id = (string)data["id"];
            Name = (string)data["name"];
            Url = (string)data["url"];
        }
        
        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Url { get => url; set => url = value; }
        public string Text0 { get => text0; set => text0 = value; }
        public string Text1 { get => text1; set => text1 = value; }
    }
}
