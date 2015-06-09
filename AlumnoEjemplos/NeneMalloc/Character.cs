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
            
            float z = (float)Math.Cos((float)rotation.Y - Geometry.DegreeToRadian(-90f)) * movement;
            float x = (float)Math.Sin((float)rotation.Y - Geometry.DegreeToRadian(-90f)) * movement;
           
            move(new Vector3(x, 0 , z));
        }
        public void moveForward(float movement)
        {
            move(this.calculateNewPosition(movement, this.rotation));
        }

         public void rotateY(float angle)
        {
            this.rotation.Y += Geometry.DegreeToRadian(angle);
            this.rotation.Y = this.rotation.Y % 360;
        }

         public void rotateX(float angle)
         {
             this.rotation.X += Geometry.DegreeToRadian(angle);
             this.rotation.X = this.betWeenPosAndNeg(this.rotation.X, 90f);
         }

         public Vector3 calculateNewPosition(float movement, Vector3 aRotation)
         {
             float Yz = (float)Math.Cos((float)aRotation.Y);
             float Yx = (float)Math.Sin((float)aRotation.Y);

             Vector3 normalY = new Vector3(Yx, 0 , Yz);
             normalY.Normalize();
             return normalY * movement;
         }

         private float betWeenPosAndNeg(float value, float range)
         {
             int sign = value >= 0 ? 1 : -1;
             return sign * Math.Min(Math.Abs(value), range);
         }
        public abstract void move(Vector3 pos);
    }
}
