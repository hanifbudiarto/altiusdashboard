using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class ElsaSettingPage : ContentPage
	{
		const string KEY_DB = "database";
		const string KEY_INS = "instance";

		public ElsaSettingPage()
		{
			InitializeComponent();
			Title = "Elsa Settings";
			btnSave.Clicked += onSaveClicked;

			loadSavedSettings();
		}

		async void onSaveClicked(object sender, EventArgs e)
		{
			string database = entryDatabase.Text;
			string instance = entryInstance.Text;

			await LocalPreferences.SaveApplicationProperty<string>(KEY_DB, database);
			await LocalPreferences.SaveApplicationProperty<string>(KEY_INS, instance);

			SettingVariable.Database = database;
			SettingVariable.Instance = instance;

			await DisplayAlert("Info", "Successfully saved", "OK");
		}

		void loadSavedSettings() 
		{
			string database = "";
			if (LocalPreferences.IsPropertyExists(KEY_DB)) 
			{
				database = LocalPreferences.LoadApplicationProperty<string>(KEY_DB);
			}

			string instance = "";
			if (LocalPreferences.IsPropertyExists(KEY_INS))
			{				
				instance = LocalPreferences.LoadApplicationProperty<string>(KEY_INS);
			}

			entryDatabase.Text = database;
			entryInstance.Text = instance;

			SettingVariable.Database = database;
			SettingVariable.Instance = instance;
		}

	}
}
