namespace AlumnoEjemplos.NeneMalloc.Lights.States
{
    public class FixedLight : LightState
    {
        public float Intensity { get; set; }

        public FixedLight(float intensity)
        {
            this.Intensity = intensity;
        }

        public void setRandom(float random)
        {
            
        }
    }
}
