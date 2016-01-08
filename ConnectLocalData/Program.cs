using System;
using System.IO;
using System.Data.SqlClient;

using System.Net;
using System.Data;
using Microsoft.VisualBasic.FileIO;

namespace ConnectLocalData
{


    static class Program
    {
       


        static void Main()
        {



            string connection = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = F:\\CONNECTLOCALDATA\\CONNECTLOCALDATA\\ASSESSMENTDB.MDF; Integrated Security = True; Connect Timeout = 30";
           
            string Filename = "output.csv";
            string path = Directory.GetCurrentDirectory();
            

            //downloads the file from API
            using (WebClient wc = new WebClient())
            {
                try
                {
                    wc.DownloadFile("http://ondemand.websol.barchart.com/getQuote.csv?apikey=510a0e5d6b43d16805589ee06a0e8e8c&symbols=IBM,FB,AAPL", Filename);
                }
                catch (Exception e) { }
            }

            //create datable to dump into SQL
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(path + "\\" + Filename))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            

            //Dump data into SQL
            using (SqlConnection conn = new SqlConnection(connection))
            {
                using (SqlCommand comm = new SqlCommand())
                {
                    comm.Connection = conn;
                    for (int i = 0; i < csvData.Rows.Count; i++)
                    {
                        comm.CommandText = "INSERT INTO Test2 (symbol,name,dayCode,serverTimestamp,mode,lastPrice,tradeTimestamp,netChange,percentChange,unitCode,[open],high,low,[close],numTrades,dollarVolume,flag,volume,previousVolume)VALUES('" + csvData.Rows[i].ItemArray[0] + "','" + csvData.Rows[i].ItemArray[1] + "', '" + csvData.Rows[i].ItemArray[2] + "','" + csvData.Rows[i].ItemArray[3] + "','" + csvData.Rows[i].ItemArray[4] + "','" + csvData.Rows[i].ItemArray[5] + "','" + csvData.Rows[i].ItemArray[6] + "','" + csvData.Rows[i].ItemArray[7] + "','" + csvData.Rows[i].ItemArray[8] + "','" + csvData.Rows[i].ItemArray[9] + "','" + csvData.Rows[i].ItemArray[10] + "','" + csvData.Rows[i].ItemArray[11] + "','" + csvData.Rows[i].ItemArray[12] + "','" + csvData.Rows[i].ItemArray[13] + "','" + csvData.Rows[i].ItemArray[14] + "','" + csvData.Rows[i].ItemArray[15] + "','" + csvData.Rows[i].ItemArray[16] + "','" + csvData.Rows[i].ItemArray[17] + "','" + csvData.Rows[i].ItemArray[18] + "');";
                        try
                        {
                            conn.Open();
                            comm.ExecuteNonQuery();
                            conn.Close();
                        }
                        catch (SqlException f)
                        {
                            Console.WriteLine(f.Message);
                            
                        }
                    }
                }


            }
        }
    }
}