using System;
using System.Threading.Tasks;
using System.Windows;
using Face_Detection.Class;

namespace Face_Detection
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            //啟用人臉偵測
            Face.Init();
            //載入所有鏡頭
            Cameras_ViewTab.ItemsSource = Webcam.GetAllWebcam();

            UI.SetUI(this);
        }

        private void StartDisplay_Click(object sender, RoutedEventArgs e)
        {
            if (Webcam.GetWebCam() != -1)
            {
                Face.faceRecognition_Timer.Start();
                new Task(new Action(() => { Webcam.Start(); })).Start();
            }
            else
            {
                MessageBox.Show("Please Select Webcam！");
            }

        }

        private void StopDisplay_Click(object sender, RoutedEventArgs e)
        {
            Webcam.Stop();
            Face.Stop();
            User.Reset();
        }

        private void Cameras_ViewTab_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Device camera = ((Device)e.NewValue);

            Webcam.SetWebCam(camera.id);
        }

        private void AddUser_Button_Click(object sender, RoutedEventArgs e)
        {
            string user_name = AddUser_Textbox.Text;
            if (user_name == "")
            {
                MessageBox.Show("Please enter name！", "ERROR", MessageBoxButton.OK);
            }
            else
            {
                new Task(() => User.AddUser(user_name)).Start();
            }
        }

    }
}
