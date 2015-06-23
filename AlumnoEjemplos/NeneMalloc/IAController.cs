using System.Linq;
using AlumnoEjemplos.NeneMalloc.Utils;
using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;

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
            if (characterClosestCheckpoint.Neighbors.Contains(avatarClosestCheckpoint) || avatarClosestCheckpoint == characterClosestCheckpoint)
            {
                Objective = this.Avatar.Position;
            }
            else
            {
                //Encontrar el algoritmo del camino más corto de un checkpoint al otro
                var nextCheckpoint = characterClosestCheckpoint.Neighbors.First(c => c.CanArriveTo(avatarClosestCheckpoint));
                Objective = nextCheckpoint.Position;      
            }

            Order = new Order();

            Vector3 dir = Objective - Character.Position;

            dir = new Vector3(dir.X, 0f, dir.Z);

            Vector3 rotation = new Vector3(0f, (float)Math.Asin(dir.X / dir.Length()), 0f);
            var angleRadians = -Convert.ToSingle(Math.Atan2(dir.X, dir.Z));
            GuiController.Instance.UserVars.setValue("angulo", Geometry.RadianToDegree(angleRadians - this.Character.Rotation.Y));
            Order.rotateY = (Geometry.RadianToDegree(angleRadians - this.Character.Rotation.Y));

            if (MathUtil.Equals(Order.rotateY,0, 0.5f ))
            {
                this.Order.moveForward = 1;
            }
        }
    }
}
