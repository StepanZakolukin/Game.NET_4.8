using Model;
using System.Windows.Forms;

namespace WindowsForm.Model
{
    public class Bullet : MobileGameObject
    {
        public Bullet(string image, Point positionOnTheMap, int angleInDegrees, Control control)
            : base(image, positionOnTheMap, angleInDegrees, control)
        {
            Priority = 200; 
        }
    }
}
