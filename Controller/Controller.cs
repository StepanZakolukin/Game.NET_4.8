﻿using System;
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
            BotCreationTimer.Interval = 10000;
            BotCreationTimer.Tick += (object sender, EventArgs args) => Model.CreateABot();
            BotCreationTimer.Start();

            BotManagementTimer = new Timer();
            BotManagementTimer.Interval = 500;
            BotManagementTimer.Tick += (object sender, EventArgs args) => Model.SetTheBotsInMotion();
            BotManagementTimer.Start();

            MainTimer = new Timer();
            MainTimer.Tick += UpdateTheModel;
            MainTimer.Start();
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

        public void ToShoot(object sender, EventArgs e) => Model.Player.Shoot();

        public void RotateThePlayer(object sender, MouseEventArgs e)
        {
            if (e.Delta >= 120) Model.Player.TurnRight();
            else if (e.Delta <= -120) Model.Player.TurnLeft();
        }

        private void UpdateTheModel(object sender, EventArgs args) => Model.ExecuteTheCommandsOfTheHeroes();
    }
}
