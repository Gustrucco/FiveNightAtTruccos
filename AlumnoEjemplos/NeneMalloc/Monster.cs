using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.NeneMalloc
{
    public class Monster : Character
    {
        public TgcSkeletalMesh mesh { get; set; }

        public Monster(Vector3 initialPos, Avatar avatar, string meshPath) : base(initialPos)
        {
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + meshPath,
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\",
                new string[] { 
		        GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\" + "Walk-TgcSkeletalAnim.xml"
	            });
            

            //Escalarlo porque es muy chico
            mesh.Scale = new Vector3(1.5f, 1.5f, 1.5f);

            //Rotarlo 180° porque esta mirando para el otro lado
            mesh.rotateY(Geometry.DegreeToRadian(180f));
            mesh.Position = this.Position + new Vector3(0f, -45f, 0f);
            this.Controller = new IAController(avatar);
            this.Controller.Character = this;
        }

        public override void RotateY(float angle)
        {
            this.Rotation = new Vector3(0f, angle, 0f);
            mesh.Rotation = this.Rotation;
        }

        public override void Move(Vector3 pos)
        {
            base.Move(pos);
            mesh.move(pos);
        }

        public virtual void Render()
        {
            mesh.playAnimation("Walk", true);
            mesh.animateAndRender();
        }
    }
}