using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CRUDMahasiswaADO
{
    public partial class Form1 : Form
    {
        private readonly string connectionString =
            "Data Source=LAPTOP-5R80O1Q5\\MSSQLSERVER01;Initial Catalog=DBAkademikADO;Integrated Security=True";

        private BindingSource bindingSource = new BindingSource();
        private DataTable dtMahasiswa = new DataTable();

        // =====================================================
        // METHOD SIMPAN LOG — mencatat error ke tabel LogError
        // =====================================================
        private void SimpanLog(string pesan)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO LogError VALUES(GETDATE(), @pesan)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@pesan", pesan);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                // Jika gagal simpan log, abaikan agar tidak loop error
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbJK.Items.Clear();
            cmbJK.Items.Add("L");
            cmbJK.Items.Add("P");

            // Setting Grid
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // BindingNavigator
            bindingNavigator1.BindingSource = bindingSource;

            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_GetMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dtMahasiswa = new DataTable();
                            da.Fill(dtMahasiswa);

                            bindingSource.DataSource = dtMahasiswa;
                            dataGridView1.DataSource = bindingSource;

                            BindControls();
                        }
                    }
                }

                HitungTotal();
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void HitungTotal()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CountMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter outputParam = new SqlParameter("@Total", SqlDbType.Int);
                        outputParam.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(outputParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        lblTotal.Text = "Total Mahasiswa: " + outputParam.Value.ToString();
                    }
                }
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void BindControls()
        {
            txtNIM.DataBindings.Clear();
            txtNama.DataBindings.Clear();
            cmbJK.DataBindings.Clear();
            dtpTanggalLahir.DataBindings.Clear();
            txtAlamat.DataBindings.Clear();
            txtKodeProdi.DataBindings.Clear();

            txtNIM.DataBindings.Add("Text", bindingSource, "NIM");
            txtNama.DataBindings.Add("Text", bindingSource, "Nama");
            cmbJK.DataBindings.Add("Text", bindingSource, "JenisKelamin");
            dtpTanggalLahir.DataBindings.Add("Value", bindingSource, "TanggalLahir");
            txtAlamat.DataBindings.Add("Text", bindingSource, "Alamat");
            txtKodeProdi.DataBindings.Add("Text", bindingSource, "KodeProdi");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    MessageBox.Show("Koneksi berhasil");
                }
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        // =====================================================
        // INSERT — MODUL 12: menggunakan SqlTransaction
        // Jika salah satu gagal, semua dibatalkan (Rollback)
        // =====================================================
        private void btnInsert_Click(object sender, EventArgs e)
        {
            // Validasi input
            if (txtNIM.Text == "")
            {
                MessageBox.Show("NIM harus diisi");
                txtNIM.Focus();
                return;
            }

            if (txtNama.Text == "")
            {
                MessageBox.Show("Nama harus diisi");
                txtNama.Focus();
                return;
            }

            if (cmbJK.Text == "")
            {
                MessageBox.Show("Jenis Kelamin harus dipilih");
                cmbJK.Focus();
                return;
            }

            if (txtKodeProdi.Text == "")
            {
                MessageBox.Show("Kode Prodi harus diisi");
                txtKodeProdi.Focus();
                return;
            }

            // Buka koneksi dan mulai transaksi
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                // Step 1: Insert data mahasiswa via stored procedure
                SqlCommand cmd = new SqlCommand("sp_InsertMahasiswa", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                cmd.Parameters.AddWithValue("@JenisKelamin", cmbJK.Text);
                cmd.Parameters.AddWithValue("@TanggalLahir", dtpTanggalLahir.Value.Date);
                cmd.Parameters.AddWithValue("@Alamat", txtAlamat.Text);
                cmd.Parameters.AddWithValue("@KodeProdi", txtKodeProdi.Text);
                cmd.Parameters.AddWithValue("@TanggalDaftar", DateTime.Now);
                cmd.ExecuteNonQuery();

                // Step 2: Catat aktivitas ke LogAktivitasSalah dalam transaksi yang sama
                SqlCommand cmdLog = new SqlCommand(
                    @"INSERT INTO LogAktivitasSalah (aktivitas, waktu) VALUES (@aktivitas, GETDATE())",
                    conn,
                    trans);
                cmdLog.Parameters.AddWithValue("@aktivitas", "INSERT MAHASISWA : " + txtNIM.Text);
                cmdLog.ExecuteNonQuery();

                // Semua berhasil — commit transaksi
                trans.Commit();

                MessageBox.Show("Data berhasil ditambahkan");
                LoadData();
            }
            catch (SqlException ex)
            {
                // Gagal — batalkan semua perubahan
                trans.Rollback();
                SimpanLog("ROLLBACK INSERT : " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                // Gagal — batalkan semua perubahan
                trans.Rollback();
                SimpanLog("GENERAL ERROR : " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Selalu tutup koneksi
                conn.Close();
            }
        }

        // =====================================================
        // UPDATE — dengan SqlException dan logging
        // =====================================================
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateMahasiswa", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@NIM", txtNIM.Text);
                        cmd.Parameters.AddWithValue("@Nama", txtNama.Text);
                        cmd.Parameters.AddWithValue("@JenisKelamin", cmbJK.Text);
                        cmd.Parameters.AddWithValue("@TanggalLahir", dtpTanggalLahir.Value.Date);
                        cmd.Parameters.AddWithValue("@Alamat", txtAlamat.Text);
                        cmd.Parameters.AddWithValue("@KodeProdi", txtKodeProdi.Text);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result < 0)
                        {
                            MessageBox.Show("Data berhasil diupdate");
                            LoadData();
                        }
                        else
                        {
                            MessageBox.Show("Data tidak ditemukan");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        // =====================================================
        // DELETE — dengan SqlException dan logging
        // =====================================================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult resultConfirm = MessageBox.Show(
                    "Yakin ingin menghapus data?",
                    "Konfirmasi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultConfirm == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_DeleteMahasiswa", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add("@NIM", SqlDbType.Char, 11).Value = txtNIM.Text;

                            conn.Open();
                            int result = cmd.ExecuteNonQuery();

                            if (result < 0)
                            {
                                MessageBox.Show("Data berhasil dihapus");
                                LoadData();
                            }
                            else
                            {
                                MessageBox.Show("Data tidak ditemukan");
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        private void btnResetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        IF OBJECT_ID('dbo.Mahasiswa_Backup') IS NOT NULL
                        BEGIN
                            DELETE FROM dbo.Mahasiswa;
                            INSERT INTO dbo.Mahasiswa
                            SELECT * FROM dbo.Mahasiswa_Backup;
                        END";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Data berhasil direset");
                LoadData();
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("SQL Error : " + ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show("General Error : " + ex.Message);
            }
        }

        // =====================================================
        // TEST SQL INJECTION — query sengaja tidak aman
        // Masukkan ' OR 1=1 -- di txtNIM untuk uji trigger
        // =====================================================
        private void btnTestInjection_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // QUERY TIDAK AMAN - String concatenation (rentan SQL Injection)
                    string query =
                        "UPDATE Mahasiswa SET Nama='" + txtNama.Text +
                        "' WHERE NIM='" + txtNIM.Text + "'";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int result = cmd.ExecuteNonQuery();
                        MessageBox.Show(result + " baris terupdate");
                    }
                }

                LoadData();
            }
            catch (SqlException ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                SimpanLog(ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label6_Click(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void button6_Click(object sender, EventArgs e) { }

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}