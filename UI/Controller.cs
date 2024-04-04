using System.Windows.Forms;
using WindowsForm;

namespace MainWindow
{
    public static class Controller
    {
        public static void Player1KeyDown(object sender, KeyEventArgs e)
        {
            var control = sender as Control;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    GameModel.Player1.GoForward(control);
                    break;
                case Keys.Down:
                    GameModel.Player1.GoBack(control);
                    break;
                case Keys.Right:
                    GameModel.Player1.TurnRight(control);
                    break;
                case Keys.Left:
                    GameModel.Player1.TurnLeft(control);
                    break;
                default: break;
            }
        }
    }
}
