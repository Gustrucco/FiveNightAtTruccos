
namespace AlumnoEjemplos.NeneMalloc.Lights.States
{
    public class IntermittentLight : LightState
    {
        public float Intensity { get; set; }

        public void setRandom(float random)
        {
            this.Intensity = random;
        }
    }
}
