using System.Windows.Forms;
using DigiHome.GuessWho.Library;

namespace ClientWinFormsFramework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var result = AppTypeDetector.Detect();

            label1.Text = $"Detected application type: {result.Display}";

        }
    }
}
