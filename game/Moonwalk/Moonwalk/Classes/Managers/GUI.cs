using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Moonwalk.Classes.Boss;
using Moonwalk.Classes.Entities;
using Moonwalk.Classes.Entities.Base;
using Moonwalk.Classes.Helpful_Stuff;
using Moonwalk.Classes.Maps;
using Moonwalk.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Moonwalk.Classes.Managers {

    internal static class GUI {
        private const string RootFontDirectory = "Fonts/";
        private const string RootGUISpriteDirectory = "GUI/";

        private static Dictionary<string, SpriteFont> fonts = new();
        private static Dictionary<string, Texture2D> textures = new();

        private static Assortment<GUIElement> elements = new(new List<System.Type>() {typeof(GUIElement), typeof(GUIButtonElement)});

        public static List<GUIElement> GUIElementList { get { return elements.ToList(); } }

        // For managing displayed GUI elements
        public static void AddElement(GUIElement element) { elements.Add(element); }
        public static void RemoveElement(GUIElement element) { elements.Remove(element); }
        public static List<GUIElement> GetElement<T>() {
            return elements.GetAllOfType<T>().Cast<GUIElement>().ToList();
        }
        public static void Clear() { elements = new(); }


        // File content loading
        public static SpriteFont GetFont(string fontName) {
            // Load font if needed
            if (!fonts.ContainsKey(fontName)) 
                fonts.Add(fontName, Loader.LoadFont(RootFontDirectory + fontName));

            return fonts[fontName];
        }

        public static Texture2D GetTexture(string textureName) {
            // Load texture if needed
            if (!textures.ContainsKey(textureName))
                textures.Add(textureName, Loader.LoadTexture(RootGUISpriteDirectory + textureName));

            return textures[textureName];
        }

        public static void Draw(SpriteBatch batch) {
            foreach (GUIElement element in elements)
            {
                element.Draw(batch);
            }
                
        }
    }


    #region GUIElements
    public abstract class GUIElement {
        public abstract void Draw(SpriteBatch batch);
    }

    /// <summary>
    /// Text GUI whose text changes based on what the user types
    /// </summary>
    internal class GUITextField : GUIElement
    {
        private Vector2 position;
        private string text = "";
        private SpriteFont font;
        private Color color;

        public string Text
        {
            get { return text; }
        }

        public GUITextField(Vector2 position, string fontName, Color color)
            : base()
        {
            this.position = position;
            this.font = GUI.GetFont(fontName);
            this.color = color;
        }

        public void Input(StoredInput input)
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (input.IsPressed(key)
                    && !input.WasPressed(key))
                {
                    if (key != Keys.Delete && key != Keys.Enter && key != Keys.LeftShift && key != Keys.Back
                        && key != Keys.D1 && key != Keys.D2 && key != Keys.D3)
                    {
                        if (input.IsPressed(Keys.LeftShift))
                        {
                            text += key.ToString();
                        }
                        else
                        {
                            text += key.ToString().ToLower();
                        }
                        
                    }
                    else if (key == Keys.Delete || key == Keys.Back)
                    {
                        text = text.Substring(0, text.Length - 1);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.DrawString(font, text, position, color);
        }
    }

    /// <summary>
    /// A simple GUI element that draws text on the screen
    /// </summary>
    public class GUITextElement : GUIElement {
        private Vector2 position;
        private string text;
        private SpriteFont font;
        private Color color;        

        public string Text
        {
            get { return text; }
        }

        public GUITextElement(Vector2 position, ref string text, string fontName, Color color) {
            this.position = position;
            this.text = text;
            this.color = color;

            font = GUI.GetFont(fontName);
        }

        public GUITextElement(Vector2 position, string text, string fontName, Color color)
        {
            this.position = position;
            this.text = text;
            this.color = color;

            font = GUI.GetFont(fontName);
        }

        public override void Draw(SpriteBatch batch) {
            batch.DrawString(font, text, position, color);
        }
    }    

    /// <summary>
    /// A simple GUI element that draws a Texture2D on the screen
    /// </summary>
    public class GUITextureElement : GUIElement {
        protected Rectangle plane;
        protected Texture2D texture;
        protected Color color;

        public GUITextureElement(Rectangle plane, string textureName, Color color) {
            this.plane = plane;
            this.texture = GUI.GetTexture(textureName);
            this.color = color;
        }


        /*
        public GUITextureElement(Vector2 position, string textureName, Color color) :
            this(new Rectangle((int)position.X, (int)position.Y, 1, 1), textureName, color) { }
        */

        public override void Draw(SpriteBatch batch) {
            batch.Draw(texture, plane, color);
        }
    }

    public class GUIButtonElement : GUITextureElement {
        protected bool clicked;
        public bool Clicked { get { return clicked; } }
         
        public GUIButtonElement(Rectangle plane, string textureName, Color color) 
                : base(plane, textureName, color) {
            clicked = false;
            StoredInput.UserClick += CheckIfClicked;
        }

        public void CheckIfClicked(Point mouse) {
            if (plane.Contains(mouse))
                clicked = true;
        } 
    }

    /// <summary>
    /// Group of GUI elements. Everything in it has a relative position
    /// </summary>
    public class GUIGroup : GUIElement
    {
        Vector2 position;
        List<GUIElement> elements;

        public Vector2 Position
        {
            get { return position; }
        }

        public List<GUIElement> Elements 
        { get { return elements; } }

        public GUIGroup(Vector2 position) : base()
        { 
            this.position = position;
            elements = new List<GUIElement>();
        }

        public override void Draw(SpriteBatch batch)
        {
            foreach (GUIElement element in elements)
            {
                element.Draw(batch);
            }
        }

        public GUIElement Add(object[] args)
        {
            GUIElement element = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is Vector2)
                {
                    //relative position
                    args[i] = (Vector2)args[i] + position;
                }
            }

            Type[] types = typeof(GUIElement).GetTypeInfo().Assembly.GetTypes();

            foreach (Type type in types)
            {
                if (type.IsAssignableTo(typeof(GUIElement)))
                {
                    try
                    {
                        element = (GUIElement)Activator.CreateInstance(type, args);

                        elements.Add(element);
                    }
                    catch
                    {

                    }
                }
            }

            return element;
        }

        public void Remove(GUIElement element)
        {
            elements.Remove(element);
        }
    }
    #endregion
}
