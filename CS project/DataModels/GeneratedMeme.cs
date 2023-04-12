namespace CS_project
{
    class GeneratedMeme : Meme
    {
        string text1;
        string text2;
        string text3;
        string text4;
        string imgUrl;

        public GeneratedMeme()
        {
        }

        public GeneratedMeme(string id, string name, string url,
            string text1, string text2,
            string text3 = null, string text4 = null)
            : base(id, name, url)
        {
            this.text1 = text1;
            this.text2 = text2;
            this.text3 = text3;
            this.text4 = text4;
        }

        public string Text1 { get => text1; set => text1 = value; }
        public string Text2 { get => text2; set => text2 = value; }
        public string Text3 { get => text3; set => text3 = value; }
        public string Text4 { get => text4; set => text4 = value; }
        public string ImgUrl { get => imgUrl; set => imgUrl = value; }
    }
}
