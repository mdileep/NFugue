using NFugue.Midi;
using NFugue.Patterns;
using NFugue.Playing;
using NFugue.Rhythms;
using NFugue.Staccato.Preprocessors;
using NFugue.Theory;
using System;
using System.Collections.Generic;
/*
Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
*/
namespace NFugue.Samples
{
    public class HelloWorld
    {
        public static void Go()
        {
            Console.WriteLine("Hello World");

            using (Player player = new Player())
            {
                player.Play("C D E F G A B ");
            }
        }
    }

    /*	Playing multiple voices, multiple instruments, rests, chords, and durations
    This example uses the Staccato 'V' command for specifing voices, 'I' for specifying instruments
    (text within brackets is looked up in a dictionary and maps to MIDI instrument numbers),
    '|' (pipe) for indicating measures (optional), durations including 'q' for quarter duration,
    'qqq' for three quarter notes (multiple durations can be listed together), and 'h' for half,
    'w' for whole, and '.' for a dotted duration; 'R' for rest, and the chords G-Major and C-Major.
    Whitespace is not significant and can be used for visually pleasing or helpful spacing.
    */
    public class HelloWorld2
    {

        public static void Go()
        {
            Console.WriteLine("Hello World2");

            using (Player player = new Player())
            {
                player.Play("V0 I[Flute] Eq Ch. | Eq Ch. | Dq Eq Dq Cq   V1 I[Flute] Rw | Rw | GmajQQQ CmajQ");
            }
        }
    }
   
    /*	Introduction to Patterns
    Patterns are one of the fundamental units of music in JFugue. They can be manipulated in musically interesting ways.
    */

    public class IntroToPatterns
    {

        public static void Go()
        {
            Console.WriteLine("Introduction to Patterns");

            Pattern p1 = new Pattern("V0 I[Piano] Eq Ch. | Eq Ch. | Dq Eq Dq Cq");
            Pattern p2 = new Pattern("V1 I[Flute] Rw     | Rw     | GmajQQQ  CmajQ");

            using (Player player = new Player())
            {
                player.Play(p1, p2);
            }
        }
    }

    /*	Introduction to Patterns, Part 2
    Voice and instruments for a pattern can also be set through the API.
    In JFugue, methods that would normally return 'void' instead return the object itself,
    which allows you do chain commands together, as seen in this example.
    */

    public class IntroToPatterns2
    {

