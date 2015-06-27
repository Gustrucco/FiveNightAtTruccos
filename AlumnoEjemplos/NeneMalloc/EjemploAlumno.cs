using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AlumnoEjemplos.NeneMalloc.Lights;
using AlumnoEjemplos.NeneMalloc.Lights.States;
using AlumnoEjemplos.NeneMalloc.Utils;
using AlumnoEjemplos.NeneMalloc.Utils.GrillaRegular;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Example;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer.Utils._2D;
using Font = System.Drawing.Font;

namespace AlumnoEjemplos.NeneMalloc
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
        TgcSprite gameOverScreen;
        Lamp currentLamp;
        GrillaRegular grilla;
        string path;
        float timeStart = 5f;
        List<TgcArrow> ArrowsClosesCheckPoint;
        Checkpoint ClosestCheckPoint;
        List<Monster> Monsters;
        bool canRenderMonsters;
        public Mutex SemaphoreMutex = new Mutex();

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
            return "El objetivo del juego es sobrevivir a la noche de seguridad. No se puede golpear a los enemigos. Simplemente escaparse de ellos";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Código de inicialización
        /// </summary>
        public override void init()
        {
            //GuiController.Instance: acceso principal a todas las herramientas del Framework

            d3dInput = GuiController.Instance.D3dInput;
            GuiController.Instance.BackgroundColor = Color.Black;

            Cursor.Hide();
            Cursor.Position = new Point(GuiController.Instance.FullScreenPanel.Width / 2, GuiController.Instance.FullScreenPanel.Height / 2);
            Clipboard.Clear();
            //Device de DirectX para crear primitivas
            this.path = GuiController.Instance.AlumnoEjemplosMediaDir;

            TgcSceneLoader loader = new TgcSceneLoader();

            Clipboard.Clear();

            tgcScene = loader.loadSceneFromFile(
               path + "NeneMalloc\\paraPortalizar-TgcScene.xml",
               path + "NeneMalloc\\");

            //Descactivar inicialmente a todos los modelos
            tgcScene.setMeshesEnabled(false);

            lights = new List<IluminationEntity>();

            Cursor.Hide();
            Cursor.Position = new Point(GuiController.Instance.FullScreenPanel.Width / 2, GuiController.Instance.FullScreenPanel.Height / 2);

           //Cargar personaje
            avatar = new Avatar();

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

            //Cargar pantalla de juego perdido
            gameOverScreen = new TgcSprite();
            gameOverScreen.Texture = TgcTexture.createTexture(path + "NeneMalloc\\gameover.jpg");

            //Ubicar pantalla de juego ganado centrado en la pantalla
            Size screenSize = GuiController.Instance.Panel3d.Size;
            Size textureSize = GuiController.Instance.Panel3d.Size;
           
            winningScreen.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width / 2, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height, 0));
            winningScreen.Scaling = new Vector2(1.9f, 0.5f);

            gameOverScreen.Position = new Vector2(FastMath.Max(screenSize.Width / 2 - textureSize.Width, 0), FastMath.Max(screenSize.Height / 2 - textureSize.Height, 0));
            gameOverScreen.Scaling = new Vector2(1f, 0.5f);

            CollitionManager.obstaculos = obstaculos;
            
            //Cargar los enemigos
            Monsters = new List<Monster>();

            this.CreateLamps();

            currentLanternShader = TgcShaders.loadEffect(path + "NeneMalloc\\Shaders\\TgcMeshPointAndSpotLightShader.fx");
            currentLampShader = GuiController.Instance.Shaders.TgcMeshPointLightShader;
            currentAvatarShader = GuiController.Instance.Shaders.TgcSkeletalMeshPointLightShader;

            //Creacion Checkpoints
            CheckpointHelper.EjemploAlumno = this;
            Thread thread = new Thread(CheckpointHelper.BuildCheckpoints);
            thread.Start();

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

            GuiController.Instance.UserVars.addVar("angulo");

            //Reproducir todos los sonidos
            foreach (Tgc3dSound s in sounds)
            {
                s.play(true);
            }
        }

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            if (Monsters.Any(m => Math.Abs(Vector3.Length(m.Position - avatar.Position)) < 20f))
            {
                this.renderGameOver();
            }
            else
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
        }
       
        private void renderGameOver()
        {
            PlayingTime.Text = "";

            PlayingTime.render();

            foreach (Tgc3dSound s in sounds)
            {
                s.stop();
            }

            player.pause();

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            GuiController.Instance.Drawer2D.beginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            gameOverScreen.render();

            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        private void renderFinishedGame()
        {
            PlayingTime.Text = "";

            PlayingTime.render();

            foreach (Tgc3dSound s in sounds)
            {
                s.stop();
            }

            player.pause();

            //Iniciar dibujado de todos los Sprites de la escena (en este caso es solo uno)
            GuiController.Instance.Drawer2D.beginDrawSprite();

            //Dibujar sprite (si hubiese mas, deberian ir todos aquí)
            winningScreen.render();

            //Finalizar el dibujado de Sprites
            GuiController.Instance.Drawer2D.endDrawSprite();
        }

        private void renderUnfinishedGame(float elapsedTime)
        {
            List<TgcMesh> meshes = new List<TgcMesh>();
          
            if (timeStart >= 0)
            {
                timeStart -= elapsedTime;
            }
            else
            {
                avatar.Update(elapsedTime);
            }

            if (d3dInput.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                lantern.ChangeLightOnOff();
            }

            foreach (var monster in Monsters)
            {
                SemaphoreMutex.WaitOne();
                monster.Update(elapsedTime);
                monster.Render();
                SemaphoreMutex.ReleaseMutex();
            }

            //Visibilidada con portal Rendering
            tgcScene.PortalRendering.updateVisibility(avatar.Position);

            foreach (TgcMesh mesh in tgcScene.Meshes)
            {
                if (mesh.Enabled)
                {
                    meshes.Add(mesh);
                }
            }

            lantern.Position = avatar.Position;

            //Normalizar direccion de la luz
            Vector3 lightDir = this.calculateLampDirection(avatar.Rotation);
            lightDir.Normalize();

            //Calcular random por si la luz es intermitente
            this.setRandomToLamps();

            if (canRenderMonsters)
            {
                foreach (var monster in Monsters)
                {
                    Lamp closestAvatarLamp = getClosestLight(monster.Position);

                    //Cargar variables shader de la luz
                    monster.mesh.Effect.SetValue("lightColor", ColorValue.FromColor(Color.White));
                    monster.mesh.Effect.SetValue("eyePosition", TgcParserUtils.vector3ToFloat4Array(avatar.Position));
                    monster.mesh.Effect.SetValue("lightAttenuation", 0.3f);
                    monster.mesh.Effect.SetValue("lightPosition", TgcParserUtils.vector3ToFloat4Array(closestAvatarLamp.Position));
                    
                    ////Cargar variables de shader de Material. El Material en realidad deberia ser propio de cada mesh. Pero en este ejemplo se simplifica con uno comun para todos
                    monster.mesh.Effect.SetValue("materialEmissiveColor", ColorValue.FromColor(Color.Black));
                    monster.mesh.Effect.SetValue("materialAmbientColor", ColorValue.FromColor(Color.White));
                    monster.mesh.Effect.SetValue("materialDiffuseColor", ColorValue.FromColor(Color.White));
                    monster.mesh.Effect.SetValue("materialSpecularColor", ColorValue.FromColor(Color.White));
                    monster.mesh.Effect.SetValue("materialSpecularExp", 9f);

                    if (lantern.On)
                    {
                        monster.mesh.Effect.SetValue("lightIntensity", closestAvatarLamp.getIntensity() + lantern.Intensity);
                    }
                    else
                    {
                        monster.mesh.Effect.SetValue("lightIntensity", closestAvatarLamp.getIntensity());

                    }
                }              
            }

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
        }

        private void CreateLamps()
        {
            this.CreateGroundFloorLamps();
            this.CreateFirstFloorLamps();
        }

        private void CreateGroundFloorLamps()
        {
            var intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-217.1517f, -91.5322f, -1232.428f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(13.08098f, -93.5423f, 567.2851f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-150.1281f, -93.5423f, 211.5172f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(132.4148f, -93.5423f, 167.8136f));
            this.addInttermitentSound(intermittentLamp);
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-245.4898f, -91.5322f, -525.1691f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-224.0355f, -91.5322f, -736.4214f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-800.6736f, -76.8167f, -610.8485f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-473.2807f, -91.5322f, 14.82125f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-263.4782f, -92.8721f, 372.9998f));
            lights.Add(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-674.7888f, -92.8721f, 578.1655f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);

            var offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(348.0486f, -91.5322f, -729.3348f));
            lights.Add(offLamp);
            offLamp = new Lamp().WithState(new FixedLight(1)).WithPosition(new Vector3(-43.66533f, -91.5322f, -384.5498f));
            lights.Add(offLamp);
        }

        private void CreateFirstFloorLamps()
        {
            var intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-201.7801f, 45.05f, -724.91f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-204.4213f, 45.05f, -488.716f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);
            intermittentLamp = new Lamp().WithState(new IntermittentLight()).WithPosition(new Vector3(-202.9045f, 45.05f, -112.688f));
            lights.Add(intermittentLamp);
            this.addInttermitentSound(intermittentLamp);

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

        private void addInttermitentSound(IluminationEntity intermittentLamp)
        {
            sound = new Tgc3dSound(this.path + "NeneMalloc\\SonidosYMusica\\tuboDeLuz.wav", intermittentLamp.Position);
            sound.MinDistance = 5f;
            sounds.Add(sound);
        }

        private void setRandomToLamps()
        {
            foreach (Lamp light in lights)
            {
                float random = new Random().Next(2, 6);
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
            CheckpointHelper.CheckPoints.Clear();
            PlayingTime.dispose();
            winningScreen.dispose();
            lantern.dispose();
        }

        public void CreateMonsters()
        {
            var monsterList = new List<Monster>();
            var monster = new Monster(new Vector3(-11.39549f, -100f, 192.1234f), avatar, "Pilot-TgcSkeletalMesh.xml");
            monster.mesh.Effect = currentAvatarShader;
            monster.mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(monster.mesh.RenderType);
            monsterList.Add(monster);
            monster = new Monster(new Vector3(-695.4191f, -100f, -586.1326f), avatar, "BasicHuman-TgcSkeletalMesh.xml");
            monster.mesh.Effect = currentAvatarShader;
            monster.mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(monster.mesh.RenderType);
            monsterList.Add(monster);
            monster = new Monster(new Vector3(-238.9347f, -100f, -389.5356f), avatar, "WomanJeans-TgcSkeletalMesh.xml");
            monster.mesh.Effect = currentAvatarShader;
            monster.mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(monster.mesh.RenderType);
            monsterList.Add(monster);
            monster = new Monster(new Vector3(-329.9544f, 45.05f, 1767.038f), avatar, "Pilot-TgcSkeletalMesh.xml");
            monster.mesh.Effect = currentAvatarShader;
            monster.mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(monster.mesh.RenderType);
            monsterList.Add(monster);
            monster = new Monster(new Vector3(-828.4836f, 45.05f, 1765.216f), avatar, "WomanJeans-TgcSkeletalMesh.xml");
            monster.mesh.Effect = currentAvatarShader;
            monster.mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(monster.mesh.RenderType);
            monsterList.Add(monster);
            monster = new Monster(new Vector3(-478.084f, 45.05f, 146.0769f), avatar, "BasicHuman-TgcSkeletalMesh.xml");
            monster.mesh.Effect = currentAvatarShader;
            monster.mesh.Technique = GuiController.Instance.Shaders.getTgcSkeletalMeshTechnique(monster.mesh.RenderType);
            monsterList.Add(monster);

            SemaphoreMutex.WaitOne();
            Monsters = monsterList;
            SemaphoreMutex.ReleaseMutex();

            canRenderMonsters = true;
        }
    }
}
