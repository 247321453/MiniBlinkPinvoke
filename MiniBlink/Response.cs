using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MiniBlinkPinvoke
{
    public class Response : IDisposable
    {
        private IntPtr job;
        public Response(IntPtr job)
        {
            this.job = job;
        }

        public void SetMimeType(string MimeType)
        {
            BlinkBrowserPInvoke.wkeNetSetMIMEType(job, Marshal.StringToCoTaskMemAnsi(MimeType));
        }

        public void SetUnicodeData(string data)
        {
            if (data == null)
            {
                data = "";
            }
            BlinkBrowserPInvoke.wkeNetSetData(job, Marshal.StringToCoTaskMemUni(data), Encoding.Unicode.GetBytes(data).Length);
        }

        public void SetAnsiData(string data)
        {
            if (data == null)
            {
                data = "";
            }
            BlinkBrowserPInvoke.wkeNetSetData(job, Marshal.StringToCoTaskMemAnsi(data), Encoding.Default.GetBytes(data).Length);
        }

        public void Close()
        {
            job = IntPtr.Zero;
        }

        public bool IsPostMethod()
        {
            return BlinkBrowserPInvoke.wkeNetGetRequestMethod(job) == wkeRequestType.kWkeRequestTypePost;
        }

        public List<PostBody> GetBodys()
        {
            List<PostBody> posts = null;
            if (IsPostMethod())
            {
                IntPtr elementsPtr = BlinkBrowserPInvoke.wkeNetGetPostBody(job);
                if (elementsPtr != IntPtr.Zero)
                {
                    try
                    {
                        wkePostBodyElements elements = (wkePostBodyElements)Marshal.PtrToStructure(elementsPtr, typeof(wkePostBodyElements));
                        if (elements.element != IntPtr.Zero)
                        {
                            posts = new List<PostBody>();
                            for (int i = 0; i < elements.elementSize; i++)
                            {
                                IntPtr item = Marshal.ReadIntPtr(elements.element, i);
                                wkePostBodyElement e = (wkePostBodyElement)Marshal.PtrToStructure(item, typeof(wkePostBodyElement));
                                try
                                {
                                    PostBody body = new PostBody();
                                    body.fileLength = e.fileLength;
                                    body.fileStart = e.fileStart;
                                    body.fileStart = e.fileStart;
                                    body.filePath = e.filePath == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(BlinkBrowserPInvoke.wkeGetString(e.filePath));
                                    if (e.data != IntPtr.Zero)
                                    {
                                        wkeMemBuf buf = (wkeMemBuf)Marshal.PtrToStructure(e.data, typeof(wkeMemBuf));
                                        if (buf.data != IntPtr.Zero)
                                        {
                                            body.data = BlinkBrowserPInvoke.IntptrToBytes(buf.data, buf.length);
                                        }
                                    }
                                    posts.Add(body);
                                }
                                finally
                                {
                                    // BlinkBrowserPInvoke.wkeNetFreePostBodyElement(item);
                                }
                            }
                        }
                    }
                    catch
                    {
                        // MessageBox.Show("出错：" + e.Message + "\n" + e.StackTrace);
                    }
                    finally
                    {
                        BlinkBrowserPInvoke.wkeNetFreePostBodyElements(elementsPtr);
                    }
                }
            }
            return posts;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
