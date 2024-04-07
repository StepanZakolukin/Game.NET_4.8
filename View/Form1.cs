using System.Drawing;
using System.Windows.Forms;
using Model;

namespace MainWindow
{
    public partial class MyForm : Form
    {
        public static Size FieldDimensions;
        public static MyForm Current;
        public MyForm(string[][] wallMap)
        {
            Current = this;
            Text = "Танки";
            InitializeComponent();
            BackColor = Color.Black;
            Size = new Size(1072, 699);
            FieldDimensions = Size;
            // двойная буфферизация изоображения;
            DoubleBuffered = true;
            KeyDown += new KeyEventHandler(Controller.Controller.PlayerKeyDown);
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
