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

    }
}
