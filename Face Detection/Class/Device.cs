namespace Face_Detection.Class
{
    public class Device
    {
        ///<summary>裝置名子</summary>
        public string name { get; set; }

        ///<summary>裝置ID</summary>
        public int id { get; set; }

        ///<summary>初始化</summary>
        ///<param name="name">裝置名子</param>
        ///<param name="name">鏡頭ID</param>
        public Device(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }
}
