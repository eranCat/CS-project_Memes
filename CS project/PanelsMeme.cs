namespace CS_project
{
    class PanelsMeme : GeneratedMeme
    {
        string text3;
        string text4;
        public PanelsMeme(string id, string text1, string text2, string text3, string text4)
            : base(id, text1, text2)
        {
            this.text3 = text3;
            this.text4 = text4;
        }
    }
}
