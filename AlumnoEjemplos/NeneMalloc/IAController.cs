using System.Linq;
using AlumnoEjemplos.NeneMalloc.Utils;
using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectSound;

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

            var characterClosestCheckpoint = CheckpointHelper.GetClosestCheckPoint(this.Character.Position);
            var avatarClosestCheckpoint = CheckpointHelper.GetClosestCheckPoint(this.Avatar.Position);
            //if (characterClosestCheckpoint.Neighbors.Contains(avatarClosestCheckpoint) || avatarClosestCheckpoint == characterClosestCheckpoint)
            //{
            //    Objective = this.Avatar.Position;
            //}
            //else
            //{
                //Encontrar el algoritmo del camino más corto de un checkpoint al otro
                var nextCheckpoint = characterClosestCheckpoint.Neighbors.First(c => c.CanArriveTo(avatarClosestCheckpoint));
                Objective = avatarClosestCheckpoint.Position;      
            //}
            
            Order = new Order();
            this.Order.moveForward = 1;

            Vector3 dir = Objective - Character.Position;
            dir.Normalize();

            Vector3 rotation = new Vector3(0, (float)Math.Asin(dir.X), 0f);

            this.Character.RotateY(-Geometry.RadianToDegree(rotation.Y - this.Character.Rotation.Y));
            //this.Character.Move(this.Avatar.Position - this.Character.Position);
            //Vector3 vectorX = new Vector3(Objective.X - Character.Position.X, 0f, 0f);
            //vectorX.Normalize();
            //float rotationWithObjective = Convert.ToSingle(Math.Asin(vectorX.X));
            ////Si el monstruo no esta mirando, rotamos lo mas que podemos
            //if (Single.IsNaN(rotationWithObjective))
            //{
            //    return;
            //}
            //if (!MathUtil.Equals(Character.Rotation.Y, rotationWithObjective))
            //{
            //    Order.rotateY = rotationWithObjective - Character.Rotation.Y;
            //}
            //else
            //{
            //    Order.rotateY = 0;
            //    Order.moveForward = 1;
            //}          
        }
    }
}
