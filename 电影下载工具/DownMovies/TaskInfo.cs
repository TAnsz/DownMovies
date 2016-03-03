using System;
using System.Collections.Generic;

namespace DownMovies
{
    /// <summary>
    /// 数据存储类
    /// </summary>
    public class TaskData
    {
        /// <summary>
        /// 已获取信息的页面列表
        /// </summary>
        public Dictionary<string,PageTask> PageDownloaded { get; set; }

        /// <summary>
        /// 等待获取信息的页面队列任务
        /// </summary>
        public Queue<PageTask> WaitForDownloadPageTasks { get; set; }
        /// <summary>
        /// 电影保存的位置
        /// </summary>
        public Dictionary<string, MoviesDownloadTaskInfo> DownloadMovies { get; set; }

        /// <summary>
        /// 等待下载的电影页面
        /// </summary>
        public Queue<MoviesDownloadTask> MoveDownloadTasks { get; set; }
        /// <summary>
        /// 设置或获取是否完整下载
        /// </summary>
        public bool FullDownloaded { get; set; }

        public TaskData()
        {
            PageDownloaded=new Dictionary<string, PageTask>(StringComparer.OrdinalIgnoreCase);
            DownloadMovies=new Dictionary<string, MoviesDownloadTaskInfo>(StringComparer.OrdinalIgnoreCase);
            WaitForDownloadPageTasks=new Queue<PageTask>();
            MoveDownloadTasks=new Queue<MoviesDownloadTask>();
        }
    }
}