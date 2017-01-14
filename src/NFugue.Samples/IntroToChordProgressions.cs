using NFugue.Playing;
using NFugue.Theory;
using System;

namespace NFugue.Samples
{


    /*	Introduction to Chord Progressions
    It's easy to create a Chord Progression in JFugue. You can then play it,
    or you can see the notes that comprise the any of the chords in the progression.
    */
    [Title("C#4:Introduction to Chord Progressions")]
    public class IntroToChordProgressions
    {

        public static void Run()
        {
            ChordProgression cp = new ChordProgression("I IV V");

            Chord[] chords = cp.SetKey("C")
                               .GetChords();

            foreach (Chord chord in chords)
            {
                Console.Write("Chord " + chord + " has these notes: ");
                Note[] notes = chord.GetNotes();

                foreach (Note note in notes)
                {
                    Console.Write(note + " ");
                }
                Console.WriteLine();
            }

            using (Player player = new Player())
            {
                player.Play(cp);
            }
        }
    }
}