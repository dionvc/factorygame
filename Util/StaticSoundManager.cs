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
        static Music music;
        public static void SetGlobalVolume(string tag, float value)
        {
            Listener.GlobalVolume = value;
        }

        public static void PlayMusic()
        {
            Listener.GlobalVolume = 100.0f;
            
            if (music == null || music.Status == SoundStatus.Stopped)
            {
                if(music == null)
                {
                    music = new Music("Sound/peculiartheme.ogg");
                }
                music.PlayingOffset = Time.Zero;
                music.Play();
                music.Position = new Vector3f(0, 0, 0);
                music.RelativeToListener = true;
                Console.WriteLine(music.Volume);
                Console.WriteLine(music.Duration.AsSeconds());
                Console.WriteLine("Music: " + music.Position.X + ":" + music.Position.Z);
                Console.WriteLine("Listener: " + Listener.Position.X + ":" + Listener.Position.Z);
            }
            Console.WriteLine(Enum.GetName(typeof(SoundStatus), music.Status));
        }

        public static void StopMusic()
        {
            if(music.Status == SoundStatus.Playing)
            {
                music.Stop();
                Console.WriteLine(Enum.GetName(typeof(SoundStatus), music.Status));
            }
        }
    }
}
