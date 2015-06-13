using System.Collections.Generic;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.NeneMalloc;
using AlumnoEjemplos.NeneMalloc.Utils;
using Microsoft.DirectX.DirectInput;
using System.Windows.Forms;
using System.Drawing;

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
        TgcSkeletalMesh personaje;
        Avatar avatar;
        Lantern lantern;

        /// <summary>
        /// Categor�a a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el �rbol de la derecha de la pantalla.
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
        /// Completar con la descripci�n del TP
        /// </summary>
        public override string getDescription()
        {
            return "El objetivo del juego es sobrevivir a la noche de seguridad. No se puede golpear a los enemigos. Simplemente iluminarlos para espantarlos";
        }

        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            //GuiController.Instance: acceso principal a todas las herramientas del Framework
            
            //Device de DirectX para crear primitivas
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            string path = GuiController.Instance.AlumnoEjemplosMediaDir;
            TgcSceneLoader loader = new TgcSceneLoader();
            tgcScene = loader.loadSceneFromFile(
               path + "NeneMalloc\\Escenario-TgcScene.xml",
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
       


            //Modifier para ver BoundingBox
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);
            GuiController.Instance.Modifiers.addBoolean("showSceneBoundingBox", "SceneBouding Box", false);
            GuiController.Instance.UserVars.addVar("isColliding");
            GuiController.Instance.UserVars.addVar("Pos");
            GuiController.Instance.UserVars.addVar("Normal");
            GuiController.Instance.UserVars.addVar("Y");
            GuiController.Instance.UserVars.addVar("LastPos");
            GuiController.Instance.UserVars.addVar("Mesh renderizados");
            GuiController.Instance.UserVars.addVar("Velocidad Caida");
            GuiController.Instance.UserVars.addVar("Falling");
            GuiController.Instance.UserVars.addVar("MouseReleased");
            //Modifiers para desplazamiento del personaje
            GuiController.Instance.Modifiers.addFloat("VelocidadCaminar", 1f, 400f, 250f);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 1f, 360f, 120f);

        }


        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            //Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            
            //Obtener boolean para saber si hay que mostrar Bounding Box
           // bool showBB = (bool)GuiController.Instance.Modifiers.getValue("showBoundingBox");
          
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
            //Render personaje
            avatar.render(elapsedTime);
            

        }

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            tgcScene.disposeAll();
            //avatar.dispose();
        }

    }
}
