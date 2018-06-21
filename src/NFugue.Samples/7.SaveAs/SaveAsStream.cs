using NFuge.Converters.NFugue.Converters;
using NFugue.Midi;
using NFugue.Midi.Conversion;
using NFugue.Patterns;
using NFugue.Theory;
using System.IO;

namespace NFugue.Samples
{
	/// <summary>
	/// Saves the pattern as Midi Memory Stream
	/// </summary>
	[Title(7, "Save Pattern to Midi and Wave Stream", 2)]
	class SaveAsStream
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
