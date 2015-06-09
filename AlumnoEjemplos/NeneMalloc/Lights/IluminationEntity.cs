using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.NeneMalloc.Lights
{
    public abstract class IluminationEntity
    {
        public Vector3 Position { get; set; }
        public TgcTexture Texture { get; set; }
        public VertexBuffer vertexBuffer { get; set; }
        CustomVertex.PositionColoredTextured[] vertices;
        public Color Color { get; set; }
        public Effect Effect { get; set; }
        public string Technique { get; set; }
        public Vector3 Size { get; set; }
    
        public IluminationEntity()
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
        
        public IluminationEntity WithPosition(Vector3 position)
        {
            this.Position = position;
            return this;
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

    }
}
