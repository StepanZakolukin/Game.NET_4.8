using System;
using System.Windows.Forms;
using WindowsForm.Model;

namespace MainWindow
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new MyForm(new Model(new Playground())));
        }
    }
}