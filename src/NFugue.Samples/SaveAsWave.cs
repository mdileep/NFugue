﻿using NFuge.Converters.NFuge.Converters;
using NFugue.Midi;
using NFugue.Midi.Conversion;
using NFugue.Patterns;
using NFugue.Theory;
using System.IO;

namespace NFugue.Samples
{
	[Title("C#9:Save Pattern to Wave File")]
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
			MidiFileConverter.SavePatternToMidi(pattern, midiFile);
			WaveConverter.Convert(midiFile, midiFile + ".wav");

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
