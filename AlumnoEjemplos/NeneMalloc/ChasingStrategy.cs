using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.NeneMalloc
{
    public interface ChasingStrategy
    {
        Vector3 getNewPosition();
    }
}
