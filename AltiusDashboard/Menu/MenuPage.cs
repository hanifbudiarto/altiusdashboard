
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AltiusDashboard
{
    class MenuPage : ContentPage
    {
        public Action<string, ContentPage> OnMenuSelect
        {
            get;
            set;
        }

        public MenuPage()
        {
            Title = "Menu";
            Icon = "ic_menu.png";

			this.BackgroundColor = Color.FromHex("#33302e");
            Padding = new Thickness(10, 20);

            var categories = new List<MenuCategory>()
            {
				new MenuCategory("Dashboard", () => new DashboardPage()),
				new MenuCategory("Report List", () => new MainReportPage()),
				new MenuCategory("Sign Out", () => new AltiusDashboardPage()),
            };

            var dataTemplate = new DataTemplate(typeof(TextCell));
            dataTemplate.SetBinding(TextCell.TextProperty, "Name");
			dataTemplate.SetValue(TextCell.TextColorProperty, Color.White);

			var listView = new ListView()
			{
				ItemsSource = categories,
				ItemTemplate = dataTemplate,
				SeparatorColor = Color.FromHex("#888888"),
				BackgroundColor = Color.FromHex("#33302e")
            };

            //listView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
            //    if (OnMenuSelect != null)
            //    {
            //        var menuCategory = (MenuCategory)e.SelectedItem;
            //        var menuName = menuCategory.Name;
            //        var categoryPage = menuCategory.PageFn();
            //        OnMenuSelect(menuName, categoryPage);
            //    }
            //};

			listView.ItemTapped += (sender, e) => 
			{
                if (OnMenuSelect != null)
                {
					var menuCategory = ((ListView)sender).SelectedItem as MenuCategory;
					var menuName = menuCategory.Name;
					var categoryPage = menuCategory.PageFn();

					OnMenuSelect(menuName, categoryPage);

					((ListView)sender).SelectedItem = null;
                }
			};

            Content = listView;
        }

    }
}