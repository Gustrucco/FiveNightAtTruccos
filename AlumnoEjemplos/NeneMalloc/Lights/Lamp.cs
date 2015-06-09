using System.Drawing;
using AlumnoEjemplos.NeneMalloc.Lights.States;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NeneMalloc.Lights
{
    public class Lamp : IluminationEntity
    {
        public LightState State { get; set; }
        
        public static Lamp fromSize(Vector3 center, Vector3 size, Color color)
        {
            var lamp = new Lamp();
            lamp.setPositionSize(center, size);
            lamp.Color = color;
            return lamp;
        }
        
        public void render()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            TgcTexture.Manager texturesManager = GuiController.Instance.TexturesManager;

            //renderizar
            if (Texture != null)
            {
                texturesManager.shaderSet(Effect, "texDiffuseMap", Texture);
            }
            else
            {
                texturesManager.clear(0);
            }
            texturesManager.clear(1);


            d3dDevice.VertexDeclaration = GuiController.Instance.Shaders.VdecPositionColoredTextured;
            Effect.Technique = this.Technique;
            d3dDevice.SetStreamSource(0, vertexBuffer, 0);

            //Render con shader
            Effect.Begin(0);
            Effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, 12);
            Effect.EndPass();
            Effect.End();
        }

        public Lamp WithState(LightState state)
        {
            this.State = state;
            return this;
        }

        public float getIntensity()
        {
            return this.State.Intensity;
        }

        public void setRandom(float random)
        {
            this.State.setRandom(random);
        }
    }
}
