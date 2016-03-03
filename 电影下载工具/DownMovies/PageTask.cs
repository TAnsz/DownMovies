using System;
using System.Collections.Generic;

namespace DownMovies
{
    /// <summary>
    /// 电影页面类
    /// </summary>
    public class PageTask
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 电影名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public  string Url { get; set; }
        /// <summary>
        /// 下载目标
        /// </summary>
        public string Root { get; set; }
        public string DownloadUrl { get; set; }
        

        public PageTask()
        {

        }

        public PageTask(int id, string name, string url) : this()
        {
            Id = id;
            Name = name;
            Url = url;
        }

    }
}