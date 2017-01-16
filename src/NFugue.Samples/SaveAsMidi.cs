using NFugue.Midi;
using NFugue.Midi.Conversion;
using NFugue.Patterns;
using NFugue.Playing;
using NFugue.Theory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFugue.Samples
{
	[Title("C#9:Save Pattern to Midi File")]
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

			//pattern.SaveAsMidi(midiFile);
			//MidiFileConverter.SavePatternToMidi(pattern, midiFile);

			MemoryStream ms = new MemoryStream();
			pattern.SaveAsMidi(ms);

			FileStream fileStream = new FileStream(midiFile, FileMode.Create);
			ms.WriteTo(fileStream);
		}
	}
}
