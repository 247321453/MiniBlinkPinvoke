namespace MiniBlinkPinvoke
{
    using System.Text;
    public class WebResouresResult
    {
        public bool Success;
        public string data;
        public string MimeType;
        public Encoding encoding;

        public WebResouresResult() {
            this.Success = false;
        }

        public WebResouresResult(string data, string MimeType, Encoding encoding) {
            this.Success = true;
            this.data = data;
            this.MimeType = MimeType;
            this.encoding = encoding;
        }
    }
}
