using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upload_ExcelAndCsvFile_Project.CommonLayer.Model;
using Upload_XmlAndCsvFile_Project.CommonLayer.Model;

namespace Upload_XmlAndCsvFile_Project.DataAccessLayer
{
    public interface IUploadFileDL
    {
        public Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, string Path);

        public Task<UploadCSVFileResponse> UploadCSVFile(UploadCSVFileRequest request, string Path);
        public Task<ReadRecordResponse> ReadRecord(ReadRecordRequest request);
        public Task<DeleteRecordResponse> DeleteRecord(DeleteRecordRequest request);
    }
}
