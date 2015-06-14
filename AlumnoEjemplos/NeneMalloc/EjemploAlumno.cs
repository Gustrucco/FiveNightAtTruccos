using System.Collections.Generic;
using System.Linq;
using TgcViewer.Example;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.NeneMalloc;
using AlumnoEjemplos.NeneMalloc.Utils;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.DirectX;

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
               path + "NeneMalloc\\EscenarioCambios16-TgcScene.xml",
               path + "NeneMalloc\\");
           //Cargar personaje
            avatar = new Avatar();
            avatar.init();

            //Cargar linterna
            lantern = new Lantern();
            lantern.init();
            
            obstaculos = new List<TgcBoundingBox>();
            foreach (TgcMesh mesh in tgcScene.Meshes)
            {

                obstaculos.Add(mesh.BoundingBox);
            }
            
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
            GuiController.Instance.Modifiers.addFloat("VelocidadCaminar", 1f, 400f, 250f);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 1f, 360f, 120f);
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
            if (timeStart >= 0)
            {
                timeStart -= elapsedTime;
            }
            else
            {
                avatar.update(elapsedTime);
            }

            ArrowsClosesCheckPoint = CheckpointHelper.PrepareClosestCheckPoint(avatar.Position, ClosestCheckPoint, out ClosestCheckPoint);
            GuiController.Instance.UserVars.setValue("CheckPointPos", ClosestCheckPoint.Position);
            avatar.render();

            int count = 0;
            this.tgcScene.renderAll();
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
