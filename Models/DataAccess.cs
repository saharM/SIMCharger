using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIMCharger.Models
{
    class DataAccess
    {   
        public static Task<List<SearchResult>> SearchAsyncTask(string mobileNum, DateTime mobileActivateDate, DateTime mobileLastChargeDate)
		{
			Debug.WriteLine(string.Format("Starting search for Mobile Number: {0}, Active Date: {1}, Last Charge Date: {2}", mobileNum, mobileActivateDate, mobileLastChargeDate));

            SqlConnection conn = new SqlConnection(@"Data Source=172.25.25.3\sql2008;Initial Catalog=TDS;Persist Security Info=True;User ID=db3;Password=db3P@ss");
            conn.Open();
            SqlCommand comm = new SqlCommand("", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "MMCGPRSPosMobileNumberSelect";
            comm.Parameters.Add(new SqlParameter("@GPRSMobileID", DBNull.Value));
            comm.Parameters.Add(new SqlParameter("@ActivationDate", mobileActivateDate));
            comm.Parameters.Add(new SqlParameter("@LastChargeDate", mobileLastChargeDate));
            comm.Parameters.Add(new SqlParameter("@GPRSMobileNumber", mobileNum));

            var searchResult = Task.Run<List<SearchResult>>( () => 
            {
                try
                {
                    List<SearchResult> results = new List<SearchResult>();
                    
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        SearchResult result = new SearchResult();
                        result.MobileNumber = reader["GPRSMobileNumber"].ToString();
                        result.ActivationDate = reader["ActivationDate"].ToString();
                        result.LastChargeDate = reader["LastChargeDate"].ToString();
                        result.SimSerial = reader["SimSerial"].ToString();
                        result.RegFullNameOfOwner = reader["RegFullNameOfOwner"].ToString();
                        results.Add(result);
                    }
                    return results;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    conn.Close();
                }
            });
			//await searchResult;

			Debug.WriteLine(string.Format("Finished search for Mobile Number: {0}, Active Date: {1}, Last Charge Date: {2}", mobileNum, mobileActivateDate, mobileLastChargeDate));

			return searchResult;
		}

        public static void UpdateMobileDates(string mobileNum, DateTime mobileActivateDate, DateTime mobileLastChargeDate)
		{
			Debug.WriteLine(string.Format("Starting Update for Mobile Number: {0}, Active Date: {1}, Last Charge Date: {2}", mobileNum, mobileActivateDate, mobileLastChargeDate));

            SqlConnection conn = new SqlConnection(@"Data Source=172.25.25.3\sql2008;Initial Catalog=TDS;Persist Security Info=True;User ID=db3;Password=db3P@ss");
            conn.Open();
            SqlCommand comm = new SqlCommand("", conn);
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "MMCGPRSPosMobileNumberSelect";
            comm.Parameters.Add(new SqlParameter("@GPRSMobileNumber", mobileNum));
            comm.Parameters.Add(new SqlParameter("@ActivationDate", mobileActivateDate));
            comm.Parameters.Add(new SqlParameter("@LastChargeDate", mobileLastChargeDate));
            comm.ExecuteNonQuery();

			Debug.WriteLine(string.Format("Finished Update for Mobile Number: {0}, Active Date: {1}, Last Charge Date: {2}", mobileNum, mobileActivateDate, mobileLastChargeDate));            			
		}

    }
}
