using System;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.NeneMalloc.Utils;
using TgcViewer.Utils.Input;
using System.Collections.Generic;
namespace AlumnoEjemplos.NeneMalloc
{
    class Avatar : Character
    {

        public TgcBoundingBox BoundingBox { get; set; }
        private bool falling = false;
        private TruccoFPSCamera Camera = new TruccoFPSCamera();
        private float velocidadCaida = 0f;


        public void init()
        {

            Device d3dDevice = GuiController.Instance.D3dDevice;
            //Carga del controller

            this.Controller = new Player();
            this.Position = new Vector3(160f, -88.5f, -340f);
            this.Rotation = new Vector3(0f, 0f, 0f);
            this.Controller.character = this;
            this.BoundingBox = new TgcBoundingBox();
            this.BoundingBox.setExtremes(-new Vector3(10f, 45f, 10f), new Vector3(10f, 20f, 10f));
            this.BoundingBox.transform(Matrix.Translation(this.Position));

            //Seteamos la camara
            this.Camera.Enable = true;
            this.Camera.setCamera(this.Position, this.Position + this.calculateNewPosition(1, this.Rotation));
        }

        public void update(float elapsedTime)
        {
            base.update();

            //obtener velocidades de Modifiers
            float velocidadCaminar = (float)GuiController.Instance.Modifiers.getValue("VelocidadCaminar");
            float velocidadRotacion = (float)GuiController.Instance.Modifiers.getValue("VelocidadRotacion");
            GuiController.Instance.UserVars.setValue("Pos", GuiController.Instance.ThirdPersonCamera.Position);
            Order lastOrders = this.Controller.order;

            //Si hubo rotacion
            if (lastOrders.rotating())
            {
                //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(lastOrders.rotateY * velocidadRotacion * elapsedTime);

                this.rotateY(lastOrders.rotateY * velocidadRotacion * elapsedTime);
                this.rotateX(lastOrders.rotateX * velocidadRotacion * elapsedTime);

                
            }




            //Si hubo desplazamiento
            if (lastOrders.moving())
            {

                //Activar animacion de caminando


                //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                Vector3 lastPos = this.Position;

                //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
                this.moveForward(lastOrders.moveForward * velocidadCaminar * elapsedTime);


                if (!lastOrders.running())
                {
                    this.moveAside(lastOrders.moveAside * velocidadCaminar * elapsedTime);
                }




                //CollitionManager es un Util que sirve para la logica de colisiones todo lo que sea respecto eso, desarrollarlo en esa clase
                if (CollitionManager.detectColision(this.BoundingBox))
                {
                    List<TgcBoundingBox> boundingBoxes = CollitionManager.getColisions(this.BoundingBox);
                    Vector3 collidedPosition = this.Position;
                    Vector3 normal = new Vector3(0, 0, 0);
                    this.move(lastPos - this.Position);

                    GuiController.Instance.UserVars.setValue("isColliding", true);
                    Boolean checkedStairs = false;
                    float max = -2000f;
                    //detecto colision contra escaleras primero
                    while (!checkedStairs)
                    {
                        float min = this.BoundingBox.PMin.Y;
                        this.BoundingBox.transform(Matrix.Translation(collidedPosition + new Vector3(0, 15, 0)));

                        if (boundingBoxes.Exists(b => !CollitionManager.isColliding(this.BoundingBox, b)))
                        {
                            GuiController.Instance.UserVars.setValue("isColliding", "Escalera");
                            TgcBoundingBox boundingColliding = boundingBoxes.Find(b => !CollitionManager.isColliding(this.BoundingBox, b));
                            if (max < boundingColliding.PMax.Y)
                            {
                                Vector3 upwards = new Vector3(0, Math.Abs(min - boundingColliding.PMax.Y) + 0.05f, 0);
                                this.move(upwards);
                                collidedPosition += upwards;
                                lastPos += upwards;
                                max = boundingColliding.PMax.Y;
                            }

                            boundingBoxes.Remove(boundingColliding);
                        }
                        if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)) || boundingBoxes.Count == 0)
                        {
                            GuiController.Instance.UserVars.setValue("isColliding", "Escalera chequeada");
                            checkedStairs = true;
                        }
                    }

