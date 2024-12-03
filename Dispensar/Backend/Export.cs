using System;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using OfficeOpenXml;

namespace Dispensar
{
    public class ExcelExporter
    {
        private readonly string connectionString;

        public ExcelExporter(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void ExportDataToExcel(string tableName, string filePath)
        {
            try
            {
                DataTable dataTable = GetDataFromDatabase(tableName);

                if (dataTable.Rows.Count > 0)
                {
                    using (ExcelPackage excel = new ExcelPackage())
                    {
                        var worksheet = excel.Workbook.Worksheets.Add(tableName);

                        for (int i = 0; i < dataTable.Columns.Count; i++)
                        {
                            worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
                        }

                        for (int row = 0; row < dataTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < dataTable.Columns.Count; col++)
                            {
                                worksheet.Cells[row + 2, col + 1].Value = dataTable.Rows[row][col];
                            }
                        }

                        FileInfo excelFile = new FileInfo(filePath);
                        excel.SaveAs(excelFile);

                        Console.WriteLine($"Datele au fost exportate cu succes în {filePath}");
                    }
                }
                else
                {
                    Console.WriteLine("Nu există date de exportat.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la exportul datelor: {ex.Message}");
            }
        }

        private DataTable GetDataFromDatabase(string tableName)
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = $"SELECT * FROM {tableName}";
                    using (var cmd = new MySqlCommand(query, connection))
                    using (var adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la preluarea datelor din baza de date: {ex.Message}");
            }
            return dataTable;
        }
    }
}
