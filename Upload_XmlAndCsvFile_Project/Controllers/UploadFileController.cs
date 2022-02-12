using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Upload_XmlAndCsvFile_Project.CommonLayer.Model;
using Upload_XmlAndCsvFile_Project.DataAccessLayer;

namespace Upload_XmlAndCsvFile_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        public readonly IUploadFileDL _uploadFile;
        public UploadFileController(IUploadFileDL uploadFile)
        {
            _uploadFile = uploadFile;
        }

        [HttpPost]
        public async Task<IActionResult> UploadXMLFile([FromForm] UploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();
            string path = request.File.FileName;
            try
            {
                
                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    await request.File.CopyToAsync(stream);
                }

                response = await _uploadFile.UploadXMLFile(request, path);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                
            }

            return Ok(response);
        }


    }
}
