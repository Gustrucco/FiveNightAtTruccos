using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX;
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
        Avatar avatar;
        Lantern lantern;
        Lamp lamp;

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
            string path = GuiController.Instance.AlumnoEjemplosMediaDir;
            TgcSceneLoader loader = new TgcSceneLoader();
            tgcScene = loader.loadSceneFromFile(
               path + "NeneMalloc\\pisoCompleto-TgcScene.xml",
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

            //Mesh para la luz
            lamp = Lamp.fromSize(new Vector3(0, 0, 0), Color.Transparent);
            
            //Setear posición de la luz
            Vector3 lightPos = avatar.position;
            lamp.Position = lightPos;

            //Modifier para ver BoundingBox
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);
            GuiController.Instance.UserVars.addVar("isColliding");
            GuiController.Instance.UserVars.addVar("Pos");
            GuiController.Instance.UserVars.addVar("Normal");
            GuiController.Instance.UserVars.addVar("LastPos");
            GuiController.Instance.UserVars.addVar("Mesh renderizados");
            
            //Modifiers para desplazamiento del personaje
            GuiController.Instance.Modifiers.addFloat("VelocidadCaminar", 1f, 400f, 250f);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 1f, 360f, 120f);
        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            bool lightEnable;

            var random = new Random().Next(100);

            if (random < 40)
                lightEnable =  true;
            else
                lightEnable = false;
           
            Effect currentShader;
            Effect currentAvatarShader;

            if (lightEnable)
            {
                //Con luz: Cambiar el shader actual por el shader default que trae el framework para iluminacion dinamica con PointLight
                currentShader = GuiController.Instance.Shaders.TgcMeshPointLightShader;
                currentAvatarShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;
            }
            else
            {
                //Sin luz: Restaurar shader default
                currentShader = GuiController.Instance.Shaders.TgcMeshShader;
                currentAvatarShader = GuiController.Instance.Shaders.TgcSkeletalMeshShader;
            }
            
            this.tgcScene.renderAll();

            foreach (TgcMesh mesh in tgcScene.Meshes)
            {
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);
            }
            
            //Render personaje
            avatar.meshPersonaje.Effect = currentAvatarShader;
            avatar.meshPersonaje.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(avatar.meshPersonaje.RenderType);

            avatar.render(elapsedTime);
            lamp.render();
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            tgcScene.disposeAll();
            lamp.dispose();
            //avatar.dispose();
        }

    }
}
