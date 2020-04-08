using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SFML.Graphics;

namespace EngineeringCorpsCS
{
    class TextureAtlas
    {
        public Dictionary<string, IntRect> textures { get; }
        public Texture atlas { get; }
        public bool full;
        public TextureAtlas(uint width, uint height)
        {
            full = false;
            atlas = new Texture(width, height);
            textures = new Dictionary<string, IntRect>();
        }
    }
    class TextureAtlases
    {
        Dictionary<string, int> textureAtlasIndex;
        List<TextureAtlas> textureAtlases;
        Texture nullTexture;

        /// <summary>
        /// A textureAtlas collection.  Packs all png graphics resources into textureAtlases.  Contains null texture provision.
        /// </summary>
        public TextureAtlases()
        {
            textureAtlases = new List<TextureAtlas>();
            textureAtlasIndex = new Dictionary<string, int>();
            textureAtlases.Add(new TextureAtlas(4096, 4096));
            nullTexture = new Texture("Graphics/DebugTextures/NullTexture.png");
        }

        /// <summary>
        /// Load the textures into the texture container and pack them into texture atlases
        /// </summary>
        public void LoadTextures(string[] packTogether)
        {
            //Get subdirectories
            string[] directories = Directory.GetDirectories(Directory.GetCurrentDirectory() + Props.graphicsPath);
            for(int i = 0; i < directories.Length; i++)
            {

                if (packTogether.Contains(new DirectoryInfo(directories[i]).Name))
                {
                    Console.WriteLine("Packing Alone: \n\t" + Path.GetDirectoryName(directories[i]));
                    PackSubdirectoryAlone(directories[i], false, true);
                }
                else
                {
                    //Pack the subdirectory into the atlases
                    Console.WriteLine("Packing Regularly: \n\t" + directories[i]);
                    PackSubdirectory(directories[i]);
                }
            }
        }

        /// <summary>
        /// Returns the atlas texture and outputs the bounds within the texture that the desired image is located
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bounds"></param>
        /// <returns></returns>
        public Texture GetTexture(string name, out IntRect bounds)
        {
            int index;
            if(textureAtlasIndex.TryGetValue(name, out index)) 
            {
                if (textureAtlases[index].textures.TryGetValue(name, out bounds))
                {
                    return textureAtlases[index].atlas;
                }
            }
            bounds = new IntRect(0, 0, (int)nullTexture.Size.X, (int)nullTexture.Size.Y);
            return nullTexture;
        }

        public Sprite GetTextureAsSprite(string name)
        {
            int index;
            IntRect bounds;
            if (textureAtlasIndex.TryGetValue(name, out index))
            {
                if (textureAtlases[index].textures.TryGetValue(name, out bounds))
                {
                    return new Sprite(textureAtlases[index].atlas, bounds);
                }
            }
            return new Sprite(nullTexture);
        }

