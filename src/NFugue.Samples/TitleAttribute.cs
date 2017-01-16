using System;

namespace NFugue.Samples
{
	internal class TitleAttribute : Attribute
	{
		public TitleAttribute(int chapter, string title, int order = 0, string description = "")
		{
			Charpter = chapter;
			Title = title;
			Order = order;
			Description = description;
		}
		public int Charpter { get; set; }
		public int Order { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
	}
}