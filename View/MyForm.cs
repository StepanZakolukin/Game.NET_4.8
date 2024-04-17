using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WindowsForm.Model;

namespace MainWindow
{
    public partial class MyForm : Form
    {
        private readonly Model Model;
        private readonly Controller.Controller Controller;
        private float ImageSize { get; set; }
        private PointF InitialCoordinateOfTheMap { get; set; }
        private RectangleF CoordinatesOfTheInscription;
        private Button PauseButton { get; set; }
        private Button StartButton { get; set; }
        public MyForm(Model model)
        {
            Model = model;
            Controller = new Controller.Controller(model);

            KeyPreview = true;
            DoubleBuffered = true;

            BackColor = Color.Black;
            Size = new Size() { Height = 450, Width = 800 };
            WindowState = FormWindowState.Maximized;
            Text = "Защитник Брестской Крепости";

            OpenTheMainMenu();

            Load += (sender, args) => OnSizeChanged(EventArgs.Empty);
            InitializeComponent();
        }

        public void StartGame(object sender, EventArgs e)
        {
            CloseTheMainMenu();
            Controller.ActivateTimers();
            StartTheGame();
        }

        void OpenTheMainMenu()
        {
            BackgroundImage = Image.FromFile(@"..\..\Images\стена4.jpg");
            BackgroundImageLayout = ImageLayout.Zoom;

            //Paint += DrawTheMainMenu;

            StartButton = new Button()
            {
                BackgroundImage = Image.FromFile(@"..\..\Images\кнопка.png"),
                BackgroundImageLayout = ImageLayout.Zoom,
                BackColor = Color.FromArgb(110, 59, 13)
            };
            Controls.Add(StartButton);

            Controller.TimerMenuStart();
            StartButton.Click += StartGame;
            SizeChanged += UpdateTheFieldsForTheMenu;
            Controller.Timer.Tick += UpdateTheMenuRendering;
        }

        void UpdateTheMenuRendering(object sender, EventArgs e) => Invalidate();

        public void CloseTheMainMenu()
        {
            Controls.Clear();
            Controller.TimerMenuStop();
            SizeChanged -= UpdateTheFieldsForTheMenu;
            StartButton.Click -= StartGame;
            Paint -= DrawTheMainMenu;
            Controller.Timer.Tick -= UpdateTheMenuRendering;
            BackgroundImage = null;
        }

        void DrawTheMainMenu(object sender, PaintEventArgs e)
        {
            //var wallMap = File.ReadAllText(@"..\..\View\Screensaver.txt").Split('\n')
            //    .Select(st => st.Split('\t'))
            //    .ToArray();
            //var wall = Image.FromFile(@"..\..\Images\кирпич.jpg");
            //var stone = Image.FromFile(@"..\..\Images\камень.jpg");

            //for (var y = 0; y < wallMap.Length; y++)
            //    for (var x = 0; x < wallMap[y].Length; x++)
            //    {
            //        if (wallMap[y][x] == "0")e.Graphics.DrawImage(stone, RecalculateTheCoordinatesOnTheForm(new System.Drawing.Point(x, y)));
            //        else e.Graphics.DrawImage(wall, RecalculateTheCoordinatesOnTheForm(new System.Drawing.Point(x, y)));
            //    }

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.DrawString("Защитник Брестской Крепости", new Font("Arial", Math.Max(ImageSize / 6 * 5, 1), FontStyle.Bold), Brushes.Black, new RectangleF(new PointF(InitialCoordinateOfTheMap.X + 1 * ImageSize, InitialCoordinateOfTheMap.Y + 3 * ImageSize),
                new Size((int)(14 * ImageSize), (int)(3 * ImageSize))), new StringFormat() { Alignment = StringAlignment.Center });
        }

        void UpdateTheFieldsForTheMenu(object sender, EventArgs e)
        {
            ImageSize = Math.Min(ClientSize.Height / 18, ClientSize.Width / 16);

            InitialCoordinateOfTheMap = new PointF((ClientSize.Width - ImageSize * 16) / 2, (ClientSize.Height - ImageSize * 18) / 2);

            StartButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * 4), (int)(InitialCoordinateOfTheMap.Y + ImageSize * 12));
            StartButton.Size = new Size() { Width = (int)(8 * ImageSize), Height = (int)(ImageSize * 2) };
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
            SizeChanged -= UpdateFieldValues;
            Model.StateChanged -= Invalidate;

            Controller.StopTimers();
        }

        public void StartTheGame()
        {
            PauseButton = new Button()
            {
                BackColor = Color.White,
                BackgroundImage = Image.FromFile(@"..\..\Images\PauseButton.png"),
                BackgroundImageLayout = ImageLayout.Zoom
            };
            Controls.Add(PauseButton);

            Paint += DrawingTheModel;
            Paint += WithdrawTheGameScore;
            Model.StateChanged += Invalidate;
            UpdateFieldValues("", new EventArgs());

            SizeChanged += UpdateFieldValues;

            PauseButton.Click += Controller.PutItOnPause;
            Click += Controller.ToShoot;
            KeyDown += Controller.MakeAMove;
            MouseWheel += Controller.RotateThePlayer;
        }

        void WithdrawTheGameScore(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.DrawString($"Счёт: {Model.NumberOfActiveBots}", new Font("Arial", Math.Max(ImageSize / 2, 1), FontStyle.Bold), Brushes.White, CoordinatesOfTheInscription);
        }

        void DrawingTheModel(object sender, PaintEventArgs e)
        {
            var stone = Image.FromFile(@"..\..\Images\камень.jpg");
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (var x = 0; x < Model.Map.Width; x++)
                for (var y = 0; y < Model.Map.Height; y++)
                {
                    var image = Model.Map[x, y].Picture;
                    var coordinatesOnTheForm = RecalculateTheCoordinatesOnTheForm(new System.Drawing.Point(x, y));

                    if (Model.Map[x, y] is Wall) e.Graphics.DrawImage(image, coordinatesOnTheForm);
                    else
                    {
                        e.Graphics.DrawImage(stone, coordinatesOnTheForm);

                        if (!(Model.Map[x, y] is Stone))
                            e.Graphics.DrawImage(image, RotateAnArrayOfPoints(coordinatesOnTheForm, Model.Map[x, y].AngleInDegrees * Math.PI / 180));
                    }
                }
        }

        PointF[] RecalculateTheCoordinatesOnTheForm(System.Drawing.Point positionOnTheMap)
        { 
            return new PointF[] {
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y),
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X + ImageSize, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y),
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y + ImageSize) };
        }

        void UpdateFieldValues(object sender, EventArgs e)
        {
            ImageSize = Math.Min(ClientSize.Height / (Model.Map.Height + 1), ClientSize.Width / Model.Map.Width);

            InitialCoordinateOfTheMap = new PointF((ClientSize.Width - ImageSize * Model.Map.Width) / 2,
                (ClientSize.Height - ImageSize * (Model.Map.Height + 1)) / 2 + ImageSize);

            CoordinatesOfTheInscription = new RectangleF(
                new PointF(InitialCoordinateOfTheMap.X, InitialCoordinateOfTheMap.Y - ImageSize / 2 * 1.34f),
                new SizeF(InitialCoordinateOfTheMap.X + ImageSize * (Model.Map.Width - 1), ImageSize));

            PauseButton.Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + ImageSize * (Model.Map.Width - 1)),
                (int)(InitialCoordinateOfTheMap.Y - ImageSize));
            PauseButton.Size = new Size((int)ImageSize, (int)ImageSize);
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