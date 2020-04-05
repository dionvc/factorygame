using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Audio;

namespace EngineeringCorpsCS
{
    class StaticSoundManager
    {
        static Music music = new Music("Sound/peculiartheme.ogg");
        public static void SetGlobalVolume(string tag, float value)
        {
            Listener.GlobalVolume = value;
        }

        public static void Play()
        {
            Listener.GlobalVolume = 100.0f;
            Console.WriteLine(Enum.GetName(typeof(SoundStatus), music.Status));
            Console.WriteLine(music.Duration.AsSeconds());
            music.Play();
            Console.WriteLine(Enum.GetName(typeof(SoundStatus), music.Status));
        }
    }
}
