using CS_project.DataModels;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

namespace CS_project
{
    public class GeneratedMeme : Meme
    {
        protected string text1;
        protected string text2;
        protected long uid;//a unique id generated for each meme

        public GeneratedMeme()
        {
        }

        public GeneratedMeme(string id, string name, string text1, string text2, string url = null, long uid = -1)
            : base(id, name, url)
        {
            this.text1 = text1;
            this.text2 = text2;
            this.uid = uid != -1 ? uid : generateRandomID();
        }

        private int generateRandomID()
        {
            long mili = DateTime.Now.Millisecond;
            return new Random((int)mili).Next();
        }

        public virtual MemeJson ToMemeJson()
        {
            return new MemeJson(GetType().Name, this);
        }

        public string Text1 { get => text1; set => text1 = value; }
        public string Text2 { get => text2; set => text2 = value; }
        public long Uid { get => uid; set => uid = value; }
    }

    class SadMeme : GeneratedMeme
    {

        public SadMeme(string id, string name, string url, string text1, string text2, long uid = -1)
            : base(id, name, text1, text2, url, uid)
        {
        }

        public SadMeme(GeneratedMeme meme) : this(meme.Id, meme.Name, meme.Url, meme.Text1, meme.Text2, meme.Uid)
        {
            
        }

        public SadMeme()
        {
        }

        public override MemeJson ToMemeJson()
        {
            return new MemeJson(GetType().Name,this);
        }
    }

    class FunnyMeme : GeneratedMeme
    {
        public FunnyMeme(string id, string name, string url, string text1, string text2, long uid = -1)
            : base(id, name, text1, text2, url, uid)
        {
        }

        public FunnyMeme(GeneratedMeme meme) :
            this(meme.Id, meme.Name, meme.Url, meme.Text1, meme.Text2, meme.Uid)
        {
           
        }

        public FunnyMeme()
        {
        }

        public override MemeJson ToMemeJson()
        {
            return new MemeJson(GetType().Name, this);
        }
    }
}
