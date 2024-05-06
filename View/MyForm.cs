using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsForm.Model;
using WindowsForm.Controller;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MainWindow
{
    public partial class MyForm : Form
    {
        private GameModel model;
        private Controller controller;

        private float sizeOfTheGridCell;
        private PointF initialCoordinate;

        private int level;
        private int maximumAvailableLevel;

        private Button pauseButton;
        private Button playButton;
        private Button buttonToGoToTheMenu;
        private Button restartGameButton;
        private Button startOverButton;

        private readonly Button[,] LevelButtons;
        private readonly EventHandler[,] LevelTriggerFunctions;

        private readonly Dictionary<bool, Image> pauseImages;

        public MyForm(GameModel model)
        {
            this.model = model;
            LevelButtons = new Button[3, 6];
            LevelTriggerFunctions = new EventHandler[LevelButtons.GetLength(0), LevelButtons.GetLength(1)];
            FillInTheMatrixWithFunctions();

            pauseImages = new Dictionary<bool, Image>()
            {
                [true] = Image.FromFile(@"..\..\Images\PauseTurnOn.png"),
                [false] = Image.FromFile(@"..\..\Images\PauseTurnOff.png")
            };

            //настройка WinForm
            KeyPreview = true;
            DoubleBuffered = true;
            BackColor = Color.Black;
            WindowState = FormWindowState.Maximized;
            Text = "Защитник Брестской крепости";

            OpenTheMainMenu();

            SizeChanged += UpdateFieldValues;
            Load += (sender, args) => OnSizeChanged(EventArgs.Empty);

            InitializeComponent();
        }

        #region ГЛАВНОЕ МЕНЮ
        void OpenTheMainMenu()
        {
            BackgroundImageLayout = ImageLayout.Zoom;
            BackgroundImage = Image.FromFile(@"..\..\Images\MainMenu.jpg");

            playButton = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\Play.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            playButton.FlatAppearance.BorderColor = Color.DarkRed;
            playButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            playButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 0, 0, 0);

            Controls.Add(playButton);

            RecalculateTheDimensionsInTheMainMenu("", new EventArgs());

            playButton.Click += OpenTheLevelSelectionWindow;
            SizeChanged += RecalculateTheDimensionsInTheMainMenu;
        }

        void CloseTheMainMenu()
        {
            Controls.Clear();
            SizeChanged -= RecalculateTheDimensionsInTheMainMenu;
            playButton.Click -= OpenTheLevelSelectionWindow;
            BackgroundImage = null;
        }

        void RecalculateTheDimensionsInTheMainMenu(object sender, EventArgs e)
        {
            playButton.Location = new System.Drawing.Point((int)(initialCoordinate.X + sizeOfTheGridCell * 16.3),
                (int)(initialCoordinate.Y + sizeOfTheGridCell * 11));
            playButton.Size = new Size() { Width = (int)(6 * sizeOfTheGridCell), Height = (int)(2 * sizeOfTheGridCell) };
            playButton.FlatAppearance.BorderSize = (int)sizeOfTheGridCell / 9;
        }
        #endregion

        #region ИГРА
        public void OpenTheGame()
        {
            CreateGamePanelButtons();
            RecalculateTheValuesOfTheGameButtons("", new EventArgs());
            SizeChanged += RecalculateTheValuesOfTheGameButtons;

            model = new GameModel(new Playground(), level);
            model.StateChanged += Invalidate;
            model.TheGameIsOver += StartTheNextLevel;

            controller = new Controller(model);
            controller.ActivateTimers();
            controller.PauseIsPressed += ChangeThePausePicture;

            Paint += DrawingTheModel;
            Paint += DrawAGamePanel;
            Paint += DrawTheStartOfTheLevel;
            Invalidate();

            controller.StopTimers();

            Click += StartTheGame;
        }

        void StartTheGame(object sender, EventArgs e)
        {
            Click -= StartTheGame;
            Paint -= DrawTheStartOfTheLevel;

            controller.ActivateTimers();
            ActivateGameManagement();
        }

        void ActivateGameManagement()
        {
            level = model.Level;

            Click += controller.ToShoot;
            pauseButton.Click += controller.PutItOnPause;
            restartGameButton.Click += RestartTheGame;

            KeyDown += controller.MakeAMove;
            MouseWheel += controller.RotateThePlayer;
        }

        void DeactivateGameManagement()
        {
            Click -= controller.ToShoot;
            restartGameButton.Click -= RestartTheGame;
            pauseButton.Click -= controller.PutItOnPause;

            KeyDown -= controller.MakeAMove;
            MouseWheel -= controller.RotateThePlayer;
        }

        void CreateGamePanelButtons()
        {
            pauseButton = new Button()
            {
                BackColor = Color.FromArgb(0, 0, 0, 0),
                BackgroundImage = pauseImages[true],
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat
            };
            pauseButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            pauseButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            Controls.Add(pauseButton);

            restartGameButton = new Button()
            {
                BackColor = Color.FromArgb(0, 0, 0, 0),
                BackgroundImage = Image.FromFile(@"..\..\Images\RestartGameButton.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                FlatStyle = FlatStyle.Flat
            };
            restartGameButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
            restartGameButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 255, 255, 255);
            Controls.Add(restartGameButton);
        }

        void EraseThePlayingField()
        {
            Controls.Clear();

            Paint -= DrawingTheModel;
            Paint -= DrawAGamePanel;

            SizeChanged -= RecalculateTheValuesOfTheGameButtons;

            model.StateChanged -= Invalidate;
            model.TheGameIsOver -= OpenTheResultsWindow;

            controller.StopTimers();
        }

        public void StartTheNextLevel()
        {
            if (!model.Map[model.Player.Location].Contains(model.Player) || model.Level == 50)
                OpenTheResultsWindow();
            else
            {
                EraseThePlayingField();
                DeactivateGameManagement();
                level++;
                OpenTheGame();
            }
        }

        void RestartTheGame(object sender, EventArgs e)
        {
            EraseThePlayingField();
            DeactivateGameManagement();
            OpenTheGame();
        }

        void DrawingTheModel(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (var x = 0; x < model.Map.Width; x++)
                for (var y = 0; y < model.Map.Height; y++)
                    foreach (var creture in model.Map[x, y])
                    {
                        var image = creture.Picture;
                        var coordinatesOnTheForm = RecalculateTheCoordinatesOnTheForm(new System.Drawing.Point(x, y));
                        e.Graphics.DrawImage(image, RotateAnArrayOfPoints(coordinatesOnTheForm, creture.AngleInDegrees * Math.PI / 180));
                    }
        }

        void DrawTheStartOfTheLevel(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            DrawAPicture(@"..\..\Images\haze.png", new PointF(0, 0), ClientSize, e.Graphics);

            e.Graphics.DrawRectangle(new Pen(Color.DarkRed, sizeOfTheGridCell / 5), new Rectangle(
                new System.Drawing.Point((int)(initialCoordinate.X + 9f * sizeOfTheGridCell), (int)(initialCoordinate.Y + sizeOfTheGridCell * 5.3f)),
                new Size((int)(14 * sizeOfTheGridCell), (int)(sizeOfTheGridCell * 3.5f))));

            DrawTheText(e, $"Уровень {level}", new RectangleF(
                new PointF(initialCoordinate.X + 9f * sizeOfTheGridCell, initialCoordinate.Y + sizeOfTheGridCell * 6f),
                new SizeF(14 * sizeOfTheGridCell, sizeOfTheGridCell * 2.5f)), Brushes.DarkRed,
                new StringFormat() { Alignment = StringAlignment.Center }, 1.5f * sizeOfTheGridCell);
        }

        void DrawAGamePanel(object sender, PaintEventArgs e)
        {
            var strings = new string[] { "", "0" };

            DrawTheText(e, $"Счёт: {model.NumberOfBotsDestroyed}", new RectangleF(new PointF(initialCoordinate.X + 14.9f * sizeOfTheGridCell,
                initialCoordinate.Y - sizeOfTheGridCell / 2 * 1.34f), new SizeF(3.5f * sizeOfTheGridCell, sizeOfTheGridCell)),
                Brushes.White, new StringFormat() { Alignment = StringAlignment.Far }, sizeOfTheGridCell / 2);

            DrawAPicture(@"..\..\Images\star.png", new PointF(initialCoordinate.X + 18.4f * sizeOfTheGridCell,
                initialCoordinate.Y - sizeOfTheGridCell * 0.7f), new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), e.Graphics);

            DrawTheText(e, $"Уровень: {model.Level}",
                new RectangleF(new PointF(initialCoordinate.X + sizeOfTheGridCell * 23f, initialCoordinate.Y - sizeOfTheGridCell / 2 * 1.34f),
                new SizeF(5f * sizeOfTheGridCell, sizeOfTheGridCell)), Brushes.White, new StringFormat()
                { Alignment = StringAlignment.Center }, sizeOfTheGridCell / 2);

            for (var i = 0; i < model.Player.Health; i++)
                DrawAPicture(@"..\..\Images\heart.png", new PointF(initialCoordinate.X + i * sizeOfTheGridCell,
                    initialCoordinate.Y - sizeOfTheGridCell * 0.7f), new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), e.Graphics);

            DrawTheText(e,
                $"0{model.AmountOfTimeUntilTheEndOfTheRound / 60}:" + strings[2 - (model.AmountOfTimeUntilTheEndOfTheRound % 60).ToString().Length] + $"{model.AmountOfTimeUntilTheEndOfTheRound % 60}",
                new RectangleF(new PointF(initialCoordinate.X + 8.8f * sizeOfTheGridCell, initialCoordinate.Y - sizeOfTheGridCell / 2 * 1.34f),
                new SizeF(2.5f * sizeOfTheGridCell, sizeOfTheGridCell)), Brushes.White, new StringFormat(), sizeOfTheGridCell / 2);

            DrawAPicture(@"..\..\Images\Timer.png", new PointF(initialCoordinate.X + 8.1f * sizeOfTheGridCell,
                initialCoordinate.Y - sizeOfTheGridCell * 0.7f), new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), e.Graphics);
        }

        void ChangeThePausePicture(bool gameIsRunning) => pauseButton.BackgroundImage = pauseImages[gameIsRunning];

        void RecalculateTheValuesOfTheGameButtons(object sender, EventArgs e)
        {
            pauseButton.Location = new System.Drawing.Point((int)(initialCoordinate.X + sizeOfTheGridCell * (model.Map.Width - 1)),
                (int)(initialCoordinate.Y - sizeOfTheGridCell));
            pauseButton.Size = new Size((int)sizeOfTheGridCell, (int)sizeOfTheGridCell);

            restartGameButton.Location = new System.Drawing.Point((int)(initialCoordinate.X + sizeOfTheGridCell * (model.Map.Width - 2)),
                (int)(initialCoordinate.Y - sizeOfTheGridCell));
            restartGameButton.Size = pauseButton.Size;
        }
        #endregion

        #region ТАБЛИЦА УРОВНЕЙ
        void OpenTheLevelSelectionWindow(object sender, EventArgs e)
        {
            CloseTheMainMenu();

            var dataFromTheFile = File.ReadAllLines(@"..\..\Model\Record.txt").FirstOrDefault();
            maximumAvailableLevel = Math.Max(dataFromTheFile == null ? 0 : int.Parse(dataFromTheFile), 1);

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

            if ((row * 10) + column + 1 <= maximumAvailableLevel)
            {
                LevelButtons[row, column].Text = string.Format("{0}\n\n{1, 5}",(row * 10) + column + 1, "0/14");
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
                        level = i * 10 + j + 1;
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

        void RecalculateTheValuesOfTheLevelButtons(object sender, EventArgs e)
        {
            (var numberOfRows, var numberOfColumns) = (LevelButtons.GetLength(0), LevelButtons.GetLength(1));
            var buttonSize = new Size((int)(sizeOfTheGridCell * 4), (int)(sizeOfTheGridCell * 4));
            var distanceBetweenTheButtons = (int)(sizeOfTheGridCell / 2);
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
                    LevelButtons[row, column].Font = new Font(new FontFamily("Courier New"), Math.Max(sizeOfTheGridCell / 1.5f, 1), FontStyle.Bold);
                }
        }
        #endregion

        #region ОКНО РЕЗУЛЬТАТОВ
        void OpenTheResultsWindow()
        {
            level = model.Level;

            controller.StopTimers();
            pauseButton.Enabled = false;
            restartGameButton.Enabled = false;
            DeactivateGameManagement();

            InitializeTheButtonsInTheResultsMenu();
            RecalculateTheCoordinatesOfTheButtonsOfTheResetWindow("", new EventArgs());

            Paint += DrawTheResultsWindow;

            SizeChanged += RecalculateTheCoordinatesOfTheButtonsOfTheResetWindow;

            startOverButton.Click += StartOver;
            buttonToGoToTheMenu.Click += ReturnToTheMenu;

        }
        void CloseTheResultsWindow()
        {
            Controls.Clear();

            Paint -= DrawTheResultsWindow;
            SizeChanged -= RecalculateTheCoordinatesOfTheButtonsOfTheResetWindow;

            startOverButton.Click -= StartOver;
            buttonToGoToTheMenu.Click -= ReturnToTheMenu;
        }

        void ReturnToTheMenu(object sender, EventArgs e)
        {
            EraseThePlayingField();
            CloseTheResultsWindow();
            OpenTheMainMenu();
        }

        void InitializeTheButtonsInTheResultsMenu()
        {
            buttonToGoToTheMenu = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\ButtonToGoToTheMenu.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(buttonToGoToTheMenu);
            buttonToGoToTheMenu.FlatAppearance.BorderColor = Color.DarkRed;
            buttonToGoToTheMenu.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);

            startOverButton = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\StartOver.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(0, 0, 0, 0),
                FlatStyle = FlatStyle.Flat
            };
            Controls.Add(startOverButton);
            startOverButton.FlatAppearance.BorderColor = Color.DarkRed;
            startOverButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 0, 0, 0);
        }

        void StartOver(object sender, EventArgs e)
        {
            EraseThePlayingField();
            CloseTheResultsWindow();
            OpenTheGame();
        }

        void DrawTheResultsWindow(object sender, PaintEventArgs e)
        {
            DrawTheBackgroundOfTheResultsWindow(e.Graphics);

            DrawTheText(e, "Игра завершена!",
                new RectangleF(new PointF(initialCoordinate.X + 8 * sizeOfTheGridCell, initialCoordinate.Y + sizeOfTheGridCell * 5f),
                new SizeF(16 * sizeOfTheGridCell, sizeOfTheGridCell * 1.5f)), Brushes.DarkRed, new StringFormat()
                { Alignment = StringAlignment.Center }, sizeOfTheGridCell / 1.2f);

            DrawTheText(e, String.Format("{0, -6} {1, 9}", "Счёт", model.NumberOfBotsDestroyed),
                new RectangleF(new PointF(initialCoordinate.X + sizeOfTheGridCell * 10.3f, initialCoordinate.Y + sizeOfTheGridCell * 6.5f),
                new SizeF(16 * sizeOfTheGridCell, sizeOfTheGridCell * 1.5f)), Brushes.White, new StringFormat(), sizeOfTheGridCell / 1.34f);

            DrawAPicture(@"..\..\Images\star.png", new PointF(initialCoordinate.X + sizeOfTheGridCell * 20.8f, initialCoordinate.Y + 6.63f * sizeOfTheGridCell),
                new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), e.Graphics);

            DrawTheText(e, String.Format("{0, -6} {1, 9}", "Монеты", 0),
                new RectangleF(new PointF(initialCoordinate.X + sizeOfTheGridCell * 10.3f, initialCoordinate.Y + 7.7f * sizeOfTheGridCell),
                new SizeF(12 * sizeOfTheGridCell, sizeOfTheGridCell)), Brushes.White, new StringFormat(), sizeOfTheGridCell / 1.34f);

            DrawAPicture(@"..\..\Images\Coin.png", new PointF(initialCoordinate.X + sizeOfTheGridCell * 20.8f, initialCoordinate.Y + 7.83f * sizeOfTheGridCell),
                new SizeF(sizeOfTheGridCell * 0.7f, sizeOfTheGridCell * 0.7f), e.Graphics);
        }

        void DrawTheBackgroundOfTheResultsWindow(Graphics graphics)
        {
            DrawAPicture(@"..\..\Images\haze.png", new PointF(0, 0), ClientSize, graphics);

            DrawAPicture(@"..\..\Images\ResultsWindow.png", new PointF(initialCoordinate.X + 9.5f * sizeOfTheGridCell,
                initialCoordinate.X + 4f * sizeOfTheGridCell), new SizeF(13 * sizeOfTheGridCell, 7.5f * sizeOfTheGridCell), graphics);
        }

        void RecalculateTheCoordinatesOfTheButtonsOfTheResetWindow(object sender, EventArgs e)
        {
            buttonToGoToTheMenu.Location = new System.Drawing.Point((int)(initialCoordinate.X + sizeOfTheGridCell * 10.5f),
                (int)(initialCoordinate.Y + sizeOfTheGridCell * 9.3));
            buttonToGoToTheMenu.Size = new Size() { Width = (int)(5 * sizeOfTheGridCell), Height = (int)(2 * sizeOfTheGridCell) };

            startOverButton.Size = buttonToGoToTheMenu.Size;
            startOverButton.Location = new System.Drawing.Point((int)(initialCoordinate.X + sizeOfTheGridCell * 16.5f),
                (int)(initialCoordinate.Y + sizeOfTheGridCell * 9.3));

            startOverButton.FlatAppearance.BorderSize = (int)sizeOfTheGridCell / 9;
            buttonToGoToTheMenu.FlatAppearance.BorderSize = (int)sizeOfTheGridCell / 9; ;
        }
        #endregion

        #region ПРОЧЕЕ
        void DrawTheText(PaintEventArgs e, string text, RectangleF location, Brush brushes, StringFormat format, float fontSize)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            e.Graphics.DrawString(text, new Font("Courier New", Math.Max(fontSize, 1), FontStyle.Bold), brushes, location, format);
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

        void UpdateFieldValues(object sender, EventArgs e)
        {
            sizeOfTheGridCell = Math.Min(ClientSize.Height / (model.Map.Height + 1), ClientSize.Width / model.Map.Width);

            initialCoordinate = new PointF((ClientSize.Width - sizeOfTheGridCell * model.Map.Width) / 2,
                (ClientSize.Height - sizeOfTheGridCell * (model.Map.Height + 1)) / 2 + sizeOfTheGridCell);
        }

        PointF[] RecalculateTheCoordinatesOnTheForm(System.Drawing.Point positionOnTheMap)
        { 
            return new PointF[] {
                new PointF(positionOnTheMap.X * sizeOfTheGridCell + initialCoordinate.X, positionOnTheMap.Y * sizeOfTheGridCell + initialCoordinate.Y),
                new PointF(positionOnTheMap.X * sizeOfTheGridCell + initialCoordinate.X + sizeOfTheGridCell, positionOnTheMap.Y * sizeOfTheGridCell + initialCoordinate.Y),
                new PointF(positionOnTheMap.X * sizeOfTheGridCell + initialCoordinate.X, positionOnTheMap.Y * sizeOfTheGridCell + initialCoordinate.Y + sizeOfTheGridCell) };
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