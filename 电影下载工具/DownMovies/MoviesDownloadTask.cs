namespace DownMovies
{
    /// <summary>
    /// 电影页面任务类
    /// </summary>
    public class MoviesDownloadTask
    {
        /// <summary>
        /// 说明
        /// </summary>
        public string Notice { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Img { get; set; }
        /// <summary>
        /// 电影下载地址
        /// </summary>
         public string Url { get; set; }
        /// <summary>
        /// 下载目标
        /// </summary>
        public string DownloadRoot { get; set; }

        public MoviesDownloadTask(string url, string root)
        {
            Url = url;
            DownloadRoot = root;
        }

        public MoviesDownloadTask()
        {
            
        }
    }
}