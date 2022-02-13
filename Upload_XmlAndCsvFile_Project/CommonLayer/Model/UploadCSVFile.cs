using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upload_XmlAndCsvFile_Project.CommonLayer.Model
{
    public class UploadCSVFileRequest
    {
        public IFormFile File { get; set; }
    }

    public class UploadCSVFileResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
