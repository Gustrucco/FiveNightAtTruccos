using Microsoft.DirectX;

namespace AlumnoEjemplos.NeneMalloc
{
    public abstract class Controller
    {
        protected Order Order { get; set; }

        public Character Character { get; set; }

        public Vector3 GenericUp { get { return new Vector3(0.0f, 1.0f, 0.0f); } }

        public abstract void Update();

        public Order getLastOrder()
        {
            var anOrder = Order;
            Order = null;
            return anOrder;
        }
    }
}
