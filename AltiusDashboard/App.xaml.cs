using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class App : Application
	{
		public App()
		{
			C1.Xamarin.Forms.Core.LicenseManager.Key = License.Key;  

			InitializeComponent();
			MainPage = new NavigationPage(new LoginPage());
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
