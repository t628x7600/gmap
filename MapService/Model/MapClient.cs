using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.MapProviders;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
namespace MapService.Model
{
    /// <summary>
    /// Map 控制服務相關API
    /// </summary>
    public class MapClinet
    {

        /// <summary>
        /// Map 控制元件
        /// </summary>
        private GMapControl mapControl =null;

        /// <summary>
        /// Map 藍圖
        /// </summary>
        private GMapOverlay mapOverlay = null;

        /// <summary>
        /// 標記的key value集合
        /// </summary>
        private Dictionary<string,GMapMarker> marker = new Dictionary<string,GMapMarker>();

        public MapClinet(GMapOverlay mapOverlay,GMapControl control)
        {
            this.mapControl = control;
            this.mapOverlay = mapOverlay;
            //緩存位置
            mapControl.CacheLocation = Environment.CurrentDirectory + "\\GMapCache\\"; //緩存位置

            //使用google 地圖
            mapControl.MapProvider = GMapProviders.GoogleChinaMap;

            //地圖最小縮放比例
            mapControl.MinZoom = 5;  //最小比例

            //地圖最大縮放比例
            mapControl.MaxZoom = 24;

            //地圖當前縮放比例
            mapControl.Zoom = 15;

            //不顯示中心十字點
            mapControl.ShowCenter = false;

            mapControl.DragButton = MouseButtons.Left; //左鍵拖拽地圖

            //地圖中心位置： 高科大楠梓校區
            mapControl.Position = new PointLatLng(22.7246559, 120.312414);
            
            //加入地圖
            mapControl.Overlays.Add(mapOverlay);

            GMarkerGoogle tmp;
            tmp = InsertMarker(mapControl.Position, GMarkerGoogleType.blue);

            InsertToolText(tmp, "當前位置");

            tmp = InsertMarker(new PointLatLng(22.7264891, 120.3089218), GMarkerGoogleType.red);

            InsertToolText(tmp, "車禍事故");

            tmp = InsertMarker(new PointLatLng(22.7203373, 120.3182168), GMarkerGoogleType.red);

            InsertToolText(tmp, "車禍事故");
        }

        /// <summary>
        /// 插入marker
        /// </summary>
        /// <param name="latLng"></param>
        /// <param name="type"></param>
        public GMarkerGoogle InsertMarker(PointLatLng latLng, GMarkerGoogleType type) 
        {
            if (this.mapOverlay == null)
                return null;

            GMarkerGoogle marker = new GMarkerGoogle(latLng, type);

            this.mapOverlay.Markers.Add(marker);
            return marker;
        }

        public GMarkerGoogle InsertMarker(PointLatLng latLng,Bitmap picture)
        {
            if (this.mapOverlay == null)
                return null;

            GMarkerGoogle marker = new GMarkerGoogle(latLng, picture);

            this.mapOverlay.Markers.Add(marker);
            return marker;
        }


        public void InsertToolText(GMarkerGoogle marker,string str)
        {
            marker.ToolTipText = str;
            marker.ToolTip.Fill = new SolidBrush(Color.FromArgb(100, Color.Black));
            marker.ToolTip.Foreground = Brushes.White;
            marker.ToolTip.TextPadding = new Size(20, 20);
        }

        /// <summary>
        /// 取得經緯度位置
        /// </summary>
        /// <returns></returns>
        public PointLatLng GetPoint()
        {
            if (mapControl != null)
                return mapControl.Position;
            throw new ArgumentNullException("GMapControl is not create");
        }

        public async Task<string> GeocoderAsync(PointLatLng latLng)
        {
            string key = "AIzaSyA9cQx9K4sOFlibH61lujKHuGAokbEmy9g";
            string googleApi = string.Format(@"https://maps.googleapis.com/maps/api/geocode/json?" +
                $"latlng={latLng.Lat},{latLng.Lng}&key={key}");


            string result = await GetAddressAsync(googleApi);

            return result;
        }

        private static async Task<string> GetAddressAsync(string apiStr)
        {
            using (HttpClient client = new HttpClient())
            {
                // Call asynchronous network methods in a try/catch block to handle exceptions
                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiStr);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);
                    var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseBody);

                    return responseBody;
                }
                catch (HttpRequestException e)
                {
                    throw e;
                }
            }
        }


        public static string GetAddress()
        {
            string result = await GeocoderAsync(GetPoint());
        }




    }
}
