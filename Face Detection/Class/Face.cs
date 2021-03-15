using OpenCvSharp;
using OpenCvSharp.Extensions;
using FaceRecognitionDotNet;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Timers;
using Timer = System.Timers.Timer;
using Rect = OpenCvSharp.Rect;

namespace Face_Detection.Class
{
    public class Face
    {
        private static FaceRecognition faceRecognition;
        private static FaceEncoding[] encodings;//編碼過的人臉      
        public static Mat face;//當前人臉
        private static FileInfo[] face_Encs;//資料庫人臉
        public static Timer faceRecognition_Timer;
        public static Timer findFace_Timer;
        public static bool webcam_Is_Open_Or_Not;
        private static readonly object key = new object();//用來防止Face被不同執行續同時讀寫

        public static void Init()
        {
            //建置人臉辨識需要的檔案
            Directory.CreateDirectory("encs");

            faceRecognition = FaceRecognition.Create("dlib-models-master");

            //載入資料庫裡所有人臉
            LoadFace();
            InitFaceRecognitionTimer();
            InitFindFaceTimer();
        }
        //初始化人臉辨識計時器
        private static void InitFaceRecognitionTimer()
        {
            faceRecognition_Timer = new Timer(1000);
            faceRecognition_Timer.Elapsed += FaceRecognition_Tick;
            faceRecognition_Timer.AutoReset = false;
            faceRecognition_Timer.Enabled = false;
        }
        //初始化尋找人臉計時器
        private static void InitFindFaceTimer()
        {
            findFace_Timer = new Timer(30);
            findFace_Timer.Elapsed += FindFace_Tick;
            findFace_Timer.AutoReset = false;
            findFace_Timer.Enabled = false;
        }

        /// <summary>
        /// 尋找臉
        /// </summary>
        /// <param name="frame">鏡頭畫面</param>
        private static void FindFace(Mat frame)
        {
            //將原始畫面編碼
            using (Image frame_image = FaceRecognition.LoadImage(BitmapConverter.ToBitmap(frame)))
            {
                lock (key)
                {
                    //尋找臉
                    var loc = faceRecognition.FaceLocations(frame_image);
                    //如果有找到臉
                    if (loc.Count() != 0)
                    {
                        Rect face_location = new Rect(loc.First().Left, loc.First().Top, loc.First().Right - loc.First().Left, loc.First().Bottom - loc.First().Top);
                        Webcam.face_location = face_location;

                        face = new Mat(frame, face_location);
                    }
                    else
                    {
                        face = null;
                    }
                }
            }
        }

        /// <summary>
        ///辨識人臉
        /// </summary>
        private static void Recognition()
        {
            //將人臉編碼
            FaceEncoding face_encode = null;
            lock (key)
            {
                if (face == null)
                {
                    return;
                }
                face_encode = GetFaceEncoding().FirstOrDefault();
            }
            if (face_encode != null)
            {
                using (face_encode)
                {
                    //尋找使用者
                    int user_enc = FindUser(face_encode);
                    //如果有找到
                    if (user_enc != -1)
                    {
                        string user_name = face_Encs[user_enc].Name.Replace(".enc", "");
                        User.SetName(user_name);
                    }
                    else
                    {
                        User.Reset();
                    }
                }
            }

        }

        /// <summary>
        /// 載入人臉數據庫
        /// </summary>
        private static void LoadFace()
        {
            //載入所有人臉enc
            face_Encs = new DirectoryInfo("encs").GetFiles("*.enc");
            //放到將enc轉檔放到enodings
            encodings = new FaceEncoding[face_Encs.Length];

            int index = 0;
            foreach (FileInfo face_enc in face_Encs)
            {
                string enc_file = "encs/" + face_enc.Name;
                using (FileStream fs = new FileStream(enc_file, FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    encodings[index++] = (FaceEncoding)bf.Deserialize(fs);
                }
            }
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        public static void AddUser(string user_name)
        {
            lock (key)
            {
                //尋找臉
                if (face != null)
                {
                    try
                    {
                        //將人臉編碼
                        FaceEncoding face_encode = null;
                        face_encode = GetFaceEncoding().FirstOrDefault();

                        //將編碼完成的人臉寫入成enc檔案放到數據庫
                        var bf = new BinaryFormatter();
                        using (var fs = new FileStream("encs/" + user_name + ".enc", FileMode.OpenOrCreate))
                        {
                            bf.Serialize(fs, face_encode);
                        }

                        //重新載入人臉數據庫
                        LoadFace();
                        MessageBox.Show("User addition successfully！");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("User addition failed，Please try again！" + ex.Message + "\n" + ex.ToString());
                    }
                }
                else
                {
                    MessageBox.Show("Can't find face,Please try again！");
                }
            }
        }

        //判別使用者
        private static int FindUser(FaceEncoding face_encode)
        {
            int result = -1;
            double[] distances = FaceRecognition.FaceDistances(encodings, face_encode).ToArray();
            double min_distance = 1;

            //跟資料庫裡的使用者作比對
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] < min_distance && distances[i] < 0.4)
                {
                    min_distance = distances[i];
                    result = i;
                }
            }

            return result;
        }

        //人臉辨識
        private static void FaceRecognition_Tick(object source, ElapsedEventArgs e)
        {
            //辨識人臉
            try
            {
                Recognition();
            }
            catch (Exception ex)
            {
                MessageBox.Show("人臉辨識\n" + ex.Message + "\n" + ex.ToString());
            }
            if (webcam_Is_Open_Or_Not)
            {
                faceRecognition_Timer.Start();
            }
        }

        //尋找人臉
        private static void FindFace_Tick(object source, ElapsedEventArgs e)
        {
            //尋找人臉
            try
            {
                FindFace(Webcam.GetFrame());
            }
            catch (Exception ex)
            {
                MessageBox.Show("尋找人臉\n" + ex.Message + "\n" + ex.ToString());
            }
            if (webcam_Is_Open_Or_Not)
            {
                findFace_Timer.Start();
            }
        }

        public static void Stop()
        {
            webcam_Is_Open_Or_Not = false;

            faceRecognition_Timer.Stop();
            findFace_Timer.Stop();

        }

        //將臉的畫面編碼
        private static IEnumerable<FaceEncoding> GetFaceEncoding()
        {
            var targetImage = FaceRecognition.LoadImage(BitmapConverter.ToBitmap(face));
            return faceRecognition.FaceEncodings(targetImage);
        }
    }
}
