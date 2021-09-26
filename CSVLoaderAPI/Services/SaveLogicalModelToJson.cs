using CSVLoaderAPI.Data;
using CSVLoaderAPI.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSVLoaderAPI.Services
{
    public interface ISaveLogicalModelToJson
    {
        string ConvertEntityToJson<T>(List<T> entityRecords);
        void SerializeObjectToFileStream(string json);
    }

    public class SaveLogicalModelToJson : ISaveLogicalModelToJson
    {
        public string ConvertEntityToJson<T>(List<T> entityRecords)
        {
            string jsonReturnValue = string.Empty;
            jsonReturnValue = JsonConvert.SerializeObject(entityRecords, Formatting.Indented);
            return jsonReturnValue;
        }

        public void SerializeObjectToFileStream(string json)
        {
            //open file stream
            using (StreamWriter file = File.CreateText(Path.Combine(Environment.CurrentDirectory, "Products.json")))
            {
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, json);
            }

        }

    }
}
