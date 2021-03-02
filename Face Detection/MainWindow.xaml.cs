using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Face_Detection
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        VideoCapture camera;
        bool camera_stop = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            camera = new VideoCapture(0);
            camera_stop = false;
            Start();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            camera_stop = true;
        }

        private void Start()
        {
            if (camera.IsOpened())
            {
                while (true)
                {
                    image.Source = GetCameraImage();
                    if (camera_stop)
                    {
                        image.Source = null;
                        break;
                    }
                    Cv2.WaitKey(30);
                }
            }
        }

        private ImageSource GetCameraImage()
        {
            Mat frame = new Mat();
            camera.Read(frame);

            ImageSource result;
            result = MatToImageSource(frame);
            return result;
        }

        private BitmapImage MatToImageSource(Mat frame)
        {
            Bitmap bitmap;
            bitmap = BitmapConverter.ToBitmap(frame);

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


    }
}
