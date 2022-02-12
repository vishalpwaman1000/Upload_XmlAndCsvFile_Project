using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upload_XmlAndCsvFile_Project.CommonLayer.Model;

namespace Upload_XmlAndCsvFile_Project.ServiceLayer
{
    public interface IUploadFileSL
    {
        public Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, string path);
    }
}
