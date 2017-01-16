using NFugue.Patterns;
using System.IO;

namespace NFugue.Midi.Conversion
{
    public static class PatternProducerExtensions
    {
        /// <summary>
        /// Saves pattern produced by the <paramref name="patternProducer"/> to a MIDI file
        /// </summary>
        /// <param name="patternProducer">Producer from which the pattern should be saved</param>
        /// <param name="filePath">Path to the MIDI file</param>
        public static void SaveAsMidi(this IPatternProducer patternProducer, string filePath)
        {
            MidiFileConverter.SavePatternToMidi(patternProducer, filePath);
        }

        /// <summary>
        /// Saves pattern produced by the <paramref name="patternProducer"/> to a MIDI file
        /// </summary>
        /// <param name="patternProducer">Producer from which the pattern should be saved</param>
        /// <param name="stream">Stream to the MIDI file</param>
        public static void SaveAsMidi(this IPatternProducer patternProducer, Stream stream)
        {
            MidiFileConverter.SavePatternToStream(patternProducer, stream);
        }
    }
}