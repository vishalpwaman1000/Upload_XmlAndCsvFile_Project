using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upload_XmlAndCsvFile_Project.CommonUtility
{
    public class SqlQueries
    {

        static IConfiguration _configuration = new ConfigurationBuilder()
            .AddXmlFile("SqlQueries.xml", true, true)
            .Build();

        public static string InsertBulkUploadData { get { return _configuration["InsertBulkUploadData"]; } }
        public static string ReadRecord { get { return _configuration["ReadRecord"]; } }
        public static string DeleteRecord { get { return _configuration["DeleteRecord"]; } }

    }
}
