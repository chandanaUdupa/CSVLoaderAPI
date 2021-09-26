using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace CSVLoaderAPI.Utility
{
    public static class FileHelper
    {

        /// <summary>
        /// To check if the file uploaded is a csv file or not
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsValidFile(string fileName)
        {
            string[] file = fileName.Split('.');
            string fileExt = file[file.Length - 1];
            if (fileExt.ToLower() != "csv")
            {
                return false;
            }
            return true;
        }



    }
}
