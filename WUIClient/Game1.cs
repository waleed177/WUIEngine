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
        private SpriteBatch spriteBatchUI;
        public Texture2D UIRect;
        public SpriteFont arial;

        public GameObject world;
        public static  GameObject gizmoWorld;
        private GameObject canvas;
        public Camera camera;

        public static NetworkManager networkManager;
        public static ClientAssetManager assetManager;
        public static PlayerController localPlayer;

        private Tools.Tool currentTool = null;
        private Tools.MoveTool moveTool;
        private Tools.ScaleTool scaleTool;

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

            gizmoWorld = new GameObject(Objects.Empty, false) {
                multiplayer = false
            };

            IsMouseVisible = true;
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
            spriteBatchUI = new SpriteBatch(GraphicsDevice);
            UIRect = Content.Load<Texture2D>("UI");
            arial = Content.Load<SpriteFont>("arial");

            moveTool = new Tools.MoveTool();
            scaleTool = new Tools.ScaleTool();

            GameObject btn = new GameObject();
            btn.transform.Position = new Vector2(140, 0);
            btn.transform.Size = new Vector2(100, 32);
            btn.AddChild(new MouseClickableComponent(true));
            btn.AddChild(new RawTextureRenderer() { texture = UIRect, color = Color.White });
            btn.AddChild(new TextRenderer("MoveTool", Color.Black));
            btn.AddChild(new ButtonComponent());
            btn.GetFirst<MouseClickableComponent>().mouseClickable.OnMouseLeftClickUp += (sender) => ToolSelect(moveTool);
            canvas.AddChild(btn);

            GameObject btn2 = new GameObject();
            btn2.transform.Position = new Vector2(240, 0);
            btn2.transform.Size = new Vector2(100, 32);
            btn2.AddChild(new MouseClickableComponent(true));
            btn2.AddChild(new RawTextureRenderer() { texture = UIRect, color = Color.White });
            btn2.AddChild(new TextRenderer("ScaleTool", Color.Black));
            btn2.AddChild(new ButtonComponent());
            btn2.GetFirst<MouseClickableComponent>().mouseClickable.OnMouseLeftClickUp += (sender) => ToolSelect(scaleTool);
            canvas.AddChild(btn2);

            GameObject btn3 = new GameObject();
            btn3.transform.Position = new Vector2(640, 0);
            btn3.transform.Size = new Vector2(100, 32);
            btn3.AddChild(new MouseClickableComponent(true));
            btn3.AddChild(new RawTextureRenderer() { texture = UIRect, color = Color.White });
            btn3.AddChild(new TextRenderer("Save", Color.Black));
            btn3.AddChild(new ButtonComponent());
            btn3.GetFirst<MouseClickableComponent>().mouseClickable.OnMouseLeftClickUp += SaveButton_Clicked;
            canvas.AddChild(btn3);

            FilePanel filePanel = new FilePanel();
            filePanel.transform.Position = new Vector2(0, 0);
            canvas.AddChild(filePanel);
            filePanel.OpenDirectory(@"C:\Users\waldohp\Desktop\Files\GameEngine WUI test folder");
            filePanel.OnItemDrop += FilePanel_OnItemDrop;


        }

        private void SaveButton_Clicked(GameObject sender) {
            client.Send(new WUIShared.Packets.SaveWorldPacket() {
                name = "TEST"
            });
        }

        private void ToolSelect(Tools.Tool tool) {
            if (currentTool != null) {
                currentTool.Deselect();
                if (currentTool != tool) {
                    currentTool = tool;
                    currentTool.Select();
                } else
                    currentTool = null;

            } else {
                currentTool = tool;
                currentTool.Select();
            }
        }

        private void FilePanel_OnItemDrop(FilePanel sender, string fileName) {
            Texture2D spriteAtlas = assetManager.GetAsset<Texture2D>(Path.GetFileName(fileName));
            FileStream fileStream = null;
            if (spriteAtlas == null) {
                fileStream = new FileStream(fileName, FileMode.Open);
                spriteAtlas = Texture2D.FromStream(GraphicsDevice, fileStream);
                byte[] texture = new byte[fileStream.Length];
                fileStream.Position = 0;
                fileStream.Read(texture, 0, texture.Length);
                fileStream.Dispose();
                assetManager.SetAsset(Path.GetFileName(fileName), texture);
            }

            GameObject obj = new GameObject();
            obj.transform.Position = WMouse.WorldPosition;
            obj.transform.Size = new Vector2(64, 32);
            obj.AddChild(new MouseClickableComponent(false));
            obj.AddChild(new DragComponent());
            obj.AddChild(new RawTextureRenderer() { texture = spriteAtlas, color = Color.White });
            world.AddChild(obj);
            obj.GetFirst<RawTextureRenderer>().OnNetworkReady += Obj_OnNetworkReady;
            void Obj_OnNetworkReady(GameObject sender2) {
                obj.GetFirst<RawTextureRenderer>().SetTexture(Path.GetFileName(fileName));
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

            if (currentTool != null)
                currentTool.Update();
            WMouse.Update();
            WKeyboard.Update();
            gizmoWorld.Update(deltaTime);
            world.Update(deltaTime);
            canvas.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            camera.Update(this);
            spriteBatch.Begin(transformMatrix: camera.transformMatrix);
            world.Render(spriteBatch, deltaTime);
            gizmoWorld.Render(spriteBatch, deltaTime);
            spriteBatch.End();

            spriteBatchUI.Begin();
            canvas.Render(spriteBatchUI, deltaTime);
            spriteBatchUI.End();

            base.Draw(gameTime);
        }
    }
}
