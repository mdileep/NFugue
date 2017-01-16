using NFugue.Playing;

namespace NFugue.Samples
{
	/// <summary>
	/// Hello World
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	[Title(1, "Hello World", 1)]
	public class HelloWorld
	{
		public static void Run()
		{
			using (Player player = new Player())
			{
				player.Play("C D E F G A B ");
			}
		}
	}
}