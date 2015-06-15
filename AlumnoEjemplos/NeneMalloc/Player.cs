using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Utils.Input;
using System.Windows.Forms;
using System.Drawing;

namespace AlumnoEjemplos.NeneMalloc
{
    public class Player : Controller
    {
        bool mouseReleased;

        public override void Update()
        {
            this.Order = new Order();
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                Order.moveForward = d3dInput.keyDown(Key.LeftShift) ? 2 : 1;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                Order.moveForward = d3dInput.keyDown(Key.LeftShift) ? -2 : -1; ;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                Order.moveAside = 1;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                Order.moveAside = -1;
            }

            if (d3dInput.keyPressed(Key.LeftControl))
            {
                mouseReleased = !mouseReleased;
            }

            Order.printCheckPoint = d3dInput.keyPressed(Key.P);

            GuiController.Instance.UserVars.setValue("MouseReleased", mouseReleased);
            if (mouseReleased)
            {
                Cursor.Show();
            }
            else
            {
                Order.rotateX = d3dInput.YposRelative;
                Order.rotateY = d3dInput.XposRelative;
                Cursor.Hide();
                Cursor.Position = new Point( GuiController.Instance.FullScreenPanel.Width/2,GuiController.Instance.FullScreenPanel.Height/2);
            }
           

          
        }
    }
}
