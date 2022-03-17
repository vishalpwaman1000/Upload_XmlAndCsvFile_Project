using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
//using System.IO;
using System.Threading.Tasks;
using Upload_ExcelAndCsvFile_Project.CommonLayer.Model;
using Upload_XmlAndCsvFile_Project.CommonLayer.Model;
using Upload_XmlAndCsvFile_Project.DataAccessLayer;

namespace Upload_XmlAndCsvFile_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        public readonly IUploadFileDL _uploadFileDL;
        public UploadFileController(IUploadFileDL uploadFileDL)
        {
            _uploadFileDL = uploadFileDL;
        }

        [HttpPost]
        [Route("UploadExcelFile")]
        public async Task<IActionResult> UploadExcelFile([FromForm] UploadXMLFileRequest request)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();
            string path = "UploadFileFolder/"+request.File.FileName;
            try
            {
                
                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    await request.File.CopyToAsync(stream);
                }

                response = await _uploadFileDL.UploadXMLFile(request, path);

                string[] files = Directory.GetFiles("UploadFileFolder/");
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("UploadCSVFile")]
        public async Task<IActionResult> UploadCSVFile([FromForm] UploadCSVFileRequest request)
        {
            UploadCSVFileResponse response = new UploadCSVFileResponse();
            string path = "UploadFileFolder/" + request.File.FileName;
            try
            {

                using (FileStream stream = new FileStream(path, FileMode.CreateNew))
                {
                    await request.File.CopyToAsync(stream);
                }

                response = await _uploadFileDL.UploadCSVFile(request, path);

                string[] files = Directory.GetFiles("UploadFileFolder/");
                foreach (string file in files)
                {
                    System.IO.File.Delete(file);
                    Console.WriteLine($"{file} is deleted.");
                }

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;

            }

            return Ok(response);
        }

        [HttpPost]
        [Route("ReadRecord")]
        public async Task<IActionResult> ReadRecord(ReadRecordRequest request)
        {
            ReadRecordResponse response = new ReadRecordResponse();
            
            try
            {
                response = await _uploadFileDL.ReadRecord(request);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return Ok(response);
        }

        [HttpDelete]
        [Route("DeleteRecord")]
        public async Task<IActionResult> DeleteRecord(DeleteRecordRequest request)
        {
            DeleteRecordResponse response = new DeleteRecordResponse();

            try
            {
                response = await _uploadFileDL.DeleteRecord(request);
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
