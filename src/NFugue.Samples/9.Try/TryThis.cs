using NFugue.Playing;
using NFugue.Rhythms;
using NFugue.Theory;

namespace NFugue.Samples
{
	/// <summary>
	/// All That, in One Line of Code?
	/// Try this. The main line of code even fits within the 140-character limit of a tweet.
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	[Title(9, "All That, in One Line of Code", 2)]
	public class TryThis
	{

		public static void Run()
		{
			new Player().Play(new ChordProgression("I IV vi V").EachChordAs("$_i $_i Ri $_i"), new Rhythm().AddLayer("..X...X...X...XO"));
		}
	}
}