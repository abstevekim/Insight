using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Getting Connection ...");
            Console.WriteLine();

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString =
              "Data Source=(localdb)\\MSSQLLocalDB;" +
              "Initial Catalog=JsonTest;" +
              "Integrated Security=SSPI;";
            conn.Open();

            HttpClient client = new HttpClient();

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // ----------------------------------------------//
            // Insert api data to [Activity_tbl] in database
            // ----------------------------------------------//
            StringBuilder strBuilder = new StringBuilder();

            using (var response = await client.GetAsync("https://www.boredapi.com/api/activity"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine("1.  First API Call ...");
                Console.WriteLine("1.1 Check API response data ...");

                Console.WriteLine(apiResponse);

                dynamic data = JObject.Parse(apiResponse);

                strBuilder.Append("INSERT INTO [Activity_tbl] (activity, type, participants, price, link, [key], accessibility) VALUES ");
                strBuilder.Append("('").Append(data.activity).Append("'");
                strBuilder.Append(", '").Append(data.type).Append("'");
                strBuilder.Append(", ").Append(data.participants);
                strBuilder.Append(", ").Append(data.price);
                strBuilder.Append(", '").Append(data.link).Append("'");
                strBuilder.Append(", '").Append(data.key).Append("'");
                strBuilder.Append(", ").Append(data.accessibility).Append(")");

            }

            string sqlQuery = strBuilder.ToString();
            using (SqlCommand command = new SqlCommand(sqlQuery, conn)) 
            {
                command.ExecuteNonQuery(); //execute the Query
                Console.WriteLine();
                Console.WriteLine("1.2 Query Executed. insert Activaity-data to database !!");
            }
            Console.WriteLine();
            Console.WriteLine("1.3 First API call done.");

            // ---------------------------------------------------------------------- //
            // Insert api data to [ChartName_tbl] & [ChartName_Bpi_tbl] in database
            // ---------------------------------------------------------------------- //
            StringBuilder strBuilder_chartName = new StringBuilder();
            StringBuilder strBuilder_chartName_Bpi = new StringBuilder();

            using (var response = await client.GetAsync("https://api.coindesk.com/v1/bpi/currentprice.json"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("2.  Second API Call ...");
                Console.WriteLine("2.1 Check API response data ...");
                Console.WriteLine();

                Console.WriteLine(apiResponse);

                dynamic data = JObject.Parse(apiResponse);

                strBuilder_chartName.Append("INSERT INTO [ChartName_tbl] (updated, updatedISO, updateduk, disclaimer, chartName) VALUES ");
                strBuilder_chartName.Append("('").Append(data.time.updated).Append("'");
                strBuilder_chartName.Append(", '").Append(data.time.updatedISO).Append("'");
                strBuilder_chartName.Append(", '").Append(data.time.updateduk).Append("'");
                strBuilder_chartName.Append(", '").Append(data.disclaimer).Append("'");
                strBuilder_chartName.Append(", '").Append(data.chartName).Append("')");

                string sqlQuery2 = strBuilder_chartName.ToString();
                using (SqlCommand command = new SqlCommand(sqlQuery2, conn)) 
                {
                    command.ExecuteNonQuery(); //execute the Query
                    Console.WriteLine();
                    Console.WriteLine("1.2 Query Executed. insert ChartName-data to database !!");
                    Console.WriteLine();
                }

                dynamic data_bpi = data.bpi;

                foreach (dynamic area in data.bpi)
                {
                    foreach  (var info in area) {

                        strBuilder_chartName_Bpi.Append("INSERT INTO [ChartName_Bpi_tbl] (chartName, updated, code, symbol, rate, description, rate_float) VALUES ");
                        strBuilder_chartName_Bpi.Append("('").Append(data.chartName).Append("'");
                        strBuilder_chartName_Bpi.Append(", '").Append(data.time.updated).Append("'");
                        strBuilder_chartName_Bpi.Append(", '").Append(info.code).Append("'");
                        strBuilder_chartName_Bpi.Append(", '").Append(info.symbol).Append("'");
                        strBuilder_chartName_Bpi.Append(", '").Append(info.rate).Append("'");
                        strBuilder_chartName_Bpi.Append(", '").Append(info.description).Append("'");
                        strBuilder_chartName_Bpi.Append(", ").Append(info.rate_float).Append(")");

                        string sqlQuery3 = strBuilder_chartName_Bpi.ToString();
                        using (SqlCommand command = new SqlCommand(sqlQuery3, conn)) 
                        {
                            command.ExecuteNonQuery(); //execute the Query
                            Console.WriteLine();
                            Console.WriteLine("1.3 Query Executed. insert ChartName-bpi [" +  info.code + "] to database !!");
                        }

                        strBuilder_chartName_Bpi.Clear();
                    }

                }

                Console.WriteLine();
                Console.WriteLine("1.4 Second API call done.");

            }

        }

    }
}