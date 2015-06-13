using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Input;
using System.Windows.Forms;
using System.Drawing;

namespace AlumnoEjemplos.NeneMalloc
{
    public class Player : Controller
    {
        bool mouseReleased = false;
        public override void update()
        {
            this.order = new Order();
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                order.moveForward = d3dInput.keyDown(Key.LeftShift) ? 2 : 1;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                order.moveForward = d3dInput.keyDown(Key.LeftShift) ? -2 : -1; ;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                order.moveAside = 1;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                order.moveAside = -1;
            }
            if (d3dInput.keyPressed(Key.LeftControl))
            {
                mouseReleased = !mouseReleased;
            }
            GuiController.Instance.UserVars.setValue("MouseReleased", mouseReleased);
            if (mouseReleased)
            {
                Cursor.Show();
            }
            else
            {
                order.rotateX = d3dInput.YposRelative;
                order.rotateY = d3dInput.XposRelative;
                Cursor.Hide();
                Cursor.Position = new Point( GuiController.Instance.FullScreenPanel.Width/2,GuiController.Instance.FullScreenPanel.Height/2);
            }
           

          
        }
    }
}
