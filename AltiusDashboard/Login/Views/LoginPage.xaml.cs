using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			InitializeComponent();

			btnLogin.Clicked += OnLoginClicked;
			btnSetting.Clicked += (sender, e) => 
			{
				Navigation.PushAsync(new ElsaSettingPage());
			};
			onLoading(false);

			NavigationPage.SetHasNavigationBar(this, false);

			if (LocalPreferences.IsPropertyExists("database"))
			{
				SettingVariable.Database = LocalPreferences.LoadApplicationProperty<string>("database");
			}

			if (LocalPreferences.IsPropertyExists("instance"))
			{
				SettingVariable.Instance = LocalPreferences.LoadApplicationProperty<string>("instance");
			}

			Debug.WriteLine("database instance" + SettingVariable.Database +" " + SettingVariable.Instance);

		}

		protected override async void OnAppearing() 
		{
			if (isLoggedIn())
			{
				string usr = LocalPreferences.LoadApplicationProperty<string>("username");
				string pwd = LocalPreferences.LoadApplicationProperty<string>("password");

				entryUsername.Text = usr;
				entryPassword.Text = pwd;
				onLoading(true);
				bool valid = await Task.Run(() => validUser(usr, pwd));

				if (!valid)
				{
					await DisplayAlert("Login failed", "Please check your username or password", "OK");
					onLoading(false);
					return;
				}
				loggingIn(usr, pwd);
			}
		}

		async void OnLoginClicked(object sender, EventArgs e)
		{
            onLoading(true);
			string username = entryUsername.Text;
			string password = entryPassword.Text;

			bool valid = await Task.Run(() => validUser(username, password));

			if (!valid)
			{
				await DisplayAlert("Login failed", "Please check your username or password", "OK");
				onLoading(false);
				return;
			}

			loggingIn(username, password);
		}

		void onLoading(bool loading)
		{
			layoutLoading.IsVisible = loading;
			loginForm.IsVisible = !loading;
		}

		async Task loggingIn(string usr, string pwd)
		{
			onLoading(true);

			SettingVariable.Username = usr;
			SettingVariable.Password = pwd;
			DependencyService.Get<IDbDataFetcher>().reloadConnectionString();

			Debug.WriteLine("trying to login "+ usr + ", "+ pwd);

			List<Branch> branch = await Task.Run(() => getBranch(usr));
			Debug.WriteLine("branch ditemukan " + branch.Count);
			string[] branchName = new string[branch.Count];
			int counter = 0;
			foreach (Branch b in branch)
			{
				branchName[counter] = b.NamaCabang;
				counter++;
			}

			var action = await DisplayActionSheet("Choose Branch", null, null, branchName);

			foreach (Branch b in branch) 
			{
				if (action == b.NamaCabang)
				{
					SettingVariable.Cabang = b;
					NavigationPage page = new NavigationPage(new MasterDetail());
					Xamarin.Forms.Application.Current.MainPage = page;
					//await Navigation.PushAsync(new MasterDetail());
				}
			}


			onLoading(false);
		}

		async Task<List<Branch>> getBranch(string username) 
		{
			string query = Query.getBranchQuery(username);
			Debug.WriteLine(query);
			var list = DependencyService.Get<IDbDataFetcher>().getBranch(query);
			return list;
		}

		async Task<bool> validUser(string username, string password)
		{				
			SettingVariable.Username = username;
			SettingVariable.Password = password;
			DependencyService.Get<IDbDataFetcher>().reloadConnectionString();

			string query = "select kode,nama,email,password from staff where email = '" + username + "'";
			Debug.WriteLine(query);
			var list = DependencyService.Get<IDbDataFetcher>().getUser(query);
			if (list != null && list.Count > 0)
			{
				await LocalPreferences.SaveApplicationProperty<bool>("isLoggedIn", true);
				await LocalPreferences.SaveApplicationProperty<string>("username", username);
				await LocalPreferences.SaveApplicationProperty<string>("password", password);

				return true;
			}

			return false;
		}

		bool isLoggedIn()
		{
			if (LocalPreferences.IsPropertyExists("isLoggedIn"))
			{
				return LocalPreferences.LoadApplicationProperty<bool>("isLoggedIn");
			}
			return false;
		}

	}
}
