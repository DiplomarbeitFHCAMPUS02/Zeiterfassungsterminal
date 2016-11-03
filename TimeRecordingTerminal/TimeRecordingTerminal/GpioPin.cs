using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Media;
using TimeRecordingTerminal;

namespace TimeRecordingTerminal
{
    /// <summary>
    /// A class to control the GPIO Pins on a Raspberry PI
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Function in <see cref="GPIOPin"/> to play a sound.
        /// </summary>
        /// <param name="pin">The Pin of the Buzzer (GPIO PIN!)</param>
        /// <param name="duration">The time how long a single tone should last for.</param>
        /// <param name="repeats">The humber of repeats</param>
        public static void playsound(int repeats)
        {
            for(int i=0;i<repeats;i++)
            {
                SoundPlayer sp = new SoundPlayer(System.IO.Directory.GetCurrentDirectory() + "/beep.wav");
                sp.PlaySync();
            }
        }
    }
}
