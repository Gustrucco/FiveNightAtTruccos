using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;

namespace AlumnoEjemplos.NeneMalloc
{
    public abstract class Character
    {
        public Vector3 rotation ;

        public Vector3 position {get;set;}

        public Controller controller { get; set; }

        public void render()
        {
            this.controller.render();
        }
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

         public void rotateX(float angle)
         {
             this.rotation.X += Geometry.DegreeToRadian(angle);
         }

        public abstract void move(Vector3 pos);
    }
}
