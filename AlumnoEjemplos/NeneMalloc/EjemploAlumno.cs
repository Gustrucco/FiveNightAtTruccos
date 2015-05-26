using System.Collections.Generic;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
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
        TgcBox piso;
        TgcScene tgcScene;
        List<TgcBoundingBox> obstaculos;
        TgcSkeletalMesh personaje;
        List<Tgc3dSound> sounds;

        Tgc3dSound sound;
        Avatar avatar;
        Lantern lantern;

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
            string path = GuiController.Instance.AlumnoEjemplosDir;
            TgcSceneLoader loader = new TgcSceneLoader();
            tgcScene = loader.loadSceneFromFile(
               path + "AlumnoMedia\\pisoCompleto-TgcScene.xml",
               path + "AlumnoMedia\\");

            var musicPath = path + "AlumnoMedia\\NeneMalloc\\SonidosYMusica\\Eyes Wide Shut.mp3";

            //Cargar sonidos
            sounds = new List<Tgc3dSound>();

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

            /////////
            //var obstaculo = TgcBox.fromSize(
            //     new Vector3(160f, -88.5f, -100f),
            //     new Vector3(80, 80, 80),
            //     TgcTexture.createTexture(d3dDevice, GuiController.Instance.ExamplesMediaDir + "Texturas\\baldosaFacultad.jpg"));
            // obstaculos.Add(obstaculo.BoundingBox);

            // sound = new Tgc3dSound(path + "AlumnoMedia\\NeneMalloc\\SonidosYMusica\\risa de loco.wav", obstaculo.Position);
            // sound.MinDistance = 25f;
            //sounds.Add(sound);
            ///////////////

            CollitionManager.obstaculos = obstaculos;

           //Camara en primera persona, tipo videojuego FPS
           //GuiController.Instance.FpsCamera.Enable = true;
           //Configurar posicion y hacia donde se mira
           //GuiController.Instance.FpsCamera.setCamera(new Vector3(0, 0, -20), new Vector3(0, 0, 0));
       
            //Modifier para ver BoundingBox
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);
            GuiController.Instance.UserVars.addVar("isColliding");
            GuiController.Instance.UserVars.addVar("Pos");
            GuiController.Instance.UserVars.addVar("LastPos");
            GuiController.Instance.UserVars.addVar("Mesh renderizados");
            //Modifiers para desplazamiento del personaje
            GuiController.Instance.Modifiers.addFloat("VelocidadCaminar", 1f, 400f, 250f);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 1f, 360f, 120f);

            //Hacer que el Listener del sonido 3D siga al personaje
            GuiController.Instance.DirectSound.ListenerTracking = personaje;

            //Cargar musica
            GuiController.Instance.Mp3Player.FileName = musicPath;
            TgcMp3Player player = GuiController.Instance.Mp3Player;
            player.play(true);

            ////Ejecutar en loop los sonidos
            foreach (Tgc3dSound s in sounds)
            {
                s.play(true);
            }
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

            int count = 0;
            this.tgcScene.renderAll();
            GuiController.Instance.UserVars.setValue("Mesh renderizados", count);
            //Render personaje
            avatar.render(elapsedTime);
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            tgcScene.disposeAll();
            //avatar.dispose();
            foreach (Tgc3dSound sound in sounds)
            {
                sound.dispose();
            }
        }

    }
}
