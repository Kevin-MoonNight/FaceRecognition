using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using DirectShowLib;

namespace Face_Detection.Class
{
    static class Webcam
    {
        private static VideoCapture webcam = new VideoCapture();
        private static bool webcam_stop = true;
        private static int webcam_id = -1;
        public static Rect face_location;

        /// <summary>
        /// 開始傳輸畫面
        /// </summary>
        /// <param name="display">主視窗放圖片的物件</param>
        /// <param name="mainWindow">主視窗物件 用來更新UI</param>
        public static void Start()
        {
            webcam_stop = false;
            //開啟鏡頭
            webcam.Open(webcam_id);
            if (webcam.IsOpened())
            {
                Face.findFace_Timer.Start();
                Face.webcam_Is_Open_Or_Not = true;
                while (!webcam_stop)
                {
                    //刷新畫面      
                    UI.UpdateDisplay(GetCameraImage());
                    //等待
                    Cv2.WaitKey(30);
                }
                //更新畫面
                UI.UpdateDisplay(null);
            }
            webcam.Release();
        }

        /// <summary>
        /// 暫停傳輸畫面
        /// </summary>
        public static void Stop()
        {
            webcam_stop = true;
        }

        /// <summary>
        /// 獲取鏡頭畫面
        /// </summary>
        private static BitmapImage GetCameraImage()
        {
            //讀取畫面
            Mat frame = GetFrame();
            //將臉人位置畫出來
            if (face_location != null && Face.face != null)
            {
                frame.Rectangle(face_location, Scalar.Green, 4, LineTypes.Link8);
            }

            //轉換格式後輸出
            return MatToImageSource(frame);
        }

        /// <summary>
        /// 讀取畫面
        /// </summary>
        public static Mat GetFrame()
        {
            Mat frame = new Mat();
            webcam.Read(frame);

            return frame;
        }

        /// <summary>
        /// 將Mat轉成ImageSource型式
        /// </summary>
        /// <param name="frame">鏡頭畫面</param>
        private static BitmapImage MatToImageSource(Mat frame)
        {
            using (Bitmap bitmap = BitmapConverter.ToBitmap(frame))
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.StreamSource = memory;
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.EndInit();

                return result;
            }
        }

        /// <summary>
        /// 設定鏡頭ID
        /// </summary>
        public static void SetWebCam(int id)
        {
            webcam_id = id;
        }

        /// <summary>
        /// 獲取鏡頭ID
        /// </summary>
        public static int GetWebCam()
        {
            return webcam_id;
        }

        /// <summary> 
        /// 獲得所有鏡頭裝置名稱及ID 
        /// </summary>
        public static List<Device> GetAllWebcam()
        {
            //讀取所有鏡頭裝置
            DsDevice[] devices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            //依序添加到List裡面
            List<Device> webcams = new List<Device>();
            for (int i = 0; i < devices.Length; i++)
            {
                webcams.Add(new Device(devices[i].Name, i));
            }

            return webcams;
        }

        public static bool IsOpened(){
            return webcam.IsOpened();
        }
 
    }
}
