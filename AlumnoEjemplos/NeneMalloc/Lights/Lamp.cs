using System.Drawing;
using AlumnoEjemplos.NeneMalloc.Lights.States;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NeneMalloc.Lights
{
    public class Lamp
    {
        public Color Color { get; set; }
        public Vector3 Size { get; set; }
        public Vector3 Position { get; set; }
        public Effect Effect { get; set; }
        public TgcTexture Texture { get; set; }
        public string Technique { get; set; }
        public LightState State { get; set; }

        CustomVertex.PositionColoredTextured[] vertices;

        readonly VertexBuffer vertexBuffer;

        public Lamp()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            vertices = new CustomVertex.PositionColoredTextured[36];
            vertexBuffer = new VertexBuffer(typeof(CustomVertex.PositionColoredTextured), vertices.Length, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColoredTextured.Format, Pool.Default);

            this.Position = new Vector3(0,0,0);
            this.Color = Color.Transparent;
            this.setPositionSize(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            
            //Shader
            this.Effect = GuiController.Instance.Shaders.VariosShader;
            this.Technique = TgcShaders.T_POSITION_COLORED;
        }

        public static Lamp fromSize(Vector3 center, Vector3 size, Color color)
        {
            var lamp = new Lamp();
            lamp.setPositionSize(center, size);
            lamp.Color = color;
            return lamp;
        }

        /// <summary>
        /// Configurar valores de posicion y tamaño en forma conjunta
        /// </summary>
        /// <param name="position">Centro de la lampara</param>
        /// <param name="size">Tamaño de la lampara</param>
        public void setPositionSize(Vector3 position, Vector3 size)
        {
            this.Position = position;
            this.Size = size;
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

        /// <summary>
        /// Liberar los recursos de la cja
        /// </summary>
        public void dispose()
        {
            if (this.Texture != null)
            {
                this.Texture.dispose();
            }
            if (vertexBuffer != null && !vertexBuffer.Disposed)
            {
                vertexBuffer.Dispose();
            }
        }

        public Lamp WithState(LightState state)
        {
            this.State = state;
            return this;
        }

        public float getIntensity(float random)
        {
            return this.State.getIntensity(random);
        }

        public Lamp WithPosition(Vector3 position)
        {
            this.Position = position;
            return this;
        }
    }
}
