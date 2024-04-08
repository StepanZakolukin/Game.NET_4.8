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
                case Keys.W:
                    GameModel.Player.GoForward(control);
                    break;
                case Keys.S:
                    GameModel.Player.GoBack(control);
                    break;
                case Keys.D:
                    GameModel.Player.TurnRight(control);
                    break;
                case Keys.A:
                    GameModel.Player.TurnLeft(control);
                    break;
                //case Keys.L: 
                //    GameModel.Player.Shoot(control);
                //    break;
                default: break;
            }
        }
    }
}
