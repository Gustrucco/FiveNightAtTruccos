using System;

namespace AlumnoEjemplos.NeneMalloc
{
    public class Order
    {
        public int moveForward = 0;
        public int moveAside = 0; 
        public float rotateY = 0;
        public float rotateX = 0;
        public Boolean printCheckPoint = false;
        
        //public Vector3 getDirection()
        //{
        //    Vector3 direction = this.movement;
        //    direction.Normalize();
        //    return direction;
        //}
        //public Vector3 getFinalPosition()
        //{
        //    Vector3 lastPos = this.movement+ this.lastCoordinates;
        //    return lastPos;
        //}

        public Boolean moving()
        {
            return moveForward != 0 || moveAside != 0;
        }
        public Boolean running()
        {
            return Math.Abs(moveForward) >= 2;
        }

        public Boolean rotating()
        {
            return rotateX != 0 || rotateY != 0;
        }
    }

    
}
