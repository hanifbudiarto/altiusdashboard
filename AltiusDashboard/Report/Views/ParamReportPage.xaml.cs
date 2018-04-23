using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using C1.Xamarin.Forms.Calendar;
using C1.Xamarin.Forms.Input;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class ParamReportPage : ContentPage
	{
		string queryKategori = "";
		string queryOriginal = "";
		List<ModelParsingan> parameterList = new List<ModelParsingan>();
		CancellationTokenSource cts;

		string _id;
        string _class;

		public ParamReportPage() 
		{
			InitializeComponent();
		}

		public ParamReportPage(string id, string title)
		{
			InitializeComponent();

			Title = title;
			_id = id;
		}

		protected override void OnAppearing()
		{
			cts = new CancellationTokenSource();
			layoutParameter.Children.Clear();
            getQueryReport(_id);
		}

		protected override void OnDisappearing()
		{
			cts.Cancel();
			base.OnDisappearing();
		}

		void onLoading(bool loading)
		{
			layoutLoading.IsVisible = loading;
			layoutParameter.IsVisible = !loading;
		}

		async void showReport() 
		{
			onLoading(true);
			string finalQuery = await Task.Run(() => getFinalQuery(queryKategori + queryOriginal));
            //Navigation.PushAsync(new ShowReportPage(Title, finalQuery));

            Debug.WriteLine("CLASS " + _class);
            if (_class.ToUpper().Equals("CHART"))
            {
                // go to another page
                Navigation.InsertPageBefore(new ShowChartPage(Title, finalQuery), this);
            }
            else
            {
                Navigation.InsertPageBefore(new ShowReportPage(Title, finalQuery), this);
            }
			await Navigation.PopAsync().ConfigureAwait(false);
		}


		async Task getQueryReport(string id)
		{
			onLoading(true);
			var myTask = Task.Run(() => DependencyService.Get<IDbDataFetcher>().getQueryReport(Query.getQueryReport(id)));
            Reportlist report = await myTask;
            _class = report.QueryClass;
            queryKategori = getQueryKategori(report.Query1, SettingVariable.Cabang.KodeCabang, SettingVariable.Cabang.KodeStaff);
            queryOriginal = report.Query1;

            Debug.WriteLine(queryKategori + report.Query1);
            await ScanAmbilParameter(report.Query1);
            onLoading(false);
		}

		string getFinalQuery(string query)
		{
			try
			{
				bool masihada = true;

				while (masihada == true)
				{
					Regex regexObj = new Regex(":(.*?):");
					Match matchResult = regexObj.Match(query);
					if (matchResult.Success)
					{
						//list_parameter.Add (matchResult.Value);
						// matched text: matchResult.Value
						// match start: matchResult.Index
						// match length: matchResult.Length


						//ini cmn buat nyecan namavariabel
						string namavar = "";
						try
						{
							Regex regexObj2 = new Regex(@"[\:][\s]*([a-zA-Z0-9_\s]*)[\s]*[\=][\s]*([a-zA-Z]*)[\s]*([\(]([\w\W\s]*)[\)])?[\s]*[\:]");
							Match matchResults = regexObj2.Match(matchResult.Value);
							while (matchResults.Success)
							{


								for (int i = 1; i <= matchResults.Groups.Count; i++)
								{
									Group groupObj = matchResults.Groups[i];
									if (groupObj.Success)
									{
										// matched text: groupObj.Value
										// match start: groupObj.Index
										// match length: groupObj.Length

										if (i == 1)
										{
											namavar = groupObj.Value;
										}
										else if (i == 2)
										{
										}
										else if (i == 4)
										{
										}
									}
								}


								matchResults = matchResults.NextMatch();
							}
						}
						catch (ArgumentException ex)
						{
							Debug.WriteLine(ex.Message);
						}

						//dapat namavariabel

						/////////
						//cek sebelum add
						ModelParsingan itemx = parameterList.Where(z => z.NamaVariabel.ToLower() == namavar.ToLower()).FirstOrDefault();

						if (itemx != null)
						{
							//uda ada
							//replace
							query = query.Replace(matchResult.Value, itemx.NilaiAkhir);
						}
						/////////

						matchResult = matchResult.NextMatch();
					}
					else
					{
						masihada = false;
					}

				}
			}
			catch (Exception es)
			{
				Debug.WriteLine("" + es.Message + "" + es.StackTrace);
			}

			return query;
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

		async Task ScanAmbilParameter(string query1)
		{
			try
			{
				Regex regexObj = new Regex(":(.*?):");
				Match matchResult = regexObj.Match(query1);
				while (matchResult.Success)
				{
					try
					{
						Regex regexObj2 = new Regex(@"[\:][\s]*([a-zA-Z0-9_\s]*)[\s]*[\=][\s]*([a-zA-Z]*)[\s]*([\(]([\w\W\s]*)[\)])?[\s]*[\:]");
						Match matchResults = regexObj2.Match(matchResult.Value);
						while (matchResults.Success)
						{
							string namavar = "";
							ModelParsingan item = new ModelParsingan();
							item.RAW = matchResult.Value;
							for (int i = 1; i <= matchResults.Groups.Count; i++)
							{
								Group groupObj = matchResults.Groups[i];
								if (groupObj.Success)
								{
									// matched text: groupObj.Value
									// match start: groupObj.Index
									// match length: groupObj.Length

									if (i == 1)
									{
										item.NamaVariabel = groupObj.Value;
										namavar = groupObj.Value;
									}
									else if (i == 2)
									{
										item.TipeData = groupObj.Value;
									}
									else if (i == 4)
									{
										item.QueryDalamChoice = groupObj.Value;
									}
								}
							}

							/////////
							//cek sebelum add
							ModelParsingan itemx = parameterList.Where(z => z.NamaVariabel.ToLower() == namavar.ToLower()).FirstOrDefault();

							if (itemx != null)
							{
								//uda ada
							}
							else
							{
								parameterList.Add(item);
							}
							/////////

							matchResults = matchResults.NextMatch();
						}
					}
					catch (ArgumentException ex)
					{
						Debug.WriteLine(ex.Message);
					}

					matchResult = matchResult.NextMatch();
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.Message);
			}

			Debug.WriteLine("Count the parameter : " + parameterList.Count);

			for (int i = 0; i < parameterList.Count; i++)
			{
				Debug.WriteLine("###" + i);
				Debug.WriteLine("nama:" + parameterList[i].NamaVariabel);
				Debug.WriteLine("tipe:" + parameterList[i].TipeData);
				Debug.WriteLine("query:" + parameterList[i].QueryDalamChoice);
				Debug.WriteLine("raw:" + parameterList[i].RAW);

				if (parameterList[i].TipeData.ToLower().Equals("numeric"))
				{
					GenerateTampilan_numeric(parameterList[i]);
				}
				else if (parameterList[i].TipeData.ToLower().Equals("string"))
				{
					GenerateTampilan_string(parameterList[i]);
				}
				else if (parameterList[i].TipeData.ToLower().Equals("datetime"))
				{
					GenerateTampilan_datetime(parameterList[i]);
				}
				else if (parameterList[i].TipeData.ToLower().Equals("choice"))
				{
					//await Task.Run(() => GenerateTampilan_choice(parameterList[i]));
					await GenerateTampilan_choice2(parameterList[i]);
				}
			}

			var btn = new Button
			{
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#007EAD"),
				Text = "Show Report",
				WidthRequest = 100,
				HorizontalOptions = LayoutOptions.End
			};
			btn.Clicked += (sender, e) => showReport();

			if (parameterList.Count == 0)
			{
				showReport();
			}
			else
			{
				layoutParameter.Children.Add(btn);
			}
		}

		void GenerateTampilan_numeric(ModelParsingan model)
		{
			var label = new Label
			{
				Text = model.NamaVariabel,
				FontSize = 20,
				TextColor = Color.FromHex("#999999"),
				HorizontalTextAlignment = TextAlignment.Start,
			};

			var entry = new Entry
			{
				Text = "0",
				HorizontalTextAlignment = TextAlignment.Start,
				FontSize = 18,
				PlaceholderColor = Color.FromHex("#888888"),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Keyboard = Keyboard.Numeric,
			};
			if (Device.RuntimePlatform == Device.Android) entry.TextColor = Color.White;

			entry.TextChanged += (sender, args) =>
			{

				string input = entry.Text;
				if (entry.Text.Trim().Equals(""))
				{
					input = "0";
				}

				//update nilai list//
				if (model != null)
				{
					// ada //update nilai
					model.NilaiAkhir = input;
				}
			};


			var layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 8,
				Orientation = StackOrientation.Vertical,
				Children = {
					label,
					entry
				}

			};

			if (entry.Text.Trim().Equals(""))
				entry.Text = "0";

			string input2 = entry.Text;
			//update nilai list//
			if (model != null)
			{
				// ada //update nilai
				model.NilaiAkhir = input2;
			}
			////////////////

			layoutParameter.Children.Add(layout);
		}

		void GenerateTampilan_string(ModelParsingan model)
		{
			var label = new Label
			{
				Text = model.NamaVariabel,
				FontSize = 20,
				TextColor = Color.FromHex("#999999"),
				HorizontalTextAlignment = TextAlignment.Start,
			};

			var entry = new Entry
			{
				Placeholder = model.NamaVariabel,
				HorizontalTextAlignment = TextAlignment.Start,
				FontSize = 18,
				PlaceholderColor = Color.FromHex("#888888"),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Keyboard = Keyboard.Text,
			};
			if (Device.RuntimePlatform == Device.Android) entry.TextColor = Color.White;

			entry.TextChanged += (sender, args) =>
			{
				string input = entry.Text;
				//update nilai list//
				if (model != null)
				{
					// ada //update nilai
					model.NilaiAkhir = "'" + input + "'";
				}
						////////////////
			};
			var layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 8,
				Orientation = StackOrientation.Vertical,
				Children = {
					label,
					entry
				}
			};

			string input2 = entry.Text;
			//update nilai list//
			if (model != null)
			{
				// ada //update nilai
				model.NilaiAkhir = "'" + input2 + "'";
			}
			////////////////

			layoutParameter.Children.Add(layout);

		}

		void GenerateTampilan_datetime(ModelParsingan model)
		{
			var label = new Label
			{
				Text = model.NamaVariabel,
				FontSize = 20,
				TextColor = Color.FromHex("#999999"),
				HorizontalTextAlignment = TextAlignment.Start,
			};

			var date = new DatePicker
			{
				Format = "yyyy-MM-dd",
				HorizontalOptions = LayoutOptions.FillAndExpand

			};
			if (Device.RuntimePlatform == Device.Android) date.TextColor = Color.White;

			date.DateSelected += (sender, args) =>
			{

				string format = "yyyy/MM/dd";
				string tgl = date.Date.ToString(format);

				//update nilai list//

				if (model != null)
				{
					// ada //update nilai
					model.NilaiAkhir = "'" + tgl + "'";
				}
			};
			var layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 8,
				Orientation = StackOrientation.Vertical,
				Children = {
					label,
					date
				}

			};


			string format2 = "yyyy/MM/dd";
			string tgl2 = date.Date.ToString(format2);
			//update nilai list//

			if (model != null)
			{
				// ada //update nilai
				model.NilaiAkhir = "'" + tgl2 + "'";
			}
			////////////////

			layoutParameter.Children.Add(layout);
		}

		async Task GenerateTampilan_choice2(ModelParsingan model) 
		{
			var autoComplete = new C1AutoComplete
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				SelectedBackgroundColor = Color.Accent,
				DisplayMemberPath = "Value",
			};


			var label = new Label
			{
				Text = model.NamaVariabel,
				FontSize = 20,
				TextColor = Color.FromHex("#999999"),
				HorizontalTextAlignment = TextAlignment.Start,
			};

			var layout = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Spacing = 8,
				Orientation = StackOrientation.Vertical,
				Children = {
					label,
					autoComplete
				}
			};

			layoutParameter.Children.Add(layout);

			string sqlQuery = queryKategori + model.QueryDalamChoice;
			List<ChoiceItem> list = null;
			try
			{
				list = await Task.Run(() => DependencyService.Get<IDbDataFetcher>().getChoiceData(sqlQuery), cts.Token);
			}
			catch (TaskCanceledException e) 
			{
				Debug.WriteLine(e.Message);
				return;
			}

			if (list == null)
			{
				if (PageUtils.PageTypeIsAlreadyAtTopOfStack(this, typeof(ParamReportPage)))
				{
					await DisplayAlert("Error", DependencyService.Get<IDbDataFetcher>().getErrorMessage(), "OK");
					await Task.Delay(1000);
				}
			}
			else
			{
				ObservableCollection<ChoiceItem> coll = new ObservableCollection<ChoiceItem>(list);
				autoComplete.ItemsSource = coll;
				autoComplete.SelectedIndexChanged += (sender, e) =>
				{
					ChoiceItem item = list[autoComplete.SelectedIndex];
					if (model != null)
					{
						model.NilaiAkhir = "'" + item.Kode + "'";
					}
				};
			}
		}

		public class ModelParsingan
		{
			public string NamaVariabel { get; set; }

			public string TipeData { get; set; }

			public string RAW { get; set; }

			public string NilaiAkhir { get; set; }

			public string QueryDalamChoice { get; set; }
		}
	}
}
