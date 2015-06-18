using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using AlumnoEjemplos.NeneMalloc.Lights;
using AlumnoEjemplos.NeneMalloc.Lights.States;
using AlumnoEjemplos.NeneMalloc.Utils.GrillaRegular;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSkeletalAnimation;
using AlumnoEjemplos.NeneMalloc;
using AlumnoEjemplos.NeneMalloc.Utils;
using TgcViewer.Utils._2D;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using Font = System.Drawing.Font;

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
        List<IluminationEntity> lights;
        Effect currentLampShader;
        Effect currentAvatarShader;
        Effect currentLanternShader;
        TgcD3dInput d3dInput;
        TgcText2d PlayingTime;
        Stopwatch stopwatch;
        TgcMp3Player player;
        List<Tgc3dSound> sounds;
        Tgc3dSound sound;
        TgcSprite winningScreen;
        Lamp currentLamp;
        GrillaRegular grilla;
        string path;
        float timeStart = 5f;

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
            d3dInput = GuiController.Instance.D3dInput;

            //Device de DirectX para crear primitivas
            this.path = GuiController.Instance.AlumnoEjemplosMediaDir;

            TgcSceneLoader loader = new TgcSceneLoader();

            Clipboard.Clear();

            tgcScene = loader.loadSceneFromFile(
               path + "NeneMalloc\\EscenarioFaltaIntensivas-TgcScene.xml",
               path + "NeneMalloc\\");

            lights = new List<IluminationEntity>();

            Cursor.Hide();
            Cursor.Position = new Point(GuiController.Instance.FullScreenPanel.Width / 2, GuiController.Instance.FullScreenPanel.Height / 2);

           //Cargar personaje
            avatar = new Avatar();
            avatar.init();

            //Cargar linterna
            lantern = (Lantern) new Lantern().WithPosition(avatar.Position);

            //Cargar sonidos
            sounds = new List<Tgc3dSound>();

            //Cargar musica
            GuiController.Instance.Mp3Player.FileName = path + "NeneMalloc\\SonidosYMusica\\Eyes Wide Shut.mp3";
            player = GuiController.Instance.Mp3Player;
            player.play(true);
            
            obstaculos = new List<TgcBoundingBox>();
            foreach (TgcMesh mesh in tgcScene.Meshes)
            {
                obstaculos.Add(mesh.BoundingBox);
            }

            //Cargar pantalla de juego ganado
            winningScreen = new TgcSprite();
            winningScreen.Texture = TgcTexture.createTexture(path + "NeneMalloc\\winningScreen.png");

            //Ubicar pantalla de juego ganado centrado en la pantalla
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = winningScreen.Texture.Size;
            winningScreen.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height / 2, 0));

            CollitionManager.obstaculos = obstaculos;

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

            GuiController.Instance.UserVars.addVar("Velocidad Caida");
            GuiController.Instance.UserVars.addVar("Falling");
            GuiController.Instance.UserVars.addVar("MouseReleased");

            currentLanternShader = TgcShaders.loadEffect(path + "NeneMalloc\\Shaders\\TgcMeshPointAndSpotLightShader.fx");
            currentLampShader = GuiController.Instance.Shaders.TgcMeshPointLightShader;
            //currentAvatarShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;
            //avatar.meshPersonaje.Effect = currentAvatarShader;

            //Reloj con la hora del juego

            PlayingTime = new TgcText2d();
            stopwatch = new Stopwatch();

            stopwatch.Start();
            PlayingTime.Text = stopwatch.Elapsed.ToString(@"mm\:ss") + " AM";
            PlayingTime.Align = TgcText2d.TextAlign.RIGHT;
            PlayingTime.Position = new Point(600, 400);
            PlayingTime.Size = new Size(300, 100);
            PlayingTime.Color = Color.DarkRed;
            PlayingTime.changeFont(new Font("Arial", 30, FontStyle.Bold));

            //Hacer que el Listener del sonido 3D siga al personaje
            GuiController.Instance.DirectSound.ListenerTracking = personaje;

            //Crear octree
            grilla = new GrillaRegular();
            grilla.create(tgcScene.Meshes, tgcScene.BoundingBox);
            grilla.createDebugMeshes();

            //Reproducir todos los sonidos
            foreach (Tgc3dSound s in sounds)
            {
                s.play(true);
            }
        }

        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            //Juego ganado
            if (stopwatch.Elapsed.Minutes >= 10)
            {
                this.renderFinishedGame();
            }
            else
            {
                this.renderUnfinishedGame(elapsedTime);
            }

        }

        private void renderFinishedGame()
        {
            PlayingTime.Text = "";

            PlayingTime.render();

            TgcText2d WinText = new TgcText2d();
            WinText.Text = "YOU WIN THIS TIME";
            WinText.Align = TgcText2d.TextAlign.CENTER;
            WinText.Position = new Point(300, 250);
            WinText.Size = new Size(500, 500);
            WinText.Color = Color.Indigo;
            WinText.changeFont(new Font("Arial", 50, FontStyle.Bold | FontStyle.Underline));

            WinText.render();

            foreach (Tgc3dSound s in sounds)
            {
                s.stop();
            }

            player.pause();
            ////Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            //GuiController.Instance.Drawer2D.beginDrawSprite();

            ////Dibujar sprite (si hubiese mas, deberian ir todos aqu�)
            //winningScreen.render();

            ////Finalizar el dibujado de Sprites
            //GuiController.Instance.Drawer2D.endDrawSprite();
        }

        private void renderUnfinishedGame(float elapsedTime)
        {
            List<TgcMesh> meshes = tgcScene.Meshes;

            if (d3dInput.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                lantern.ChangeLightOnOff();
            }

            bool frustumCullingEnabled = (bool)GuiController.Instance.Modifiers["culling"];
            if (frustumCullingEnabled)
            {
                TgcFrustum frustum = GuiController.Instance.Frustum;
                grilla.findVisibleMeshes(frustum);
                meshes =
                    meshes.FindAll(
                        m =>
                            m.Enabled &&
                            TgcCollisionUtils.classifyFrustumAABB(frustum, m.BoundingBox) !=
                            TgcCollisionUtils.FrustumResult.OUTSIDE);
            }

            //Actualizar cantidad de meshes dibujadas
            GuiController.Instance.UserVars.setValue("Meshes renderizadas", meshes.Count);

            lantern.Position = avatar.Position + avatar.calculateNewPosition(1, avatar.Rotation);

            //Render personaje
            //avatar.meshPersonaje.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(avatar.meshPersonaje.RenderType);

            //Calcular random por si la luz es intermitente
            this.setRandomToLamps();

            //Lamp closestAvatarLamp = getClosestLight(avatar.Position);

            //Cargar variables shader de la luz
            //avatar.meshPersonaje.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
            //avatar.meshPersonaje.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(avatar.position));
            //avatar.meshPersonaje.Effect.SetValue("lightAttenuation", 0.3f);
            //avatar.meshPersonaje.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(closestAvatarLamp.Position));
            //avatar.meshPersonaje.Effect.SetValue("lightIntensity", closestAvatarLamp.getIntensity());


            //////Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
            //avatar.meshPersonaje.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
            //avatar.meshPersonaje.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
            //avatar.meshPersonaje.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
            //avatar.meshPersonaje.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
            //avatar.meshPersonaje.Effect.SetValue("materialSpecularExp", 9f);

            if (lantern.On)
            {
                foreach (TgcMesh mesh in meshes)
                {
                    mesh.Effect = currentLanternShader;

                    currentLamp = getClosestLight(mesh.BoundingBox.calculateBoxCenter());
                    //El Technique depende del tipo RenderType del mesh
                    mesh.Technique = GuiController.Instance.Shaders.getTgcMeshTechnique(mesh.RenderType);

                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(currentLamp.Position));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(avatar.Position));
                    mesh.Effect.SetValue("lampIntensity", currentLamp.getIntensity());
                    mesh.Effect.SetValue("lightAttenuation", 0.3f);
                    
                    //Cargar variables shader de linterna
                    mesh.Effect.SetValue("lanternIntensity", lantern.Intensity);
                    mesh.Effect.SetValue("lanternPosition", TgcParserUtils.vector3ToFloat4Array(lantern.Position));
                    mesh.Effect.SetValue("spotLightDir", TgcParserUtils.vector3ToFloat3Array(calculateLampDirection(avatar.Rotation)));
                    mesh.Effect.SetValue("spotLightAngleCos", FastMath.ToRad(lantern.SpotAngle));
                    mesh.Effect.SetValue("spotLightExponent", lantern.SpotExponent);

                    //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", 9f);

                    //avatar.meshPersonaje.Effect.SetValue("lightIntensity", closestAvatarLamp.getIntensity() + lantern.Intensity); 
                    mesh.render();
                    mesh.Enabled = false;
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

                    currentLamp = getClosestLight(mesh.BoundingBox.calculateBoxCenter());

                    //Cargar variables shader de la luz
                    mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(currentLamp.Position));
                    mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(avatar.Position));
                    mesh.Effect.SetValue("lightIntensity", currentLamp.getIntensity());
                    mesh.Effect.SetValue("lightAttenuation", 0.3f);

                    //Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    mesh.Effect.SetValue("materialSpecularExp", 9f);

                    mesh.render();
                    mesh.Enabled = false;
                }
            }

            PlayingTime.Text = stopwatch.Elapsed.ToString(@"mm\:ss") + " AM";
            PlayingTime.render();

            if (timeStart >= 0)
            {
                timeStart -= elapsedTime;
            }
            else
            {
                avatar.update(elapsedTime);
            }
            avatar.render();
        }

        private void CreateLamps()
        {
            this.CreateGroundFloorLamps();
            this.CreateFirstFloorLamps();
        }

        private void CreateFirstFloorLamps()
        {
            var intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-201.7801f, 45.05f, -724.91f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-204.4213f, 45.05f, -488.716f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-202.9045f, 45.05f, -112.688f));
            lights.Add(intermittentLamp);

            var offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(-720.47f, 45.05f, -610.4592f));
            lights.Add(offLamp);
            offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(-203.2237f, 45.05f, -296.3737f));
            lights.Add(offLamp);
            offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(-339.8075f, 45.05f, 197.6817f));
            lights.Add(offLamp);
            offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(-389.4152f, 45.05f, -7.683351f));
            lights.Add(offLamp);
            offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(-448.2712f, 45.05f, -441.5271f));
            lights.Add(offLamp);
            offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(-487.5257f, 45.05f, 200.7356f));
            lights.Add(offLamp);
            offLamp = new Lamp().WithState(new FixedLight(0)).WithPosition(new Vector3(-491.3519f, 10f, 112.9243f));
            lights.Add(offLamp);
        }

        private void CreateGroundFloorLamps()
        {
            var intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-217.1517f, -91.5322f, -1232.428f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(13.08098f, -93.5423f, 567.2851f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-150.1281f, -93.5423f, 211.5172f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(132.4148f, -93.5423f, 167.8136f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-245.4898f, -91.5322f, -525.1691f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-224.0355f, -91.5322f, -736.4214f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-800.6736f, -76.8167f, -610.8485f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-473.2807f, -91.5322f, 14.82125f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-263.4782f, -92.8721f, 372.9998f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-674.7888f, -92.8721f, 578.1655f));
            lights.Add(intermittentLamp);

            var offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(348.0486f, -91.5322f, -729.3348f));
            lights.Add(offLamp);
            offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(-43.66533f, -91.5322f, -384.5498f));
            lights.Add(offLamp);
        }

        private void setRandomToLamps()
        {
            foreach (Lamp light in lights)
            {
                float random = new Random().Next(2, 10);
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
            float xzLen = (float)Math.Cos(rotation.X);
            float z = xzLen * (float)Math.Cos(rotation.Y);
            float x = xzLen * (float)Math.Sin(rotation.Y);
            float y = (float)Math.Sin(rotation.X);
            var dir = new Vector3(x, -y, z);
            dir.Normalize();
            return dir;
        }

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            tgcScene.disposeAll();
            foreach (var light in lights)
            {
                light.dispose();
            }
            PlayingTime.dispose();
            winningScreen.dispose();
            lantern.dispose();
        }

    }
}
