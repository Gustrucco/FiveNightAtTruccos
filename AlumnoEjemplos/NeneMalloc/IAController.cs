using System.Linq;
using AlumnoEjemplos.NeneMalloc.Utils;
using System;
using Microsoft.DirectX;

namespace AlumnoEjemplos.NeneMalloc
{
    public class IAController : Controller
    {
        public Avatar Avatar { get; set; }
        public Vector3 Objective;
        public ChasingStrategy ChasingStrategy { get; set;}
        public bool HasObjective { get; set; }

        public IAController(Avatar avatar)
        {
            this.Avatar = avatar;         
        }

        public override void Update()
        {

            //Reviso si ya esta parado en su lugar

            if (this.HasObjective) 
            {
                if (CheckpointHelper.GetClosestCheckPoint(this.Character.Position).Neighbors.Contains(CheckpointHelper.GetClosestCheckPoint(this.Avatar.Position)))
                {
                    Objective = this.Avatar.Position;
                }
                else
                {
                    var closestCheckpoint = CheckpointHelper.GetClosestCheckPoint(this.Character.Position);
                    var avatarClosestCheckpoint = CheckpointHelper.GetClosestCheckPoint(this.Avatar.Position);
                    //Encontrar el algoritmo del camino más corto de un checkpoint al otro
                    var nextCheckpoint = closestCheckpoint.Neighbors.First(c => c.CanArriveTo(avatarClosestCheckpoint));
                    Objective = nextCheckpoint.Position;      
                }
            }

            Order = new Order();
            Vector3 vectorX = new Vector3(Objective.X - Character.Position.X, 0f, 0f);
            vectorX.Normalize();
            float rotationWithObjective = Convert.ToSingle(Math.Asin(vectorX.X));
            //Si el monstruo no esta mirando, rotamos lo mas que podemos
            if (Single.IsNaN(rotationWithObjective))
            {
                return;
            }
            if (!MathUtil.Equals(Character.Rotation.Y, rotationWithObjective))
            {
                Order.rotateY = rotationWithObjective - Character.Rotation.Y;
            }
            else
            {
                Order.rotateY = 0;
                Order.moveForward = 1;
            }          
        }
    }
}
