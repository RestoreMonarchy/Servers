using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RestoreMonarchy.Database.FileDatabase
{
    public class JsonFileDatabase : IFileDatabase
    {
        private readonly string _dataPath;        

        public JsonFileDatabase(string dir, string name)
        {
            NameIdentifier = name;
            _dataPath = Path.Combine(dir, name + ".json");            
        }

        public string NameIdentifier { get; private set; }

        public void SaveObject(object obj)
        {
            string objData = JsonConvert.SerializeObject(obj, Formatting.Indented);

            using (StreamWriter stream = new StreamWriter(_dataPath, false))
            {
                stream.Write(objData);
            }
        }

        public List<T> ReadObject<T>()
        {
            List<T> types = new List<T>();
            if (File.Exists(_dataPath))
            {
                using (StreamReader stream = File.OpenText(_dataPath))
                {
                    string dataText = stream.ReadToEnd();
                    types = JsonConvert.DeserializeObject<List<T>>(dataText);
                }
            }

            return types;
        }
    }
}
