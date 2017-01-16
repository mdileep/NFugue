using NFugue.Patterns;
using NFugue.Playing;
using NFugue.Rhythms;

namespace NFugue.Samples
{
	/// <summary>
	/// 
	/// </summary>
	[Title(11,"Play Ground",1)]
	class PlayGround
	{
		public static void Run()
		{
			/*
			 ChordProgression :  
			 MAJOR:            W-W-H-W-W-W-H
			 Major Chord    :  1-3-5
							   C    D   E   F   G   A   B
									D   E   F#  G   A   B   C#5
										E   F#  G#  A   B   C#5  Eb5
											F   G   A   Bb  C5   D5   E5
												G   A   B   C5   D5   E5   F#5
													A   B   C#5  D5   E5   F#5    G#5
														B4  C#5  Eb5  E5   F#5    G#5   Bb5
			 Valid Distributions: o , d , 7 , 7%6
			 $Replacelment is based on intervals
			Rythm:
				{'.', "Ri"},
				{'O', "[BASS_DRUM]i"},
				{'o', "Rs [BASS_DRUM]s"},
				{'S', "[ACOUSTIC_SNARE]i"},
				{'s', "Rs [ACOUSTIC_SNARE]s"},
				{'^', "[PEDAL_HI_HAT]i"},
				{'`', "[PEDAL_HI_HAT]s Rs"},
				{'*', "[CRASH_CYMBAL_1]i"},
				{'+', "[CRASH_CYMBAL_1]s Rs"},
				{'X', "[HAND_CLAP]i"},
				{'x', "Rs [HAND_CLAP]s"},
			 */

			//Pattern pattern = new ChordProgression("i iii v")
			//				.SetKey("D")
			//				.Distribute("7")
			//				.AllChordsAs("$0 $1 $2")
			//				.EachChordAs("$0 $1 $2 $3")
			//				.GetPattern();

			/*
			 $0:            C  E  G
			 $0 $1          C  E  
							E  G# 
							G  B
			 $0 $1 $2       C  E  G 
							E  G# B
							G  B  D
			 */



			Rhythm rhythm = new Rhythm().AddLayer("OOOO")
										.AddLayer(".SS.")
										;

			Pattern pattern = rhythm.GetPattern();

			using (Player player = new Player())
			{
				player.Play(pattern);
			}
		}
	}
}
