using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.NeneMalloc
{
    public abstract class Controller
    {
        public Order order { get; set; }

        public Character character { get; set; }

        public abstract void render();
    }
}
