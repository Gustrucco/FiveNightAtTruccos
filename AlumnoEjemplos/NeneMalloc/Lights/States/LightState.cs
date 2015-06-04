namespace AlumnoEjemplos.NeneMalloc.Lights.States
{
    public interface LightState
    {
        float Intensity { get; set; }
        void setRandom(float random);
    }
}
