using Newtonsoft.Json.Linq;

namespace CS_project
{
    public class Meme
    {
        protected string id;
        protected string name;
        protected string url;

        public Meme(string id, string name, string url)
        {
            this.id = id;
            this.name = name;
            this.url = url;
        }

        public Meme()
        {
        }

        //public Meme(JObject data)
        //{
        //    Id = (string)data["id"];
        //    Name = (string)data["name"];
        //    Url = (string)data["url"];
        //}

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Url { get => url; set => url = value; }
    }
}
