using NFugue.Patterns;
using NFugue.Playing;
using NFugue.Staccato.Preprocessors;
using System;
using System.Collections.Generic;

namespace NFugue.Samples
{
	/// <summary>
	/// Use"Replacement Maps" to Generate Fractal Music
	/// A Lindenmayer system is a string rewriting system that can be used to create fractal shapes.
	/// You can use JFugue's ReplacementMap capability to create music based on string rewrite rules!
	/// (File this one under, "I didn't intentionally create a fractal music tool,it just kinda happened.Oops.")
	/// </summary>
	/// <remarks>
	/// Ported from https://github.com/dmkoelle/jfugue/blob/master/jfugue-sample 
	///</remarks>
	[Title(6, @"Use ""Replacement Maps"" to Generate Fractal Music", 2)]
	public class LSystemMusic
	{

		public static void Run()
		{
			// Specify the transformation rules for this Lindenmayer system
			var rules = new Dictionary<string, string> {
															{"Cmajw", "Cmajw Fmajw"},
															{"Fmajw", "Rw Bbmajw"},
															{"Bbmajw", "Rw Fmajw"},
															{"C5q", "C5q G5q E6q C6q"},
															{"E6q", "G6q D6q F6i C6i D6q"},
															{"G6i+D6i", "Rq Rq G6i+D6i G6i+D6i Rq"},
															{"axiom", "axiom V0 I[Flute] Rq C5q V1 I[Tubular_Bells] Rq Rq Rq G6i+D6i V2 I[Piano] Cmajw E6q " + "V3 I[Warm] E6q G6i+D6i V4 I[Voice] C5q E6q" }
														 };


			// Set up the ReplacementMapPreprocessor to iterate 3 times
			// and not require brackets around replacements
			ReplacementMapPreprocessor rmp = new ReplacementMapPreprocessor();
			rmp.ReplacementMap = rules;
			rmp.Iterations = 4;
			rmp.RequiresAngleBrackets = false;

			// Create a Pattern that contains the L-System axiom
			Pattern axiom = new Pattern("T120 " + "V0 I[Flute] Rq C5q " + "V1 I[Tubular_Bells] Rq Rq Rq G6i+D6i " + "V2 I[Piano] Cmajw E6q " + "V3 I[Warm] E6q G6i+D6i " + "V4 I[Voice] C5q E6q");

			using (Player player = new Player())
			{
				Console.WriteLine(rmp.Preprocess(axiom.ToString(), null));
				player.Play(rmp.Preprocess(axiom.ToString(), null));
			}
		}
	}
}



