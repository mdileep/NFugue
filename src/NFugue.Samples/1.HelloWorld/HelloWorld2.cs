using NFugue.Playing;

namespace NFugue.Samples
{
	/// <summary>
	/// Playing multiple voices, multiple instruments, rests, chords, and durations
	/// This example uses the Staccato 'V' command for specifing voices, 'I' for specifying instruments
	/// (text within brackets is looked up in a dictionary and maps to MIDI instrument numbers),
	/// '|' (pipe) for indicating measures(optional), durations including 'q' for quarter duration,
	/// 'qqq' for three quarter notes(multiple durations can be listed together), and 'h' for half,
	/// 'w' for whole, and '.' for a dotted duration; 'R' for rest, and the chords G-Major and C-Major.
	/// Whitespace is not significant and can be used for visually pleasing or helpful spacing.
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	[Title(1, "Hello World2", 2)]
	public class HelloWorld2
	{
		public static void Run()
		{
			using (Player player = new Player())
			{
				player.Play("V0 I[Flute] Eq Ch. | Eq Ch. | Dq Eq Dq Cq   V1 I[Flute] Rw | Rw | GmajQQQ CmajQ");
			}
		}
	}
}