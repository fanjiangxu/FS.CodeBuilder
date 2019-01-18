using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FS.CodeBuilder
{
    /// <summary>
    /// 操作文件生成
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 写内容到指定文件（不存在就创建）
        /// </summary>
        /// <param name="InfoStr">内容</param>
        /// <param name="FilePath">文件地址</param>
        /// <param name="encoding">编码</param>
        /// <param name="append">是否追加</param>
        public static void WriteInfoToFile(string infoStr, string filePath, Encoding encoding, bool append=false)
        {
            FileStream stream = null;
            System.IO.StreamWriter writer = null;
            try
            {
                //string logPath =FilePath ;
                if (File.Exists(filePath) == false)
                {
                    stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                    writer = new System.IO.StreamWriter(stream, encoding);
                }
                else
                {
                    //存在就覆盖
                    writer = new System.IO.StreamWriter(filePath, append, encoding);
                }
                writer.Write(infoStr);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer.Dispose();
                }
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }
            }
        }
    }
}
