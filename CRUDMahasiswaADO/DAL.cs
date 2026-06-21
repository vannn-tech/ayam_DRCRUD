using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRUDMahasiswaADO
{
    internal class DAL
    {
        static string connectionString = "Data Source=LAPTOP-5R80O1Q5\\MSSQLSERVER01;Initial Catalog=DBAkademikADO;Integrated Security=True";

        public static string GetConnectionString()
        {
            string connectionString = "Data Source=LAPTOP-5R80O1Q5\\MSSQLSERVER01;Initial Catalog=DBAkademikADO;Integrated Security=True";
            return connectionString;
        }

        SqlConnection conn = new SqlConnection(GetConnectionString());

        SqlDataAdapter da; 
        DataTable dtMahasiswa;
        DataTable dtProdi;

        public static string GetLoacalIPAddress()
        {
            string localIP = string.Empty;
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting local IP address: " + ex.Message);
            }
            return localIP;
        }

        public int CountMhs()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outputParam);
            cmd.ExecuteNonQuery();
            return Convert.ToInt32(outputParam.Value);
        }


    }
}
