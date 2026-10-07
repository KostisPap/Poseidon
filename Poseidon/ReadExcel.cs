using ExcelDataReader;
using Poseidon.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Poseidon
{
    internal class ReadExcel
    {
        public List<MPSection> GetMPGeometry(string locationName)
        {
            string rootFolder = Environment.GetEnvironmentVariable("POSEIDON_ROOT") ?? Directory.GetCurrentDirectory();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string directory = Path.Combine(rootFolder, "data", "geometry");
            string[] file = System.IO.Directory.GetFiles(directory, "*Geometry*");

            List<MPSection> mpGeometry = new List<MPSection>();

            using (var stream = File.Open(file[0], FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {

                        ConfigureDataTable = tableReader => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                            /* Skip the first 4 rows */
                            ReadHeaderRow = rowReader => { for (int i = 0; i < 3; i++) { rowReader.Read(); } }
                        }
                    });

                    // The result of each spreadsheet is in result.Tables
                    var waterDepthTable = result.Tables["MP_Geometry"];

                    for (int row = 0; row < waterDepthTable.Rows.Count; row++)
                    {
                        var items = waterDepthTable.Rows[row].ItemArray;
                        /* Check if the location name is the same as the input name */
                        if (items[1].ToString().Length > 4)
                        {
                            if (items[1].ToString()[..3] == locationName)
                            {
                                int row2 = row + 2;
                                items = waterDepthTable.Rows[row2].ItemArray;
                                do
                                {

                                    MPSection sectionTemp = new();

                                    sectionTemp.Index = double.Parse(items[1].ToString());
                                    sectionTemp.Length = double.Parse(items[2].ToString()) / 1000.0;
                                    sectionTemp.DiaTop = double.Parse(items[3].ToString()) / 1000.0;
                                    sectionTemp.DiaBot = double.Parse(items[4].ToString()) / 1000.0;
                                    if (Double.TryParse(items[5].ToString(), out double thickness))
                                        sectionTemp.Thickness = thickness / 1000.0;

                                    row2++;
                                    items = waterDepthTable.Rows[row2].ItemArray;

                                    mpGeometry.Add(sectionTemp);
                                } while (items[1].ToString() != "");
                                break;
                            }
                        }
                    }
                }
            }
            return mpGeometry;
        }

        public Location GetLocationDepths(string locationName)
        {
            string rootFolder = Environment.GetEnvironmentVariable("POSEIDON_ROOT") ?? Directory.GetCurrentDirectory();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            string directory = Path.Combine(rootFolder, "data", "geometry");
            string[] file = System.IO.Directory.GetFiles(directory, "*Water Depth*");

            Location location = new Location();

            using (var stream = File.Open(file[0], FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // 2. Use the AsDataSet extension method
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {

                        ConfigureDataTable = tableReader => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                            /* Skip the 1st and 2nd empty rows and values in 3rd row become headers */
                            ReadHeaderRow = rowReader => { for (int i = 0; i < 2; i++) { rowReader.Read(); } }
                        }
                    });

                    // The result of each spreadsheet is in result.Tables
                    var waterDepthTable = result.Tables["LocationWaterDepths"];

                    foreach (DataRow row in waterDepthTable.Rows)
                    {
                        var items = row?.ItemArray;
                        /* Check if the location name is the same as the input name */
                        if (items[2].ToString() == locationName)
                        {
                            location.Name = items[2].ToString();
                            location.Depth = double.Parse(items[3].ToString());
                            break;
                        }
                    }
                }
            }
            return location;
        }
    }
}
