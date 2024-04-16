using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsForm.Model;

namespace MainWindow
{
    public partial class MyForm : Form
    {
        private readonly Model Model;
        private float ImageSize { get; set; }
        private PointF InitialCoordinateOfTheMap { get; set; }
        private RectangleF CoordinatesOfTheInscription;
        private readonly Button PauseButton;
        public MyForm(Model model)
        {
            Model = model;
            KeyPreview = true;
            DoubleBuffered = true;
            BackColor = Color.Black;
            Size = new Size() { Height = 450, Width = 800 };
            WindowState = FormWindowState.Maximized;
            Text = "Последний защитник Брестской Крепости";

            PauseButton = new Button()
            {
                BackColor = Color.White,
                Location = new System.Drawing.Point((int)(InitialCoordinateOfTheMap.X + 31 * ImageSize), 0),
                Size = new Size((int)ImageSize, (int)ImageSize),
                BackgroundImage = Image.FromFile(@"..\..\Images\PauseButton.png"),
                BackgroundImageLayout = ImageLayout.Zoom
            };
            Controls.Add(PauseButton);

            var controller = new Controller.Controller(model);
            PauseButton.Click += controller.PutItOnPause;
            Click += controller.ToShoot;
            KeyDown += controller.MakeAMove;
            MouseWheel += controller.RotateThePlayer;

            Paint += DrawingTheModel;
            Paint += DrawTheText;
            Load += (sender, args) => OnSizeChanged(EventArgs.Empty);
            SizeChanged += (object sender, EventArgs e) => UpdateFieldValues();

            model.StateChanged += Invalidate;

            InitializeComponent();
        }

        void DrawTheText(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.DrawString($"Уничтожено врагов: {Model.NumberOfActiveBots}", new Font("Arial", Math.Max(ImageSize / 2, 1), FontStyle.Bold), Brushes.White, CoordinatesOfTheInscription);
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

        void UpdateFieldValues()
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