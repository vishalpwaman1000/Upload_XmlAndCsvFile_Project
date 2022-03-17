using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upload_ExcelAndCsvFile_Project.CommonLayer.Model
{
    public class ReadRecordRequest
    {
        public int RecordPerPage { get; set; }
        public int PageNumber { get; set; }
    }

    public class ReadRecordResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CurrentPage { get; set; }
        public double TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public List<ReadRecord> readRecord { get; set; }
    }

    public class ReadRecord
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string EmailID { get; set; }
        public string MobileNumber { get; set; }
        public int Age { get; set; }
        public int Salary { get; set; }
        public string Gender { get; set; }
    }
}
