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
        public AssetsManger()
        {
        }

        public bool OnUrlLoad(string url, Response response)
        {
            if (url.StartsWith("mb://"))
            {
                Regex regex = new Regex(@"mb://", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
                string str = regex.Replace(url, "");
                LoadResource(url, str, response);
                return true;
            }
            else if (url.StartsWith("assets://"))
            {
                Regex regex = new Regex(@"assets://", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
                string str = regex.Replace(url, "");
                LoadResource(url, str, response);
                return true;
            }
            return false;
        }

        protected virtual string PathToResName(string path)
        {
            return path.Replace('/', '.');
        }

        protected virtual Stream Open(string path)
        {
            string rsName = PathToResName(path);
            Assembly Assemblys = Assembly.GetExecutingAssembly();
            return Assemblys.GetManifestResourceStream("MiniBlinkPinvoke." + rsName);
        }

        protected virtual void LoadResource(string url, string path, Response response)
        {
            Stream sm = Open(path);
            if (sm == null)
            {
                ResNotFond(url, response);
                return;
            }
            string data = ReadData(sm);
            response.SetMimeType(GetMimeType(url));
            response.SetAnsiData(data);
        }

        protected virtual string ReadData(Stream sm)
        {
            try
            {
                string strLine;
                using (StreamReader m_stream = new StreamReader(sm, Encoding.Default))
                {
                    try
                    {
                        m_stream.BaseStream.Seek(0, SeekOrigin.Begin);
                    }
                    catch {
                    }
                    strLine = m_stream.ReadToEnd();
                }
                string data = strLine;
                return strLine;
            }
            finally
            {
                sm.Close();
            }
        }

        protected virtual void ResNotFond(string url, Response response)
        {
            response.SetMimeType("text/html");
            response.SetAnsiData("<html><head><title>404没有找到资源</title></head><body>404没有找到资源:<br/>" + url + "</body></html>");
        }

        protected virtual string GetMimeType(string url)
        {
            string path = url.Split('?')[0];
            if (path.EndsWith(".css"))
            {
                return "text/css";
            }
            else if (path.EndsWith(".png"))
            {
                return "image/png";
            }
            else if (path.EndsWith(".webp"))
            {
                return "image/webp";
            }
            else if (path.EndsWith(".gif"))
            {
                return "image/gif";
            }
            else if (path.EndsWith(".jpg") || path.EndsWith(".jpeg"))
            {
                return "image/jpg";
            }
            else if (path.EndsWith(".js"))
            {
                return "application/javascript";
            }
            else
            {
                return "text/html";
            }
        }

    }
}
