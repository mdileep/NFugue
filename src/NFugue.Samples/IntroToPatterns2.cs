using NFugue.Midi;
using NFugue.Patterns;
using NFugue.Playing;
using System;

namespace NFugue.Samples
{

    /*	Introduction to Patterns, Part 2
    Voice and instruments for a pattern can also be set through the API.
    In JFugue, methods that would normally return 'void' instead return the object itself,
    which allows you do chain commands together, as seen in this example.
    */
    [Title("C#3:Introduction to Patterns 2")]
    public class IntroToPatterns2
    {

        public static void Run()
        {
            Pattern p1 = new Pattern("Eq Ch. | Eq Ch. | Dq Eq Dq Cq").SetVoice(0)
                                                                     .SetInstrument(Instrument.AcousticGrandPiano);

            Pattern p2 = new Pattern("Rw     | Rw     | GmajQQQ  CmajQ").SetVoice(1)
                                                                        .SetInstrument(Instrument.Flute);
            using (Player player = new Player())
            {
                player.Play(p1, p2);
            }
        }
    }
}