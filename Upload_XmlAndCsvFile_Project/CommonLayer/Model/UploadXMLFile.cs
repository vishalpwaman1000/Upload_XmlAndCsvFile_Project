using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upload_XmlAndCsvFile_Project.CommonLayer.Model
{
    public class UploadXMLFileRequest
    {
        public IFormFile File { get; set; }
    }

    public class UploadXMLFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ExcelBulkUploadParameter
    {
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public string MobileNumber { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public string Gender { get; set; }
    }
}
