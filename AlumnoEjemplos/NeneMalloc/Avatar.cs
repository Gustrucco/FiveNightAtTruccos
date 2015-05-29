using System;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.NeneMalloc.Utils;
using TgcViewer.Utils.Input;
namespace AlumnoEjemplos.NeneMalloc
{
    class Avatar : Character
    {
        public TgcSkeletalMesh meshPersonaje;
        public TgcBoundingBox BoundingBox { get; set; }
        
        public void init()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            //Carga del controller
            this.controller = new Player();
            this.position = new Vector3(160f, -88.5f, -340f);
            this.controller.character = this;
            this.BoundingBox = new TgcBoundingBox();
            this.BoundingBox.setExtremes( - new Vector3(10f,45f, 10f), new Vector3(10f, 20f, 10f));
            this.BoundingBox.transform(Matrix.Translation(this.position));
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
            meshPersonaje.Position = this.position + new Vector3(0f, -45f, 0f);
            

            meshPersonaje.Scale = new Vector3(0.50f, 0.50f, 0.50f);
            ////Rotarlo 180° porque esta mirando para el otro lado
            meshPersonaje.rotateY(Geometry.DegreeToRadian(180f));
            

            //Seteamos la camara
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(this.position, 40, -100);
        }

        public void render(float elapsedTime)
        {
            base.render();

            //obtener velocidades de Modifiers
            float velocidadCaminar = (float)GuiController.Instance.Modifiers.getValue("VelocidadCaminar");
            float velocidadRotacion = (float)GuiController.Instance.Modifiers.getValue("VelocidadRotacion");
            GuiController.Instance.UserVars.setValue("Pos", GuiController.Instance.ThirdPersonCamera.Position);
            Order lastOrders = this.controller.order;

            //Si hubo rotacion
            if (lastOrders.rotating())
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(lastOrders.rotateY * velocidadRotacion * elapsedTime);
                
                
                meshPersonaje.rotateY(rotAngle);
                meshPersonaje.rotateX(Geometry.DegreeToRadian(lastOrders.rotateX * velocidadRotacion * elapsedTime));

                this.rotateY(lastOrders.rotateY * velocidadRotacion * elapsedTime);
                this.rotateX(lastOrders.rotateX * velocidadRotacion * elapsedTime);
                //this.BoundingBox.transform(Matrix.RotationY(this.rotation.Y)* Matrix.Translation(this.position));
                
                
                GuiController.Instance.ThirdPersonCamera.rotateY(rotAngle);
            }

            //Si hubo desplazamiento
            if (lastOrders.moving())
            {
                //Activar animacion de caminando
                meshPersonaje.playAnimation("Caminando", true);

                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = this.position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
                this.moveForward(lastOrders.moveForward * velocidadCaminar * elapsedTime);
                

                if (!lastOrders.running()) 
                {
                    this.moveAside(lastOrders.moveAside * velocidadCaminar * elapsedTime);
                    //this.BoundingBox.transform(Matrix.RotationY((float)rotation.Y - Geometry.DegreeToRadian(90f)) * Matrix.Translation(this.calculateNewPosition(lastOrders.moveForward * velocidadCaminar * elapsedTime, this.rotation)) * Matrix.RotationY((float)rotation.Y + Geometry.DegreeToRadian(90f)));
                  
                }
                this.BoundingBox.transform(Matrix.Translation(this.position));
    
                GuiController.Instance.UserVars.setValue("LastPos", lastPos);
                //GuiController.Instance.UserVars.setValue("Pos", this.position);
                
               //CollitionManager es un Util que sirve para la logica de colisiones todo lo que sea respecto eso, desarrollarlo en esa clase
                if (CollitionManager.detectColision(this.BoundingBox))
                {
                    Vector3 collidedPosition = this.position;
                    Vector3 normal = new Vector3(0,0,0);
                    this.position = lastPos;
                    this.meshPersonaje.Position = lastPos + new Vector3(0, -45, 0);
                    this.BoundingBox.transform(Matrix.Translation(lastPos));
                    GuiController.Instance.UserVars.setValue("isColliding", true);

                   //detecto colision contra escaleras primero
                    this.BoundingBox.transform(Matrix.Translation(collidedPosition + new Vector3(0, 15, 0)));
                    if (!CollitionManager.detectColision(this.BoundingBox))
                    {
                        this.move(new Vector3(0, 15, 0));
                    }
                    else
                    {
                        this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(1, 0, 0)));
                        GuiController.Instance.UserVars.setValue("Normal", "CalculandoNormal");
                        if (CollitionManager.detectColision(this.BoundingBox))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                            normal = new Vector3(1, 0, 0) * -1;
                        }

                        this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(-1, 0, 0)));
                        if (CollitionManager.detectColision(this.BoundingBox))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                            normal = new Vector3(-1, 0, 0) * -1;
                        }

                        this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(0, 0, 1)));
                        if (CollitionManager.detectColision(this.BoundingBox))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                            normal = new Vector3(0, 0, 1) * -1;
                        }

                        this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(0, 0, -1)));
                        if (CollitionManager.detectColision(this.BoundingBox))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                            normal = new Vector3(0, 0, -1) * -1;
                        }
                        if (!normal.Equals(new Vector3(0, 0, 0)))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "Calculando movimiento" + (Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.position)), normal)));

                            this.BoundingBox.transform(Matrix.Translation(this.position + Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.position)), normal)));
                            if (!CollitionManager.detectColision(this.BoundingBox))
                            {
                                GuiController.Instance.UserVars.setValue("Normal", "SeteandoNormal");
                                this.move(Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.position)), normal));
                            }
                        }
                    }    
                }
                else
                {
                    GuiController.Instance.UserVars.setValue("isColliding", false);
                }
                TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
               

            }

            //Si no se esta moviendo, activar animacion de Parado
            else
            {
                meshPersonaje.playAnimation("Parado", true);
            }

            bool showBB = (bool)GuiController.Instance.Modifiers.getValue("showBoundingBox");
            //Hacer que la camara siga al personaje en su nueva posicion
            GuiController.Instance.ThirdPersonCamera.Target = this.position;

            meshPersonaje.animateAndRender();
            if (showBB)
            {
                this.BoundingBox.render();
            }
        }
        
        public override void move(Vector3 pos)
        {
            this.meshPersonaje.move(pos);
            this.position += pos;
            
        }
    }
}
