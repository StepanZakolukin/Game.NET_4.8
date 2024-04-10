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

            Timer.Tick += UpdateTheModel;

            Timer.Start();
        }

        public void MakeAMove(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W: GameModel.Player.GoForwad(); break;
                case Keys.S: GameModel.Player.GoBack(); break;
                case Keys.D: GameModel.Player.GoRight(); break;
                case Keys.A: GameModel.Player.GoLeft(); break;
                default: break;
            }
        }

        public void ToShoot(object sender, EventArgs e)
        {
            GameModel.Player.Shoot();
        }

        public void RotateThePlayer(object sender, MouseEventArgs e)
        {
            if (e.Delta >= 120) GameModel.Player.TurnRight();
            else if (e.Delta <= -120) GameModel.Player.TurnLeft();
        }

        private void UpdateTheModel(object sender, EventArgs args)
        {
            Model.BeginAct();
            Model.EndAct();
            GameModel.Bot.MakeAMove();
        }
    }
}
