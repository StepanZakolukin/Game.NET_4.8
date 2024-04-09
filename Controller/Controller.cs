using System;
using System.Windows.Forms;
using WindowsForm.Model;

namespace Controller
{
    public class Controller
    {
        public readonly Timer Timer;
        private readonly GameModel Model;
        public Controller(GameModel model) 
        {
            Model = model;
            Timer = new Timer();

            Timer.Tick += (object sender, EventArgs args) =>
            {
                Model.BeginAct();
                Model.EndAct();
            };

            Timer.Start();
        }

        public void MakeAMove(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: Model.Player.GoForwad(); break;
                case Keys.S: Model.Player.GoBack(); break;
                case Keys.D: Model.Player.GoRight(); break;
                case Keys.A: Model.Player.GoLeft(); break;
                default: break;
            }
        }

        public void ToShoot(object sender, EventArgs e)
        {
            Model.Player.Shoot();
        }

        public void RotateThePlayer(object sender, MouseEventArgs e)
        {
            if (e.Delta >= 120) Model.Player.TurnRight();
            else if (e.Delta <= -120) Model.Player.TurnLeft();
        }
    }
}
