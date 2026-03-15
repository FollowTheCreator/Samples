using System.IO;
using System.Text;

namespace IRT.Modules.DataTransfer.Generic.Helpers.Infrastucture
{
    public class EncodingStringWriter : StringWriter
    {
        private readonly Encoding encoding;

        public EncodingStringWriter(StringBuilder builder, Encoding encoding)
            : base(builder)
        {
            this.encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return encoding; }
        }
    }
}
