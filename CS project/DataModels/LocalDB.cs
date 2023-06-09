﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public void SaveMemes(List<GeneratedMeme> memes, string path)
        {
            var jsons = memes.Select(meme => meme.ToMemeJson()).ToList();
            saveDataToFile(jsons, path);
        }

        public int SaveToList(GeneratedMeme meme, string path = "meme.json")
        {
            var list = LoadListOfMemeJsonFromFile(path);
            bool hasUpdated = false;
            foreach (var jmeme in list)
            {
                if(jmeme.Meme.Uid == meme.Uid)
                {
                    jmeme.Meme = meme;
                    hasUpdated = true;
                    break;
                }
            }
            if (!hasUpdated)
            {
                var temp = meme.ToMemeJson();
                list.Add(temp);
            }

            bool hasSaved = saveDataToFile(list, path);

            if (hasSaved)
            {
                return hasUpdated ? 1 : 0;
            }

            return -1;
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

        //public GeneratedMeme OpenFromFile(string path = "meme.json")
        //{
        //    var memeJson = GetDataFromFile<MemeJson>(path);

        //    var meme = ConvertJsonToGeneratedMemeByType(memeJson);

        //    return meme;
        //}

        //private GeneratedMeme ConvertJsonToGeneratedMemeByType(MemeJson dict)
        //{

        //    switch (dict.Type)
        //    {
        //        case "FunnyMeme":
        //            return new FunnyMeme(dict.Meme);

        //        case "SadMeme":
        //            return new SadMeme(dict.Meme);

        //        default:
        //            return dict.Meme;
        //    }
        //}
    }
}
