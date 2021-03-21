using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GraphicsProgramming
{
	class Lesson3 : Lesson
	{
		private Effect effect;
		Vector3 LightPosition = Vector3.Right * 2 + Vector3.Up * 2 + Vector3.Backward * 2;

		Model sphere, cube;
		Texture2D day, night, clouds, moon;
		TextureCube sky;

		float yaw, pitch;
		int prevX, prevY;

		public override void Update( GameTime gameTime )
		{
			MouseState mstate = Mouse.GetState();

			if( mstate.LeftButton == ButtonState.Pressed )
			{
				yaw -= ( mstate.X - prevX ) * 0.005f;
				pitch -= ( mstate.Y - prevY ) * 0.005f;

				pitch = MathF.Min( MathF.Max( pitch, -MathF.PI * 0.45f ), MathF.PI * 0.45f );
			}

			prevX = mstate.X;
			prevY = mstate.Y;
		}

		public override void LoadContent( ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch )
		{
			effect = Content.Load<Effect>("Lesson3");

			day = Content.Load<Texture2D>( "day" );
			night = Content.Load<Texture2D>( "night" );
			clouds = Content.Load<Texture2D>( "clouds" );
			moon = Content.Load<Texture2D>( "2k_moon" );
			sky = Content.Load<TextureCube>( "sky_cube" );

			sphere = Content.Load<Model>( "uv_sphere" );
			cube = Content.Load<Model>( "cube" );

			LoadModelEffects( sphere );
			LoadModelEffects( cube );
		}

		public override void Draw( GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch )
		{
			GraphicsDevice device = graphics.GraphicsDevice;

			float time = ( float )gameTime.TotalGameTime.TotalSeconds;
			LightPosition = Vector3.Left * 200;

			Vector3 cameraPos = ( -Vector3.Forward * 10 );
			cameraPos = Vector3.Transform( cameraPos, Quaternion.CreateFromYawPitchRoll( yaw, pitch, 0 ) );

			Matrix World = Matrix.CreateWorld( Vector3.Zero, Vector3.Forward, Vector3.Up );
			Matrix View = Matrix.CreateLookAt( cameraPos, Vector3.Zero, Vector3.Up );

			SetEffectParameters( device, time, cameraPos, World, View );

			device.Clear( Color.Black );

			// Sky
			effect.CurrentTechnique = effect.Techniques["Sky"];
			device.DepthStencilState = DepthStencilState.None;
			device.RasterizerState = RasterizerState.CullNone;
			RenderModel( cube, Matrix.CreateTranslation( cameraPos ) );

			// Earth
			device.DepthStencilState = DepthStencilState.Default;
			device.RasterizerState = RasterizerState.CullCounterClockwise;
			effect.CurrentTechnique = effect.Techniques["Earth"];
			RenderModel( sphere, Matrix.CreateScale( 0.01f ) *
			Matrix.CreateRotationZ( time ) *
			Matrix.CreateRotationY( MathF.PI / 180 * 23 ) * World );

            // Moon
            effect.CurrentTechnique = effect.Techniques["Moon"];
            RenderModel(sphere, Matrix.CreateTranslation(Vector3.Down * 8) *
            Matrix.CreateScale(0.0033f) *
            Matrix.CreateRotationZ(time - time * 0.03333333f) * World);

            // Moon 2
            effect.CurrentTechnique = effect.Techniques["Moon"];
            RenderModel(sphere, Matrix.CreateTranslation(Vector3.Down * -16) *
            Matrix.CreateScale(0.0033f) *
            Matrix.CreateRotationZ(time - time * 0.6f) * World);
        }

		private void SetEffectParameters( GraphicsDevice device, float time, Vector3 cameraPos, Matrix World, Matrix View )
		{
			effect.Parameters["World"].SetValue( World );
			effect.Parameters["View"].SetValue( View );
			effect.Parameters["Projection"].SetValue( Matrix.CreatePerspectiveFieldOfView( ( MathF.PI / 180f ) * 25f, device.Viewport.AspectRatio, 0.001f, 1000f ) );

			effect.Parameters["DayTex"].SetValue( day );
			effect.Parameters["NightTex"].SetValue( night );
			effect.Parameters["CloudsTex"].SetValue( clouds );
			effect.Parameters["MoonTex"].SetValue( moon );
			effect.Parameters["SkyTex"].SetValue( sky );

			effect.Parameters["LightPosition"].SetValue( LightPosition );
			effect.Parameters["CameraPosition"].SetValue( cameraPos );

			effect.Parameters["Time"].SetValue( time );

			effect.CurrentTechnique.Passes[0].Apply();
		}

		public void RenderModel( Model m, Matrix parentMatrix )
		{
			Matrix[] transforms = new Matrix[m.Bones.Count];
			m.CopyAbsoluteBoneTransformsTo( transforms );

			effect.CurrentTechnique.Passes[0].Apply();

			foreach( ModelMesh mesh in m.Meshes )
			{
				effect.Parameters["World"].SetValue( parentMatrix * transforms[mesh.ParentBone.Index] );

				mesh.Draw();
			}
		}

		public void LoadModelEffects( Model m )
		{
			foreach( ModelMesh mesh in m.Meshes )
			{
				foreach( ModelMeshPart meshPart in mesh.MeshParts )
				{
					meshPart.Effect = effect;
				}
			}
		}
	}
}