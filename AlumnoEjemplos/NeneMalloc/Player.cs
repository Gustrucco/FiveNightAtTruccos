using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.NeneMalloc
{
    public class Player : Controller
    {
        public override void render()
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

            if (d3dInput.keyDown(Key.UpArrow))
            {
                order.rotateX = +1;
            }

            if (d3dInput.keyDown(Key.DownArrow))
            {
                order.rotateX = -1;
            }

            if (d3dInput.keyDown(Key.RightArrow))
            {
                order.rotateY = +1;
            }
            if (d3dInput.keyDown(Key.LeftArrow))
            {
                order.rotateY = -1;
            }
        }
    }
}
