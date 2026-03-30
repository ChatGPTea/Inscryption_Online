using System;
using System.Collections;
using System.IO;

namespace QFramework
{
    public abstract class DownLoadItem
    {
        /// <summary>
        ///     当前下载的大小
        /// </summary>
        protected long m_CurLength;

        /// <summary>
        ///     文件后缀
        /// </summary>
        protected string m_FileExt;

        /// <summary>
        ///     原文件大小
        /// </summary>
        protected long m_FileLength;

        /// <summary>
        ///     文件名，包含后缀
        /// </summary>
        protected string m_FileName;

        /// <summary>
        ///     文件名，不包含后缀
        /// </summary>
        protected string m_FileNameWithoutExt;

        /// <summary>
        ///     下载文件全路径，路径+文件名+后缀
        /// </summary>
        protected string m_SaveFilePath;

        /// <summary>
        ///     资源下载存放路径，不包含文件么
        /// </summary>
        protected string m_SavePath;

        /// <summary>
        ///     是否开始下载
        /// </summary>
        protected bool m_StartDownLoad;

        /// <summary>
        ///     网络资源URL路径
        /// </summary>
        protected string m_Url;

        public DownLoadItem(string url, string path)
        {
            m_Url = url;
            m_SavePath = path;
            m_StartDownLoad = false;
            m_FileNameWithoutExt = Path.GetFileNameWithoutExtension(m_Url);
            m_FileExt = Path.GetExtension(m_Url);
            m_FileName = string.Format("{0}{1}", m_FileNameWithoutExt, m_FileExt);
            m_SaveFilePath = string.Format("{0}/{1}{2}", m_SavePath, m_FileNameWithoutExt, m_FileExt);
        }

        public string Url => m_Url;

        public string SavePath => m_SavePath;

        public string FileNameWithoutExt => m_FileNameWithoutExt;

        public string FileExt => m_FileExt;

        public string FileName => m_FileName;

        public string SaveFilePath => m_SaveFilePath;

        public long FileLength => m_FileLength;

        public long CurLength => m_CurLength;

        public bool StartDownLoad => m_StartDownLoad;

        public virtual IEnumerator Download(Action callback = null)
        {
            yield return null;
        }

        /// <summary>
        ///     获取下载进度
        /// </summary>
        /// <returns></returns>
        public abstract float GetProcess();

        /// <summary>
        ///     获取当前下载的文件大小
        /// </summary>
        /// <returns></returns>
        public abstract long GetCurLength();

        /// <summary>
        ///     获取下载的文件大小
        /// </summary>
        /// <returns></returns>
        public abstract long GetLength();

        public abstract void Destory();
    }
}