using System.Windows.Forms;
using WindowsForm.Model;

namespace Model
{
    public class Soldier : MobileGameObject
    {
        public Soldier(string image, Point positionOnTheMap, int angleInDegrees, Control control)
            : base(image, positionOnTheMap, angleInDegrees, control)
        {
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
                    if (GameModel.graph[PositionOnTheMap].Back != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Back.Coordinates;
                    break;
                case 180:
                    if (GameModel.graph[PositionOnTheMap].Left != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Left.Coordinates;
                    break;
                case 0:
                    if (GameModel.graph[PositionOnTheMap].Right != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Right.Coordinates;
                    break;
                case 270:
                    if (GameModel.graph[PositionOnTheMap].Forward != null) PositionOnTheMap = GameModel.graph[PositionOnTheMap].Forward.Coordinates;
                    break;
            }

            ReplaceTheCharacterRenderingFunction(control);
        }

        //public void Shoot(Control control)
        //{
        //    switch (AngleInDegrees % 360)
        //    {
        //        case 90:
        //            if (GameModel.graph[PositionOnTheMap].Forward != null)
        //            {
        //                HistoryOfTheShots.Add(new Bullet(@"..\..\Model\Images\пуля.png", GameModel.graph[PositionOnTheMap].Forward.Coordinates, AngleInDegrees, control));
        //                HistoryOfTheShots.Last().GoForward(control);
        //            }
        //            break;
        //        case 180:
        //            if (GameModel.graph[PositionOnTheMap].Right != null)
        //            {
        //                HistoryOfTheShots.Add(new Bullet(@"..\..\Model\Images\пуля.png", GameModel.graph[PositionOnTheMap].Right.Coordinates, AngleInDegrees, control));
        //                HistoryOfTheShots.Last().GoForward(control);
        //            }
        //            break;
        //        case 0:
        //            if (GameModel.graph[PositionOnTheMap].Left != null)
        //            {
        //                HistoryOfTheShots.Add(new Bullet(@"..\..\Model\Images\пуля.png", GameModel.graph[PositionOnTheMap].Left.Coordinates, AngleInDegrees, control));
        //                HistoryOfTheShots.Last().GoForward(control);
        //            }    
                    
        //            break;
        //        case 270:
        //            if (GameModel.graph[PositionOnTheMap].Back != null)
        //            {
        //                HistoryOfTheShots.Add(new Bullet(@"..\..\Model\Images\пуля.png", GameModel.graph[PositionOnTheMap].Back.Coordinates, AngleInDegrees, control));
        //                HistoryOfTheShots.Last().GoForward(control);
        //            }
        //            break;
        //    }
        //}
    }
}
