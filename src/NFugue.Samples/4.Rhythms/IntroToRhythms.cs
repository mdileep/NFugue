using NFugue.Playing;
using NFugue.Rhythms;

namespace NFugue.Samples
{
	/// <summary>
	/// Introduction to Rhythms
	/// One of my favorite parts of the JFugue API is the ability to create rhythms
	/// in a fun and easily understandable way.The letters are mapped to percussive
	/// instrument sounds, like"Acoustic Snare"and"Closed Hi Hat".JFugue comes with a
	/// default"rhythm set", which is a Map<Character, String>with entries like this:put('O',"[BASS_DRUM]i"). 
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	[Title(4, "Introduction to Rhythms", 1)]
	public class IntroToRhythms
	{

		public static void Run()
		{
			Rhythm rhythm = new Rhythm().AddLayer("O..oO...O..oOO..")
										.AddLayer("..S...S...S...S.")
										.AddLayer("````````````````")
										.AddLayer("...............+");

			using (Player player = new Player())
			{
				player.Play(rhythm.GetPattern()
									   .Repeat(2));
			}
		}
	}
}