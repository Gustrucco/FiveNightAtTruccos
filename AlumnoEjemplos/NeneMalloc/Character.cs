using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;

namespace AlumnoEjemplos.NeneMalloc
{
    public abstract class Character
    {
        public Vector3 Rotation ;

        public Vector3 Position {get;set;}

        public Controller Controller { get; set; }

        public void render()
        {
            if(this.Controller!= null)
            this.Controller.render();
        }
        public void moveAside(float movement)
        {
            
            float z = (float)Math.Cos((float)Rotation.Y - Geometry.DegreeToRadian(-90f)) * movement;
            float x = (float)Math.Sin((float)Rotation.Y - Geometry.DegreeToRadian(-90f)) * movement;
           
            move(new Vector3(x, 0 , z));
        }
        public void moveForward(float movement)
        {
            move(this.calculateNewPosition(movement, this.Rotation));
        }

         public void rotateY(float angle)
        {
            this.Rotation.Y += Geometry.DegreeToRadian(angle);
            this.Rotation.Y = this.Rotation.Y % Geometry.DegreeToRadian(360);
        }

         public void rotateX(float angle)
         {
             this.Rotation.X += Geometry.DegreeToRadian(angle);
             this.Rotation.X = this.betWeenPosAndNeg(this.Rotation.X, Geometry.DegreeToRadian(90f));
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
