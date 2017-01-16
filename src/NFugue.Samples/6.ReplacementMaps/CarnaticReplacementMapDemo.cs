using NFugue.Playing;
using NFugue.Staccato.Preprocessors;

namespace NFugue.Samples
{
	/// <summary>
	/// Use "Replacement Maps" to Create Carnatic Music
	/// JFugue's ReplacementMap capability lets you use your own symbols
	/// in a music string. JFugue comes with a CarnaticReplacementMap
	/// that maps Carnatic notes to microtone frequencies.
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	[Title(6, @"Use ""Replacement Maps"" to Create Carnatic Music", 1)]
	public class CarnaticReplacementMapDemo
	{
		public static void Run()
		{
			ReplacementMapPreprocessor Processor = new ReplacementMapPreprocessor();
			Processor.ReplacementMap = ReplacementMaps.CarnaticReplacementMap;
			using (Player player = new Player())
			{
				player.Play(Processor.Preprocess("<S> <R1> <G1> <M1> <P> <D1> <N1> <S> <N1> <D1> <P> <M1> <G1> <R1> <S> <S>", null));
			}
		}
	}
}