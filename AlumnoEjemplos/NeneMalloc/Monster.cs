using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.NeneMalloc
{
    public class Monster : Character
    {
        private TgcSkeletalMesh mesh { get; set; }

        public Monster(Vector3 initialPos, Avatar avatar) : base(initialPos)
        {
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Robot-TgcSkeletalMesh.xml",
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\",
                new string[] { 
		        GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Caminando-TgcSkeletalAnim.xml"
	            });
            

            //Escalarlo porque es muy grande
            mesh.Scale = new Vector3(0.50f, 0.50f, 0.50f);
            //Rotarlo 180° porque esta mirando para el otro lado
            mesh.rotateY(Geometry.DegreeToRadian(180f));
            mesh.Position = this.Position + new Vector3(0f, -45f, 0f);
            this.Controller = new IAController(avatar);
            this.Controller.Character = this;
        }

        public override void RotateY(float angle)
        {
            base.RotateY(angle);
            mesh.rotateY(angle);
        }

        public override void Move(Vector3 pos)
        {
            base.Move(pos);
            mesh.move(pos);
        }

        public override void Render()
        {
            mesh.playAnimation("Caminando", true);
            mesh.animateAndRender();
        }
    }
}