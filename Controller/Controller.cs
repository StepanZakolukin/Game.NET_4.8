using System.Windows.Forms;
using Model;

namespace Controller
{
    public static class Controller
    {
        public static void PlayerKeyDown(object sender, KeyEventArgs e)
        {
            var control = sender as Control;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    GameModel.Player.GoForward(control);
                    break;
                case Keys.Down:
                    GameModel.Player.GoBack(control);
                    break;
                case Keys.Right:
                    GameModel.Player.TurnRight(control);
                    break;
                case Keys.Left:
                    GameModel.Player.TurnLeft(control);
                    break;
                default: break;
            }
        }
    }
}
