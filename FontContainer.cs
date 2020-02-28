using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class FontContainer
    {
        Font nullFont;
        Dictionary<string, Font> fontSet;
        public FontContainer()
        {
            fontSet = new Dictionary<string, Font>();
            nullFont = new Font("Graphics/Fonts/SairaRegular.ttf");
        }
        public void LoadFonts()
        {
            try
            {
                List<string> textureFileNames = new List<string>(Directory.EnumerateFiles(Directory.GetCurrentDirectory() + "/Graphics/Fonts", "*.ttf", SearchOption.AllDirectories));
                foreach (string textureFileName in textureFileNames)
                {

                    fontSet.Add(Path.GetFileNameWithoutExtension(textureFileName), new Font(textureFileName));
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

        public Font GetFont(string fontName)
        {
            if(fontSet.TryGetValue(fontName, out Font returnFont))
            {
                return returnFont;
            }
            else
            {
                return nullFont;
            }
        }
    }
}
