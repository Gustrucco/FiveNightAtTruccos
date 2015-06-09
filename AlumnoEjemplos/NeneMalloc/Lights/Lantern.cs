using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using TgcViewer;
using TgcViewer.Utils.Input;
using Device = Microsoft.DirectX.Direct3D.Device;

namespace AlumnoEjemplos.NeneMalloc.Lights
{
    public class Lantern : IluminationEntity
    {
        public Vector3 Direction { get; set; }
        public float SpotAngle { get; set; }
        public float SpotExponent { get; set; }
        public bool On { get; set; }

        public Lantern()
        {
            this.Direction = new Vector3(0, 0, 0.5f);
            this.SpotAngle = 50f;
            this.SpotExponent = 20f;
        }

        public void ChangeLightOnOff()
        {
            this.On = !this.On;
        }
    }
}
