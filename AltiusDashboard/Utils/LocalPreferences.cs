using System;
using System.Threading.Tasks;

namespace AltiusDashboard
{
	public class LocalPreferences
	{
		public static async Task SaveApplicationProperty<T>(string key, T value)
		{
			Xamarin.Forms.Application.Current.Properties[key] = value;
			await Xamarin.Forms.Application.Current.SavePropertiesAsync();
		}

		public static T LoadApplicationProperty<T>(string key)
		{
			return (T)Xamarin.Forms.Application.Current.Properties[key];
		}

		public static bool IsPropertyExists(string key) 
		{
			return Xamarin.Forms.Application.Current.Properties.ContainsKey(key);
		}
	}
}
