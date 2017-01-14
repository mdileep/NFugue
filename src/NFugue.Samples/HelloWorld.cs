using NFugue.Playing;
using System;

namespace NFugue.Samples
{
    [Title("C#1:Hello World")]
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