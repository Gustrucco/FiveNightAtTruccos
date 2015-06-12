using System;
using System.Collections.Generic;
using System.Drawing;
using AlumnoEjemplos.NeneMalloc.Lights;
using AlumnoEjemplos.NeneMalloc.Lights.States;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.NeneMalloc;
using AlumnoEjemplos.NeneMalloc.Utils;
using Effect = Microsoft.DirectX.Direct3D.Effect;

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
        private List<IluminationEntity> lights;
        Effect currentLampShader;
        Effect currentAvatarShader;
        Effect currentLanternShader;
        private TgcD3dInput d3dInput;

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

            d3dInput = GuiController.Instance.D3dInput;

            //Device de DirectX para crear primitivas
            string path = GuiController.Instance.AlumnoEjemplosMediaDir;

            TgcSceneLoader loader = new TgcSceneLoader();
            
            tgcScene = loader.loadSceneFromFile(
               path + "NeneMalloc\\pisoCompleto-TgcScene.xml",
               path + "NeneMalloc\\");

            lights = new List<IluminationEntity>();

           //Cargar personaje
            avatar = new Avatar();
            avatar.init();

            //Cargar linterna
            lantern = (Lantern) new Lantern().WithPosition(avatar.position);
            
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

            this.CreateLamps();

            //Modifier frustum Culling
            GuiController.Instance.Modifiers.addBoolean("culling", "Frustum culling", true);

            //Modifier para ver BoundingBox
            GuiController.Instance.Modifiers.addBoolean("showBoundingBox", "Bouding Box", false);
            GuiController.Instance.UserVars.addVar("isColliding");
            GuiController.Instance.UserVars.addVar("Pos");
            GuiController.Instance.UserVars.addVar("Normal");
            GuiController.Instance.UserVars.addVar("LastPos");
            GuiController.Instance.UserVars.addVar("Meshes renderizadas");
            
            //Modifiers para desplazamiento del personaje
            GuiController.Instance.Modifiers.addFloat("VelocidadCaminar", 1f, 400f, 250f);
            GuiController.Instance.Modifiers.addFloat("VelocidadRotacion", 1f, 360f, 120f);

            currentLanternShader = TgcShaders.loadEffect(path + "NeneMalloc\\Shaders\\TgcMeshPointAndSpotLightShader.fx");
            currentLampShader = GuiController.Instance.Shaders.TgcMeshPointLightShader;
            currentAvatarShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;
            avatar.meshPersonaje.Effect = currentAvatarShader;
        }

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            List<TgcMesh> meshes = tgcScene.Meshes;

            if (d3dInput.keyPressed(Key.L))
            {
                lantern.ChangeLightOnOff();
            }

            bool frustumCullingEnabled = (bool)GuiController.Instance.Modifiers["culling"];
            if (frustumCullingEnabled)
            {
                TgcFrustum frustum = GuiController.Instance.Frustum;
                meshes =
                    meshes.FindAll(
                        m =>
                            TgcCollisionUtils.classifyFrustumAABB(frustum, m.BoundingBox) !=
                            TgcCollisionUtils.FrustumResult.OUTSIDE);
                
                
            }
            //Actualizar cantidad de meshes dibujadas
            GuiController.Instance.UserVars.setValue("Meshes renderizadas", meshes.Count);

            lantern.Position = avatar.position;

            //Normalizar direccion de la luz
            Vector3 lightDir = this.calculateLampDirection(avatar.rotation);
            lightDir.Normalize();
            //Render personaje
            avatar.meshPersonaje.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(avatar.meshPersonaje.RenderType);
            //Calcular random por si la luz es intermitente
            this.setRandomToLamps();

            //Cargar variables shader de la luz
            avatar.meshPersonaje.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
            avatar.meshPersonaje.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(avatar.position));
            avatar.meshPersonaje.Effect.SetValue("lightAttenuation", 0.3f);

            ////Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            avatar.meshPersonaje.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            avatar.meshPersonaje.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            avatar.meshPersonaje.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            avatar.meshPersonaje.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            avatar.meshPersonaje.Effect.SetValue("materialSpecularExp", 9f);

            if (lantern.On)
            {
                foreach (TgcMesh mesh in meshes)
                {
                    mesh.Effect = currentLanternShader;

                    Lamp lamp = getClosestLight(mesh.BoundingBox.calculateBoxCenter());
                    //El Technique depende del tipo RenderType del mesh
                    mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);

                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lantern.Position));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(avatar.position));
                    mesh.Effect.SetValue("lampIntensity", lamp.getIntensity());
                    mesh.Effect.SetValue("lanternIntensity", lantern.Intensity);
                    mesh.Effect.SetValue("lightAttenuation", 0.3f);

                    //Cargar variables shader de linterna
                    mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat3Array(lightDir));
                    mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(lantern.SpotAngle));
                    mesh.Effect.SetValue("spotLightExponent", lantern.SpotExponent);

                    //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", 9f);

                    avatar.meshPersonaje.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lantern.Position));
                    avatar.meshPersonaje.Effect.SetValue("lightIntensity", lamp.getIntensity() + lantern.Intensity);
                    //Renderizar modelo (lamp.render() no hace nada por ahora)
                    mesh.render();
                }
            }
            else
            {
                //Renderizar meshes
                foreach (TgcMesh mesh in meshes)
                {
                    mesh.Effect = currentLampShader;

                    //El Technique depende del tipo RenderType del mesh
                    mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);

                    Lamp lamp = getClosestLight(mesh.BoundingBox.calculateBoxCenter());

                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lamp.Position));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(avatar.position));
                    mesh.Effect.SetValue("lightIntensity", lamp.getIntensity());
                    mesh.Effect.SetValue("lightAttenuation", 0.3f);

                    //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", 9f);

                    avatar.meshPersonaje.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(lamp.Position));
                    avatar.meshPersonaje.Effect.SetValue("lightIntensity", lamp.getIntensity());

                    //Renderizar modelo (lamp.render() no hace nada por ahora)
                    mesh.render();
                    lamp.render();
                }
            }
          
            avatar.render(elapsedTime);
        }

        private void CreateLamps()
        {
            this.CreateGroundFloorLamps();
            this.CreateFirstFloorLamps();
        }

        private void CreateFirstFloorLamps()
        {
            //Luces intermitentes

            var intermitentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-26.5f, 75f, 6.3f));
            var intermitentLamp2 = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-27.75f, 75f, -325.15f));
            var intermitentLamp3 = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(229.5f, 75f, 97.1f));

            //Luces prendidas
            var onLamp = new Lamp().WithState(new FixedLight(25)).WithPosition(new Vector3(-271.75f, 75f, 112.3f));
            var onLamp2 = new Lamp().WithState(new FixedLight(20)).WithPosition(new Vector3(-126.6f, 75f, 37f));

            //Luces apagadas
            var offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(199f, 75f, 535.45f));
            var offLamp2 = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(271.75f, 75f, 12.3f));
            var offLamp3 = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(307.25f, 75f, -224.9f));
            var offLamp4 = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(290.7f, 75f, -485.9f));

            //Se agregan las luces a la colección
            lights.Add(intermitentLamp);
            lights.Add(intermitentLamp2);
            lights.Add(intermitentLamp3);

            lights.Add(onLamp);
            lights.Add(onLamp2);

            lights.Add(offLamp);
            lights.Add(offLamp2);
            lights.Add(offLamp3);
            lights.Add(offLamp4);
        }

        private void CreateGroundFloorLamps()
        {
            //Luces intermitentes
            var intermitentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(405, -36, -831));
            var intermitentLamp2 = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(160f, -48.5f, 241.8f));
            var intermitentLamp3 = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(349.2f, -61.5f, -327.8f));

            //Luces prendidas
            var onLamp = new Lamp().WithState(new FixedLight(25)).WithPosition(new Vector3(-102.5f, -61.5f, -331.25f));
            var onLamp2 = new Lamp().WithState(new FixedLight(25)).WithPosition(new Vector3(276f, -61.5f, -129.6f));
            var onLamp3 = new Lamp().WithState(new FixedLight(25)).WithPosition(new Vector3(864.4f, -61.5f, -475.8f));
            var onLamp4 = new Lamp().WithState(new FixedLight(25)).WithPosition(new Vector3(101.9f, -61.5f, -469.3f));

            //Luces apagadas
            var offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(578.8f, -48.5f, 141.7f));
            var offLamp2 = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(322.1f, -61.5f, -151.2f));
            var offLamp3 = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(711.8f, -61.5f, -174.5f));
            var offLamp4 = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(713.3f, -61.5f, -293.2f));
            var offLamp5 = new Lamp().WithState(new FixedLight(8)).WithPosition(new Vector3(350.5f, 9f, 316.9f));

            //Se agregan las luces a la coleccion
            lights.Add(intermitentLamp);
            lights.Add(intermitentLamp2);
            lights.Add(intermitentLamp3);

            lights.Add(onLamp);
            lights.Add(onLamp2);
            lights.Add(onLamp3);
            lights.Add(onLamp4);

            lights.Add(offLamp);
            lights.Add(offLamp2);
            lights.Add(offLamp3);
            lights.Add(offLamp4);
            lights.Add(offLamp5);
        }

        private void setRandomToLamps()
        {
            foreach (Lamp light in lights)
            {
                float random = new Random().Next(5, 40);
                light.setRandom(random);
            }
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


        protected Vector3 calculateLampDirection(Vector3 rotation)
        {
            float z = (float)Math.Cos(rotation.Y);
            float x = (float)Math.Sin(rotation.Y);
            float y = (float) Math.Sin(rotation.X);
            var dir = new Vector3(x, y, z);
            return dir;
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
