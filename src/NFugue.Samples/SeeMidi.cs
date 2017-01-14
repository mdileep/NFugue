using NFugue.Midi;
using NFugue.Patterns;
using System;

namespace NFugue.Samples
{

    /*	See the Contents of a MIDI File in Human-Readable and Machine-Parseable Staccato Format
    Want to see the music in your MIDI file?Of course,you could load it in a sheet music tool.
    Here's how you can load it with JFugue. You'll get a Pattern of your music,which you can
    then pick apart in interesting ways(for example,count how many"C"notes there are...
    that's coming up in a few examples)
    */
    public class SeeMidi
    {
        public static void Run()
        {
            String fileName = "PUT YOUR MIDI FILENAME HERE";
            Pattern pattern = MidiFileConverter.LoadPatternFromMidi(fileName);
            Console.WriteLine(pattern);
        }
    }
}