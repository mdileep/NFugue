using System;

namespace NFugue.Samples
{
    internal class TitleAttribute : Attribute
    {
        public TitleAttribute(string title, string description = "")
        {
            Title = title;
            Description = description;
        }

        public string Title { get; set; }
        public string Description { get; set; }
    }
}