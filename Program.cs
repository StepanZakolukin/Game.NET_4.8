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
            Application.Run(new MyForm(
                new GameModel(new Playground(0),
                new WindowsForm.View.InfoAboutTheLevel(new string[] { "0", "false", "1", "1","1", "1", "1", "1" }))));
        }
    }
}