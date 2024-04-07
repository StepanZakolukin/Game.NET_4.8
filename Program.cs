using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MainWindow
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var wallMap = File.ReadAllText(@"..\..\Model\WallMap.txt").Split('\n')
                .Select(st => st.Split('\t'))
                .ToArray();

            Application.Run(new MyForm(wallMap));
        }
    }
}