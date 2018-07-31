using System;

using Xamarin.Forms;

namespace AltiusDashboard
{
    public class MenuCategory
    {
        public string Name { get; set; }
        public Func<ContentPage> PageFn { get; set; }

        public MenuCategory(string name, Func<ContentPage> pageFn)
        {
            Name = name;
            PageFn = pageFn;
        }
    }
}