namespace CS_project
{
    public class GeneratedMeme : Meme
    {
        string text1;
        string text2;

        public GeneratedMeme()
        {
        }

        public GeneratedMeme(string id, string name, string text1,string text2, string url = null)
            : base(id, name,url)
        {
            this.text1 = text1;
            this.text2 = text2;
        }

        public string Text1 { get => text1; set => text1 = value; }
        public string Text2 { get => text2; set => text2 = value; }
    }

    class SadMeme : GeneratedMeme
    {
        public SadMeme(string id, string name, string url,string text1, string text2)
            : base(id, name, text1, text2, url)
        {
        }

        public SadMeme(GeneratedMeme meme) : this(meme.Id, meme.Name, meme.Url, meme.Text1, meme.Text2)
        {
            
        }
    }

    class FunnyMeme : GeneratedMeme
    {
        public FunnyMeme(string id, string name, string url,string text1, string text2)
            : base(id, name, text1, text2, url)
        {
        }

        public FunnyMeme(GeneratedMeme meme) :
            this(meme.Id, meme.Name, meme.Url, meme.Text1, meme.Text2)
        {
           
        }
    }
}
