using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Face_Detection.Class
{
    static class UI
    {
        private static MainWindow ui;
        private static Image display;

        /// <summary>
        /// 設定更新視窗
        /// </summary>
        /// <param name="ui"></param>
        public static void SetUI(MainWindow ui)
        {
            UI.ui = ui;
            display = ui.Display_Image;
        }

        /// <summary>
        /// 獲得主視窗
        /// </summary>
        public static MainWindow GetUI()
        {
            return ui;
        }

        /// <summary>
        /// 更新UI
        /// </summary>
        /// <param name="image">圖片</param>
        public static void UpdateDisplay(BitmapImage image)
        {
            image?.Freeze();//凍結圖片 不讓圖片可以被修改 防止進入委派時 畫面會消失的問題
            display.Dispatcher.Invoke(new Action(() => { display.Source = image; }));
        }

        /// <summary>
        /// 更新使用者名稱
        /// </summary>
        /// <param name="name">名子</param>
        public static void UpdateUserName(string name)
        {
            ui.UserName_Label.Dispatcher.BeginInvoke(new Action(() => { ui.UserName_Label.Content = "User：" + name; }));
        }

    }
}
