namespace Face_Detection.Class
{
    public class User
    {
        private static string name;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name">使用者名子</param>
        public User(string name)
        {
            User.name = name;
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="user_name">使用者id</param>
        public static void AddUser(string user_name)
        {
            if (Webcam.IsOpened())
            {
                Face.AddUser(user_name);
            }
        }

        /// <summary>
        /// 判斷有無登入
        /// </summary>
        public static bool IsLoginOrNot()
        {
            bool result = false;
            if (GetName() != null)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 設定名子
        /// </summary>
        /// <param name="name">名子</param>
        public static void SetName(string name)
        {
            User.name = name;
            UI.UpdateUserName(User.name);
        }

        /// <summary>
        /// 獲得名子
        /// </summary>
        public static string GetName()
        {
            return name;
        }

        /// <summary>
        /// 重設使用者
        /// </summary>
        public static void Reset()
        {
            name = null;
            UI.UpdateUserName("");
        }

    }
}
