using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.DirectX;
using TgcViewer.Example;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.NeneMalloc;
using AlumnoEjemplos.NeneMalloc.Utils;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.MiGrupo
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        TgcBox piso;
        TgcScene tgcScene;
        List<TgcBoundingBox> obstaculos;
        Avatar avatar;
        Lantern lantern;
        float timeStart = 5f;
        List<TgcArrow> ArrowsClosesCheckPoint;
        Checkpoint ClosestCheckPoint;
        List<Monster> Monsters;

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
            Cursor.Hide();
            Cursor.Position = new Point(GuiController.Instance.FullScreenPanel.Width / 2, GuiController.Instance.FullScreenPanel.Height / 2);
            Clipboard.Clear();
            //Device de DirectX para crear primitivas
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            string path = GuiController.Instance.AlumnoEjemplosMediaDir;
            TgcSceneLoader loader = new TgcSceneLoader();
            tgcScene = loader.loadSceneFromFile(
               path + "NeneMalloc\\EscenarioFaltaIntensivas-TgcScene.xml",
               path + "NeneMalloc\\");
           //Cargar personaje
            avatar = new Avatar();

            //Cargar linterna
            lantern = new Lantern();
            lantern.init();
            
            obstaculos = new List<TgcBoundingBox>();
            foreach (TgcMesh mesh in tgcScene.Meshes)
            {

                obstaculos.Add(mesh.BoundingBox);
            }

            //Cargar los enemigos
            //Monsters = new List<Monster>();

            //var monster = new Monster(new Vector3(140.3071f, -91.425f, 246.465f), avatar);
            //Monsters.Add(monster);
            
            CollitionManager.obstaculos = obstaculos;
           //Camara en primera persona, tipo videojuego FPS
           //GuiController.Instance.FpsCamera.Enable = true;
           //Configurar posicion y hacia donde se mira
           //GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));


            CheckpointHelper.BuildCheckpoints();
            CheckpointHelper.GenerateGraph();
            //CheckpointHelper.add(new Checkpoint(new Vector3(140.3071f, -91.425f, 246.465f)), Floor.GroundFloor);

            //Modifier para ver BoundingBox
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);
            GuiController.Instance.Modifiers.addBoolean("showSceneBoundingBox", "SceneBouding Box", false);
            GuiController.Instance.UserVars.addVar("isColliding");
            GuiController.Instance.UserVars.addVar("Pos");
            GuiController.Instance.UserVars.addVar("Normal");
            GuiController.Instance.UserVars.addVar("Y");
            GuiController.Instance.UserVars.addVar("LastPos");
            GuiController.Instance.UserVars.addVar("Mesh renderizados");
            GuiController.Instance.UserVars.addVar("Checkpoints");
            GuiController.Instance.UserVars.addVar("Velocidad Caida");
            GuiController.Instance.UserVars.addVar("Falling");
            GuiController.Instance.UserVars.addVar("MouseReleased");
            GuiController.Instance.UserVars.addVar("CheckPointPos");
            //Modifiers para desplazamiento del personaje
            GuiController.Instance.Modifiers.addEnum("PisoCheckPoint",typeof(Floor),Floor.GroundFloor);

        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            //Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
            //Obtener boolean para saber si hay que mostrar Bounding Box
           // bool showBB = (bool)GuiController.Instance.Modifiers.getValue("showBoundingBox");
            var meshes = new List<TgcMesh>();
            if (timeStart >= 0)
            {
                timeStart -= elapsedTime;
            }
            else
            {
                avatar.Update(elapsedTime);
            }

            //foreach (var monster in Monsters)
            //{
            //    monster.Update(elapsedTime);
            //    monster.Render();
            //    GuiController.Instance.UserVars.setValue("Pos", monster.Position);
            //}

            //Analizar cada malla contra el Frustum - con fuerza bruta
            TgcFrustum frustum = GuiController.Instance.Frustum;
            foreach (TgcMesh mesh in tgcScene.Meshes)
            {
                //Nos ocupamos solo de las mallas habilitadas
                if (mesh.Enabled)
                {
                    //Solo mostrar la malla si colisiona contra el Frustum
                    TgcCollisionUtils.FrustumResult r = TgcCollisionUtils.classifyFrustumAABB(frustum, mesh.BoundingBox);
                    if (r != TgcCollisionUtils.FrustumResult.OUTSIDE)
                    {
                        
                        meshes.Add(mesh);
                    }
                }
            }

            ArrowsClosesCheckPoint = CheckpointHelper.PrepareClosestCheckPoint(avatar.Position, ClosestCheckPoint, out ClosestCheckPoint);
            GuiController.Instance.UserVars.setValue("CheckPointPos", "Pos:"+ ClosestCheckPoint.Position.ToString() +"/"+ ClosestCheckPoint.id);
            avatar.Render();

            int count = 0;
            foreach (var tgcMesh in meshes)
            {
                tgcMesh.render();
            }
            bool showBB = (bool)GuiController.Instance.Modifiers.getValue("showSceneBoundingBox");
            
            if (showBB)
            {    
                foreach (TgcMesh mesh in this.tgcScene.Meshes)
	            {
                    mesh.BoundingBox.render();
	            }
            }

            GuiController.Instance.UserVars.setValue("Mesh renderizados", count);
            GuiController.Instance.UserVars.setValue("Checkpoints", CheckpointHelper.CheckPoints.Sum( c => c.Value.Count));
            //Render personaje
            ArrowsClosesCheckPoint.ForEach(a => a.render());

            if (GuiController.Instance.D3dInput.keyDown(Key.Space))
            {
                var texto = new TgcText2d();
                texto.Text = ClosestCheckPoint.id.ToString();
                texto.Position = new Point(300, 100);
                texto.Size = new Size(300, 100);
                texto.render();
            }
             

            CheckpointHelper.renderAll();
            
            

        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        
        public override void close()
        {
            tgcScene.disposeAll();
            //avatar.dispose();
        }

    }
}
