namespace NFugue.Samples
{


    /*	Anticipate Musical Events Before They Occur
    You might imagine creating new types of ParserListeners,like an AnimationParserListener,
    that depend on knowing about the musical events before they happen.For example,perhaps
    your animation is of a robot playing a drum or strumming a guitar.Before the note makes
    a sound,the animation needs to get its virtual hands in the right place,so you might
    want a notice 500ms earlier that a musical event is about to happen.To bend time with JFugue,
    use a combination of the TemporalPLP class and Player.delayPlay(). delayPlay() creates a
    new thread that first waits the specified amount of time before playing. If you do this,
    make sure to call delayPlay() before plp.parse().
    */

    public class TemporalExample
    {

        private const string MUSIC = "C D E F G A B";
        private const long TEMPORAL_DELAY = 500;

        public static void Run()
        {
            // Part 1. Parse the original music
            //StaccatoParser parser = new StaccatoParser();
            //TemporalPLP plp = new TemporalPLP();

            //parser.addParserListener(plp);
            //parser.parse(MUSIC);

            //// Part 2. Send the events from Part 1, and play the original music with a delay
            //DiagnosticParserListener dpl = new DiagnosticParserListener(); // Or your AnimationParserListener!

            //plp.addParserListener(dpl);
            //new Player().delayPlay(TEMPORAL_DELAY, MUSIC);
            //plp.parse();
        }
    }
}