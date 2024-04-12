using System;
using System.Windows.Forms;
using WindowsForm.Model;

namespace Controller
{
    public class Controller
    {
        private readonly Timer MainTimer;
        private readonly Model Model;
        private readonly Timer BotManagementTimer;
        private readonly Timer BotCreationTimer;
        public Controller(Model model) 
        {
            Model = model;

            BotCreationTimer = new Timer();
            BotCreationTimer.Interval = 8000;
            BotCreationTimer.Tick += (object sender, EventArgs args) => Model.CreateABot();
            BotCreationTimer.Start();

            BotManagementTimer = new Timer();
            BotManagementTimer.Interval = 600;
            BotManagementTimer.Tick += (object sender, EventArgs args) => Model.SetTheBotsInMotion();
            BotManagementTimer.Start();

            MainTimer = new Timer();
            MainTimer.Tick += UpdateTheModel;
            MainTimer.Start();
        }

        public void PutItOnPause(object sender, EventArgs e)
        {
            BotCreationTimer.Enabled = !BotCreationTimer.Enabled;
            BotManagementTimer.Enabled = !BotManagementTimer.Enabled;
            MainTimer.Enabled = !MainTimer.Enabled;
        }

        public void MakeAMove(object sender, KeyEventArgs e)
        {
            if (MainTimer.Enabled == false) return;
            switch (e.KeyCode)
            {
                case Keys.W: Model.Player.GoForwad(); break;
                case Keys.S: Model.Player.GoBack(); break;
                case Keys.D: Model.Player.GoRight(); break;
                case Keys.A: Model.Player.GoLeft(); break;
                case Keys.Enter: PutItOnPause(sender, e); break;
                default: break;
            }
        }

        public void ToShoot(object sender, EventArgs e)
        {
            if (MainTimer.Enabled == false) return;
            Model.Player.Shoot();
        }

        public void RotateThePlayer(object sender, MouseEventArgs e)
        {
            if (MainTimer.Enabled == false) return;
            if (e.Delta >= 120) Model.Player.TurnRight();
            else if (e.Delta <= -120) Model.Player.TurnLeft();
        }

        void UpdateTheModel(object sender, EventArgs args) => Model.ExecuteTheCommandsOfTheHeroes();
    }
}
