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

        
    }
}