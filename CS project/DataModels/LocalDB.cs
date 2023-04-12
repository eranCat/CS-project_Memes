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
            File.WriteAllText(path, json);
        }
        public GeneratedMeme OpenFromFile(string path = "meme.json")
        {
            GeneratedMeme meme = null;
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                meme = JsonSerializer.Deserialize<GeneratedMeme>(json);
            }
            return meme;
        }
    }
}
