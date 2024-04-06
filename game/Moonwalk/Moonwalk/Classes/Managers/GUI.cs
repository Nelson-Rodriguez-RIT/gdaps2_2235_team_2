﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;


namespace Moonwalk.Classes.Managers {

    internal static class GUI {
        private const string RootFontDirectory = "Fonts/";
        private const string RootGUISpriteDirectory = "GUI/";

        private static Dictionary<string, SpriteFont> fonts = new();
        private static Dictionary<string, Texture2D> textures = new();

        private static LinkedList<GUIElement> elements = new();


        // For managing displayed GUI elements
        public static void AddElement(GUIElement element) { elements.AddLast(element); }
        public static void RemoveElement(GUIElement element) { elements.Remove(element); }
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
                element.Draw(batch);
        }
    }


    #region GUIElements

    public abstract class GUIElement {
        public abstract void Draw(SpriteBatch batch);
    }

    /// <summary>
    /// A simple GUI element that draws text on the screen
    /// </summary>
    public class GUITextElement : GUIElement {
        private Vector2 position;
        private string text;
        private SpriteFont font;
        private Color color;


        public GUITextElement(Vector2 position, string text, string fontName, Color color) {
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
        private Rectangle plane;
        private Texture2D texture;
        private Color color;

        public GUITextureElement(Rectangle plane, string textureName, Color color) {
            this.plane = plane;
            this.texture = GUI.GetTexture(textureName);
            this.color = color;
        }

        public GUITextureElement(Vector2 position, string textureName, Color color) :
            this(new Rectangle((int)position.X, (int)position.Y, 1, 1), textureName, color) { }

        public override void Draw(SpriteBatch batch) {
            batch.Draw(texture, plane, color);
        }
    }

    /// <summary>
    /// A GUI element that uses pointers to access data at a specific variable,
    /// allow for values to be dynamic and change in real time. Outputed via text
    /// <summary>
    public unsafe class GUIDynamicTextElement : GUIElement {
        private Vector2 position;
        // Using pointers because I want to access data inside an object
        // that is dynamic in order to have constantly update value.
        // Additional, literals can be used for static data
        private void*[] pDynamicValues = null;

        private string fontName;

        public GUIDynamicTextElement(Vector2 position, string fontName, void*[] values) {
            this.position = position;
            this.fontName = fontName;

            pDynamicValues = new void*[values.Length];
            for (int index = 0; index < pDynamicValues.Length; index++)
                pDynamicValues[index] = values[index];
        }

        public override void Draw(SpriteBatch batch) {
            string output = "";

            foreach (void* pValue in pDynamicValues)
                output += (*(object*)pValue).ToString();

            batch.DrawString(GUI.GetFont(fontName), output, position, Color.White);
        }
    }

    #endregion
}
