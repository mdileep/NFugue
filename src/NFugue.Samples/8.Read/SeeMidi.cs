using NFugue.Midi.Conversion;
using NFugue.Patterns;
using System;
using System.IO;

namespace NFugue.Samples
{
	/// <summary>
	/// See the Contents of a MIDI File in Human-Readable and Machine-Parseable Staccato Format
	/// Want to see the music in your MIDI file? Of course,you could load it in a sheet music tool.
	/// Here's how you can load it with JFugue. You'll get a Pattern of your music, which you can
	/// then pick apart in interesting ways(for example, count how many"C"notes there are...
	/// that's coming up in a few examples)
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	[Title(8,"See Midi as pattern",1)]
	public class SeeMidi
	{
		public static void Run()
		{
			string midiFile = "TwelveBarBlues.mid";
			if (!new FileInfo(midiFile).Exists)
			{
				Console.WriteLine($"{midiFile}  not found.");
				Console.Read();
				return;
			}
			Pattern pattern = MidiFileConverter.LoadPatternFromMidi(midiFile);
			Console.WriteLine(pattern);
			Console.Read();
		}
	}
}