                    if (boundingBoxes.Count == 0)
                    {
                        this.moveForward(lastOrders.moveForward * velocidadCaminar * elapsedTime);

                        if (!lastOrders.running())
                        {
                            this.moveAside(lastOrders.moveAside * velocidadCaminar * elapsedTime);
                        }
                    }
                    else
                    {
                        //Slide
                        this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(velocidadCaminar * elapsedTime, 0, 0)));
                        GuiController.Instance.UserVars.setValue("Normal", "CalculandoNormal");
                        if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                            normal += new Vector3(1, 0, 0) * -1;
                        }

                        this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(-velocidadCaminar * elapsedTime, 0, 0)));
                        if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                            normal += new Vector3(-1, 0, 0) * -1;
                        }

                        this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(0, 0, velocidadCaminar * elapsedTime)));
                        if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                            normal += new Vector3(0, 0, -1) * -1;
                        }

                        this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(0, 0, -velocidadCaminar * elapsedTime)));
                        if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)))
                        {
                            GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                            normal += new Vector3(0, 0, -1) * -1;
                        }
                        if (!normal.Equals(new Vector3(0, 0, 0)))
                        {
                            normal.Normalize();
                            GuiController.Instance.UserVars.setValue("Normal", "Calculando movimiento" + (Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.Position)), normal)));

                            if (!touchingSomething(Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.Position)), normal)))
                            {
                                GuiController.Instance.UserVars.setValue("Normal", "SeteandoNormal");
                                this.move(Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.Position)), normal));
                            }
                        }
                    }

                }
                else
                {
                    GuiController.Instance.UserVars.setValue("isColliding", false);
                }

            }

            //Si no se esta moviendo, activar animacion de Parado

            if (!this.touchingSomething(new Vector3(0, -0.1f, 0)))
            {
                GuiController.Instance.UserVars.setValue("isColliding", "flotando");
                if (!this.falling && this.touchingSomething(new Vector3(0, -15f, 0)))
                {

                    GuiController.Instance.UserVars.setValue("isColliding", "escaleraBajada");
                    float min = this.BoundingBox.PMin.Y;
                    this.BoundingBox.transform(Matrix.Translation(this.BoundingBox.Position + new Vector3(0, -15f, 0)));
                    List<TgcBoundingBox> boundingBoxes2 = CollitionManager.getColisions(this.BoundingBox);

                    //GuiController.Instance.UserVars.setValue("Y", boundingBoxes2.Count);
                    this.move(new Vector3(0, 0.05f - Math.Abs(min - boundingBoxes2.Find(b => CollitionManager.isColliding(this.BoundingBox, b)).PMax.Y), 0));
                    //GuiController.Instance.UserVars.setValue("Y", "bajo" + Math.Abs(this.BoundingBox.PMin.Y - boundingBoxes2.Find(b => CollitionManager.isColliding(this.BoundingBox, b)).PMax.Y)*-1);
                }
                else
                {
                    GuiController.Instance.UserVars.setValue("isColliding", "gravedad");
                    this.falling = true;
                    this.updateFallingSpeed(elapsedTime);
                    if (this.hitSomethingAtPath(new Vector3(0, -1f, 0), elapsedTime * velocidadCaida))
                    {
                        GuiController.Instance.UserVars.setValue("isColliding", "gravedadGround");
                        TgcBoundingBox boundingBoxResult;
                        CollitionManager.getClosestBoundingBox(new TgcRay(this.Position, new Vector3(0, 1f, 0)), out boundingBoxResult, this.BoundingBox);
                        this.move(new Vector3(0, -1f, 0) * Math.Abs(boundingBoxResult.PMax.Y - this.BoundingBox.PMin.Y));
                        this.falling = false;
                        this.velocidadCaida = 0f;
                    }
                    else
                    {
                        this.move(new Vector3(0, -1f, 0) * elapsedTime * velocidadCaida);
                    }
                    GuiController.Instance.UserVars.setValue("Velocidad Caida", this.velocidadCaida);
                }
                GuiController.Instance.UserVars.setValue("Falling", this.falling);
            }


        }
        public Boolean touchingSomething(Vector3 vector)
        {
            this.BoundingBox.transform(Matrix.Translation(this.Position + vector));
            Boolean result = CollitionManager.detectColision(this.BoundingBox);
            this.BoundingBox.transform(Matrix.Translation(this.Position));
            return result;
        }

        private void updateFallingSpeed(float elapsedTime)
        {
            this.velocidadCaida += 98f * elapsedTime;
        }

        private Boolean hitSomethingAtPath(Vector3 direction, float quantity)
        {
            TgcBoundingBox boundingBox;
            TgcRay rayCast = new TgcRay(this.Position, direction);

            return CollitionManager.getClosestBoundingBox(rayCast, out boundingBox, this.BoundingBox) && Vector3.Length(CollitionManager.getClosesPointBetween(rayCast, boundingBox) - this.Position) <= quantity;
        }

        public override void move(Vector3 pos)
        {
            this.Position += pos;
            this.Camera.setPosition(this.Position);
            this.BoundingBox.transform(Matrix.Translation(this.Position));
        }

        public override void render()
        {
            if ((bool)GuiController.Instance.Modifiers.getValue("showBoundingBox"))
            {
                this.BoundingBox.render();
            }
        }

        override public  void rotateX(float angle)
        {
            base.rotateX(angle);
            this.Camera.rotate(0, angle,0);
        }
        override public void rotateY(float angle)
        {
            base.rotateY(angle);
            this.Camera.rotate(angle, 0, 0);
        }
    }
}