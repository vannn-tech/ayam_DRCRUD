using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting; // Pastikan namespace ini ditambahkan untuk Chart

namespace CRUDMahasiswaADO
{
    public partial class DashboardRekap : Form
    {
        // 8. Tambahkan Code berikut pada class Form Dashboard
        DAL dbLogic = new DAL();
        bool isInitializing = true;
        DataTable dt;
        int button = 0;

        public DashboardRekap()
        {
            InitializeComponent();
        }

        // 10. Method loadDataChart()
        public void loadDataChart()
        {
            // chartProdi = Chart (area grafik besar)
            chartProdi.Series.Clear();
            chartProdi.Titles.Clear();
            chartProdi.Legends.Clear();
            chartProdi.ChartAreas.Clear();

            ChartArea ca = new ChartArea("MainArea");
            ca.AxisX.Title = "Program Studi";
            ca.AxisY.Title = "Jumlah Mahasiswa";
            ca.AxisX.LabelStyle.Angle = -45;
            ca.BackColor = Color.Transparent;
            chartProdi.ChartAreas.Add(ca);

            try
            {
                if (button == 1)
                {
                    dt = dbLogic.getDataChartByTahun(dtpTanggalMasuk.Value);
                }
                else
                {
                    dt = dbLogic.getAllDataChart();
                }

                SeriesChartType tipe = (SeriesChartType)cmbTipe.SelectedValue;
                if (tipe == SeriesChartType.Column)
                {
                    Series s = new Series("Mahasiswa");
                    s.ChartType = SeriesChartType.Column;

                    foreach (DataRow row in dt.Rows)
                    {
                        string prodi = row["NamaProdi"].ToString();
                        int jumlah = Convert.ToInt32(row["JmlhMhs"]);
                        s.Points.AddXY(prodi, jumlah);
                    }
                    chartProdi.Series.Add(s);
                }
                else
                {
                    Series s = new Series("Jumlah Mahasiswa");
                    s.ChartType = tipe;
                    s.IsValueShownAsLabel = true;
                    s.Label = "#VAL";
                    s.LegendText = "#VALX";

                    foreach (DataRow row in dt.Rows)
                    {
                        string prodi = row["NamaProdi"].ToString();
                        int jumlah = Convert.ToInt32(row["JmlhMhs"]);
                        s.Points.AddXY(prodi, jumlah);
                    }
                    chartProdi.Series.Add(s);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal load data: " + ex.Message);
            }

            Title title = new Title("Jumlah Mahasiswa per Program Studi", Docking.Top, new Font("Arial", 14, FontStyle.Bold), Color.DarkBlue);
            chartProdi.Titles.Add(title);
            Legend Legend = new Legend("MainLegend");
            Legend.Docking = Docking.Right;
            chartProdi.Legends.Add(Legend);
        }

        // 12. a. Button Load Click
        private void btnLoad_Click(object sender, EventArgs e)
        {
            button = 1;
            loadDataChart();
        }

        // 12. b. Button Reset Click
        private void btnReset_Click(object sender, EventArgs e)
        {
            button = 0;
            loadDataChart();
        }

        // 12. c. Button Data Mahasiswa Click
        private void btnDataMahasiswa_Click(object sender, EventArgs e)
        {
            DataMahasiswa frm1 = new DataMahasiswa();
            frm1.Show();
            this.Hide();
        }

        // === FIX UTAMA ===
        // Event ini harus terhubung ke chart1.SelectedIndexChanged (combo Kolom/Pie).
        // Cek di Designer.cs, baris yang mirip:
        //   this.chart1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
        // Method ini harus diisi logikanya (bukan dibiarkan kosong),
        // supaya saat dropdown tipe chart diganti, grafik langsung reload.

        // Jaga-jaga kalau ternyata yang terhubung ke chart1 adalah comboBox2.
        // Kalau setelah dicek di Designer ternyata comboBox2 ini TIDAK terhubung
        // ke kontrol apapun (artifact/sisa generate Designer), boleh dihapus.

        // Method bawaan form yang tidak digunakan, dibiarkan kosong
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void DashboardRekap_Load(object sender, EventArgs e) 
        {
            dtpTanggalMasuk.MinDate = new DateTime(2000, 1, 1);
            dtpTanggalMasuk.Format = DateTimePickerFormat.Custom;
            dtpTanggalMasuk.CustomFormat = "yyyy";
            dtpTanggalMasuk.ShowUpDown = true;
            dtpTanggalMasuk.MaxDate = DateTime.Now;

            // chart1 = ComboBox (untuk pilih tipe chart: Kolom/Pie)
            cmbTipe.DropDownStyle = ComboBoxStyle.DropDownList;
            var items = new List<KeyValuePair<string, SeriesChartType>>()
            {
                new KeyValuePair<string, SeriesChartType>("Kolom", SeriesChartType.Column),
                new KeyValuePair<string, SeriesChartType>("Pie", SeriesChartType.Pie)
            };

            isInitializing = true;

            cmbTipe.DataSource = items;
            cmbTipe.DisplayMember = "Key";
            cmbTipe.ValueMember = "Value";
            cmbTipe.SelectedIndex = 0;

            isInitializing = false;
            loadDataChart();
        }

        private void dtpTanggalMasuk_ValueChanged(object sender, EventArgs e)
        {

        }

        private void chartProdi_Click(object sender, EventArgs e)
        {

        }

        private void cmbTipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(isInitializing) 
                return;

            if (button == 1)
            {
                loadDataChart(); // disarankan tetap reload, modul asli kosong di branch ini
            }
            else
            {
                loadDataChart();
            }
        }
    }
}