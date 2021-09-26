
using CSVLoaderAPI.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CSVLoaderAPI.BusinessLogic
{
    public interface ICSVLoaderAPIBusinessLogic
    {
        List<Product> ConvertToLogicalModel(IFormFile file);

        bool SavesAsJson(List<Product> products);

        bool InsertCSVRecords(List<Product> products);
    }
}
