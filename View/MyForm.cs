using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsForm.Model;
using WindowsForm.Controller;
using System.IO;
using System.Linq;

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
        private Button StartOverButton { get; set; }
        private readonly Button[,] LevelButtons;
        private readonly EventHandler[,] LevelTriggerFunctions;
        private int MaximumAvailableLevel;
        private int Level;

        public MyForm(GameModel model)
        {
            Model = model;
            KeyPreview = true;
            DoubleBuffered = true;

            LevelButtons = new Button[3, 6];
            LevelTriggerFunctions = new EventHandler[LevelButtons.GetLength(0), LevelButtons.GetLength(1)];
            FillInTheMatrixWithFunctions();

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

        //управление отображением
        #region
        void OpenTheMainMenu()
        {
            BackgroundImage = Image.FromFile(@"..\..\Images\MainMenu.jpg");
            BackgroundImageLayout = ImageLayout.Zoom;

            StartButton = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\Play.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            StartButton.FlatAppearance.BorderColor = Color.DarkRed;
            StartButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            StartButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 0, 0, 0);

            Controls.Add(StartButton);

            UpdateTheFieldsForTheMenu("", new EventArgs());

            StartButton.Click += OpenTheLevelSelectionWindow;
            SizeChanged += UpdateTheFieldsForTheMenu;
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
            CreateGamePanelButtons();
            RecalculateTheValuesOfTheGameButtons("", new EventArgs());

            Model = new GameModel(new Playground(), Level);

            Controller = new Controller(Model);
            Controller.ActivateTimers();

            Model.StateChanged += Invalidate;
            Model.TheGameIsOver += StartTheNextRound;

            Paint += DrawingTheModel;
            Paint += DrawAGamePanel;

            Controller.StopTimers();

            SizeChanged += RecalculateTheValuesOfTheGameButtons;

            Paint += DrawTheStartOfTheRound;
            Invalidate();
            Click += StartGame;
        }

        void StartGame(object sender, EventArgs e)
        {
            Click -= StartGame;
            Controller.ActivateTimers();
            ActivateTheGameControls();
            Paint -= DrawTheStartOfTheRound;
        }

        public void StartTheNextRound()
        {
            if (!Model.Map[Model.Player.Location].Contains(Model.Player) || Model.Round == 50)
                OpenTheResultsWindow();
            else
            {
                EraseThePlayingField();
                DeactivateGameControls();
                Level++;
                OpenTheGame();
            }
        }

        void RestartTheGame(object sender, EventArgs e)
        {
            EraseThePlayingField();
            DeactivateGameControls();
            OpenTheGame();
        }

        void StartOver(object sender, EventArgs e)
        {
            EraseThePlayingField();
            CloseTheResultsWindow();
            OpenTheGame();
        }

        void ActivateTheGameControls()
        {
            PauseButton.Click += Controller.PutItOnPause;
            Click += Controller.ToShoot;
            KeyDown += Controller.MakeAMove;
            MouseWheel += Controller.RotateThePlayer;
            Level = Model.Round;
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

        void CloseTheMainMenu()
        {
            Controls.Clear();
            SizeChanged -= UpdateTheFieldsForTheMenu;
            StartButton.Click -= OpenTheLevelSelectionWindow;
            BackgroundImage = null;
        }

        void OpenTheLevelSelectionWindow(object sender, EventArgs e)
        {
            CloseTheMainMenu();

            var dataFromTheFile = File.ReadAllLines(@"..\..\Model\Record.txt").FirstOrDefault();
            MaximumAvailableLevel = Math.Max(dataFromTheFile == null ? 0 : int.Parse(dataFromTheFile), 1);

            for (var row = 0; row < LevelButtons.GetLength(0); row++)
                for (var column = 0; column < LevelButtons.GetLength(1); column++)
                    ConfigureTheLevelLaunchButton(row, column);

            SizeChanged += RecalculateTheValuesOfTheLevelButtons;
            RecalculateTheValuesOfTheLevelButtons("", new EventArgs());
        }

        void ConfigureTheLevelLaunchButton(int row, int column)
        {
            LevelButtons[row, column] = new Button()
            {
                TextAlign = ContentAlignment.MiddleLeft,
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat,
            };

            if ((row * 10) + column + 1 <= MaximumAvailableLevel)
            {
                LevelButtons[row, column].Text = string.Format("{0}\n\n{1, 5}",(row * 10) + column + 1, "0/17");
                LevelButtons[row, column].BackgroundImage = Image.FromFile(@"..\..\Images\LevelIsOpen.png");
                LevelButtons[row, column].Click += LevelTriggerFunctions[row, column];
            }
            else LevelButtons[row, column].BackgroundImage = Image.FromFile(@"..\..\Images\LevelIsClosed.png");

            LevelButtons[row, column].FlatAppearance.MouseDownBackColor = Color.Black;
            Controls.Add(LevelButtons[row, column]);
        }

        void FillInTheMatrixWithFunctions()
        {
            for (var row = 0; row < LevelTriggerFunctions.GetLength(0); row++)
                for (var column = 0; column < LevelTriggerFunctions.GetLength(1); column++)
                {
                    (var i, var j) = (row, column);

                    LevelTriggerFunctions[i, j] = (object sender, EventArgs e) =>
                    {
                        Level = i * 10 + j + 1;
                        CloseTheLevelSelectionWindow();
                        OpenTheGame();
                    };
                }
        }

        void CloseTheLevelSelectionWindow()
        {
            Controls.Clear();

            for (var i = 0; i < LevelButtons.GetLength(0); i++)
                for (var j = 0; j < LevelButtons.GetLength(1); j++)
                    LevelButtons[i, j].Click -= LevelTriggerFunctions[i, j];

            SizeChanged -= RecalculateTheValuesOfTheLevelButtons;
        }

        void UpdateTheFieldsForTheMenu(object sender, EventArgs e)
        {
            StartButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * 16.3),
                (int)(InitialCoordinateOfTheMap.Y + ImageSize * 11));
            StartButton.Size = new Size() { Width = (int)(6 * ImageSize), Height = (int)(2 * ImageSize) };
            StartButton.FlatAppearance.BorderSize = (int)ImageSize / 9;
        }

        void OpenTheResultsWindow()
        {
            Controller.StopTimers();
            PauseButton.Enabled = false;
            RestartGameButton.Enabled = false;
            DeactivateGameControls();

            InitializeTheButtonsInTheResultsMenu();
            Paint += DrawTheResultsWindow;

            UpdateTheCoordinatesOfTheMenuTransitionButton("", new EventArgs());

            SizeChanged += UpdateTheCoordinatesOfTheMenuTransitionButton;
            ButtonToGoToTheMenu.Click += ReturnToTheMenu;

            Level = Model.Round;
            StartOverButton.Click += StartOver;
        }

        void InitializeTheButtonsInTheResultsMenu()
        {
            ButtonToGoToTheMenu = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\ButtonToGoToTheMenu.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(ButtonToGoToTheMenu);
            ButtonToGoToTheMenu.FlatAppearance.BorderColor = Color.DarkRed;
            ButtonToGoToTheMenu.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);

            StartOverButton = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\StartOver.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(StartOverButton);
            StartOverButton.FlatAppearance.BorderColor = Color.DarkRed;
            StartOverButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
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
            Controls.Clear();
            Paint -= DrawTheResultsWindow;
            SizeChanged -= UpdateTheCoordinatesOfTheMenuTransitionButton;
            ButtonToGoToTheMenu.Click -= ReturnToTheMenu;
            StartOverButton.Click -= StartOver;
        }

        void ReturnToTheMenu(object sender, EventArgs e)
        {
            EraseThePlayingField();
            CloseTheResultsWindow();
            OpenTheMainMenu();
        }
        #endregion

        //отрисовка отображения
        #region
        void DrawTheStartOfTheRound(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            DrawAPicture(@"..\..\Images\haze.png", new PointF(0, 0), ClientSize, e.Graphics);

            e.Graphics.DrawRectangle(new Pen(Color.DarkRed, ImageSize / 5), new Rectangle(
                new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + 9f * ImageSize), (int)(InitialCoordinateOfTheMap.Y + ImageSize * 5.3f)),
                new Size((int)(14 * ImageSize), (int)(ImageSize * 3.5f))));

            DrawTheText(e, $"Уровень {Level}", new RectangleF(
                new PointF(InitialCoordinateOfTheMap.X + 9f * ImageSize, InitialCoordinateOfTheMap.Y + ImageSize * 6f),
                new SizeF(14 * ImageSize, ImageSize * 2.5f)), Brushes.DarkRed,
                new StringFormat() { Alignment = StringAlignment.Center }, 1.5f * ImageSize);
        }

        void DrawTheResultsWindow(object sender, PaintEventArgs e)
        {
            DrawTheBackgroundOfTheResultsWindow(e.Graphics);

            DrawTheText(e, "Игра завершена!",
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + 8 * ImageSize, InitialCoordinateOfTheMap.Y + ImageSize * 5f),
                new SizeF(16 * ImageSize, ImageSize * 1.5f)), Brushes.DarkRed, new StringFormat() { Alignment = StringAlignment.Center }, ImageSize / 1.2f);

            DrawTheText(e, String.Format("{0, -6} {1, 9}", "Счёт",  Model.NumberOfBotsDestroyed),
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + ImageSize * 10.3f, InitialCoordinateOfTheMap.Y + ImageSize * 6.5f),
                new SizeF(16 * ImageSize, ImageSize * 1.5f)), Brushes.White, new StringFormat(), ImageSize / 1.34f);

            DrawAPicture(@"..\..\Images\star.png", new PointF(InitialCoordinateOfTheMap.X + ImageSize * 20.8f, InitialCoordinateOfTheMap.Y + 6.63f * ImageSize),
                new SizeF(ImageSize * 0.7f, ImageSize * 0.7f), e.Graphics);

            DrawTheText(e, String.Format("{0, -6} {1, 9}", "Монеты", 0),
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + ImageSize * 10.3f, InitialCoordinateOfTheMap.Y + 7.7f * ImageSize),
                new SizeF(12 * ImageSize, ImageSize)), Brushes.White, new StringFormat(), ImageSize / 1.34f);

            DrawAPicture(@"..\..\Images\Coin.png", new PointF(InitialCoordinateOfTheMap.X + ImageSize * 20.8f, InitialCoordinateOfTheMap.Y + 7.83f * ImageSize),
                new SizeF(ImageSize * 0.7f, ImageSize * 0.7f), e.Graphics);
        }

        void DrawTheBackgroundOfTheResultsWindow(Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            DrawAPicture(@"..\..\Images\haze.png", new PointF(0, 0), ClientSize, graphics);

            DrawAPicture(@"..\..\Images\ResultsWindow.png", new PointF(InitialCoordinateOfTheMap.X + 9.5f * ImageSize,
                InitialCoordinateOfTheMap.X + 4f * ImageSize), new SizeF(13 * ImageSize, 7.5f * ImageSize), graphics);
        }

        void DrawAGamePanel(object sender, PaintEventArgs e)
        {
            var strings = new string[] { "", "0"};

            DrawTheText(e, $"Счёт: {Model.NumberOfBotsDestroyed}",
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + 14.9f * ImageSize, InitialCoordinateOfTheMap.Y - ImageSize / 2 * 1.34f),
                new SizeF(3.5f * ImageSize, ImageSize)), Brushes.White, new StringFormat() { Alignment = StringAlignment.Far}, ImageSize / 2);

            DrawAPicture(@"..\..\Images\star.png", new PointF(InitialCoordinateOfTheMap.X + 18.4f * ImageSize,
                InitialCoordinateOfTheMap.Y - ImageSize * 0.7f), new SizeF(ImageSize * 0.7f, ImageSize * 0.7f), e.Graphics);

            DrawTheText(e, $"Уровень: {Model.Round}",
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + ImageSize * 23f, InitialCoordinateOfTheMap.Y - ImageSize / 2 * 1.34f),
                new SizeF(5f * ImageSize, ImageSize)), Brushes.White, new StringFormat()
                { Alignment = StringAlignment.Center}, ImageSize / 2);

            for (var i = 0; i < Model.Player.Health; i++)
                DrawAPicture(@"..\..\Images\heart.png", new PointF(InitialCoordinateOfTheMap.X + i * ImageSize,
                    InitialCoordinateOfTheMap.Y - ImageSize * 0.7f), new SizeF(ImageSize * 0.7f, ImageSize * 0.7f), e.Graphics);

            DrawTheText(e,
                $"0{Model.AmountOfTimeUntilTheEndOfTheRound / 60}:" + strings[2 - (Model.AmountOfTimeUntilTheEndOfTheRound % 60).ToString().Length] + $"{Model.AmountOfTimeUntilTheEndOfTheRound % 60}",
                new RectangleF(new PointF(InitialCoordinateOfTheMap.X + 8.8f * ImageSize, InitialCoordinateOfTheMap.Y - ImageSize / 2 * 1.34f),
                new SizeF(2.5f * ImageSize, ImageSize)), Brushes.White, new StringFormat(), ImageSize / 2);

            DrawAPicture(@"..\..\Images\Timer.png", new PointF(InitialCoordinateOfTheMap.X + 8.1f * ImageSize, 
                InitialCoordinateOfTheMap.Y - ImageSize * 0.7f), new SizeF(ImageSize * 0.7f, ImageSize * 0.7f), e.Graphics);
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

        void DrawAPicture(string pathToTheFile, PointF location, SizeF size, Graphics graphics)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            graphics.DrawImage(Image.FromFile(pathToTheFile), new PointF[]
            {
                location,
                new PointF(location.X + size.Width, location.Y),
                new PointF(location.X, location.Y + size.Height)
            });
        }
        #endregion

        //рассчёт/пересчёт координат отображения
        #region
        /// пересчитывает размеры и координаты сетки кнопок запуска уровней
        void RecalculateTheValuesOfTheLevelButtons(object sender, EventArgs e)
        {
            (var numberOfRows, var numberOfColumns) = (LevelButtons.GetLength(0), LevelButtons.GetLength(1));
            var buttonSize = new Size((int)(ImageSize * 4), (int)(ImageSize * 4));
            var distanceBetweenTheButtons = (int)(ImageSize / 2);
            var startingPoint = new PointF(
                (ClientSize.Width - buttonSize.Width * numberOfColumns - ((numberOfColumns - 1) * distanceBetweenTheButtons)) / 2f,
                (ClientSize.Height - buttonSize.Height * numberOfRows - ((numberOfRows - 1) * distanceBetweenTheButtons)) / 2f);

            for (var row = 0; row < numberOfRows; row++)
                for (var column = 0; column < numberOfColumns; column++)
                {
                    LevelButtons[row, column].Location = new System.Drawing.Point(
                        (int)(startingPoint.X + buttonSize.Width * column + column * distanceBetweenTheButtons),
                        (int)(startingPoint.Y + buttonSize.Height * row + row * distanceBetweenTheButtons));
                    LevelButtons[row, column].Size = buttonSize;
                    LevelButtons[row, column].Font = new Font(new FontFamily("Courier New"), Math.Max(ImageSize / 1.5f, 1), FontStyle.Bold);
                }
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
            ButtonToGoToTheMenu.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * 10.5f),
                (int)(InitialCoordinateOfTheMap.Y + ImageSize * 9.3));
            ButtonToGoToTheMenu.Size = new Size() { Width = (int)(5 * ImageSize), Height = (int)(2 * ImageSize) };

            StartOverButton.Size = ButtonToGoToTheMenu.Size;
            StartOverButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * 16.5f),
                (int)(InitialCoordinateOfTheMap.Y + ImageSize * 9.3));

            StartOverButton.FlatAppearance.BorderSize = (int)ImageSize / 9;
            ButtonToGoToTheMenu.FlatAppearance.BorderSize = (int)ImageSize / 9; ;
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
        #endregion
    }
}