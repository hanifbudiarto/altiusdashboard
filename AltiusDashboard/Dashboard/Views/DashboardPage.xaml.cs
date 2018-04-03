using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using C1.Xamarin.Forms.Chart;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class DashboardPage : ContentPage
	{
		List<DashboardQuery> dashboardQueryList = new List<DashboardQuery>();
		CancellationTokenSource cts;

		public DashboardPage()
		{
			InitializeComponent();
			Title = "Dashboard";
		}

		protected override void OnAppearing()
		{
			root.Children.Clear();
			cts = new CancellationTokenSource();
			getListDashboardQuery();
		}

		protected override void OnDisappearing()
		{
			cts.Cancel();
			base.OnDisappearing();
		}

		void addChart()
		{
			List<Sample> list = new List<Sample>();
			for (int i = 0; i < 5; i++)
			{
				Sample sample = new Sample("Sample " + i, i + 1);
				list.Add(sample);
			}

			FlexPie pie1 = new FlexPie
			{
				ItemsSource = list,
				HeightRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BindingName = "Name",
				Binding = "Value"
			};


			FlexPie pie2 = new FlexPie
			{
				ItemsSource = list,
				HeightRequest = 300,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				BindingName = "Name",
				Binding = "Value",
			};

			FlexChart chart1 = new FlexChart
			{
				ItemsSource = list,
				HeightRequest = 300,
				BindingX = "Name",
			};
			ChartSeries chartSeries = new ChartSeries
			{
				Binding = "Value",
				SeriesName = "Nilai"
			};
			chart1.Series.Add(chartSeries);
			root.Children.Add(pie1);
			root.Children.Add(chart1);
			root.Children.Add(pie2);
		}


		class Sample
		{
			public string Name { get; set; }
			public double Value { get; set; }
			public Sample(string name, double value)
			{
				this.Name = name;
				this.Value = value;
			}
		}

		async void getListDashboardQuery()
		{
			var myTask = Task.Run(() => DependencyService.Get<IDbDataFetcher>().getDashboardQueries(Query.getDashboardQuery()));
			List<DashboardQuery> result = await myTask;
			if (result == null)
			{
				await DisplayAlert("Error", DependencyService.Get<IDbDataFetcher>().getErrorMessage(), "OK");
				return;
			}


			foreach (DashboardQuery d in result)
			{
				d.Query1 = getQueryKategori(d.Query1, SettingVariable.Cabang.KodeCabang, SettingVariable.Cabang.KodeStaff) + d.Query1;
				//d.Query1 = Regex.Replace(d.Query1, @"{(.*)}", "");
			}

			bool isPie = true;
			foreach (DashboardQuery d in result)
			{
				Debug.WriteLine(d.Query1);
				StackLayout stack = new StackLayout
				{
					Padding = new Thickness(0, 16),
					Orientation = StackOrientation.Vertical
				};

				ActivityIndicator indicator = new ActivityIndicator
				{
					IsRunning = true,
					IsVisible = true
				};

				Label loading = new Label
				{
					Text = "Loading",
					HorizontalTextAlignment = TextAlignment.Center
				};

				if (Device.RuntimePlatform == Device.Android) 
				{
					indicator.Color = Color.FromHex("#ffffff");
					indicator.HeightRequest = 20;
					indicator.WidthRequest = 20;
					loading.TextColor = Color.FromHex("#ffffff");
				}

				stack.Children.Add(indicator);
				stack.Children.Add(loading);
				root.Children.Add(stack);

				List<DashboardModel> list = null;
				try
				{
					list = await Task.Run(() => DependencyService.Get<IDbDataFetcher>().getDashboard(d.Query1), cts.Token);
					await Task.Delay(1500);
				}
				catch (TaskCanceledException e) 
				{
					Debug.WriteLine(e.Message);
					return;
				}

				Label label = new Label
				{
					HorizontalTextAlignment = TextAlignment.Center,
					Text = d.Judul
				};


				if (Device.RuntimePlatform == Device.Android)
				{
					label.TextColor = Color.FromHex("#ffffff");
				}

				if (list != null && list.Count > 0)
				{
					if (isPie)
					{
						FlexPie myPie = new FlexPie
						{
							ItemsSource = list,
							HeightRequest = 300,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							BindingName = "Name",
							Binding = "Value"
						};

						stack.Children.Clear();
						stack.Children.Add(label);
						stack.Children.Add(myPie);

						isPie = false;
					}
					else
					{
						FlexChart myChart = new FlexChart
						{
							ItemsSource = list,
							HeightRequest = 300,
							BindingX = "Name",
						};
						ChartSeries chartSeries = new ChartSeries
						{
							Binding = "Value"
						};
						myChart.Series.Add(chartSeries);

						stack.Children.Clear();
						stack.Children.Add(label);
						stack.Children.Add(myChart);

						isPie = true;
					}
				}
				else 
				{
					stack.Children.Clear();
					stack.Children.Add(label);
					Label noData = new Label
					{
						HorizontalTextAlignment = TextAlignment.Center,
						Text = "No Data Available"
					};

					if (Device.RuntimePlatform == Device.Android)
					{
						noData.TextColor = Color.FromHex("#ffffff");
					}

					stack.Children.Add(noData);
				}


			}
		}

		string getQueryKategori(string query1, string cabang, string kodeStaff)
		{
			string q = "";

			if (query1.IndexOf("#kategori", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				q += "\n if (select object_id('tempdb..#Kategori')) is not null \n"
					+ "drop table #Kategori \n"
					+ "select * into #Kategori from Kategori where Cabang = '" + cabang + "' \n";
			}

			if (query1.IndexOf("#staff", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				q += "\n if (select object_id('tempdb..#staff')) is not null \n"
					+ "drop table #staff \n"
					+ "select * into #staff from staff where kode = '" + kodeStaff + "' \n";
			}

			return q;
		}
	}
}
