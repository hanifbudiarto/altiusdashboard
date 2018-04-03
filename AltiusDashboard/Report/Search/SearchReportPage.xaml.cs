using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class SearchReportPage : ContentPage
	{
		public SearchReportPage()
		{
			InitializeComponent();
			Title = "Search Report";

			btnSearch.Clicked += OnClickedSearch;
			listView.ItemTapped += onItemReportTapped;
		}

		void onItemReportTapped(object sender, ItemTappedEventArgs e)
		{
			var item = e.Item as ItemReport;
			Navigation.PushAsync(new ParamReportPage(item.Id, item.Title));
		}

		void OnClickedSearch(object sender, EventArgs e)
		{
			getItemReport(entrySearch.Text.ToString());
		}

		async void getItemReport(string keyword)
		{
			onLoading(true);
			var myTask = Task.Run(() => DependencyService.Get<IDbDataFetcher>().getItemReport(Query.getItemReportByKeyword(keyword)));
			// your thread is free to do other useful stuff right nw
			//DoOtherUsefulStuff();
			// after a while you need the result, await for myTask:
			List<ItemReport> result = await myTask;
			if (result == null) await DisplayAlert("Error", DependencyService.Get<IDbDataFetcher>().getErrorMessage(), "OK");
			else listView.ItemsSource = result;

			onLoading(false);
		}

		void onLoading(bool loading)
		{
			layoutLoading.IsVisible = loading;
			listView.IsVisible = !loading;
		}
	}
}
