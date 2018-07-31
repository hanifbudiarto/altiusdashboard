
using Xamarin.Forms;

namespace AltiusDashboard
{
    public class MasterDetail : MasterDetailPage
    {
        public MasterDetail()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            #if __IOS__
                IsGestureEnabled = false;
            #endif

            var menuPage = new MenuPage();
            menuPage.OnMenuSelect = (menuName, categoryPage) => {
                if (menuName.Equals("Sign Out"))
                {
                    onLogout();
                }
                else
                {
                    Detail = new NavigationPage(categoryPage);
                    IsPresented = false;
                }
            };

            Master = menuPage;


			Detail = new NavigationPage(new MainReportPage());           
        }

        private async void onLogout()
        {
			var answer = await DisplayAlert("Exit", "Do you want to exit ALTiUS Dashboard?", "Yes", "No");
			if (answer)
			{
				await LocalPreferences.SaveApplicationProperty<bool>("isLoggedIn", false);
				NavigationPage page = new NavigationPage(new LoginPage());
				Xamarin.Forms.Application.Current.MainPage = page;
			}

			IsPresented = false;
        }
    }
}
