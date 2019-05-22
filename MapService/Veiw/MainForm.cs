using System;
using System.Windows.Forms;
using MapService.Model;
using GMap.NET.WindowsForms;
using MapService.Veiw;

namespace MapService
{
    public partial class MainForm : Form
    {
        private GMapOverlay markersOverlay = new GMapOverlay("markers"); //放置marker的圖層
        private MapClinet service;
        private ReportForm reportForm = new ReportForm();
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            service = new MapClinet(markersOverlay,this.gMapControl1);
            
        }
        private async void Alarm_btn_Click(object sender, EventArgs e)
        {
            string addr = await service.GetAddress();
            this.Hide();
            reportForm.Show();
        }
    }
}
