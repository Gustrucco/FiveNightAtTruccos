using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlumnoEjemplos.NeneMalloc
{
    public abstract class Character
    {
        public Vector3 rotation ;

        public void moveAside(float movement)
        {
            
            float z = (float)Math.Cos((float)rotation.Y - Geometry.DegreeToRadian(90f)) * movement;
            float x = (float)Math.Sin((float)rotation.Y - Geometry.DegreeToRadian(90f)) * movement;
           
            move(new Vector3(x, 0, z));
        }

        public void moveForward(float movement)
        {
            float z = (float)Math.Cos((float)rotation.Y) * movement;
            float x = (float)Math.Sin((float)rotation.Y) * movement;

            move(new Vector3(x,0,z));
        }

         public void rotateY(float angle)
        {
            this.rotation.Y += Geometry.DegreeToRadian(angle);
        }

        public abstract void move(Vector3 pos);
    }
}
