using System;
using System.Drawing;
using System.Windows.Forms;
using WindowsForm.Model;

namespace MainWindow
{
    public partial class MyForm : Form
    {
        private readonly MyForm Current;
        public float ImageSize { get; private set; } 
        public PointF InitialCoordinateOfTheMap { get; private set; }

        public MyForm(GameModel model)
        {
            Current = this;
            Text = "Последний защитник Брестской Крепости";
            InitializeComponent();
            BackColor = Color.Black;
            Size = new Size(1072, 699);
            DoubleBuffered = true;
            var controller = new Controller.Controller(model);
            controller.Timer.Tick += TimerTick;
            Paint += DrawAModel;
            KeyDown += new KeyEventHandler(controller.MakeAMove);
            Click += new EventHandler(controller.ToShoot);
            MouseWheel += new MouseEventHandler(controller.RotateThePlayer);
        }

        private void TimerTick(object sender, EventArgs args)
        {
            UpdateFieldValues();
            Invalidate();
        }

        private void DrawAModel(object sender, PaintEventArgs e)
        {
            var stone = Image.FromFile(@"..\..\Images\камень.jpg");
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (var x = 0; x < GameModel.Map.MapWidth; x++)
                for (var y = 0; y < GameModel.Map.MapHeight; y++)
                {
                    var image = GameModel.Map[x, y].Picture;
                    var coordinatesOnTheForm = RecalculateTheCoordinatesOnTheForm(new System.Drawing.Point(x, y));

                    if (GameModel.Map[x, y] is Wall) e.Graphics.DrawImage(image, coordinatesOnTheForm);
                    else
                    {
                        e.Graphics.DrawImage(stone, coordinatesOnTheForm);

                        if (!(GameModel.Map[x, y] is Stone))
                            e.Graphics.DrawImage(image, RotateAnArrayOfPoints(coordinatesOnTheForm, GameModel.Map[x, y].AngleInDegrees * Math.PI / 180));
                    }
                }
        }

        private PointF[] RecalculateTheCoordinatesOnTheForm(System.Drawing.Point positionOnTheMap)
        { 
            return new PointF[] {
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y),
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X + ImageSize, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y),
                new PointF(positionOnTheMap.X * ImageSize + InitialCoordinateOfTheMap.X, positionOnTheMap.Y * ImageSize + InitialCoordinateOfTheMap.Y + ImageSize) };
        }

        private void UpdateFieldValues()
        {
            ImageSize = Math.Min(Current.Size.Height / 18, Current.Size.Width / 32);

            InitialCoordinateOfTheMap = new PointF((Current.Size.Width - ImageSize * 32) / 2 - ImageSize / 4,
                (Current.Size.Height - ImageSize * 18) / 2 - ImageSize / 2);
        }

        private PointF[] RotateAnArrayOfPoints(PointF[] points, double turn)
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

        private PointF RotateAPoint(PointF point, double angleInRadians)
        {
            var d = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            var angle = Math.Atan2(point.Y, point.X) + angleInRadians;

            return new PointF((float)(Math.Cos(angle) * d), (float)(Math.Sin(angle) * d));
        }
    }
}
