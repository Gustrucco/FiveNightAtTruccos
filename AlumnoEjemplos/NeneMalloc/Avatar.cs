using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.NeneMalloc.Utils;
namespace AlumnoEjemplos.NeneMalloc
{
    class Avatar : Character
    {
        public TgcSkeletalMesh meshPersonaje;
        
        public void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            //Carga del controller
            this.controller = new Player();

            this.controller.character = this;

            //Carga del personaje (no es necesario debido a la modalidad FPS camera)
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            meshPersonaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Robot-TgcSkeletalMesh.xml",
                GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\",
                new string[] { 
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Caminando-TgcSkeletalAnim.xml",
                    GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\" + "Parado-TgcSkeletalAnim.xml",
                });

           // Le cambiamos la textura para diferenciarlo un poco
            meshPersonaje.changeDiffuseMaps(new TgcTexture[] { TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\Robot\\Textures\\" + "uvwGreen.jpg") });

            ////Configurar animacion inicial
            meshPersonaje.playAnimation("Parado", true);
            ////Escalarlo porque es muy grande
            meshPersonaje.Position = new Vector3(0, -45, 0);
            meshPersonaje.Scale = new Vector3(0.75f, 0.75f, 0.75f);
            ////Rotarlo 180° porque esta mirando para el otro lado
            meshPersonaje.rotateY(Geometry.DegreeToRadian(180f));
            this.rotateY(180f);

            //Seteamos la camara
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(meshPersonaje.Position, 200, -300);  
        }

        public void render(float elapsedTime)
        {
            base.render();

            //obtener velocidades de Modifiers
            float velocidadCaminar = (float)GuiController.Instance.Modifiers.getValue("VelocidadCaminar");
            float velocidadRotacion = (float)GuiController.Instance.Modifiers.getValue("VelocidadRotacion");

            Order lastOrders = this.controller.order;

            //Si hubo rotacion
            if (lastOrders.rotating())
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(lastOrders.rotateY * velocidadRotacion * elapsedTime);

                meshPersonaje.rotateY(rotAngle);
                //meshPersonaje.rotateX(Geometry.DegreeToRadian(lastOrders.rotateX * velocidadRotacion * elapsedTime));
                this.rotateY(lastOrders.rotateY * velocidadRotacion * elapsedTime);
                this.rotateX(lastOrders.rotateX * velocidadRotacion * elapsedTime);
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            //Si hubo desplazamiento
            if (lastOrders.moving())
            {
                //Activar animacion de caminando
                meshPersonaje.playAnimation("Caminando", true);

                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = meshPersonaje.Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
                this.moveForward(lastOrders.moveForward * velocidadCaminar * elapsedTime);

                if (!lastOrders.running()) 
                {
                    this.moveAside(lastOrders.moveAside * velocidadCaminar * elapsedTime);
                }                  
               
                //CollitionManager es un Util que sirve para la logica de colisiones todo lo que sea respecto eso, desarrollarlo en esa clase
                if (CollitionManager.detectColision(this.meshPersonaje.BoundingBox))
                {
                    meshPersonaje.Position = lastPos;
                }
            }

            //Si no se esta moviendo, activar animacion de Parado
            else
            {
                meshPersonaje.playAnimation("Parado", true);
            }
            bool showBB = (bool)GuiController.Instance.Modifiers.getValue("showBoundingBox");
            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = meshPersonaje.Position;

            meshPersonaje.animateAndRender();
            if (showBB)
            {
                meshPersonaje.BoundingBox.render();
            }
        }
        
        public override void move(Vector3 pos)
        {
            meshPersonaje.move(pos);
        }
    }
}
