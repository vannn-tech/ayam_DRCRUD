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

        public DataTable GetMhs()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            da = new SqlDataAdapter(cmd);
            dtMahasiswa = new DataTable();
            da.Fill(dtMahasiswa);
            return dtMahasiswa;
        }

        public void InsertMhs(string nim, string nama, string alamat, string jeniskelamin, DateTime tanggallahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlTransaction trans = conn.BeginTransaction();
            try
            {
                SqlCommand command = new SqlCommand("sp_InsertMahasiswa", conn);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("PNIM", nim);
                command.Parameters.AddWithValue("pNama", nama);
                command.Parameters.AddWithValue("pAlamat", alamat);
                command.Parameters.AddWithValue("pTanggalLahir", tanggallahir);
                command.Parameters.AddWithValue("pJenisKelamin", jeniskelamin);
                command.Parameters.AddWithValue("pKodeProdi", kodeProdi);
                command.Parameters.AddWithValue("pFoto", foto);
                command.ExecuteNonQuery();
                trans.Commit();
            }
            catch (Exception ex)
            {
                trans.Rollback();
            }
            finally
            {
                conn.Close();
            }
        }

        public void UpdateMhs(string nim, string nama, string alamat, string jeniskelamin, DateTime tanggallahir, string kodeProdi, byte[] foto)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand command = new SqlCommand("sp_UpdateMahasiswa", conn);
            command.Parameters.AddWithValue("PNIM", nim);
            command.Parameters.AddWithValue("pNama", nama);
            command.Parameters.AddWithValue("pAlamat", alamat);
            command.Parameters.AddWithValue("pJenisKelamin", jeniskelamin);
            command.Parameters.AddWithValue("pTanggalLahir", tanggallahir);
            command.Parameters.AddWithValue("pKodeProdi", kodeProdi);
            command.Parameters.AddWithValue("pFoto", foto);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteNonQuery();
        }

        public void DeleteMhs(string nim)
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn);
            cmd.Parameters.AddWithValue("pNIM", nim);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }


    }
}
