using Newtonsoft.Json.Linq;

namespace CS_project
{
    class Meme
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

        public Meme(JObject data)
        {
            Id = (string)data["id"];
            Name = (string)data["name"];
            Url = (string)data["url"];
        }
        
        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Url { get => url; set => url = value; }
    }

    class GeneratedMeme:Meme
    {
        string tex1;
        string text2;
        string imgUrl;

        public GeneratedMeme(string id, string topText, string bottomText)
        {
            this.id = id;
            this.tex1 = topText;
            this.text2 = bottomText;
        }

        public string TopText { get => tex1; set => tex1 = value; }
        public string BottomText { get => text2; set => text2 = value; }
        public string ImgUrl { get => imgUrl; set => imgUrl = value; }
    }

    class PanelsMeme : GeneratedMeme
    {
        string text3;
        string text4;
        public PanelsMeme(string id, string text1, string text2,string text3, string text4) 
            : base(id, text1, text2)
        {
            this.text3 = text3;
            this.text4 = text4;
        }
    }
}
