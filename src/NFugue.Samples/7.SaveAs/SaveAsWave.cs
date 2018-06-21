using NFuge.Converters;
using NFugue.Midi;
using NFugue.Midi.Conversion;
using NFugue.Patterns;
using NFugue.Theory;
using System.IO;

namespace NFugue.Samples
{
	/// <summary>
	/// Saves the pattern as a Wave file to the Disk
	/// </summary>
	[Title(7, "Save Pattern to Wave File", 3)]
	class SaveAsWave
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
			MidiFileConverter.SavePatternToMidi(pattern, "F." + midiFile);
			WaveConverter.Convert(midiFile, "F." + midiFile + ".wav");

			using (MemoryStream midiStream = new MemoryStream())
			{
				pattern.SaveAsMidi(midiStream);

				FileStream midiFileStream = new FileStream(midiFile, FileMode.Create);
				midiStream.WriteTo(midiFileStream);

				using (MemoryStream waveStream = new MemoryStream())
				{
					WaveConverter.Convert(".mid", midiStream, waveStream);

					FileStream waveFileStream = new FileStream(midiFile + ".wav", FileMode.Create);
					waveStream.WriteTo(waveFileStream);
				}
			}

		}
	}
}
