﻿using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsForm.Model;
using WindowsForm.Controller;

namespace MainWindow
{
    public partial class MyForm : Form
    {
        private GameModel Model { get; set; }
        private Controller Controller;
        private float ImageSize { get; set; }
        private PointF InitialCoordinateOfTheMap { get; set; }
        private RectangleF CoordinatesOfTheInscription;
        public static Button PauseButton { get; private set; }
        private static Image[] PauseImages { get; set; }
        private Button StartButton { get; set; }
        public MyForm(GameModel model)
        {
            Model = model;
            KeyPreview = true;
            DoubleBuffered = true;

            BackColor = Color.Black;
            Size = new Size() { Height = 450, Width = 800 };
            WindowState = FormWindowState.Maximized;
            Text = "Защитник Брестской крепости";

            PauseImages = new[] { Image.FromFile(@"..\..\Images\PauseTurnOn.png"), Image.FromFile(@"..\..\Images\PauseTurnOff.png") };
            SizeChanged += UpdateFieldValues;

            OpenTheMainMenu();

            Load += (sender, args) => OnSizeChanged(EventArgs.Empty);
            InitializeComponent();
        }

        public void StartTheGame(object sender, EventArgs e)
        {
            CloseTheMainMenu();
            OpenTheGame();
            Controller.ActivateTimers();
        }

        void OpenTheMainMenu()
        {
            BackgroundImage = Image.FromFile(@"..\..\Images\MainMenu.jpg");
            BackgroundImageLayout = ImageLayout.Zoom;

            StartButton = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\StartTheGame.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
            };
            Controls.Add(StartButton);

            UpdateTheFieldsForTheMenu("", new EventArgs());

            StartButton.Click += StartTheGame;
            SizeChanged += UpdateTheFieldsForTheMenu;
        }

        public static void ChangeThePausePicture() => 
            PauseButton.BackgroundImage = PauseButton.BackgroundImage == PauseImages[(int)Pause.TurnOn] ? PauseImages[(int)Pause.TurnOff] : PauseImages[(int)Pause.TurnOn];

        public void CloseTheMainMenu()
        {
            Controls.Clear();;
            SizeChanged -= UpdateTheFieldsForTheMenu;
            StartButton.Click -= StartTheGame;
            BackgroundImage = null;
        }

        void UpdateTheFieldsForTheMenu(object sender, EventArgs e)
        {
            StartButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * 16.3), (int)(InitialCoordinateOfTheMap.Y + ImageSize * 11));
            StartButton.Size = new Size() { Width = (int)(6 * ImageSize), Height = (int)(2 * ImageSize) };
        }

        void DisableGameManagementAndRendering()
        {
            Controls.Clear();

            PauseButton.Click -= Controller.PutItOnPause;
            Click -= Controller.ToShoot;
            KeyDown -= Controller.MakeAMove;
            MouseWheel -= Controller.RotateThePlayer;
            Paint -= DrawingTheModel;
            Paint -= WithdrawTheGameScore;
            SizeChanged -= RecalculateTheValuesOfTheGameButtons;
            Model.StateChanged -= Invalidate;
            Model.TheGameIsOver -= DisableGameManagementAndRendering;
            Model.TheGameIsOver -= OpenTheMainMenu;

            Controller.StopTimers();
        }

        public void OpenTheGame()
        {
            Model = new GameModel(new Playground());
            Controller = new Controller(Model);
            Model.StateChanged += Invalidate;
            Model.TheGameIsOver += DisableGameManagementAndRendering;
            Model.TheGameIsOver += OpenTheMainMenu;

            PauseButton = new Button()
            {
                BackColor = Color.FromArgb(0, 0, 0, 0),
                BackgroundImage = PauseImages[(int)Pause.TurnOn],
                BackgroundImageLayout = ImageLayout.Zoom
            };

            Controls.Add(PauseButton);

            Paint += DrawingTheModel;
            Paint += WithdrawTheGameScore;
            
            RecalculateTheValuesOfTheGameButtons("", new EventArgs());
            SizeChanged += RecalculateTheValuesOfTheGameButtons;

            PauseButton.Click += Controller.PutItOnPause;
            Click += Controller.ToShoot;
            KeyDown += Controller.MakeAMove;
            MouseWheel += Controller.RotateThePlayer;
        }

        void WithdrawTheGameScore(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.DrawString($"Счёт: {Model.NumberOfActiveBots}", new Font("Courier New", Math.Max(ImageSize / 2, 1), FontStyle.Bold), Brushes.White, CoordinatesOfTheInscription);
        }

        void DrawingTheModel(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (var x = 0; x < Model.Map.Width; x++)
                for (var y = 0; y < Model.Map.Height; y++)
                    foreach (var creture in Model.Map[x, y])
                    {
                        var image = creture.Picture;
                        var coordinatesOnTheForm = RecalculateTheCoordinatesOnTheForm(new System.Drawing.Point(x, y));
                        e.Graphics.DrawImage(image, RotateAnArrayOfPoints(coordinatesOnTheForm, creture.AngleInDegrees * Math.PI / 180));
                    }
        }

        void RecalculateTheValuesOfTheGameButtons(object sender, EventArgs e)
        {
            PauseButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * (Model.Map.Width - 1)),
                (int)(InitialCoordinateOfTheMap.Y - ImageSize));
            PauseButton.Size = new Size((int)ImageSize, (int)ImageSize);
        }

        void UpdateFieldValues(object sender, EventArgs e)
        {
            ImageSize = Math.Min(ClientSize.Height / (Model.Map.Height + 1), ClientSize.Width / Model.Map.Width);

            InitialCoordinateOfTheMap = new PointF((ClientSize.Width - ImageSize * Model.Map.Width) / 2,
                (ClientSize.Height - ImageSize * (Model.Map.Height + 1)) / 2 + ImageSize);

            CoordinatesOfTheInscription = new RectangleF(
                new PointF(InitialCoordinateOfTheMap.X, InitialCoordinateOfTheMap.Y - ImageSize / 2 * 1.34f),
                new SizeF(InitialCoordinateOfTheMap.X + ImageSize * (Model.Map.Width - 1), ImageSize));
        }

        PointF[] RecalculateTheCoordinatesOnTheForm(System.Drawing.Point positionOnTheMap)
        { 
            return new PointF[] {
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y),
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X + ImageSize, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y),
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y + ImageSize) };
        }

        PointF[] RotateAnArrayOfPoints(PointF[] points, double turn)
        {
            var centre = new PointF(points[0].X + (points[1].X - points[0].X) / 2, points[0].Y + (points[2].Y - points[0].Y) / 2);

            var point1 = RotateAPoint(new PointF(points[0].X - centre.X, points[0].Y - centre.Y), turn);
            var point2 = RotateAPoint(new PointF(points[1].X - centre.X, points[1].Y - centre.Y), turn);
            var point3 = RotateAPoint(new PointF(points[2].X - centre.X, points[2].Y - centre.Y), turn);

            return new PointF[]
            {
                new PointF(centre.X + point1.X,centre.Y + point1.Y),
                new PointF(centre.X + point2.X, centre.Y + point2.Y),
                new PointF(centre.X + point3.X, centre.Y + point3.Y)
            };
        }

        PointF RotateAPoint(PointF point, double angleInRadians)
        {
            var d = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            var angle = Math.Atan2(point.Y, point.X) + angleInRadians;

            return new PointF((float)(Math.Cos(angle) * d), (float)(Math.Sin(angle) * d));
        }
    }
}