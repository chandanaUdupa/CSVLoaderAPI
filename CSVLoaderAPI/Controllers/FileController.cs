using CSVLoaderAPI.BusinessLogic;
using CSVLoaderAPI.Entities;
using CSVLoaderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace CSVLoaderAPI.Controllers
{
    [Produces("application/json")]
    // In case, a newer version of our API is released in the future
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly ILogger _log;
        private readonly ICSVLoaderAPIBusinessLogic _businessLogic;

        public FileController(ILogger<FileController> logger, ICSVLoaderAPIBusinessLogic businessLogic)
        {
            _log = logger;
            _businessLogic = businessLogic;
        }

        /// <summary>
        /// API to import the csv file
        /// then save the data in 
        /// a) database b) disk
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/v1/File/UploadAndSave
        ///     
        /// </remarks>
        /// <param name="file">Upload csv file</param>
        /// <returns>A newly created user</returns>
        /// <response code="200">Returns the file location of json</response>
        /// <response code="400">If the file is null</response>   
        /// <response code="500">If there is some error</response>
        // POST: api/v1/File/UploadAndSave
        [HttpPost("Upload")]
        [DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult UploadAndSave(IFormFile file)
        {
            try
            {
                //Asynchronous reading with the Request.ReadFormAsync() expression
                //var formCollection = await Request.ReadFormAsync();
                //var file = formCollection.Files.First();

                // If file is not uploaded
                if (file.Length > 0)
                {
                    List<Product> products = new List<Product>();

                    // If file is csv file
                    if (FileHelper.IsValidFile(file.FileName))
                    {
                        // Transform the data into a logical model
                        products = _businessLogic.ConvertToLogicalModel(file);
                        
                        // Check if file does not contain any row
                        if (products.Count == 0)
                        {
                            return BadRequest("Empty file!!! Please upload a file with minimum one row");
                        }

                        // Actions :
                        // Store the data (two locations - a Database and JSON file)
                        // Database - MS SQL (using Sqlite)
                        // JSON file on the disk
                        if (_businessLogic.InsertCSVRecords(products) && _businessLogic.SavesAsJson(products))
                            return Ok("CSV File records are saved both in database as well as in json. Json File location is: " + Path.Combine(Environment.CurrentDirectory, "Products.json"));
                        else
                            return StatusCode(500, "Something went wrong!! Please raise a ticket");

                    }
                    else
                        return  BadRequest("Please upload csv file only!");
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (ArgumentNullException ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(500, new MessageError(ex.Message, ex.StackTrace));
            }
            catch (InvalidOperationException ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(500, new MessageError(ex.Message, ex.StackTrace));
            }
            catch (Exception ex)
            {
                _log.CSVLoaderAPISingleLogError(ex.Message + " Stack trace: " + ex.StackTrace);
                return StatusCode(500, new MessageError(ex.Message, ex.StackTrace));
            }
        }
    }
}
