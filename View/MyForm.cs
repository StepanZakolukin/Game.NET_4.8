using System;
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
        public static Button PauseButton { get; private set; }
        private static Image[] PauseImages { get; set; }
        private Button StartButton { get; set; }
        private Button ButtonToGoToTheMenu { get; set; }
        private Button RestartGameButton { get; set; }
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
                FlatStyle = FlatStyle.Flat
            };
            StartButton.FlatAppearance.BorderColor = Color.DarkRed;
            StartButton.FlatAppearance.BorderSize = 4;
            StartButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            StartButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 0, 0, 0);

            Controls.Add(StartButton);

            UpdateTheFieldsForTheMenu("", new EventArgs());

            StartButton.Click += StartTheGame;
            SizeChanged += UpdateTheFieldsForTheMenu;
            Paint += DisplayARecord; 
        }

        void EraseThePlayingField()
        {
            Paint -= DrawingTheModel;
            Paint -= DrawAGamePanel;

            Controls.Clear();
            SizeChanged -= RecalculateTheValuesOfTheGameButtons;
            Model.StateChanged -= Invalidate;
            Model.TheGameIsOver -= OpenTheResultsWindow;

            Controller.StopTimers();
        }

        public void OpenTheGame()
        {
            Model = new GameModel(new Playground());
            Controller = new Controller(Model);
            Model.StateChanged += Invalidate;
            Model.TheGameIsOver += OpenTheResultsWindow;

            CreateGamePanelButtons();

            Paint += DrawingTheModel;
            Paint += DrawAGamePanel;

            RecalculateTheValuesOfTheGameButtons("", new EventArgs());
            SizeChanged += RecalculateTheValuesOfTheGameButtons;

            ActivateTheGameControls();
        }

        void RestartTheGame(object sender, EventArgs e)
        {
            EraseThePlayingField();
            DeactivateGameControls();
            OpenTheGame();
            Controller.ActivateTimers();
        }

        void ActivateTheGameControls()
        {
            PauseButton.Click += Controller.PutItOnPause;
            Click += Controller.ToShoot;
            KeyDown += Controller.MakeAMove;
            MouseWheel += Controller.RotateThePlayer;
            RestartGameButton.Click += RestartTheGame;
        }

        void CreateGamePanelButtons()
        {
            PauseButton = new Button()
            {
                BackColor = Color.FromArgb(0, 0, 0, 0),
                BackgroundImage = PauseImages[(int)Pause.TurnOn],
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat
            };
            PauseButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            PauseButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            Controls.Add(PauseButton);

            RestartGameButton = new Button()
            {
                BackColor = Color.FromArgb(0, 0, 0, 0),
                BackgroundImage = Image.FromFile(@"..\..\Images\RestartGameButton.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat
            };
            RestartGameButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            RestartGameButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            Controls.Add(RestartGameButton);
        }

        public static void ChangeThePausePicture() => 
            PauseButton.BackgroundImage = PauseButton.BackgroundImage == PauseImages[(int)Pause.TurnOn] ? 
            PauseImages[(int)Pause.TurnOff] : PauseImages[(int)Pause.TurnOn];

        public void CloseTheMainMenu()
        {
            Controls.Clear();
            SizeChanged -= UpdateTheFieldsForTheMenu;
            StartButton.Click -= StartTheGame;
            BackgroundImage = null;
            Paint -= DisplayARecord;
        }

        void UpdateTheFieldsForTheMenu(object sender, EventArgs e)
        {
            StartButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * 16.3),
                (int)(InitialCoordinateOfTheMap.Y + ImageSize * 11));
            StartButton.Size = new Size() { Width = (int)(6 * ImageSize), Height = (int)(2 * ImageSize) };
        }

        void OpenTheResultsWindow()
        {
            Controller.StopTimers();
            PauseButton.Enabled = false;
            RestartGameButton.Enabled = false;
            DeactivateGameControls();

            Paint += DrawTheResultsWindow;

            ButtonToGoToTheMenu = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\ButtonToGoToTheMenu.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            ButtonToGoToTheMenu.FlatAppearance.BorderColor = Color.DarkRed;
            ButtonToGoToTheMenu.FlatAppearance.BorderSize = 4;
            ButtonToGoToTheMenu.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            Controls.Add(ButtonToGoToTheMenu);

            UpdateTheCoordinatesOfTheMenuTransitionButton("", new EventArgs());

            SizeChanged += UpdateTheCoordinatesOfTheMenuTransitionButton;
            ButtonToGoToTheMenu.Click += ReturnToTheMenu;
        }

        void DeactivateGameControls()
        {
            PauseButton.Click -= Controller.PutItOnPause;
            RestartGameButton.Click -= RestartTheGame;
            Click -= Controller.ToShoot;
            KeyDown -= Controller.MakeAMove;
            MouseWheel -= Controller.RotateThePlayer;
        }

        void CloseTheResultsWindow()
        {
            Paint -= DrawTheResultsWindow;
            Controls.Clear();
            SizeChanged -= UpdateTheCoordinatesOfTheMenuTransitionButton;
            ButtonToGoToTheMenu.Click -= ReturnToTheMenu;
        }

        void ReturnToTheMenu(object sender, EventArgs e)
        {
            EraseThePlayingField();
            CloseTheResultsWindow();
            OpenTheMainMenu();
        }

        void DrawTheResultsWindow(object sender, PaintEventArgs e)
        {
            DrawTheBackgroundOfTheResultsWindow(e.Graphics);

            DrawAnAsterisk(new PointF(InitialCoordinateOfTheMap.X + 20 * ImageSize, InitialCoordinateOfTheMap.Y + ImageSize * 8.65f),
                new SizeF(ImageSize * 0.7f, ImageSize * 0.7f), e.Graphics);

            DrawTheText(e, "Игра завершена.",
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + 8 * ImageSize, InitialCoordinateOfTheMap.Y + ImageSize * 4.5f),
                new SizeF(16 * ImageSize, ImageSize * 1.5f)), Brushes.White, new StringFormat() { Alignment = StringAlignment.Center }, ImageSize / 1.2f);

            if (Model.RecordHasBeenUpdated)
                DrawTheText(e, "Вы обновили рекорд!",
                    new RectangleF(new PointF(InitialCoordinateOfTheMap.X + 8 * ImageSize, InitialCoordinateOfTheMap.Y + ImageSize * 6f),
                    new SizeF(16 * ImageSize, ImageSize * 1.5f)), Brushes.Red, new StringFormat() { Alignment = StringAlignment.Center }, ImageSize / 1.2f);

            DrawTheText(e, String.Format("{0, -7} {1, 6}", "Счёт:",  Model.NumberOfPoints),
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + ImageSize * 10.7f, InitialCoordinateOfTheMap.Y + 7.5f * ImageSize),
                new SizeF(12 * ImageSize, ImageSize)), Brushes.White, new StringFormat() { Alignment = StringAlignment.Near }, ImageSize / 1.34f);

            DrawTheText(e, String.Format("{0, -7} {1, 6}", "Рекорд:", Model.Record),
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + ImageSize * 10.7f, InitialCoordinateOfTheMap.Y + 8.5f * ImageSize),
                new SizeF(12 * ImageSize, ImageSize)), Brushes.White, new StringFormat() { Alignment = StringAlignment.Near }, ImageSize / 1.34f);

            DrawAnAsterisk(new PointF(InitialCoordinateOfTheMap.X + ImageSize * 20f, InitialCoordinateOfTheMap.Y + 7.65f * ImageSize),
                new SizeF(ImageSize * 0.7f, ImageSize * 0.7f), e.Graphics);
        }

        void DrawTheBackgroundOfTheResultsWindow(Graphics graphics)
        {
            graphics.DrawImage(Image.FromFile(@"..\..\Images\haze.png"), new PointF[]
            {
                new PointF(0, 0),
                new PointF(ClientSize.Width, 0),
                new PointF(0, ClientSize.Height)
            });

            graphics.DrawImage(Image.FromFile(@"..\..\Images\ResultsWindow.png"), new PointF[]
            {
                new PointF(InitialCoordinateOfTheMap.X + 9 * ImageSize, InitialCoordinateOfTheMap.X + 3 * ImageSize),
                new PointF(InitialCoordinateOfTheMap.X + 23 * ImageSize, InitialCoordinateOfTheMap.X + 3 * ImageSize),
                new PointF(InitialCoordinateOfTheMap.X + 9 * ImageSize, InitialCoordinateOfTheMap.X + 13 * ImageSize),
            });
        }

        void DrawAGamePanel(object sender, PaintEventArgs e)
        {
            DrawTheText(e, $"Рекорд: {Model.Record}",
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + 22f * ImageSize, InitialCoordinateOfTheMap.Y - ImageSize / 2 * 1.34f),
                new SizeF(6 * ImageSize, ImageSize)), Brushes.White, new StringFormat() { Alignment = StringAlignment.Far}, ImageSize / 2);

            DrawAnAsterisk(new PointF(InitialCoordinateOfTheMap.X + 28.2f * ImageSize, InitialCoordinateOfTheMap.Y - ImageSize * 0.7f),
                new SizeF(ImageSize * 0.7f, ImageSize * 0.7f), e.Graphics);

            DrawTheText(e, $"Счёт: {Model.NumberOfPoints}",
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + ImageSize * 10f, InitialCoordinateOfTheMap.Y - ImageSize / 2 * 1.34f),
                new SizeF(6 * ImageSize, ImageSize)), Brushes.White, new StringFormat()
                { Alignment = StringAlignment.Far}, ImageSize / 2);

            DrawAnAsterisk(new PointF(InitialCoordinateOfTheMap.X + 16.2f * ImageSize, InitialCoordinateOfTheMap.Y - ImageSize * 0.7f),
                new SizeF(0.7f * ImageSize, 0.7f * ImageSize), e.Graphics);

            for (var i = 0; i < Model.Player.Health; i++)
            {
                e.Graphics.DrawImage(Image.FromFile(@"..\..\Images\heart.png"), new PointF[]
                {
                    new PointF(InitialCoordinateOfTheMap.X + i * ImageSize, InitialCoordinateOfTheMap.Y - ImageSize * 0.7f),
                    new PointF(InitialCoordinateOfTheMap.X + i * ImageSize + 0.7f * ImageSize, InitialCoordinateOfTheMap.Y - ImageSize * 0.7f),
                    new PointF(InitialCoordinateOfTheMap.X + i * ImageSize, InitialCoordinateOfTheMap.Y)
                });
            }
        }

        void DrawTheText(PaintEventArgs e, string text, RectangleF location, Brush brushes, StringFormat format, float fontSize)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.DrawString(text, new Font("Courier New", Math.Max(fontSize, 1), FontStyle.Bold), brushes, location, format);
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

        void DisplayARecord(object sender, PaintEventArgs e)
        {
            DrawTheText(e, $"Рекорд: {Model.Record}", new RectangleF(
                new PointF(InitialCoordinateOfTheMap.X + ImageSize * 10f, InitialCoordinateOfTheMap.Y + 3.5f * ImageSize),
                new SizeF(10 * ImageSize, ImageSize * 1.34f)), Brushes.DarkRed, new StringFormat()
                { Alignment = StringAlignment.Far }, ImageSize / 1.34f);

            DrawAnAsterisk(new PointF(InitialCoordinateOfTheMap.X + 20f * ImageSize, InitialCoordinateOfTheMap.Y + 3.4f * ImageSize), new SizeF(ImageSize, ImageSize), e.Graphics);
        }

        void DrawAnAsterisk(PointF location, SizeF size, Graphics graphics)
        {
            graphics.DrawImage(Image.FromFile(@"..\..\Images\star.png"), new PointF[]
            {
                location,
                new PointF(location.X + size.Width, location.Y),
                new PointF(location.X, location.Y + size.Height)
            });
        }

        void RecalculateTheValuesOfTheGameButtons(object sender, EventArgs e)
        {
            PauseButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * (Model.Map.Width - 1)),
                (int)(InitialCoordinateOfTheMap.Y - ImageSize));
            PauseButton.Size = new Size((int)ImageSize, (int)ImageSize);

            RestartGameButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * (Model.Map.Width - 2)),
                (int)(InitialCoordinateOfTheMap.Y - ImageSize));
            RestartGameButton.Size = PauseButton.Size;
        }

        void UpdateTheCoordinatesOfTheMenuTransitionButton(object sender, EventArgs e)
        {
            ButtonToGoToTheMenu.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * 13.5f),
                (int)(InitialCoordinateOfTheMap.Y + ImageSize * 10.3));
            ButtonToGoToTheMenu.Size = new Size() { Width = (int)(5 * ImageSize), Height = (int)(2 * ImageSize) };
        }

        void UpdateFieldValues(object sender, EventArgs e)
        {
            ImageSize = Math.Min(ClientSize.Height / (Model.Map.Height + 1), ClientSize.Width / Model.Map.Width);

            InitialCoordinateOfTheMap = new PointF((ClientSize.Width - ImageSize * Model.Map.Width) / 2,
                (ClientSize.Height - ImageSize * (Model.Map.Height + 1)) / 2 + ImageSize);
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