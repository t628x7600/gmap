using System;
using System.Windows.Forms;
using MapService.Model;
using GMap.NET.WindowsForms;
namespace MapService
{
    public partial class Form1 : Form
    {
        private GMapOverlay markersOverlay = new GMapOverlay("markers"); //放置marker的圖層
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MapClinet service = new MapClinet(markersOverlay,this.gMapControl1);
            
        }
    }
}
