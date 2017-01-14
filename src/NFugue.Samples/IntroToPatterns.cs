using NFugue.Patterns;
using NFugue.Playing;
using System;

namespace NFugue.Samples
{

    /*	Introduction to Patterns
    Patterns are one of the fundamental units of music in JFugue. They can be manipulated in musically interesting ways.
    */
    [Title("C#3:Introduction to Patterns")]
    public class IntroToPatterns
    {
        public static void Run()
        {
            Pattern p1 = new Pattern("V0 I[Piano] Eq Ch. | Eq Ch. | Dq Eq Dq Cq");
            Pattern p2 = new Pattern("V1 I[Flute] Rw     | Rw     | GmajQQQ  CmajQ");

            using (Player player = new Player())
            {
                player.Play(p1, p2);
            }
        }
    }
}