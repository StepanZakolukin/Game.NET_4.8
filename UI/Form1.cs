using System.Drawing;
using System.Windows.Forms;
using WindowsForm;

namespace MainWindow
{
    public partial class MyForm : Form
    {
        public static Size FieldDimensions;

        public MyForm(string[][] wallMap)
        {
            Text = "Танки";
            InitializeComponent();
            MaximizeBox = false;
            Size = new Size(1072, 699);
            FieldDimensions = new Size(1060, 655);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            // двойная буфферизация изоображения;
            DoubleBuffered = true;
            KeyDown += new KeyEventHandler(Controller.Player1KeyDown);
            GameModel.Start(this, wallMap);

            var timer = new Timer();

            timer.Tick += (sender, args) =>
            {
                Invalidate();
            };
            timer.Start();
        }
    }
}
