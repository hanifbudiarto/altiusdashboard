
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class MainReportPage : ContentPage
	{

		CancellationTokenSource cts;

		public MainReportPage()
		{
			InitializeComponent();

			Title = "Report List";
			listView.ItemTapped += onMainReportTapped;
		}

		protected override void OnAppearing()
		{
			cts = new CancellationTokenSource();
            getMainReport();
		}

		protected override void OnDisappearing()
		{
			cts.Cancel();
			base.OnDisappearing();
		}

		async void OnSearchClicked(object sender, EventArgs e)
		{ 
			await Navigation.PushAsync(new SearchReportPage());
		}

		void onMainReportTapped(object sender, ItemTappedEventArgs e)
		{
			var item = e.Item as MainReport;
			Navigation.PushAsync(new ItemReportPage(item.Dept));
		}

		async Task getMainReport()
		{
			onLoading(true);
			List<MainReport> result = null;
			try
			{
				result = await Task.Run(() => DependencyService.Get<IDbDataFetcher>().getMainReport(Query.getMainReport()), cts.Token);
			}
			catch (TaskCanceledException e) 
			{
				Debug.WriteLine(e.Message);
				return;
			}

			if (result == null)
			{
				if (PageUtils.PageTypeIsAlreadyAtTopOfStack(this, typeof(MainReportPage)))
				{
					await DisplayAlert("Error", DependencyService.Get<IDbDataFetcher>().getErrorMessage(), "OK");
				}
			}
			else listView.ItemsSource = result;
			// you can now use the results of loading:
			//ProcessResult(result);
			onLoading(false);
		}

		void onLoading(bool loading)
		{
			layoutLoading.IsVisible = loading;
			listView.IsVisible = !loading;
		}
	}
}
