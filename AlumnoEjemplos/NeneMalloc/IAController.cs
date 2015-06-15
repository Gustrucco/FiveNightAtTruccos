using System.Linq;
using AlumnoEjemplos.NeneMalloc.Utils;

namespace AlumnoEjemplos.NeneMalloc
{
    public class IAController : Controller
    {
        public Avatar Avatar { get; set; }

        public IAController(Avatar avatar)
        {
            this.Avatar = avatar;         
        }

        public override void Update()
        {
            var closestCheckpoint = CheckpointHelper.GetClosestCheckPoint(this.Character.Position);
            var avatarClosestCheckpoint = CheckpointHelper.GetClosestCheckPoint(this.Avatar.Position);
            //Encontrar el algoritmo del camino más corto de un checkpoint al otro
            var nextCheckpoint = closestCheckpoint.Neighbors.First( c => c.CanArriveTo(avatarClosestCheckpoint));
            //Set CheckPointObjetivo
            //this.Character.MoveTo(nextCheckpoint.Position);
        }
    }
}
