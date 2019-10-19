using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Automation;
using TextToSpeechConverter;
using AudioPlayer;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TextSelectionExtractor
{
    class CTextSelectionExtractor
    {
        public string GetText()
        {
            return Clipboard.GetText(TextDataFormat.Text);
        }

    }
}
