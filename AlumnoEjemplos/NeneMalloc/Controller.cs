using Microsoft.DirectX;

namespace AlumnoEjemplos.NeneMalloc
{
    public abstract class Controller
    {
        public Order order { get; set; }

        public Character character { get; set; }

        public Vector3 GenericUp { get { return new Vector3(0.0f, 1.0f, 0.0f); } }

        public abstract void render();
    }
}
