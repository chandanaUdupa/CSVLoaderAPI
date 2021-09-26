using CSVLoaderAPI.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace CSVLoaderAPI.Data
{
    public interface IInsertToDatabase
    {
        void InsertCSVRecords(List<Product> products);
    }

    public class InsertToDatabase : IInsertToDatabase
    {
        ILogger _log;
        public InsertToDatabase(ILogger<InsertToDatabase> logger)
        {
            _log = logger;
        }


        /// <summary>
        /// Insert CSV Records to sqlite database
        /// </summary>
        /// <param name="products"></param>
        public void InsertCSVRecords(List<Product> products)
        {
            try
            {
                // Creates new sqlite database if it is not found
                using (var conn = new SQLiteConnection(
                    @"Data Source=" + Path.Combine(Environment.CurrentDirectory + @"\Data\" + "ProductDB.db")))
                {

                    conn.Open();

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    // SQLBulkCopy is not supported in Sqlite. Hence, using transaction with Insert into for bulk insert
                    using (var cmd = new SQLiteCommand(conn))
                    {
                        using (var transaction = conn.BeginTransaction())
                        {
                            foreach (Product product in products)
                            {
                                cmd.CommandText =
                                    "INSERT OR REPLACE INTO Product (Key, ArtikelCode, ColorCode, Description, Price, DiscountPrice, DeliveredIn, Q1, Size, Color) VALUES (@key, @artikelCode, @colorCode, @description, @price, @discountPrice, @deliveredIn, @q1, @size, @color);";

                                cmd.Parameters.Add("@key", System.Data.DbType.String);
                                cmd.Parameters["@key"].Value = product.Key;

                                cmd.Parameters.Add("@artikelCode", System.Data.DbType.String);
                                cmd.Parameters["@artikelCode"].Value = product.ArtikelCode;

                                cmd.Parameters.Add("@colorCode", System.Data.DbType.String);
                                cmd.Parameters["@colorCode"].Value = product.ColorCode;

                                cmd.Parameters.Add("@description", System.Data.DbType.String);
                                cmd.Parameters["@description"].Value = product.Description;

                                cmd.Parameters.Add("@price", System.Data.DbType.Int32);
                                cmd.Parameters["@price"].Value = product.Price;

                                cmd.Parameters.Add("@discountPrice", System.Data.DbType.Int32);
                                cmd.Parameters["@discountPrice"].Value = product.DiscountPrice;

                                cmd.Parameters.Add("@deliveredIn", System.Data.DbType.String);
                                cmd.Parameters["@deliveredIn"].Value = product.DeliveredIn;

                                cmd.Parameters.Add("@q1", System.Data.DbType.String);
                                cmd.Parameters["@q1"].Value = product.Q1;

                                cmd.Parameters.Add("@size", System.Data.DbType.Int32);
                                cmd.Parameters["@size"].Value = product.Size;

                                cmd.Parameters.Add("@color", System.Data.DbType.String);
                                cmd.Parameters["@color"].Value = product.Color;

                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                        }
                    }

                    Console.WriteLine("{0} seconds with one transaction.",
                      stopwatch.Elapsed.TotalSeconds);

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw;
            }
        }

    }
}
