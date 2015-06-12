using Microsoft.DirectX;

namespace AlumnoEjemplos.NeneMalloc.Lights
{
    public class Lantern : IluminationEntity
    {
        public Vector3 Direction { get; set; }
        public float SpotAngle { get; set; }
        public float SpotExponent { get; set; }
        public float Intensity { get; set; }
        public bool On { get; set; }

        public Lantern()
        {
            this.Direction = new Vector3(0, 0, 0.5f);
            this.SpotAngle = 40f;
            this.SpotExponent = 15f;
            this.Intensity = 25f;
        }

        public void ChangeLightOnOff()
        {
            this.On = !this.On;
        }
    }
}
