using System.IO;
using System.Text.Json;

namespace CS_project.DataModels
{
    class LocalDB
    {
        private static LocalDB instance = new LocalDB();

        private LocalDB()
        {
        }

        public static LocalDB Instance { get { return instance; } }


        public void SaveData(GeneratedMeme meme,string path = "meme.json")
        {
            string json = JsonSerializer.Serialize(meme);
            if (meme is FunnyMeme)
            {
               //set flag to funny
            }
            else if (meme is SadMeme)
            {
                //set flag to sad
            }
            File.WriteAllText(path, json);
        }
        public GeneratedMeme OpenFromFile(string path = "meme.json")
        {
            GeneratedMeme meme = null;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                string type = "Funny";
                switch (type)
                {
                    case "Funny":
                        meme = JsonSerializer.Deserialize<FunnyMeme>(json);
                        break;

                    case "Sad":
                        meme = JsonSerializer.Deserialize<SadMeme>(json);
                        break;

                    default:
                        meme = JsonSerializer.Deserialize<GeneratedMeme>(json);
                        break;
                }
            }
            return meme;
        }
    }
}
