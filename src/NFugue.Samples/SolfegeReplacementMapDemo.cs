using NFugue.Patterns;
using NFugue.Playing;
using NFugue.Staccato.Preprocessors;
using System;

namespace NFugue.Samples
{
    /*	Use"Replacement Maps" to Play Solfege
    JFugue comes with a SolfegeReplacementMap,which means you can program music using
    "Do Re Me Fa So La Ti Do."The ReplacementMapParser converts those solfege tones to
    C D E F G A B.Using Replacement Maps,which are simply Map<String,String>,you can
    create any kind of music in a pattern that will be converted to musical notes
    (or whatever you put in the values of your Map).
    */
    [Title(@"C#7:Use ""Replacement Maps"" to Play Solfege")]
    public class SolfegeReplacementMapDemo
    {

        public static void Run()
        {
            ReplacementMapPreprocessor Processor = new ReplacementMapPreprocessor();
            Processor.ReplacementMap = ReplacementMaps.SolfegeReplacementMap;
            Processor.RequiresAngleBrackets = false;
            Processor.IsCaseSensitive = false;

            using (Player player = new Player())
            {
                player.Play(new Pattern(Processor.Preprocess("do re mi fa so la ti do", null))); // This will play "C D E F G A B"

                // This next example brings back the brackets so durations can be added
                Processor.RequiresAngleBrackets = true;
                player.Play(new Pattern(Processor.Preprocess("<Do>q <Re>q <Mi>h | <Mi>q <Fa>q <So>h | <So>q <Fa>q <Mi>h | <Mi>q <Re>q <Do>h", null)));
            }
        }
    }
}