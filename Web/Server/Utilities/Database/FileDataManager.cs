using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Web.Server.Utilities.Database
{
    public class FileDataManager
    {
        public string DataPath { get; private set; }
        public FileDataManager(string dir, string fileName)
        {
            DataPath = Path.Combine(dir, fileName);
        }

        public void SaveObject(object obj)
        {
            string objData = JsonConvert.SerializeObject(obj, Formatting.Indented);

            using (StreamWriter stream = new StreamWriter(DataPath, false))
            {
                stream.Write(objData);
            }
        }

        ///<summary>We want to pass exception to the caller, therefore T is out parameter and return is boolean. 
        ///When caller gets false then you may want to unload plugin.</summary>
        public List<T> ReadObject<T>()
        {
            List<T> types = new List<T>();
            if (File.Exists(DataPath))
            {
                using (StreamReader stream = File.OpenText(DataPath))
                {
                    string dataText = stream.ReadToEnd();
                    types = JsonConvert.DeserializeObject<List<T>>(dataText);
                }
            }

            return types;
        }
    }
}