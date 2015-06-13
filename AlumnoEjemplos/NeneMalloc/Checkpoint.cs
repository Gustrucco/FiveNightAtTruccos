using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.NeneMalloc
{
    class Checkpoint
    {
        public Boolean Checked {get;set;}
        public List<Checkpoint> Neighborhoods {get;set;}
        public Vector3 Position { get; set;}
        public TgcArrow Arrow { get; set;}

        public Checkpoint(Vector3 aPosition)
        {
            this.Position = aPosition;
            this.Checked = false;
            this.Arrow = new TgcArrow();
            this.Arrow.Thickness = 15f;
            this.Arrow.PStart = aPosition;
            this.Arrow.PEnd = aPosition + new Vector3(0, 15, 0);
            this.Arrow.BodyColor = Color.Red;
            this.Arrow.HeadColor = Color.Red;
            this.Arrow.HeadSize = new Vector2(10, 10);
        }
        public void render()
        {
            Arrow.render();
        }
    }
}
