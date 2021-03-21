using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphicsProgramming
{
    class Les1 : Lesson
    {
        VertexPositionColor[] vertices =
        {
            new VertexPositionColor( new Vector3(-0.5f, 0.5f, 0.5f), Color.Red),
            new VertexPositionColor( new Vector3(0.5f, -0.5f, 0.5f), Color.Green),
            new VertexPositionColor( new Vector3(-0.5f, -0.5f, 0.5f), Color.Blue),
            new VertexPositionColor( new Vector3(0.5f, 0.5f, 0.5f), Color.Yellow)
        };

        int[] indices =
        {
            //triangle 1
            0,1,2,
            //triangle 2
            0,1,3

        };

        BasicEffect effect;
        public override void Initialize() 
        { 
        
        }
        
        public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch) 
        {
            effect = new BasicEffect(graphics.GraphicsDevice);
        }
        public override void Update(GameTime gameTime) 
        { 
        
        }
        public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch) 
        { 
        
        }
    }
}