using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class TextureContainer
    {
        Dictionary<String, Texture> textureSet;
        Texture nullTexture;
        public TextureContainer()
        {
            textureSet = new Dictionary<string, Texture>();
            nullTexture = new Texture("Graphics/DebugTextures/NullTexture.png");
        }

        /// <summary>
        /// Loads all textures in graphics directory for accessing during runtime
        /// </summary>
        public void LoadTextures()
        {
            try
            {
                List<string> textureFileNames = new List<string>(Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "/Graphics","*.png",SearchOption.AllDirectories));
                foreach(string textureFileName in textureFileNames)
                {
                    
                    textureSet.Add(Path.GetFileNameWithoutExtension(textureFileName), new Texture(textureFileName));
                    Console.WriteLine("Loaded texture" + Path.GetFileNameWithoutExtension(textureFileName));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Returns a texture based on name given, will return placeholder texture if not found
        /// </summary>
        /// <param name="textureName"></param>
        /// <returns></returns>
        public Texture GetTexture(string textureName)
        {
            if(textureSet.TryGetValue(textureName, out Texture returnTexture))
            {
                return returnTexture;
            }
            else
            {
                return nullTexture;
            }
        }
        public Sprite GetTextureAsSprite(string textureName)
        {
            Texture returnTexture;
            textureSet.TryGetValue(textureName, out returnTexture);
            if (returnTexture != null)
            {
                return new Sprite(returnTexture);
            }
            else
            {
                return new Sprite(nullTexture);
            }
        }
    }
}
