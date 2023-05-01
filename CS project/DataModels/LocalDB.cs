﻿using System.Collections.Generic;
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


        public void SaveData(GeneratedMeme meme, string path = "meme.json")
        {
            string type = meme.GetType().Name;
            var temp = new MemeJson(type, meme);

            saveDataToFile(path, temp);
        }

        public void SaveToList(GeneratedMeme meme, string path = "meme.json")
        {
            string type = meme.GetType().Name;
            var temp = new MemeJson(type, meme);

            var list = LoadListOfMemeJsonFromFile(path);
            list.Add(temp);

            saveDataToFile(path, list);
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
                var obj = JsonSerializer.Deserialize<T>(data);
                return obj;
            }

            return default;
        }

        private bool saveDataToFile<T>(string path, T data)
        {
            string jsonSerialized = JsonSerializer.Serialize(data);
            File.WriteAllText(path, jsonSerialized);
            return true;
        }
    }
}
