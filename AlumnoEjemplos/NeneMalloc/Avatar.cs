using System;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.NeneMalloc.Utils;
using System.Collections.Generic;
using System.Windows.Forms;
namespace AlumnoEjemplos.NeneMalloc
{
    class Avatar : Character
    {

        private TruccoFPSCamera Camera = new TruccoFPSCamera();
        private int checkPoint = 0;

        public override float VelocidadCaminar
        {
            get { return 100f; }
        }
        public override float VelocidadRotacion
        {
            get { return 120f; }
        }

        public Avatar() : base(new Vector3(160f, -88.5f, -340f))
        {
            //Carga del controller
            this.Controller = new Player();
            this.Controller.character = this;

            //Seteamos la camara
            this.Camera.Enable = true;
            this.Camera.setCamera(this.Position, this.Position + this.CalculateNewPosition(1, this.Rotation));
        }
        public override void RotateX(float angle)
        {
            base.RotateX(angle);
            this.Camera.rotate(0f, angle, 0f);

        }
        public override void RotateY(float angle)
        {
            base.RotateY(angle);
            this.Camera.rotate( angle,0f,0f);
        }
        public override void Move(Vector3 pos)
        {
            base.Move(pos);
            this.Camera.setPosition(this.Position );
        }

    }
}
