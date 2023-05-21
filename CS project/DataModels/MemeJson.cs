using System.Text.Json.Serialization;

namespace CS_project.DataModels
{
    public class MemeJson
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }


        private GeneratedMeme _meme;

        [JsonPropertyName("meme")]
        public GeneratedMeme Meme { 
            get => _meme;
            set
            {
                switch(Type)
                {
                    case nameof(SadMeme):
                        _meme = new SadMeme(value);
                        break;

                    case nameof(FunnyMeme):
                        _meme = new FunnyMeme(value);
                        break;
                    default: _meme = value; break;
                }
            }
        }

        public MemeJson()
        {
        }

        public MemeJson(string type, GeneratedMeme meme)
        {
            this.Type = type;
            this.Meme = meme;
        }
    }
}
