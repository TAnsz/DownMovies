using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownMovies
{
    public class MoviesDownloadTaskInfo
    {
        /// <summary>
        /// 电影简介
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 实际的保存位置
        /// </summary>
        public string Location { get; set; }
    }
}
