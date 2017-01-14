using NFugue.Playing;
using NFugue.Rhythms;
using System;

namespace NFugue.Samples
{


    /*	Advanced Rhythms
        Through the Rhythm API, you can specify a variety of alternate layers that
        occur once or recur regularly. You can even create your own "RhythmAltLayerProvider"
        if you'd like to create a new behavior that does not already exist in the Rhythm API.
    */
    [Title("C#5:Advanced Rhythms")]
    public class AdvancedRhythms
    {

        public static void Run()
        {
            Rhythm rhythm = new Rhythm()
                                        .AddLayer("O..oO...O..oOO..") // This is Layer 0
                                        .AddLayer("..S...S...S...S.")
                                        .AddLayer("````````````````")
                                        .AddLayer("...............+") // This is Layer 3
                                        .AddOneTimeAltLayer(3, 3, "...+...+...+...+"); // Replace Layer 3 with this string on the 4th (count from 0) measure
                                                                                       //.SetLength(4); // Set the length of the rhythm to 4 measures
            rhythm.Length = 4;

            using (Player player = new Player())
            {
                player.Play(rhythm.GetPattern()
                                       .Repeat(2)); // Play 2 instances of the 4-measure-long rhythm
            }
        }
    }
}