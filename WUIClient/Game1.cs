using LowLevelNetworking.Shared;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using WUIClient.Components;
using WUIClient.Components.UI;
using WUIClient.Panels;
using WUIShared.Objects;

namespace WUIClient {
    public class Game1 : Game {
        public static Game1 instance;
        public static ClientBase client;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Texture2D UIRect;
        public SpriteFont arial;

        GameObject world;
        private GameObject canvas;
        public Camera camera;

        public static NetworkManager networkManager;
        public static ClientAssetManager assetManager;

        public Game1() {
            instance = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            world = new GameObject(Objects.Empty, false) {
                multiplayer = true
            };

            canvas = new GameObject(Objects.Empty, false) {
                multiplayer = false
            };

            IsMouseVisible = false;
            camera = new Camera();
            WMouse.camera = camera;


            client = new ClientBase("127.0.0.1", 3333, 8388608); //8MB Of buffer so images can be sent.
            networkManager = new NetworkManager(world);
            assetManager = new ClientAssetManager(client);
        }

        protected override void Initialize() {

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            UIRect = Content.Load<Texture2D>("UI");
            arial = Content.Load<SpriteFont>("arial");

            //GameObject btn = new GameObject();
            //btn.transform.Position = new Vector2(32, 32);
            //btn.transform.Size = new Vector2(64, 32);
            //btn.AddChild(new MouseClickableComponent(false));
            //btn.AddChild(new DragComponent());
            //btn.AddChild(new RawTextureRenderer() { texture = UIRect, color = Color.White });
            //btn.AddChild(new TextRenderer("Wow", Color.White));
            //btn.AddChild(new ButtonComponent());
            //world.AddChild(btn);

            FilePanel filePanel = new FilePanel();
            filePanel.transform.Position = new Vector2(0, 0);
            canvas.AddChild(filePanel);
            filePanel.OpenDirectory(@"C:\Users\waldohp\Desktop\Files\GameEngine WUI test folder");
            filePanel.OnItemDrop += FilePanel_OnItemDrop;

            GameObject mouse = new GameObject();
            mouse.AddChild(new FollowMouse(false));
            mouse.AddChild(new RawTextureRenderer() { texture = UIRect, color = Color.Red });
            mouse.transform.Size = new Vector2(4, 4);
            canvas.AddChild(mouse);
        }

        private void FilePanel_OnItemDrop(FilePanel sender, string fileName) {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            Texture2D spriteAtlas = Texture2D.FromStream(GraphicsDevice, fileStream);
            
            GameObject obj = new GameObject();
            obj.transform.Position = WMouse.WorldPosition;
            obj.transform.Size = new Vector2(64, 32);
            obj.AddChild(new MouseClickableComponent(false));
            obj.AddChild(new DragComponent());
            obj.AddChild(new RawTextureRenderer() { texture = spriteAtlas, color = Color.White });
            world.AddChild(obj);
            obj.GetFirst<RawTextureRenderer>().OnNetworkReady += Obj_OnNetworkReady;
            void Obj_OnNetworkReady(GameObject sender2) {
                byte[] texture = new byte[fileStream.Length];
                fileStream.Position = 0;
                fileStream.Read(texture, 0, texture.Length);
                obj.GetFirst<RawTextureRenderer>().SendTexture(texture);
                fileStream.Dispose();
            }
        }

        

        protected override void UnloadContent() {
            client.Stop();
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            client.AcceptPacketsWhileAvailable();

            WMouse.Update();
            WKeyboard.Update();
            world.Update(deltaTime);
            canvas.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            spriteBatch.Begin();
            world.Render(spriteBatch, deltaTime);
            canvas.Render(spriteBatch, deltaTime);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
