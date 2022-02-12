using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upload_XmlAndCsvFile_Project.CommonLayer.Model;
using Upload_XmlAndCsvFile_Project.DataAccessLayer;

namespace Upload_XmlAndCsvFile_Project.ServiceLayer
{
    public class UploadFileSL : IUploadFileSL
    {
        public readonly IUploadFileDL _uploadFileDL;

        public UploadFileSL(IUploadFileDL uploadFileDL)
        {
            _uploadFileDL = uploadFileDL;
        }

        public async Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, string path)
        {
            return await _uploadFileDL.UploadXMLFile(request, path);
        }
    }
}
