using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.Json;

namespace CS_project.DataModels
{
    public class LocalDB
    {
        private static LocalDB instance = new LocalDB();

        private LocalDB()
        {
        }

        public static LocalDB Instance => instance;


        public void SaveData(GeneratedMeme meme,string path = "meme.json")
        {
            string type = meme.GetType().Name;
            var temp = new MemeJson(type,meme);
            
            saveDataToFile(path, temp);
        }

        public GeneratedMeme OpenFromFile(string path = "meme.json")
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var memeJson = GetDataFromFile<MemeJson>(path);

            var meme = ConvertJsonToGeneratedMemeByType(memeJson);

            return meme;
        }

        private GeneratedMeme ConvertJsonToGeneratedMemeByType(MemeJson dict)
        {

            switch (dict.Type)
            {
                case "FunnyMeme":
                    return new FunnyMeme(dict.Meme);

                case "SadMeme":
                    return new SadMeme(dict.Meme);

                default:
                    return dict.Meme;
            }
        }

        private T GetDataFromFile<T>(string path)
        {
            if (File.Exists(path))
            {
                string data = File.ReadAllText(path);
                var obj = JsonSerializer.Deserialize<T>(data);
                return obj;
            }

            return default(T);
        }

        private bool saveDataToFile<T>(string path,T data)
        {   
            string jsonSerialized = JsonSerializer.Serialize(data);
            File.WriteAllText(path,jsonSerialized);
            return true;
        }
    }
}