        /// <summary>
        /// Attempts to group the subdirectory and its subdirectories into as few texture atlases as possible.
        /// Uses greedy packing implementation.
        /// </summary>
        /// <param name="directoryPath"></param>
        private void PackSubdirectory(string directoryPath)
        {
            //First get the images
            List<KeyValuePair<String, Image>> textureSet = new List<KeyValuePair<string, Image>>();
            List<string> textureFileNames = new List<string>(Directory.EnumerateFiles(directoryPath, "*.png", SearchOption.AllDirectories));
            foreach (string textureFileName in textureFileNames)
            {
                textureSet.Add(new KeyValuePair<string,Image>(Path.GetFileNameWithoutExtension(textureFileName), new Image(textureFileName)));
                Console.WriteLine("\tLoaded image" + Path.GetFileNameWithoutExtension(textureFileName));
            }
            //Sort the images by area
            textureSet.Sort(delegate (KeyValuePair<string, Image> kvp1, KeyValuePair<string, Image> kvp2)
            {
                uint a1 = kvp1.Value.Size.X * kvp1.Value.Size.Y;
                uint a2 = kvp2.Value.Size.X * kvp2.Value.Size.Y;
                return (int) (a2 - a1);
            });
            //Write the sorted sizes to console
            foreach (KeyValuePair<string, Image> kvp in textureSet)
            {
                Console.WriteLine("Size: " + (kvp.Value.Size.X * kvp.Value.Size.Y) + " : " + kvp.Key);
            }
            //Next, render the images into textures starting with the largest using scanline algo
            foreach (KeyValuePair<string, Image> kvp in textureSet)
            {
                //Calculate the bounding box for the image
                IntRect place = new IntRect(0, 0, (int)kvp.Value.Size.X, (int)kvp.Value.Size.Y);
                int curIndex = 0;
                //Start with a nonfull textureAtlas
                while(textureAtlases[curIndex].full == true)
                {
                    curIndex++;
                    if(curIndex >= textureAtlases.Count)
                    {
                        textureAtlases.Add(new TextureAtlas(Props.textureAtlasSize, Props.textureAtlasSize));
                    }
                }
                bool placed = false;
                //Try to keep placing the texture
                while (placed == false)
                {
                    if (textureAtlases[curIndex].textures.Count == 0) //If there is no other present textures, pack the texture immediately
                    {
                        textureAtlases[curIndex].atlas.Update(kvp.Value, 0, 0);
                        textureAtlases[curIndex].textures.Add(kvp.Key, place);
                        textureAtlasIndex.Add(kvp.Key, curIndex);
                        placed = true;
                        if (place.Left + place.Width > Props.textureAtlasSize - Props.textureAtlasFullMargin &&
                             place.Top + place.Height > Props.textureAtlasSize - Props.textureAtlasFullMargin)
                        {
                            textureAtlases[curIndex].full = true;
                        }
                    }
                    else
                    {
                        for (uint curX = 0; curX < Props.textureAtlasSize - place.Width; curX++)
                        {
                            for (uint curY = 0; curY < Props.textureAtlasSize - place.Height; curY++)
                            {
                                place.Left = (int)curX;
                                place.Top = (int)curY;
                                bool collided = false;
                                foreach (KeyValuePair<string, IntRect> rect in textureAtlases[curIndex].textures)
                                {
                                    if (place.Intersects(rect.Value))
                                    {
                                        collided = true;
                                    }
                                }
                                if (collided == false)
                                {
                                    textureAtlases[curIndex].atlas.Update(kvp.Value, curX, curY);
                                    textureAtlases[curIndex].textures.Add(kvp.Key, place);
                                    textureAtlasIndex.Add(kvp.Key, curIndex);
                                    curY = Props.textureAtlasSize;
                                    curX = Props.textureAtlasSize;
                                    placed = true;
                                    if(place.Left + place.Width > Props.textureAtlasSize - Props.textureAtlasFullMargin &&
                                        place.Top + place.Height > Props.textureAtlasSize - Props.textureAtlasFullMargin)
                                    {
                                        textureAtlases[curIndex].full = true;
                                    }
                                }
                            }
                        }
                    }
                    if(placed == false)
                    {
                        curIndex++;
                        if(curIndex >= textureAtlases.Count)
                        {
                            textureAtlases.Add(new TextureAtlas(4096, 4096));
                        }
                        while (textureAtlases[curIndex].full == true)
                        {
                            curIndex++;
                            if (curIndex >= textureAtlases.Count)
                            {
                                textureAtlases.Add(new TextureAtlas(Props.textureAtlasSize, Props.textureAtlasSize));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tries to pack the subdirectory alone into textureatlases.  Can be configured to throw if it needs more than one atlas.
        /// Can also mark the atlas as full even if it isn't upon finishing.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="markAsFull"></param>
        /// <param name="throwIfOverflow"></param>
        private void PackSubdirectoryAlone(string directoryPath, bool markAsFull, bool throwIfOverflow)
        {
            //First get the images
            List<KeyValuePair<String, Image>> textureSet = new List<KeyValuePair<string, Image>>();
            List<string> textureFileNames = new List<string>(Directory.EnumerateFiles(directoryPath, "*.png", SearchOption.AllDirectories));
            foreach (string textureFileName in textureFileNames)
            {
                textureSet.Add(new KeyValuePair<string, Image>(Path.GetFileNameWithoutExtension(textureFileName), new Image(textureFileName)));
                Console.WriteLine("\tLoaded image" + Path.GetFileNameWithoutExtension(textureFileName));
            }
            //Sort the images by area
            textureSet.Sort(delegate (KeyValuePair<string, Image> kvp1, KeyValuePair<string, Image> kvp2)
            {
                uint a1 = kvp1.Value.Size.X * kvp1.Value.Size.Y;
                uint a2 = kvp2.Value.Size.X * kvp2.Value.Size.Y;
                return (int)(a2 - a1);
            });
            //Write the sorted sizes to console
            foreach (KeyValuePair<string, Image> kvp in textureSet)
            {
                Console.WriteLine("Size: " + (kvp.Value.Size.X * kvp.Value.Size.Y) + " : " + kvp.Key);
            }
            //Next, render the images into textures starting with the largest using scanline algo
            int firstIndex = textureAtlases.Count;
            foreach (KeyValuePair<string, Image> kvp in textureSet)
            {
                //Calculate the bounding box for the image
                IntRect place = new IntRect(0, 0, (int)kvp.Value.Size.X, (int)kvp.Value.Size.Y);
                int curIndex = firstIndex;
                //Create a new textureAtlas to pack into
                if (curIndex >= textureAtlases.Count)
                {
                    textureAtlases.Add(new TextureAtlas(Props.textureAtlasSize, Props.textureAtlasSize));
                }
                bool placed = false;
                //Try to keep placing the texture
                while (placed == false)
                {
                    if (textureAtlases[curIndex].textures.Count == 0) //If there is no other present textures, pack the texture immediately
                    {
                        textureAtlases[curIndex].atlas.Update(kvp.Value, 0, 0);
                        textureAtlases[curIndex].textures.Add(kvp.Key, place);
                        textureAtlasIndex.Add(kvp.Key, curIndex);
                        placed = true;
                        if (place.Left + place.Width > Props.textureAtlasSize - Props.textureAtlasFullMargin &&
                             place.Top + place.Height > Props.textureAtlasSize - Props.textureAtlasFullMargin)
                        {
                            textureAtlases[curIndex].full = true;
                        }
                    }
                    else
                    {
                        for (uint curX = 0; curX < Props.textureAtlasSize - place.Width; curX++)
                        {
                            for (uint curY = 0; curY < Props.textureAtlasSize - place.Height; curY++)
                            {
                                place.Left = (int)curX;
                                place.Top = (int)curY;
                                bool collided = false;
                                foreach (KeyValuePair<string, IntRect> rect in textureAtlases[curIndex].textures)
                                {
                                    if (place.Intersects(rect.Value))
                                    {
                                        collided = true;
                                    }
                                }
                                if (collided == false)
                                {
                                    textureAtlases[curIndex].atlas.Update(kvp.Value, curX, curY);
                                    textureAtlases[curIndex].textures.Add(kvp.Key, place);
                                    textureAtlasIndex.Add(kvp.Key, curIndex);
                                    curY = Props.textureAtlasSize;
                                    curX = Props.textureAtlasSize;
                                    placed = true;
                                    if (place.Left + place.Width > Props.textureAtlasSize - Props.textureAtlasFullMargin &&
                                        place.Top + place.Height > Props.textureAtlasSize - Props.textureAtlasFullMargin)
                                    {
                                        textureAtlases[curIndex].full = true;
                                    }
                                }
                            }
                        }
                    }
                    if (placed == false)
                    {
                        if (throwIfOverflow)
                        {
                            throw new InvalidOperationException("The texture packing failed unexpectedly (or expectedly)! :( \n\tFailure Cause Directory:" + directoryPath);
                        }
                        else
                        {
                            if(markAsFull)
                            {
                                textureAtlases[curIndex].full = true;
                            }
                            curIndex++;
                            if (curIndex >= textureAtlases.Count)
                            {
                                textureAtlases.Add(new TextureAtlas(4096, 4096));
                            }
                            while (textureAtlases[curIndex].full == true)
                            {
                                curIndex++;
                                if (curIndex >= textureAtlases.Count)
                                {
                                    textureAtlases.Add(new TextureAtlas(Props.textureAtlasSize, Props.textureAtlasSize));
                                }
                            }
                        }
                    }
                }
            }
            textureAtlases[firstIndex].full = true;
        }
    }
}
