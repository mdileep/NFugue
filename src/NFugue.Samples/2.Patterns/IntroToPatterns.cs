using NFugue.Patterns;
using NFugue.Playing;

namespace NFugue.Samples
{
	/// <summary>
	/// Introduction to Patterns
	/// Patterns are one of the fundamental units of music in JFugue. They can be manipulated in musically interesting ways.
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	[Title(2, "Introduction to Patterns", 1)]
	public class IntroToPatterns
	{
		public static void Run()
		{
			Pattern p1 = new Pattern("V0 I[Piano] Eq Ch. | Eq Ch. | Dq Eq Dq Cq");
			Pattern p2 = new Pattern("V1 I[Flute] Rw     | Rw     | GmajQQQ  CmajQ");

			using (Player player = new Player())
			{
				player.Play(p1, p2);
			}
		}
	}
}