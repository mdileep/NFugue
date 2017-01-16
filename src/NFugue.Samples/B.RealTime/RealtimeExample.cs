using NFugue.Patterns;

namespace NFugue.Samples
{
	/// <summary>
	/// Play Music in Realtime
	/// Create interactive musical programs using the RealtimePlayer.
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	public class RealtimeExample
	{

		private static Pattern[] PATTERNS = new Pattern[]{new Pattern("Cmajq Dmajq Emajq"),
													 new Pattern("V0 Ei Gi Di Ci  V1 Gi Ci Fi Ei"),
													 new Pattern("V0 Cmajq V1 Gmajq")};

		public static void Run()
		{
			//RealtimePlayer player = new RealtimePlayer();
			//Random random = new Random();
			//Scanner scanner = new Scanner(System.in);
			//boolean quit = false;

			//while (quit == false)
			//{
			//    System.out.print("Enter a '+C' to start a note, " +
			//                     "'-C' to stop a note, 'i' for a random instrument, " +
			//                     "'p' for a pattern, or 'q' to quit: ");
			//    String entry = scanner.next();
			//    if (entry.startsWith("+"))
			//    {
			//        player.startNote(new Note(entry.substring(1)));
			//    }
			//    else if (entry.startsWith("-"))
			//    {
			//        player.stopNote(new Note(entry.substring(1)));
			//    }
			//    else if (entry.equalsIgnoreCase("i"))
			//    {
			//        player.changeInstrument(random.nextInt(128));
			//    }
			//    else if (entry.equalsIgnoreCase("p"))
			//    {
			//        player.play(PATTERNS[random.nextInt(PATTERNS.length)]);
			//    }
			//    else if (entry.equalsIgnoreCase("q"))
			//    {
			//        quit = true;
			//    }
			//}
			//scanner.close();
			//player.close();
		}

	}
}