        public static void Go()
        {
            Console.WriteLine("Introduction to Patterns 2");

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


    /*	Introduction to Chord Progressions
    It's easy to create a Chord Progression in JFugue. You can then play it,
    or you can see the notes that comprise the any of the chords in the progression.
    */

    public class IntroToChordProgressions
    {

        public static void Go()
        {
            Console.WriteLine("Introduction to Chord Progressions");

            ChordProgression cp = new ChordProgression("I IV V");

            Chord[] chords = cp.SetKey("C")
                               .GetChords();

            foreach (Chord chord in chords)
            {
                Console.Write("Chord " + chord + " has these notes: ");
                Note[] notes = chord.GetNotes();

                foreach (Note note in notes)
                {
                    Console.Write(note + " ");
                }
                Console.WriteLine();
            }

            using (Player player = new Player())
            {
                player.Play(cp);
            }
        }
    }


    /*	Advanced Chord Progressions
    You can do some pretty cool things with chord progressions.
    The methods below use $ to indicate an index into either the chord progression
    (in which case, the index points to the nth chord), or a specific chord
    (in which case the index points to the nth note of the chord). Underscore means
    "the whole thing". If you change the indexes, make sure you don't introduce an
    ArrayOutOfBoundsException (for example, a major chord has only three notes, so
    trying to get the 4th index, $3 (remember that this is zero-based), would cause an error).
    */

    public class AdvancedChordProgressions
    {

        public static void Go()
        {
            Console.WriteLine("Advanced Chord Progressions");

            ChordProgression cp = new ChordProgression("I IV V");

            using (Player player = new Player())
            {
                player.Play(cp.EachChordAs("$0q $1q $2q Rq"));

                player.Play(cp.AllChordsAs("$0q $0q $0q $0q $1q $1q $2q $0q"));

                player.Play(cp.AllChordsAs("$0 $0 $0 $0 $1 $1 $2 $0")
                              .EachChordAs("V0 $0s $1s $2s Rs V1 $_q"));
            }
        }
    }


    /*	Twelve-Bar Blues in Two Lines of Code
    Twelve-bar blues uses a I-IV-V chord progression.
    But really, it's the Major 7ths that you'd like to hear...
    and if you want to play each chord in arpeggio, you need a 6th
    in there as well. But creating a I7%6-IV7%6-V7%6 chord progression is messy.
    So, this code creates a I-IV-V progression, then distributes a 7%6 across
    each chord, then creates the twelve bars, and then each chord is played
    as an arpeggio with note dynamics (note on velocity - how hard you hit the note).
    Finally, the pattern is played with an Acoustic_Bass instrument at 110 BPM.
    With all of the method chaining, that is kinda done in one line of code.
    */

    public class TwelveBarBlues
    {

        public static void Go()
        {
            Console.WriteLine("Twelve-Bar Blues in Two Lines of Code");

            Pattern pattern = new ChordProgression("I IV V").Distribute("7%6")
                                                            .AllChordsAs("$0 $0 $0 $0 $1 $1 $0 $0 $2 $1 $0 $0")
                                                            .EachChordAs("$0ia100 $1ia80 $2ia80 $3ia80 $4ia100 $3ia80 $2ia80 $1ia80")
                                                            .GetPattern()
                                                            .SetInstrument(Instrument.AcousticBass)
                                                            .SetTempo(100);
            using (Player player = new Player())
            {
                player.Play(pattern);
            }

        }
    }


    /*	Introduction to Rhythms
    One of my favorite parts of the JFugue API is the ability to create rhythms
    in a fun and easily understandable way.The letters are mapped to percussive
    instrument sounds,like"Acoustic Snare"and"Closed Hi Hat".JFugue comes with a
    default"rhythm set",which is a Map<Character, String>with entries like this:put('O',"[BASS_DRUM]i").
    */

    public class IntroToRhythms
    {

        public static void Go()
        {
            Console.WriteLine("Introduction to Rhythms");

            Rhythm rhythm = new Rhythm().AddLayer("O..oO...O..oOO..")
                                        .AddLayer("..S...S...S...S.")
                                        .AddLayer("````````````````")
                                        .AddLayer("...............+");

            using (Player player = new Player())
            {
                player.Play(rhythm.GetPattern()
                                       .Repeat(2));
            }
        }
    }


    /*	Advanced Rhythms
        Through the Rhythm API, you can specify a variety of alternate layers that
        occur once or recur regularly. You can even create your own "RhythmAltLayerProvider"
        if you'd like to create a new behavior that does not already exist in the Rhythm API.
    */

    public class AdvancedRhythms
    {

        public static void Go()
        {
            Console.WriteLine("Advanced Rhythms");

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

    /*	All That, in One Line of Code?
        Try this. The main line of code even fits within the 140-character limit of a tweet.
    */

    public class TryThis
    {

        public static void Go()
        {

            new Player().Play(new ChordProgression("I IV vi V").EachChordAs("$_i $_i Ri $_i"), new Rhythm().AddLayer("..X...X...X...XO"));
        }
    }

    /*	See the Contents of a MIDI File in Human-Readable and Machine-Parseable Staccato Format
    Want to see the music in your MIDI file?Of course,you could load it in a sheet music tool.
    Here's how you can load it with JFugue. You'll get a Pattern of your music,which you can
    then pick apart in interesting ways(for example,count how many"C"notes there are...
    that's coming up in a few examples)
    */

    public class SeeMidi
    {
        public static void Go()
        {
            String fileName = "PUT YOUR MIDI FILENAME HERE";
            Pattern pattern = MidiFileConverter.LoadPatternFromMidi(fileName);
            Console.WriteLine(pattern);
        }
    }


    /*	Connecting Any Parser to Any ParserListener
    You can use JFugue to convert between music formats.
    Most commonly,JFugue is used to turn Staccato music into MIDI sound.
    Alternatively,you can play with the MIDI,MusicXML,and LilyPond parsers
    and listeners.Or,you can easily create your own parser or listener,
    and it will instantly interoperate with the other existing formats.
    (And if you convert to Staccato,you can then play the Staccato music...and edit it!)
    */

    public class ParserDemo
    {

        public static void Go()

        {

            //String fileName = "PUT A MIDI FILE HERE";
            //MidiParser parser = new MidiParser();
            //StaccatoParserListener listener = new StaccatoParserListener();

            //parser.AddParserListener(listener);
            //parser.Parse(MidiSystem.getSequence(new File(fileName)));

            //Pattern staccatoPattern = listener.getPattern();
            //Console.WriteLine(staccatoPattern);

            //Player player = new Player();
            //player.Play(staccatoPattern);
        }
    }


    /*	Create a Listener to Find Out About Music
    You can create a ParserListener to listen for any musical event
    that any parser is parsing. Here, we'll create a simple tool that
    counts how many "C" notes (of any octave) are played in any song.
    */

    public class ParserDemo2
    {

        public static void Go()

        {


            //String fileName = "PUT A MIDI FILE HERE";
            //MidiParser parser = new MidiParser(); // Remember, you can use any Parser!
            //MyParserListener listener = new MyParserListener();

            //parser.AddParserListener(listener);
            //parser.Parse(MidiSystem.getSequence(new File(fileName)));

            //Console.WriteLine("There are " + listener.counter + " 'C' notes in this music.");
        }
    }

    //class MyParserListener : ParserListenerAdapter
    //{

    //    public int counter;


    //    public override void onNoteParsed(Note note)
    //    {
    //        // A "C" note is in the 0th position of an octave
    //        if (note.GetPositionInOctave() == 0)
    //        {
    //            counter++;
    //        }
    //    }
    //}




    /*	Play Music in Realtime
        Create interactive musical programs using the RealtimePlayer.
    */

    public class RealtimeExample
    {

        private static Pattern[] PATTERNS = new Pattern[]{new Pattern("Cmajq Dmajq Emajq"),
                                                     new Pattern("V0 Ei Gi Di Ci  V1 Gi Ci Fi Ei"),
                                                     new Pattern("V0 Cmajq V1 Gmajq")};

        public static void Go()
        {

            //RealtimePlayer player = new RealtimePlayer();
            //Random random = new Random();
            //Scanner scanner = new Scanner(System.in);
            //boolean quit = false;

            //while (quit == false)
            //{
            //    System.out.print("Enter a '+C' to start a note, " +
            //                     "'-C' to stop a note, 'i' for a random instrument, " +
            //                     "'p' for a pattern, or 'q' to quit: ");
            //    String entry = scanner.next();
            //    if (entry.startsWith("+"))
            //    {
            //        player.startNote(new Note(entry.substring(1)));
            //    }
            //    else if (entry.startsWith("-"))
            //    {
            //        player.stopNote(new Note(entry.substring(1)));
            //    }
            //    else if (entry.equalsIgnoreCase("i"))
            //    {
            //        player.changeInstrument(random.nextInt(128));
            //    }
            //    else if (entry.equalsIgnoreCase("p"))
            //    {
            //        player.play(PATTERNS[random.nextInt(PATTERNS.length)]);
            //    }
            //    else if (entry.equalsIgnoreCase("q"))
            //    {
            //        quit = true;
            //    }
            //}
            //scanner.close();
            //player.close();
        }

    }


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

        public static void Go()
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


    /*	Use "Replacement Maps" to Create Carnatic Music
    JFugue's ReplacementMap capability lets you use your own symbols
    in a music string. JFugue comes with a CarnaticReplacementMap
    that maps Carnatic notes to microtone frequencies.
    */

    public class CarnaticReplacementMapDemo
    {

        public static void Go()
        {
            Console.WriteLine(@"Use ""Replacement Maps"" to Create Carnatic Music");

            ReplacementMapPreprocessor Processor = new ReplacementMapPreprocessor();
            //Refer http://www.programcreek.com/java-api-examples/index.php?source_dir=JFugue-for-Android-master/jfugue-android/src/main/java/org/staccato/maps/CarnaticReplacementMap.java
            Processor.ReplacementMap = new Dictionary<string, string>
            {
                {"S", "m261.6256"},
                {"R1", "m275.6220"},
                {"R2", "m279.0673"},
                {"R3", "m290.6951"},
                {"R4", "m294.3288"},
                {"G1", "m310.0747"},
                {"G2", "m313.9507"},
                {"G3", "m327.0319"},
                {"G4", "m331.1198"},
                {"M1", "m348.8341"},
                {"M2", "m353.1945"},
                {"M3", "m367.9109"},
                {"M4", "m372.5098"},
                {"P", "m392.4383"},
                {"D1", "m413.4330"},
                {"D2", "m418.6009"},
                {"D3", "m436.0426"},
                {"D4", "m441.4931"},
                {"N1", "m465.1121"},
                {"N2", "m470.9260"},
                {"N3", "m490.5479"},
                {"N4", "m496.6798"},
        };



            using (Player player = new Player())
            {

                player.Play(Processor.Preprocess("<S> <R1> <G1> <M1> <P> <D1> <N1> <S> <N1> <D1> <P> <M1> <G1> <R1> <S> <S>", null));
            }
        }
    }


    /*	Use"Replacement Maps" to Play Solfege
    JFugue comes with a SolfegeReplacementMap,which means you can program music using
    "Do Re Me Fa So La Ti Do."The ReplacementMapParser converts those solfege tones to
    C D E F G A B.Using Replacement Maps,which are simply Map<String,String>,you can
    create any kind of music in a pattern that will be converted to musical notes
    (or whatever you put in the values of your Map).
    */

    public class SolfegeReplacementMapDemo
    {

        public static void Go()
        {
            Console.WriteLine(@"Use ""Replacement Maps"" to Play Solfege");

            ReplacementMapPreprocessor Processor = new ReplacementMapPreprocessor();
            //Refer http://www.programcreek.com/java-api-examples/index.php?source_dir=JFugue-for-Android-master/jfugue-android/src/main/java/org/staccato/maps/SolfegeReplacementMap.java
            Processor.ReplacementMap = new Dictionary<string, string>
            {
                {"DO", "C"},
                {"RE", "D"},
                {"MI", "E"},
                {"FA", "F"},
                {"SO", "G"},
                {"SOL", "G"},
                {"LA", "A"},
                {"TI", "B"},
                {"TE", "B"},
            };
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

    /*	Use"Replacement Maps" to Generate Fractal Music
    A Lindenmayer system is a string rewriting system that can be used to create fractal shapes.
    You can use JFugue's ReplacementMap capability to create music based on string rewrite rules!
        (File this one under, "I didn't intentionally create a fractal music tool,it just kinda happened.Oops.")
    */
    public class LSystemMusic
    {

        public static void Go()
        {
            Console.WriteLine(@"Use ""Replacement Maps"" to Generate Fractal Music");

            // Specify the transformation rules for this Lindenmayer system
            var rules = new Dictionary<string, string> {
                                                            {"Cmajw", "Cmajw Fmajw"},
                                                            {"Fmajw", "Rw Bbmajw"},
                                                            {"Bbmajw", "Rw Fmajw"},
                                                            {"C5q", "C5q G5q E6q C6q"},
                                                            {"E6q", "G6q D6q F6i C6i D6q"},
                                                            {"G6i+D6i", "Rq Rq G6i+D6i G6i+D6i Rq"},
                                                            {"axiom", "axiom V0 I[Flute] Rq C5q V1 I[Tubular_Bells] Rq Rq Rq G6i+D6i V2 I[Piano] Cmajw E6q " + "V3 I[Warm] E6q G6i+D6i V4 I[Voice] C5q E6q" }
                                                         };


            // Set up the ReplacementMapPreprocessor to iterate 3 times
            // and not require brackets around replacements
            ReplacementMapPreprocessor rmp = new ReplacementMapPreprocessor();
            rmp.ReplacementMap = rules;
            rmp.Iterations = 4;
            rmp.RequiresAngleBrackets = false;

            // Create a Pattern that contains the L-System axiom
            Pattern axiom = new Pattern("T120 " + "V0 I[Flute] Rq C5q " + "V1 I[Tubular_Bells] Rq Rq Rq G6i+D6i " + "V2 I[Piano] Cmajw E6q " + "V3 I[Warm] E6q G6i+D6i " + "V4 I[Voice] C5q E6q");

            using (Player player = new Player())
            {
                Console.WriteLine(rmp.Preprocess(axiom.ToString(), null));
                player.Play(rmp.Preprocess(axiom.ToString(), null));
            }
        }
    }

}



