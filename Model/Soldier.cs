using System.Windows.Forms;
using WindowsForm.Model;

namespace Model
{
    public class Soldier : MobileGameObject
    {
        public Soldier(string image, Point positionOnTheMap, int angleInDegrees, Control control)
            : base(image, positionOnTheMap, angleInDegrees, control)
        {
            Priority = 100;
        }

        public void TurnLeft(Control control) => RotateThePlayer(control, 270);

        public void TurnRight(Control control) => RotateThePlayer(control, 90);

        public void  RotateThePlayer(Control control, int angleInDegrees)
        {
            AngleInDegrees += angleInDegrees;
            ReplaceTheCharacterRenderingFunction(control);
        }

        public void GoBack(Control control)
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (GameModel.GraphOfTheMapPaths[PositionOnTheMap].Back != null) PositionOnTheMap = GameModel.GraphOfTheMapPaths[PositionOnTheMap].Back.Coordinates;
                    break;
                case 180:
                    if (GameModel.GraphOfTheMapPaths[PositionOnTheMap].Left != null) PositionOnTheMap = GameModel.GraphOfTheMapPaths[PositionOnTheMap].Left.Coordinates;
                    break;
                case 0:
                    if (GameModel.GraphOfTheMapPaths[PositionOnTheMap].Right != null) PositionOnTheMap = GameModel.GraphOfTheMapPaths[PositionOnTheMap].Right.Coordinates;
                    break;
                case 270:
                    if (GameModel.GraphOfTheMapPaths[PositionOnTheMap].Forward != null) PositionOnTheMap = GameModel.GraphOfTheMapPaths[PositionOnTheMap].Forward.Coordinates;
                    break;
            }

            ReplaceTheCharacterRenderingFunction(control);
        }

        public void Shoot(Control control)
        {
            switch (AngleInDegrees % 360)
            {
                case 90:
                    if (GameModel.GraphOfTheMapPaths[PositionOnTheMap].Forward != null)
                    {
                        GameModel.ActivBullets.Enqueue(new Bullet(@"..\..\Model\Images\пуля.png", GameModel.GraphOfTheMapPaths[PositionOnTheMap].Forward.Coordinates, AngleInDegrees, control));
                    }
                    break;
                case 180:
                    if (GameModel.GraphOfTheMapPaths[PositionOnTheMap].Right != null)
                    {
                        GameModel.ActivBullets.Enqueue(new Bullet(@"..\..\Model\Images\пуля.png", GameModel.GraphOfTheMapPaths[PositionOnTheMap].Right.Coordinates, AngleInDegrees, control));
                    }
                    break;
                case 0:
                    if (GameModel.GraphOfTheMapPaths[PositionOnTheMap].Left != null)
                    {
                        GameModel.ActivBullets.Enqueue(new Bullet(@"..\..\Model\Images\пуля.png", GameModel.GraphOfTheMapPaths[PositionOnTheMap].Left.Coordinates, AngleInDegrees, control));
                    }

                    break;
                case 270:
                    if (GameModel.GraphOfTheMapPaths[PositionOnTheMap].Back != null)
                    {
                        GameModel.ActivBullets.Enqueue(new Bullet(@"..\..\Model\Images\пуля.png", GameModel.GraphOfTheMapPaths[PositionOnTheMap].Back.Coordinates, AngleInDegrees, control));
                    }
                    break;
            }
        }
    }
}
