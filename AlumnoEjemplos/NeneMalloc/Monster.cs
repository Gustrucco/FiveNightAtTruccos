using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcSkeletalAnimation;

namespace AlumnoEjemplos.NeneMalloc
{
    public class Monster : Character
    {
        private TgcSkeletalMesh mesh { get; set; }

        public Monster(Vector3 initialPos) : base(initialPos)
        {
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Robot-TgcSkeletalMesh.xml",
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\",
                new string[] { 
		    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Caminando-TgcSkeletalAnim.xml",
		    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Parado-TgcSkeletalAnim.xml",
	        });
        }
        public override void Render()
        {
            mesh.render();
        }
    }
}
