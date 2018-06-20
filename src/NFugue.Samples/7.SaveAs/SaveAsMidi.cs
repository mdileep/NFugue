using NFugue.Midi;
using NFugue.Midi.Conversion;
using NFugue.Patterns;
using NFugue.Theory;

namespace NFugue.Samples
{
	/// <summary>
	/// Saves the Pattern as Midi File to disk
	/// </summary>
	[Title(7, "Save Pattern to Midi File", 1)]
	class SaveAsMidi
	{
		public static void Run()
		{
			Pattern pattern = new ChordProgression("I IV V").Distribute("7%6")
															.AllChordsAs("$0 $0 $0 $0 $1 $1 $0 $0 $2 $1 $0 $0")
															.EachChordAs("$0ia100 $1ia80 $2ia80 $3ia80 $4ia100 $3ia80 $2ia80 $1ia80")
															.GetPattern()
															.SetInstrument(Instrument.AcousticBass)
															.SetTempo(100);
			string midiFile = "TwelveBarBlues.mid";
			pattern.SaveAsMidi(midiFile);
			MidiFileConverter.SavePatternToMidi(pattern, midiFile);
		}
	}
}
