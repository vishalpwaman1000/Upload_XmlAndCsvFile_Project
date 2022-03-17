using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upload_ExcelAndCsvFile_Project.CommonLayer.Model
{
    public class DeleteRecordRequest
    {
        public int UserID { get; set; }
    }

    public class DeleteRecordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
