using System.IO;
using System.Text;
using System.Windows.Forms;

namespace nets_wpf.Utility
{
    /// <summary>
    ///   Customized TextWriter Object to write to the RichTextBox
    ///   instead of the Console Output.
    /// </summary>
    public class IvleOutput : TextWriter
    {
        private readonly RichTextBox rtbx_OutputTextBox;

        public IvleOutput(RichTextBox rtbxOutputTextBox)
        {
            rtbx_OutputTextBox = rtbxOutputTextBox;
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(char c)
        {
            rtbx_OutputTextBox.Text += c;
        }
    }
}