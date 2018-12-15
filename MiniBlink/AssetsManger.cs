using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniBlinkPinvoke
{
    public class AssetsManger
    {
        private Assembly Assemblys;
        private string mResNameSpace;
        public AssetsManger() {
            this.Assemblys = Assembly.GetExecutingAssembly();
            this.mResNameSpace = "MiniBlinkPinvoke";
        }

        public AssetsManger(Assembly Assemblys, string _namespace)
        {
            this.Assemblys = Assemblys;
            this.mResNameSpace = _namespace;
        }

        public bool OnUrlLoad(string url, IntPtr job) {
            if (url.StartsWith("mb://"))
            {
                Regex regex = new Regex(@"mb://", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
                string str = regex.Replace(url, "");
                LoadResource(url, str, job);
                return true;
            }
            else if (url.StartsWith("assets://"))
            {
                Regex regex = new Regex(@"assets://", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
                string str = regex.Replace(url, "");
                LoadResource(url, PathToResName(str), job);
                return true;
            }
            return false;
        }
        
        protected virtual string PathToResName(string path) {
            return path.Replace('/', '.');
        }

        private void LoadResource(string url, string path, IntPtr job)
        {
            string rsName = PathToResName(path);
            if (Assemblys != null)
            {
                using (Stream sm = Assemblys.GetManifestResourceStream(mResNameSpace + "." + rsName))
                {
                    if (sm != null)
                    {
                        StreamReader m_stream = new StreamReader(sm, Encoding.Default);
                        m_stream.BaseStream.Seek(0, SeekOrigin.Begin);
                        string strLine = m_stream.ReadToEnd();
                        m_stream.Close();
                        string data = strLine;
                        if (url.EndsWith(".css"))
                        {
                            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("text/css"));
                        }
                        else if (url.EndsWith(".png"))
                        {
                            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/png"));
                        }
                        else if (url.EndsWith(".gif"))
                        {
                            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/gif"));
                        }
                        else if (url.EndsWith(".jpg"))
                        {
                            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("image/jpg"));
                        }
                        else if (url.EndsWith(".js"))
                        {
                            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("application/javascript"));
                        }
                        else
                        {
                            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("text/html"));
                        }
                        //wkeNetSetURL(job, url);
                        BlinkBrowserPInvoke.wkeNetSetData(job, Marshal.StringToCoTaskMemAnsi(data), Encoding.Default.GetBytes(data).Length);
                    }
                    else
                    {
                        ResNotFond(url, job);
                    }
                }
            }
            else
            {
                ResNotFond(url, job);
            }
        }

        protected virtual string Make404(string url) {
            return "<html><head><title>404没有找到资源</title></head><body>404没有找到资源"+ url + "</body></html>";
        }

        private void ResNotFond(string url, IntPtr job)
        {
            string data = Make404(url);
            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi("text/html"));
            //wkeNetSetURL(job, url);
            BlinkBrowserPInvoke.wkeNetSetData(job, Marshal.StringToCoTaskMemAnsi(data), Encoding.Default.GetBytes(data).Length);
        }
    }
}
