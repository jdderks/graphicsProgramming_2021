using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GraphicsProgramming
{
    class Lesson4 : Lesson
    {

        private Effect effect;
        private Texture2D heightmap, dirt, dirt_norm, dirt_spec, water, foam, waterNormal;
        private TextureCube sky;
        private Model cube;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Vert : IVertexType
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector3 Binormal;
            public Vector3 Tangent;
            public Vector2 Texture;

            static readonly VertexDeclaration _vertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.Binormal, 0),
                new VertexElement(36, VertexElementFormat.Vector3, VertexElementUsage.Tangent, 0),
                new VertexElement(48, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            );

            VertexDeclaration IVertexType.VertexDeclaration => _vertexDeclaration;


            public Vert(Vector3 position, Vector3 normal, Vector3 binormal, Vector3 tangent, Vector2 texture)
            {
                Position = position;
                Normal = normal;
                Binormal = binormal;
                Tangent = tangent;
                Texture = texture;
            }
        }

        private Vert[] vertices;
        private int[] indices;

        private int mouseX, mouseY;

        Vector3 cameraPos = Vector3.Up * 128f;
        Quaternion cameraRotation = Quaternion.Identity;
        float yaw, pitch;

        public override void Initialize()
        {
            mouseX = Mouse.GetState().X;
            mouseY = Mouse.GetState().Y;
        }

        public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            effect = Content.Load<Effect>("LiveLesson4");
            heightmap = Content.Load<Texture2D>("heightmap");
            dirt = Content.Load<Texture2D>("dirt_diff");
            //sky = Content.Load<TextureCube>("testcube");

            //cube = Content.Load<Model>("cube");
            //foreach (ModelMesh mesh in cube.Meshes)
            //{
            //    foreach (ModelMeshPart meshPart in mesh.MeshParts)
            //    {
            //        meshPart.Effect = effect;
            //    }
            //}

            GeneratePlane();
        }

        private void GeneratePlane(float gridSize = 8.0f, float height = 128f)
        {
            // Get pixels

            //Generate vertices & indices

            //Calculate normals
        }

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float speed = 100;

            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.LeftShift))
            {
                speed *= 2;
            }

            if (keyState.IsKeyDown(Keys.W))
            {
                cameraPos += delta * speed * Vector3.Transform(Vector3.Forward, cameraRotation);
            }
            else if (keyState.IsKeyDown(Keys.S))
            {
                cameraPos -= delta * speed * Vector3.Transform(Vector3.Forward, cameraRotation);
            }
            if (keyState.IsKeyDown(Keys.A))
            {
                cameraPos += delta * speed * Vector3.Transform(Vector3.Left, cameraRotation);
            }
            else if (keyState.IsKeyDown(Keys.D))
            {
                cameraPos += delta * speed * Vector3.Transform(Vector3.Right, cameraRotation);
            }
            if (keyState.IsKeyDown(Keys.Space))
            {
                cameraPos += delta * speed * Vector3.Transform(Vector3.Up, cameraRotation);
            }
            else if (keyState.IsKeyDown(Keys.LeftControl))
            {
                cameraPos += delta * speed * Vector3.Transform(Vector3.Down, cameraRotation);
            }

            MouseState mState = Mouse.GetState();
            int deltaX = mState.X - mouseX;
            int deltaY = mState.Y - mouseY;

            float sensitivity = 0.01f;

            yaw -= deltaX * sensitivity;
            pitch -= deltaY * sensitivity;

            pitch = Math.Clamp(pitch, -MathF.PI * .5f, MathF.PI * .5f);

            cameraRotation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0);

            mouseX = mState.X;
            mouseY = mState.Y;

            if (mState.RightButton == ButtonState.Pressed)
            {
                yaw = 0;
                pitch = 0;
            }
        }

        public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
        {
            GraphicsDevice device = graphics.GraphicsDevice;
            device.Clear(Color.Black);

            float r = (float)gameTime.TotalGameTime.TotalSeconds;

            // Build & Set Matrices
            Matrix World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            Matrix View = Matrix.CreateLookAt(cameraPos, cameraPos + Vector3.Transform(Vector3.Forward, cameraRotation), Vector3.Transform(Vector3.Up, cameraRotation));
            Matrix Projection = Matrix.CreatePerspectiveFieldOfView((MathF.PI / 180f) * 65f, device.Viewport.AspectRatio, 0.01f, 2000f);

            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);

            // Lighting Parameters
            effect.Parameters["LightDirection"].SetValue(Vector3.Normalize(Vector3.Down + Vector3.Right * 2));
            effect.Parameters["Ambient"].SetValue(new Vector3(.25f, .20f, .15f));
            effect.Parameters["CameraPosition"].SetValue(cameraPos);

            // Textures
            effect.Parameters["DirtTex"].SetValue(dirt);

            // Render Sky
            device.RasterizerState = RasterizerState.CullNone;
            device.DepthStencilState = DepthStencilState.None;
            effect.CurrentTechnique = effect.Techniques["SkyBox"];

            RenderModel(cube, Matrix.CreateTranslation(cameraPos));

            // Render Terrain
            device.RasterizerState = RasterizerState.CullCounterClockwise;
            device.DepthStencilState = DepthStencilState.Default;
            effect.CurrentTechnique = effect.Techniques["Terrain"];
            effect.Parameters["World"].SetValue(World);

            effect.CurrentTechnique.Passes[0].Apply();
            //device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
        }

        void RenderModel(Model m, Matrix parentMatrix)
        {
            Matrix[] transforms = new Matrix[m.Bones.Count];
            m.CopyAbsoluteBoneTransformsTo(transforms);

            effect.CurrentTechnique.Passes[0].Apply();

            foreach (ModelMesh mesh in m.Meshes)
            {
                effect.Parameters["World"].SetValue(parentMatrix * transforms[mesh.ParentBone.Index]);

                mesh.Draw();
            }
        }
    }
}
