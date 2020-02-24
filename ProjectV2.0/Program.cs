using System;
using System.Windows.Forms;

namespace BuildTreeOfImage
{
	static class Program
	{
        [STAThread]
        static void Main()
        {
            Form form = new Form1();
            try
            {
                Application.Run(form);
            }
            catch { }
        }
	}
}
