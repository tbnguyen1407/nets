#region USING DIRECTIVES

using System.IO;
using System.Text;
using System.Windows.Forms;

#endregion

namespace nets.utility
{
    /// <summary>
    ///   Customized TextWriter Object to write to the RichTextBox instead of the Console Output.
    ///   Author: Tran Binh Nguyen + Vu An Hoa
    /// </summary>
    public class IvleOutput : TextWriter
    {
        #region FIELD DECLARATION

        private readonly RichTextBox rtbx_OutputTextBox;

        #endregion

        #region CONSTRUCTORS

        public IvleOutput(RichTextBox rtbxOutputTextBox)
        {
            rtbx_OutputTextBox = rtbxOutputTextBox;
        }

        #endregion

        #region MAIN METHODS

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }

        public override void Write(char c)
        {
            rtbx_OutputTextBox.Text += c;
        }

        #endregion
    }
}