using ExcelDataReader;
using LumenWorks.Framework.IO.Csv;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Upload_ExcelAndCsvFile_Project.CommonLayer.Model;
using Upload_XmlAndCsvFile_Project.CommonLayer.Model;
using Upload_XmlAndCsvFile_Project.CommonUtility;

namespace Upload_XmlAndCsvFile_Project.DataAccessLayer
{
    public class UploadFileDL : IUploadFileDL
    {
        public readonly IConfiguration _configuration;
        public readonly MySqlConnection _mySqlConnection;
        public UploadFileDL(IConfiguration configuration)
        {
            _configuration = configuration;
            _mySqlConnection = new MySqlConnection(_configuration["ConnectionStrings:MySqlDBConnectionString"]);
        }

        public async Task<DeleteRecordResponse> DeleteRecord(DeleteRecordRequest request)
        {
            DeleteRecordResponse response = new DeleteRecordResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            try 
            {
                if(_mySqlConnection.State != ConnectionState.Open) 
                {
                    await _mySqlConnection.OpenAsync();
                }

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQueries.DeleteRecord, _mySqlConnection))
                {
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@UserID", request.UserID);
                    int Status = await sqlCommand.ExecuteNonQueryAsync();
                    if(Status <= 0) 
                    {
                        response.IsSuccess = false;
                        response.Message = "Delete Query Not Executed";
                        return response;
                    }
                }

            }
            catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }

        public async Task<ReadRecordResponse> ReadRecord(ReadRecordRequest request)
        {
            ReadRecordResponse response = new ReadRecordResponse();
            response.IsSuccess = true;
            response.Message = "Successful";
            int Count = 0;
            try
            {

                if(_mySqlConnection.State != ConnectionState.Open)
                {
                    await _mySqlConnection.OpenAsync();
                }

                using (MySqlCommand sqlCommand = new MySqlCommand(SqlQueries.ReadRecord, _mySqlConnection))
                {
                    int Offset = (request.PageNumber - 1) * request.RecordPerPage;
                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.Parameters.AddWithValue("@Offset", Offset);
                    sqlCommand.Parameters.AddWithValue("@RecordPerPage", request.RecordPerPage);
                    using (MySqlDataReader dataReader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (dataReader.HasRows)
                        {
                            response.readRecord = new List<ReadRecord>();
                            while(await dataReader.ReadAsync())
                            {
                                ReadRecord getdata = new ReadRecord();
                                getdata.UserName = dataReader["UserName"] != DBNull.Value ? Convert.ToString(dataReader["UserName"]) : string.Empty;
                                getdata.MobileNumber = dataReader["MobileNumber"] != DBNull.Value ? Convert.ToString(dataReader["MobileNumber"]) : string.Empty;
                                getdata.EmailID = dataReader["EmailID"] != DBNull.Value ? Convert.ToString(dataReader["EmailID"]) : string.Empty;
                                getdata.Age = dataReader["Age"] != DBNull.Value ? Convert.ToInt32(dataReader["Age"]) : 0;
                                getdata.Salary = dataReader["Salary"] != DBNull.Value ? Convert.ToInt32(dataReader["Salary"]) : 0;
                                getdata.UserId = dataReader["UserId"] != DBNull.Value ? Convert.ToInt32(dataReader["UserId"]) : 0;
                                getdata.Gender = dataReader["Gender"] != DBNull.Value ? Convert.ToString(dataReader["Gender"]) : string.Empty;
                                if(Count == 0)
                                {
                                    Count++;
                                    response.TotalRecords = dataReader["TotalRecord"] != DBNull.Value ? Convert.ToInt32(dataReader["TotalRecord"]) : 0;
                                    response.TotalPages = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(response.TotalRecords / request.RecordPerPage)));
                                    response.CurrentPage = request.PageNumber;
                                }
                                response.readRecord.Add(getdata);
                            }
                        }
                    }
                }

            }catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<UploadCSVFileResponse> UploadCSVFile(UploadCSVFileRequest request, string Path)
        {
            UploadCSVFileResponse response = new UploadCSVFileResponse();
            List<ExcelBulkUploadParameter> Parameters = new List<ExcelBulkUploadParameter>();
            response.IsSuccess = true;
            response.Message = "Successful";
            try
            {

                if(request.File.FileName.ToLower().Contains(".csv"))
                {

                    DataTable value = new DataTable();
                    //Install Library : LumenWorksCsvReader 
                    using (var csvReader = new CsvReader(new StreamReader(File.OpenRead(Path)), true))
                    {
                        value.Load(csvReader);
                    };

                    for(int i=0; i<value.Rows.Count; i++)
                    {
                        ExcelBulkUploadParameter readData = new ExcelBulkUploadParameter();
                        readData.UserName = value.Rows[i][0] != null ? Convert.ToString(value.Rows[i][0]) : "-1";
                        readData.EmailID = value.Rows[i][1] != null ? Convert.ToString(value.Rows[i][1]) : "-1";
                        readData.MobileNumber = value.Rows[i][2] != null ? Convert.ToString(value.Rows[i][2]) : "-1";
                        readData.Age = value.Rows[i][3] != null ? Convert.ToInt32(value.Rows[i][3]) : -1;
                        readData.Salary = value.Rows[i][4] != null ? Convert.ToInt32(value.Rows[i][4]) : -1;
                        readData.Gender = value.Rows[i][5] != null ? Convert.ToString(value.Rows[i][5]) : "-1";
                        Parameters.Add(readData);
                    }

                    if (Parameters.Count > 0)
                    {
                        if (ConnectionState.Open != _mySqlConnection.State)
                        {
                            await _mySqlConnection.OpenAsync();
                        }

                        foreach (ExcelBulkUploadParameter rows in Parameters)
                        {
                            using (MySqlCommand sqlCommand = new MySqlCommand(SqlQueries.InsertBulkUploadData, _mySqlConnection))
                            {
                                //UserName, EmailID, MobileNumber, Age, Salary, Gender
                                sqlCommand.CommandType = CommandType.Text;
                                sqlCommand.CommandTimeout = 180;
                                sqlCommand.Parameters.AddWithValue("@UserName", rows.UserName);
                                sqlCommand.Parameters.AddWithValue("@EmailID", rows.EmailID);
                                sqlCommand.Parameters.AddWithValue("@MobileNumber", rows.MobileNumber);
                                sqlCommand.Parameters.AddWithValue("@Age", rows.Age);
                                sqlCommand.Parameters.AddWithValue("@Salary", rows.Salary);
                                sqlCommand.Parameters.AddWithValue("@Gender", rows.Gender);
                                int Status = await sqlCommand.ExecuteNonQueryAsync();
                                if (Status <= 0)
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Query Not Executed";
                                    return response;
                                }
                            }
                        }
                    }

                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "InValid File";
                }

            }catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }
            return response;
        }

        public async Task<UploadXMLFileResponse> UploadXMLFile(UploadXMLFileRequest request, string path)
        {
            UploadXMLFileResponse response = new UploadXMLFileResponse();
            List<ExcelBulkUploadParameter> Parameters = new List<ExcelBulkUploadParameter>();
            DataSet dataSet;
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {

                if (request.File.FileName.ToLower().Contains(".xlsx"))
                {
                    FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
                    dataSet = reader.AsDataSet(
                        new ExcelDataSetConfiguration()
                        {
                            UseColumnDataType = false,
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }

                        });

                    for(int i=0; i<dataSet.Tables[0].Rows.Count; i++)
                    {
                        ExcelBulkUploadParameter rows = new ExcelBulkUploadParameter();
                        rows.UserName = dataSet.Tables[0].Rows[i].ItemArray[0] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[0]) : "-1";
                        rows.EmailID = dataSet.Tables[0].Rows[i].ItemArray[1] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[1]) : "-1";
                        rows.MobileNumber = dataSet.Tables[0].Rows[i].ItemArray[2] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[2]) : "-1";
                        rows.Age = dataSet.Tables[0].Rows[i].ItemArray[3] != null ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[3]) : -1;
                        rows.Salary = dataSet.Tables[0].Rows[i].ItemArray[4] != null ? Convert.ToInt32(dataSet.Tables[0].Rows[i].ItemArray[4]) : -1;
                        rows.Gender = dataSet.Tables[0].Rows[i].ItemArray[5] != null ? Convert.ToString(dataSet.Tables[0].Rows[i].ItemArray[5]) : "-1";
                        Parameters.Add(rows);
                    }

                    stream.Close();

                    if (Parameters.Count > 0)
                    {
                        if (ConnectionState.Open != _mySqlConnection.State)
                        {
                            await _mySqlConnection.OpenAsync();
                        }

                        foreach (ExcelBulkUploadParameter rows in Parameters)
                        {
                            using (MySqlCommand sqlCommand = new MySqlCommand(SqlQueries.InsertBulkUploadData, _mySqlConnection))
                            {
                                //UserName, EmailID, MobileNumber, Age, Salary, Gender
                                sqlCommand.CommandType = CommandType.Text;
                                sqlCommand.CommandTimeout = 180;
                                sqlCommand.Parameters.AddWithValue("@UserName", rows.UserName);
                                sqlCommand.Parameters.AddWithValue("@EmailID", rows.EmailID);
                                sqlCommand.Parameters.AddWithValue("@MobileNumber", rows.MobileNumber);
                                sqlCommand.Parameters.AddWithValue("@Age", rows.Age);
                                sqlCommand.Parameters.AddWithValue("@Salary", rows.Salary);
                                sqlCommand.Parameters.AddWithValue("@Gender", rows.Gender);
                                int Status = await sqlCommand.ExecuteNonQueryAsync();
                                if(Status <=0)
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Query Not Executed";
                                    return response;
                                }
                            }
                        }
                    }

                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid File";
                }

            }catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
                await _mySqlConnection.CloseAsync();
                await _mySqlConnection.DisposeAsync();
            }

            return response;
        }
    }
}
