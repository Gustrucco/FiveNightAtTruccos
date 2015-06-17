using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AlumnoEjemplos.NeneMalloc.Utils;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.NeneMalloc
{
    public abstract class Character
    {
        public Vector3 Rotation ;
        public bool Falling;
        public float VelocidadCaida;
        public TgcBoundingBox BoundingBox { get; set; }
        public Vector3 Position { get; set; }
        public Controller Controller { get; set; }
        public virtual float VelocidadCaminar { get; set; }
        public virtual float VelocidadRotacion { get; set; }

        protected Character(Vector3 initialPos)
        {
            this.Position = initialPos;
            this.BoundingBox = new TgcBoundingBox();
            this.BoundingBox.setExtremes(-new Vector3(10f, 45f, 10f), new Vector3(10f, 20f, 10f));
            this.BoundingBox.transform(Matrix.Translation(this.Position));
            this.Rotation = new Vector3(0f, 0f, 0f);
        }
        
        public void Update()
        {
            if(this.Controller!= null)
                this.Controller.Update();
        }

        public void MoveAside(float movement)
        {
            
            float z = (float)Math.Cos(Rotation.Y - Geometry.DegreeToRadian(-90f)) * movement;
            float x = (float)Math.Sin(Rotation.Y - Geometry.DegreeToRadian(-90f)) * movement;
           
            Move(new Vector3(x, 0 , z));
        }
        public void MoveForward(float movement)
        {
            Move(this.CalculateNewPosition(movement, this.Rotation));
        }

         public virtual void RotateY(float angle)
        {
            this.Rotation.Y += Geometry.DegreeToRadian(angle);
            this.Rotation.Y = this.Rotation.Y % Geometry.DegreeToRadian(360);
        }

         public virtual void RotateX(float angle)
         {
             this.Rotation.X += Geometry.DegreeToRadian(angle);
             this.Rotation.X = this.betWeenPosAndNeg(this.Rotation.X, Geometry.DegreeToRadian(90f));
         }

         public Vector3 CalculateNewPosition(float movement, Vector3 aRotation)
         {
             float Yz = (float)Math.Cos((float)aRotation.Y);
             float Yx = (float)Math.Sin((float)aRotation.Y);

             Vector3 normalY = new Vector3(Yx, 0 , Yz);
             normalY.Normalize();
             return normalY * movement;
         }

         private float betWeenPosAndNeg(float value, float range)
         {
             int sign = value >= 0 ? 1 : -1;
             return sign * Math.Min(Math.Abs(value), range);
         }

         public bool TouchingSomething(Vector3 vector)
         {
             this.BoundingBox.transform(Matrix.Translation(this.Position + vector));
             Boolean result = CollitionManager.detectColision(this.BoundingBox);
             this.BoundingBox.transform(Matrix.Translation(this.Position));
             return result;
         }

         protected void UpdateFallingSpeed(float elapsedTime)
         {
             this.VelocidadCaida += 98f * elapsedTime;
         }

         protected Boolean HitSomethingAtPath(Vector3 direction, float quantity)
         {
             TgcBoundingBox boundingBox;
             TgcRay rayCast = new TgcRay(this.Position, direction);

             return CollitionManager.getClosestBoundingBox(rayCast, out boundingBox, this.BoundingBox) && Vector3.Length(CollitionManager.getClosesPointBetween(rayCast, boundingBox) - this.Position) <= quantity;
         }

        public virtual void Move(Vector3 pos)
        {
            this.Position += pos;
            this.BoundingBox.transform(Matrix.Translation(this.Position));
        }

        public virtual void Render()
        {
            if ((bool)GuiController.Instance.Modifiers.getValue("showBoundingBox"))
            {
                this.BoundingBox.render();
            }
        }

        public virtual void Update(float elapsedTime)
        {
            this.Update();
            Order lastOrder = this.Controller.getLastOrder();
            if (lastOrder!= null)
            {
                //Si hubo rotacion
                if (lastOrder.rotating())
                {
                    //Rotar personaje y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware


                    this.RotateY(Math.Min(VelocidadRotacion * elapsedTime, Math.Abs(lastOrder.rotateY)) * Math.Sign(lastOrder.rotateY));
                    this.RotateX(Math.Min(VelocidadRotacion * elapsedTime, Math.Abs(lastOrder.rotateX)) * Math.Sign(lastOrder.rotateX));
                }

                //Si hubo desplazamiento
                if (lastOrder.moving())
                {
                    //Aplicar movimiento hacia adelante o atras segun la orientacion actual del Mesh
                    Vector3 lastPos = this.Position;

                    //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
                    //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
                    this.MoveForward(lastOrder.moveForward * VelocidadCaminar * elapsedTime);

                    if (!lastOrder.running())
                    {
                        this.MoveAside(lastOrder.moveAside * VelocidadCaminar * elapsedTime);
                    }

                    //CollitionManager es un Util que sirve para la logica de colisiones todo lo que sea respecto eso, desarrollarlo en esa clase
                    if (CollitionManager.detectColision(this.BoundingBox))
                    {
                        List<TgcBoundingBox> boundingBoxes = CollitionManager.getColisions(this.BoundingBox);
                        Vector3 collidedPosition = this.Position;
                        Vector3 normal = new Vector3(0, 0, 0);
                        this.Move(lastPos - this.Position);

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
                                    this.Move(upwards);
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
                            this.MoveForward(lastOrder.moveForward * VelocidadCaminar * elapsedTime);

                            if (!lastOrder.running())
                            {
                                this.MoveAside(lastOrder.moveAside * VelocidadCaminar * elapsedTime);
                            }
                        }
                        else
                        {
                            //Slide
                            this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(VelocidadCaminar * elapsedTime, 0, 0)));
                            GuiController.Instance.UserVars.setValue("Normal", "CalculandoNormal");
                            if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)))
                            {
                                GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                                normal += new Vector3(1, 0, 0) * -1;
                            }

                            this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(-VelocidadCaminar * elapsedTime, 0, 0)));
                            if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)))
                            {
                                GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                                normal += new Vector3(-1, 0, 0) * -1;
                            }

                            this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(0, 0, VelocidadCaminar * elapsedTime)));
                            if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)))
                            {
                                GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                                normal += new Vector3(0, 0, -1) * -1;
                            }

                            this.BoundingBox.transform(Matrix.Translation(lastPos + new Vector3(0, 0, -VelocidadCaminar * elapsedTime)));
                            if (boundingBoxes.Exists(b => CollitionManager.isColliding(this.BoundingBox, b)))
                            {
                                GuiController.Instance.UserVars.setValue("Normal", "HayNormal");
                                normal += new Vector3(0, 0, -1) * -1;
                            }
                            if (!normal.Equals(new Vector3(0, 0, 0)))
                            {
                                normal.Normalize();
                                GuiController.Instance.UserVars.setValue("Normal", "Calculando movimiento" + (Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.Position)), normal)));

                                if (!TouchingSomething(Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.Position)), normal)))
                                {
                                    GuiController.Instance.UserVars.setValue("Normal", "SeteandoNormal");
                                    this.Move(Vector3.Cross(Vector3.Cross(normal, (collidedPosition - this.Position)), normal));
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
                if (!this.TouchingSomething(new Vector3(0, -0.1f, 0)))
                {
                    GuiController.Instance.UserVars.setValue("isColliding", "flotando");
                    if (!this.Falling && this.TouchingSomething(new Vector3(0, -15f, 0)))
                    {

                        GuiController.Instance.UserVars.setValue("isColliding", "escaleraBajada");
                        float min = this.BoundingBox.PMin.Y;
                        this.BoundingBox.transform(Matrix.Translation(this.BoundingBox.Position + new Vector3(0, -15f, 0)));
                        List<TgcBoundingBox> boundingBoxes2 = CollitionManager.getColisions(this.BoundingBox);

                        //GuiController.Instance.UserVars.setValue("Y", boundingBoxes2.Count);
                        this.Move(new Vector3(0, 0.05f - Math.Abs(min - boundingBoxes2.Find(b => CollitionManager.isColliding(this.BoundingBox, b)).PMax.Y), 0));
                        //GuiController.Instance.UserVars.setValue("Y", "bajo" + Math.Abs(this.BoundingBox.PMin.Y - boundingBoxes2.Find(b => CollitionManager.isColliding(this.BoundingBox, b)).PMax.Y)*-1);
                    }
                    else
                    {
                        GuiController.Instance.UserVars.setValue("isColliding", "gravedad");
                        this.Falling = true;
                        this.UpdateFallingSpeed(elapsedTime);
                        if (this.HitSomethingAtPath(new Vector3(0, -1f, 0), elapsedTime * VelocidadCaida))
                        {
                            GuiController.Instance.UserVars.setValue("isColliding", "gravedadGround");
                            TgcBoundingBox boundingBoxResult;
                            CollitionManager.getClosestBoundingBox(new TgcRay(this.Position, new Vector3(0, 1f, 0)), out boundingBoxResult, this.BoundingBox);
                            this.Move(new Vector3(0, -1f, 0) * Math.Abs(boundingBoxResult.PMax.Y - this.BoundingBox.PMin.Y));
                            this.Falling = false;
                            this.VelocidadCaida = 0f;
                        }
                        else
                        {
                            this.Move(new Vector3(0, -1f, 0) * elapsedTime * VelocidadCaida);
                        }
                        GuiController.Instance.UserVars.setValue("Velocidad Caida", this.VelocidadCaida);
                    }
                    GuiController.Instance.UserVars.setValue("Falling", this.Falling);
                }
                if (lastOrder.printCheckPoint)
                {
                    Clipboard.SetText(Clipboard.GetText() + String.Format("new Checkpoint(new Vector3({0}, {1}, {2}));", this.Position.X, this.Position.Y, this.Position.Z));
                    CheckpointHelper.Add(new Checkpoint(this.Position));
                    //Clipboard.SetText("CheckPointPos", "CheckPoint" + checkPoint + ":" + this.Position,TextDataFormat.Text);
                }
            }
           
        }
    }
}
