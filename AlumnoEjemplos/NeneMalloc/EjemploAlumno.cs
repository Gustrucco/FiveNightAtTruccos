using System;
using System.Collections.Generic;
using System.Drawing;
using AlumnoEjemplos.NeneMalloc.Lights;
using AlumnoEjemplos.NeneMalloc.Lights.States;
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
        private List<Lamp> lights;

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

            lights = new List<Lamp>();

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
            var onLamp = new Lamp().WithState(new FixedLight(30)).WithPosition(new Vector3(405, -36, -831));

            var intermitentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(160f,-48.5f,241.8f));

            var offLamp = new Lamp().WithState(new FixedLight(3)).WithPosition(new Vector3(578.8f, -48.5f, 141.7f));

            lights.Add(onLamp);
            lights.Add(intermitentLamp);
            lights.Add(offLamp);

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

            GuiController.Instance.Modifiers.addColor("lightColor", Color.White);
            GuiController.Instance.Modifiers.addFloat("specularEx", 0, 20, 9f);

            //Modifiers de material
            GuiController.Instance.Modifiers.addColor("mEmissive", Color.Black);
            GuiController.Instance.Modifiers.addColor("mAmbient", Color.White);
            GuiController.Instance.Modifiers.addColor("mDiffuse", Color.White);
            GuiController.Instance.Modifiers.addColor("mSpecular", Color.White);
        }

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            Effect currentShader;
            Effect currentAvatarShader;

            currentShader = GuiController.Instance.Shaders.TgcMeshPointLightShader;
            currentAvatarShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;

            //Calcular random por si la luz es intermitente
            float random = new Random().Next(5, 30);

            //Renderizar meshes
            foreach (TgcMesh mesh in tgcScene.Meshes)
            {
                mesh.Effect = currentShader;
                //El Technique depende del tipo RenderType del mesh
                mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);

                Lamp lamp = getClosestLight(mesh.BoundingBox.calculateBoxCenter());

                //Cargar variables shader de la luz
                mesh.Effect.SetValue("lightColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["lightColor"]));
                mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lamp.Position));
                mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(avatar.position));
                mesh.Effect.SetValue("lightIntensity", lamp.getIntensity(random));
                mesh.Effect.SetValue("lightAttenuation", 0.3f);

                //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mEmissive"]));
                mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mAmbient"]));
                mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mDiffuse"]));
                mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor((Color)GuiController.Instance.Modifiers["mSpecular"]));
                mesh.Effect.SetValue("materialSpecularExp", (float)GuiController.Instance.Modifiers["specularEx"]);
                
                //Renderizar modelo
                mesh.render();
                lamp.render();
            }
            
            //Render personaje
            avatar.meshPersonaje.Effect = currentAvatarShader;
            avatar.meshPersonaje.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(avatar.meshPersonaje.RenderType);

            avatar.render(elapsedTime);
        }

        /// <summary>
        /// Devuelve la luz mas cercana a la posicion especificada
        /// </summary>
        private Lamp getClosestLight(Vector3 pos)
        {
            float minDist = float.MaxValue;
            Lamp minLight = null;

            foreach (Lamp light in lights)
            {
                float distSq = Vector3.LengthSq(pos - light.Position);
                if (distSq < minDist)
                {
                    minDist = distSq;
                    minLight = light;
                }
            }

            return minLight;
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            tgcScene.disposeAll();
            foreach (var light in lights)
            {
                light.dispose();
            }
            //avatar.dispose();
        }

    }
}
