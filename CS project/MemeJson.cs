using System.Text.Json.Serialization;

namespace CS_project.DataModels
{
    class MemeJson
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("meme")]
        public GeneratedMeme Meme { get; set; }

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
