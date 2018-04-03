
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class ItemReportPage : ContentPage
	{
		string _dept;

		public ItemReportPage() 
		{
			InitializeComponent();
		}

		public ItemReportPage(string dept)
		{
			InitializeComponent();

			Title = dept;
			listView.ItemTapped += onItemReportTapped;
			_dept = dept;
		}

		protected override void OnAppearing()
		{
			getItemReport(_dept);
		}

		void onItemReportTapped(object sender, ItemTappedEventArgs e)
		{
			var item = e.Item as ItemReport;
			Navigation.PushAsync(new ParamReportPage(item.Id, item.Title));
		}

		async void getItemReport(string dept)
		{
			onLoading(true);
			var myTask = Task.Run(() => DependencyService.Get<IDbDataFetcher>().getItemReport(Query.getItemReport(dept)));
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
