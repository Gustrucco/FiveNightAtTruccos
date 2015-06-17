using System.Linq;
using AlumnoEjemplos.NeneMalloc.Utils;
using System;
using Microsoft.DirectX;

namespace AlumnoEjemplos.NeneMalloc
{
    public class IAController : Controller
    {
        public Avatar Avatar { get; set; }
        public Vector3 objective;
        public ChasingStrategy ChasingStrategy { get; set;}

        public IAController(Avatar avatar)
        {
            this.Avatar = avatar;         
        }

        public override void Update()
        {

            //Reviso si ya esta parado en su lugar

            if (objective == null) 
            {
                if (CheckpointHelper.GetClosestCheckPoint(this.Character.Position).Neighbors.Contains(CheckpointHelper.GetClosestCheckPoint(this.Avatar.Position)))
                {
                    objective = this.Avatar.Position;
                }
                else
                {
                    var closestCheckpoint = CheckpointHelper.GetClosestCheckPoint(this.Character.Position);
                    var avatarClosestCheckpoint = CheckpointHelper.GetClosestCheckPoint(this.Avatar.Position);
                    //Encontrar el algoritmo del camino más corto de un checkpoint al otro
                    var nextCheckpoint = closestCheckpoint.Neighbors.First(c => c.CanArriveTo(avatarClosestCheckpoint));
                    objective = nextCheckpoint.Position;      
                }
                    
           
            }
            Order = new Order();
            
            float rotationWithObjective = Convert.ToSingle(Math.Asin(objective.X - Character.Position.X));
            //Si el monstruo no esta mirando, rotamos lo mas que podemos
            if (!MathUtil.Equals(Character.Rotation.Y, rotationWithObjective))
            {
                Order.rotateY = rotationWithObjective - Character.Rotation.Y;
            }
            else
            {
                Order.moveForward = 1;
            }          
        }
    }
}
