using System;
using System.IO;
using Newtonsoft.Json;

namespace DownMovies
{
    /// <summary>
    /// 下载任务上下文对象
    /// </summary>
    public class TaskContext
    {
        /// <summary>
        /// 数据目录
        /// </summary>
        public string DataRoot { get; set; }
        /// <summary>
        /// 下载目录
        /// </summary>
        public string OutputRoot { get; set; }
        /// <summary>
        /// 任务数据
        /// </summary>
        public TaskData Data { get; private set; }
        #region 单例模式

        /// <summary>
        /// 任务实例
        /// </summary>
        static TaskContext _instance;
        static readonly object LockObject = new object();

        public static TaskContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new TaskContext();
                        }
                    }
                }
                return _instance;
            }
        }

        private TaskContext()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            var root = System.Reflection.Assembly.GetExecutingAssembly().GetLocation();
            DataRoot = PathUtility.Combine(root, "data");
            OutputRoot = PathUtility.Combine(root, "下载");
            Directory.CreateDirectory(DataRoot);
            Directory.CreateDirectory(OutputRoot);
            Data = LoadData<TaskData>("task.dat");
        }

        private T LoadData<T>(string path) where T : class, new()
        {
            var file = PathUtility.Combine(DataRoot, path);
            return File.Exists(file) ? JsonConvert.DeserializeObject<T>(File.ReadAllText(file)) : new T();
        }

        public void Save()
        {
            SaveDate(Data, "task.dat");
        }

        private void SaveDate<T>(T data, string path)
        {
            var file = PathUtility.Combine(DataRoot, path);
            if (file == null) return;
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            if (data == null)
            {
                File.Delete(file);
            }
            else
            {
                File.WriteAllText(file, JsonConvert.SerializeObject(data));
            }
        }

        #endregion
    }
}