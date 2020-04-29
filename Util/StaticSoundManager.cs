using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;
using SFML.System;

namespace EngineeringCorpsCS
{
    
    class StaticSoundManager
    {
        public static Dictionary<string, SoundBuffer> SFX = new Dictionary<string, SoundBuffer>();
        public static List<Music> ambience = new List<Music>();
        public static List<Music> music = new List<Music>();
        public static int soundMax = 64;
        public static int currentMusic = 0;
        public static int currentAmbience = 0;
        public static Sound[] soundArray = new Sound[soundMax];
        static Random soundSelector = new Random(System.DateTime.Now.Second);
        public static void LoadSounds()
        {
            music.Add(new Music("Sound/Music/peculiartheme.ogg"));
            music.Add(new Music("Sound/Music/darktheme.ogg"));
            music.Add(new Music("Sound/Music/resttheme.ogg"));
            music.Add(new Music("Sound/Music/spacetheme.ogg"));
            music.Add(new Music("Sound/Music/stonetheme.ogg"));
            music.Add(new Music("Sound/Music/vangelistheme.ogg"));

            ambience.Add(new Music("Sound/Ambience/AmbienceDark.ogg"));
            ambience.Add(new Music("Sound/Ambience/AmbienceWind.ogg"));

            SFX.Add("Pick1", new SoundBuffer("Sound/SFX/Pick1.ogg"));
            SFX.Add("Pick2", new SoundBuffer("Sound/SFX/Pick2.ogg"));
            SFX.Add("Pick3", new SoundBuffer("Sound/SFX/Pick3.ogg"));
            SFX.Add("Chop1", new SoundBuffer("Sound/SFX/Chop1.ogg"));
            SFX.Add("Chop2", new SoundBuffer("Sound/SFX/Chop2.ogg"));
            SFX.Add("Chop3", new SoundBuffer("Sound/SFX/Chop3.ogg"));
            SFX.Add("Pickup", new SoundBuffer("Sound/SFX/PickUpSound.ogg"));
        }

        public static void SetGlobalVolume(string tag, float value)
        {
            Listener.GlobalVolume = value;
        }

        public static void PlayMusic()
        {
            Listener.GlobalVolume = 100.0f;
            if (music[currentMusic].Status == SoundStatus.Stopped)
            {
                currentMusic++;
                if(currentMusic>= music.Count)
                {
                    currentMusic = 0;
                }
                music[currentMusic].Volume = 0.5f;
                music[currentMusic].Play();
                music[currentMusic].PlayingOffset = Time.Zero;

                music[currentMusic].Position = new Vector3f(0, 0, 0);
                music[currentMusic].RelativeToListener = true;
                    //Console.WriteLine(music.Volume);
                    //Console.WriteLine(music.Duration.AsSeconds());
                    //Console.WriteLine("Music: " + music.Position.X + ":" + music.Position.Z);
                    //Console.WriteLine("Listener: " + Listener.Position.X + ":" + Listener.Position.Z);
            }
            //Console.WriteLine(Enum.GetName(typeof(SoundStatus), music.Status));
        }


        public static void StopMusic()
        {
            if(music[currentMusic].Status == SoundStatus.Playing)
            {
                music[currentMusic].Stop();
                //Console.WriteLine(Enum.GetName(typeof(SoundStatus), music[currentMusic].Status));
            }
        }

        public static void PlayAmbience()
        {
            if(ambience[currentAmbience].Status == SoundStatus.Stopped)
            {
                currentAmbience++;
                if (currentAmbience >= ambience.Count)
                {
                    currentAmbience = 0;
                }
                ambience[currentAmbience].Volume = 0.4f;
                ambience[currentAmbience].Play();
                ambience[currentAmbience].PlayingOffset = Time.Zero;
                ambience[currentAmbience].Play();
                ambience[currentAmbience].Position = new Vector3f(0, 0, 0);
                ambience[currentAmbience].RelativeToListener = true;
                
            }
        }
        public static void StopAmbience()
        {
            if (ambience[currentAmbience].Status == SoundStatus.Playing)
            {
                ambience[currentAmbience].Stop();
                //Console.WriteLine(Enum.GetName(typeof(SoundStatus), music[currentMusic].Status));
            }
        }

        public static void PlaySound(Vector2 position, string name)
        {
            SoundBuffer buffer;
            if (SFX.TryGetValue(name, out buffer)) {
                for (int i = 0; i < soundArray.Length; i++)
                {
                    if (soundArray[i] == null || soundArray[i].Status == SoundStatus.Stopped)
                    {
                        soundArray[i] = new Sound(buffer);
                        soundArray[i].Volume = 150.0f;
                        soundArray[i].Attenuation = 2.0f;
                        soundArray[i].Position = new Vector3f(position.x, 0, position.y);
                        soundArray[i].Play();
                    }
                }
            }
        }
        public static void PlaySound(Vector2 position, string[] names)
        {
            if(names.Length == 0)
            {
                return;
            }
            SoundBuffer buffer;
            if (SFX.TryGetValue(names[soundSelector.Next(0, names.Length)], out buffer))
            {
                for (int i = 0; i < soundArray.Length; i++)
                {
                    if (soundArray[i] == null || soundArray[i].Status == SoundStatus.Stopped)
                    {
                        if (soundArray[i] != null)
                        {
                            soundArray[i].Dispose();
                        }
                        soundArray[i] = new Sound(buffer);
                        soundArray[i].Volume = 150.0f;
                        soundArray[i].Attenuation = 2.0f;
                        soundArray[i].Position = new Vector3f(position.x, 0, position.y);
                        soundArray[i].Play();

                    }
                }
            }
        }
    }
}
