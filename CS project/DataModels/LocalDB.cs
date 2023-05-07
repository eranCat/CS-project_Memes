using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


        //public void SaveData(GeneratedMeme meme, string path = "meme.json")
        //{
        //    string type = meme.GetType().Name;
        //    var temp = new MemeJson(type, meme);

        //    saveDataToFile(temp, path);
        //}

        public void SaveMemes(List<GeneratedMeme> memes, string path)
        {
            var jsons = memes.Select(meme => {
                string type = meme.GetType().Name;
                return new MemeJson(type, meme);
            }).ToList();

            saveDataToFile(jsons, path);
        }

        public void SaveToList(GeneratedMeme meme, string path = "meme.json")
        {
            string type = meme.GetType().Name;
            var temp = new MemeJson(type, meme);

            var list = LoadListOfMemeJsonFromFile(path);
            list.Add(temp);

            saveDataToFile(list, path);
        }

        public GeneratedMeme OpenFromFile(string path = "meme.json")
        {
            var memeJson = GetDataFromFile<MemeJson>(path);

            var meme = ConvertJsonToGeneratedMemeByType(memeJson);

            return meme;
        }

        public List<GeneratedMeme> LoadListFromFile(string path)
        {
            List<MemeJson> list = LoadListOfMemeJsonFromFile(path);
            var memesList = list.Select(mjs => mjs.Meme).ToList();
            return memesList;
        }

        private List<MemeJson> LoadListOfMemeJsonFromFile(string path)
        {
            var list = GetDataFromFile<List<MemeJson>>(path);
            return list ?? new List<MemeJson>();
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
                if (data=="")
                {
                    return default;
                }
                var obj = default(T);
                try
                {
                    obj = JsonSerializer.Deserialize<T>(data);
                }
                catch(NotSupportedException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (ArgumentNullException e)
                {
                    Console.WriteLine(e.Message);
                }

                return obj;
            }

            return default;
        }

        private bool saveDataToFile<T>(T data, string path)
        {
            string jsonSerialized = JsonSerializer.Serialize(data);
            File.WriteAllText(path, jsonSerialized);
            return true;
        }
    }
}
