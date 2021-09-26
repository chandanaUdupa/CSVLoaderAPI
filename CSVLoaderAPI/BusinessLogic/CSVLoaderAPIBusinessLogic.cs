using CsvHelper;
using CSVLoaderAPI.Data;
using CSVLoaderAPI.Entities;
using CSVLoaderAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CSVLoaderAPI.BusinessLogic
{
    public class CSVLoaderAPIBusinessLogic : ICSVLoaderAPIBusinessLogic
    {
        ILogger _log;
        IInsertToDatabase _insertToDatabase;
        ISaveLogicalModelToJson _saveLogicalModelToJson;

        public CSVLoaderAPIBusinessLogic(ILogger<CSVLoaderAPIBusinessLogic> logger, IInsertToDatabase insertToDatabase, ISaveLogicalModelToJson saveLogicalModelToJson)
        {
            _log = logger;
            _insertToDatabase = insertToDatabase;
            _saveLogicalModelToJson = saveLogicalModelToJson;
        }

        /// <summary>
        /// Read csv and convert to logical data model
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public List<Product> ConvertToLogicalModel(IFormFile file)
        {
            try
            {
                List<Product> products = new List<Product>();
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    // CsvReader also validates the header by default, if anything is missing it throws exception
                    products = csv.GetRecords<Product>().ToList();
                }

                return products;
            }
            catch (ArgumentException ex)
            {
                _log.LogError("Invald File: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Saves the product values in a file as json
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public bool SavesAsJson(List<Product> products)
        {
            bool isSuccessfullyCreated = false;
            try
            {
                string json = _saveLogicalModelToJson.ConvertEntityToJson(products);
                _saveLogicalModelToJson.SerializeObjectToFileStream(json);
                isSuccessfullyCreated = true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
            return isSuccessfullyCreated;
        }


        /// <summary>
        /// Insert products to database
        /// </summary>
        /// <param name="products"></param>
        /// <returns></returns>
        public bool InsertCSVRecords(List<Product> products)
        {
            bool isSuccessfullyInserted = false;
            try
            {
                _insertToDatabase.InsertCSVRecords(products);
                isSuccessfullyInserted = true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
            return isSuccessfullyInserted;
        }

    }
}
