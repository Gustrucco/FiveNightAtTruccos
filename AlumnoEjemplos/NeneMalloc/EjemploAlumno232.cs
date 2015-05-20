using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.NeneMalloc;
using AlumnoEjemplos.NeneMalloc.Utils;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        List<TgcBox> obstaculos;
        Avatar avatar;
        TgcScene tgcScene;
        TgcBox boxCamara;
        float velocidadCaminar;
        float velocidadRotacion;
        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "Grupo 10";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "El objetivo del juego es sobrevivir a la noche de seguridad. No se puede golpear a los enemigos. Simplemente iluminarlos para espantarlos";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            //Device de DirectX para crear primitivas
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            String path = GuiController.Instance.AlumnoEjemplosDir;
            TgcSceneLoader loader = new TgcSceneLoader();
            tgcScene = loader.loadSceneFromFile(
               path + "AlumnoMedia\\pisoCompleto2-TgcScene.xml",
               path + "AlumnoMedia\\");

            boxCamara = TgcBox.fromSize(new Vector3(357f, -128.5f, -356f), new Vector3(15, 15, 15), Color.White);

            //UserVar para informar si hay choque o no
            GuiController.Instance.UserVars.addVar("Hay choque");
            GuiController.Instance.FpsCamera.Enable = true;
            GuiController.Instance.FpsCamera.setCamera(boxCamara.Position, new Vector3(-140f, 40f, -120f));
            GuiController.Instance.FpsCamera.MovementSpeed = 200f;
            

            //FPS Camara
            //GuiController.Instance.FpsCamera.Enable = true;
            //GuiController.Instance.FpsCamera.setCamera(boxCamara.Position, new Vector3(-140f, 40f, -120f));
            //GuiController.Instance.FpsCamera.MovementSpeed = 200f;
            //GuiController.Instance.FpsCamera.JumpSpeed = 200f;

           //Camara en primera persona, tipo videojuego FPS
           //GuiController.Instance.FpsCamera.Enable = true;
           //Configurar posicion y hacia donde se mira
           //GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));

            velocidadCaminar = 200f;
            velocidadRotacion = 5f;
        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;



            //Calcular proxima posicion de personaje segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;

            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = -velocidadCaminar;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                moveForward = velocidadCaminar;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                rotate = velocidadRotacion;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                rotate = -velocidadRotacion;
                rotating = true;
            }

            Vector3 lastPos = boxCamara.Position;

            //La velocidad de movimiento tiene que multiplicarse por el elapsedTime para hacerse independiente de la velocida de CPU
            //Ver Unidad 2: Ciclo acoplado vs ciclo desacoplado
            boxCamara.moveOrientedY(moveForward * elapsedTime);

            if (moving)
            {
                bool collide = false;
                foreach (TgcMesh malla in tgcScene.Meshes)
                {
                    TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(boxCamara.BoundingBox, malla.BoundingBox);
                    if (result == TgcCollisionUtils.BoxBoxResult.Adentro || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
                    {
                        collide = true;
                        break;
                    }

                }
            
            if (collide)
            {
                //personaje.Position = lastPos;
                GuiController.Instance.UserVars.setValue("Hay choque", collide);
                boxCamara.Position = lastPos;

            }
            else {
                GuiController.Instance.UserVars.setValue("Hay choque", collide);
            }
            }
      

            boxCamara.render();
            tgcScene.renderAll();

           
            //Render piso
           // piso.render();

            //Render obstaculos
            ///foreach (TgcBox obstaculo in obstaculos)
            //{
            //    obstaculo.render();
            //    if (showBB)
            //    {
            //        obstaculo.BoundingBox.render();
            //    }
                
            //}
            
            //Render personaje
            //avatar.render(elapsedTime);
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            boxCamara.dispose();
            tgcScene.disposeAll();
            //piso.dispose();
            //foreach (TgcBox obstaculo in obstaculos)
            //{
            //    obstaculo.dispose();
            //}
            //personaje.dispose();
        }

    }
}
