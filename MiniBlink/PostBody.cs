using System.Web;

namespace MiniBlinkPinvoke
{
    public class PostBody
    {
        public int size;
        public byte[] data;
        public string filePath;
        public long fileStart;
        public long fileLength;

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return "@file::///" + filePath;
            }
            return "data=" + GetContent();
        }

        public string GetContent() {
            if (data == null) {
                return null;
            }
            return HttpUtility.UrlDecode(System.Text.Encoding.Default.GetString(data));
        }
    }
}
