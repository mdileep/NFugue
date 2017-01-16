using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NFugue.Samples
{
	/// <summary>
	/// 
	/// </summary>
	public class SampleRunner
	{
		public void Run()
		{

			Console.Title = "NFugue Samples";

			Console.WriteLine(Console.Title);
			Console.WriteLine("");

			Console.WriteLine("Select an option to run the Sample: ");
			Console.WriteLine("");

			var samples = LoadSamples();

			int choice = GetUserChoice(samples);
			if (choice >= 1 && choice <= samples.Count)
			{
				Type sample = samples[choice - 1];
				RunSample(sample);
				return;
			}

			Console.WriteLine("     Running all samples  ");
			foreach (Type sample in samples)
			{
				RunSample(sample);
			}

		}

		private int GetUserChoice(List<Type> samples)
		{
			for (int i = 1; i <= samples.Count; i++)
			{
				Type sample = samples[i - 1];
				var title = GetTitle(sample);
				Console.WriteLine(i + "." + title);
			}

			Console.WriteLine("");
			Console.WriteLine("");

			Console.Write("Enter your choice or press any key and Enter to run all Samples: ");

			string strChoice = Console.ReadLine();

			Console.WriteLine("");
			Console.WriteLine("");

			int choice = 0;
			return int.TryParse(strChoice, out choice) ? choice : 0;
		}

		private string GetTitle(Type sample)
		{
			object[] attributes = sample.GetCustomAttributes(typeof(TitleAttribute), false);
			if (attributes == null || attributes.Length == 0)
			{
				return null;
			}
			return ((TitleAttribute)attributes[0]).Title;
		}

		private void RunSample(Type sample)
		{
			var title = GetTitle(sample);

			Console.WriteLine($"Running {title}");

			sample.GetMethod("Run", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
		}

		private List<Type> LoadSamples()
		{
			return typeof(Program).Assembly
								.GetTypes()
								.Where(t => t.IsClass && !t.IsAbstract &&
										t.GetCustomAttributes(typeof(TitleAttribute), false).Length != 0 &&
										t.GetMethod("Run", BindingFlags.Public | BindingFlags.Static) != null)
								.OrderBy(t => (((TitleAttribute)t.GetCustomAttributes(typeof(TitleAttribute), false)[0]).Charpter))
								.ThenBy(t => (((TitleAttribute)t.GetCustomAttributes(typeof(TitleAttribute), false)[0]).Order))
								.ToList();
		}
	}
}