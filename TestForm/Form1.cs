using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility.Core;
using Utility.Core.Streams;

namespace TestForm
{
    public partial class Form1 : Form
    {
        public InvokerService Invoker { get; set; }

        public Form1()
        {
            InitializeComponent();

            Func<string> GetInput = () => InputBox.Text;

            Func<string, string> GetOutput = s =>
            {
                OutputBox.Text += Environment.NewLine;
                OutputBox.Text += s;
                return string.Empty;
            };

            Invoker = new InvokerService("Main", new StreamProvider(GetInput, GetOutput), true);
        }

        private async void InputBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                await Invoker.Run();
            }
        }
    }
}
