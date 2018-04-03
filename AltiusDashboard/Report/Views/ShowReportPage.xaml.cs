
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using C1.CollectionView;
using C1.Xamarin.Forms.Grid;
using C1.Xamarin.Forms.Input;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class ShowReportPage : ContentPage
	{
		ReportModel _report;
		List<string> groupModels = new List<string>();

		public ShowReportPage() 
		{
			InitializeComponent();
		}

		public ShowReportPage(string title, string query)
		{
			InitializeComponent();


			Title = title;
			Debug.WriteLine(query);

			groupby.Children.Clear();
			grid.ScrollPositionChanged += (sender, e) => 
			{
				summary.ScrollPosition = new Point(grid.ScrollPosition.X, summary.ScrollPosition.Y);
			};
            getDataReport(query);
		}

		private async void OnFilterClicked(object sender, EventArgs e) 
		{
			var filtering = grid.CollectionView as ISupportFiltering;
			if (filtering != null) 
			{
				var filterForm = new FilterForm();
				var currentFilters = GetCurrentFilters(filtering.FilterExpression);
				foreach (var column in grid.Columns) 
				{
					if (column.DataType == typeof(string)) 
					{
						var stringFilter = new StringFilter();
						stringFilter.Field = column.Binding;
						stringFilter.FieldName = column.ActualHeader;
						var existingFilter = currentFilters.FirstOrDefault(f => f.FilterPath == column.Binding);
						if (existingFilter != null)
						{
							stringFilter.Operation = existingFilter.FilterOperation;
							stringFilter.Value = existingFilter.Value.ToString();
						}
						else 
						{
							stringFilter.Operation = FilterOperation.Contains;
						}
						filterForm.Filters.Add(stringFilter);
					}
				}

				try
				{
					await filterForm.ShowAsync(Navigation);
					var filters = new List<FilterExpression>();
					foreach (var filter in filterForm.Filters) 
					{
						if (!string.IsNullOrWhiteSpace(filter.Value))
						{
							filters.Add(new FilterUnaryExpression(filter.Field, filter.Operation, filter.Value));
						}
					}
					await filtering.FilterAsync(FilterExpression.Combine(FilterCombination.And, filters.ToArray()));
				}
				catch (OperationCanceledException) { }
			}
		}

		private IEnumerable<FilterUnaryExpression> GetCurrentFilters(FilterExpression filterExpression) 
		{
			var uf = filterExpression as FilterUnaryExpression;
			if (uf != null) 
			{
				yield return uf;
			}

			var bf = filterExpression as FilterBinaryExpression;
			if (bf != null) 
			{
				foreach (var lf in GetCurrentFilters(bf.LeftExpression)) 
				{
					yield return lf;
				}

				foreach (var rf in GetCurrentFilters(bf.RightExpression))
				{
					yield return rf;
				}
			}
			yield break;
		}

		async Task getDataReport(string query)
		{
			onLoading(true);
			var myTask = Task.Run(() => DependencyService.Get<IDbDataFetcher>().getReportData(query));

			_report = await myTask;
			if (_report == null)
			{
				await DisplayAlert("Error", DependencyService.Get<IDbDataFetcher>().getErrorMessage(), "OK");
				onLoading(false);
				return;
			}


			Debug.WriteLine("++++++++ Summary");
			List<Model> emptyList = new List<Model>();
			C1CollectionView<Model> collSummary = new C1CollectionView<Model>(emptyList);
			summary.ItemsSource = collSummary;
			for (int i = 0; i < _report.Sum.Count; i++) 
			{
				double val = _report.Sum[i].Value;
				Debug.WriteLine(val);
				if (val.Equals(-999999))
				{
					summary.Columns[i].Header = "-";
				}
				else 
				{
					summary.Columns[i].Header = string.Format("{0:n}", val);
				}
				summary.Columns[i].HeaderHorizontalAlignment = LayoutAlignment.End;
			}
			Debug.WriteLine("++++++++ Summary");


			if (_report.Data != null && _report.Data.Count > 0)
			{
				C1CollectionView<Model> collview = new C1CollectionView<Model>(_report.Data);
				grid.ItemsSource = collview;
				ObservableCollection<Column> columns = new ObservableCollection<Column>();
				for (int i = 0; i < _report.Headers.Count; i++) 
				{
					grid.Columns[i].Header = _report.Headers[i].ColumnName;
					columns.Add(_report.Headers[i]);
				}

				group.ItemsSource = columns;
				group.SelectedIndexChanged += onGroupingChanged;
				add.Clicked += onClickedAdd;
				clear.Clicked += onClickedClear;

			}
			Debug.WriteLine("Report data :" + _report.Data.Count);
            onLoading(false);
			onDataExist(_report.Data.Count > 0);
		}

		void onClickedClear(object sender, EventArgs e)
		{
			groupby.Children.Clear();
			groupModels.Clear();

			C1CollectionView<Model> collview = new C1CollectionView<Model>(_report.Data);
			grid.ItemsSource = collview;
			for (int i = 0; i<_report.Headers.Count; i++) 
			{
				grid.Columns[i].Header = _report.Headers[i].ColumnName;			
			}
		}

		void onClickedAdd(object sender, EventArgs e)
		{
			if (group.SelectedIndex != -1) 
			{
				string m = "Model" + (group.SelectedIndex + 1);
				if (groupModels.Exists(x => x == m)) return;

				Column col = (Column) group.SelectedItem;
				groupModels.Add("Model" + (group.SelectedIndex + 1));

				var lbl = new Label
				{
					TextColor = Color.FromHex("#888888"),
					Text = col.ColumnName,
					FontSize = 10
				};
				groupby.Children.Add(lbl);


				startGrouping();
			}
		}

		async void startGrouping() 
		{
			GroupDescription[] groupDesc = new GroupDescription[groupModels.Count];

			for (int i=0; i<groupModels.Count; i++) 
			{
				GroupDescription gd = new GroupDescription(groupModels[i]);
				groupDesc[i] = gd;
			}

			C1CollectionView<Model> collview = new C1CollectionView<Model>(_report.Data);
			await collview.GroupAsync(groupDesc); 
			grid.ItemsSource = collview;

			for (int i = 0; i<_report.Headers.Count; i++) 
			{
				grid.Columns[i].Header = _report.Headers[i].ColumnName;			
			}
		}

		async void onGroupingChanged(object sender, EventArgs e)
		{
			//C1CollectionView<Model> collview = new C1CollectionView<Model>(_report.Data);
			//await collview.GroupAsync(new GroupDescription("Model" + (group.SelectedIndex +1))); 
			//grid.ItemsSource = collview;

			//for (int i = 0; i<_report.Headers.Count; i++) 
			//{
			//	grid.Columns[i].Header = _report.Headers[i].ColumnName;
			//}
		}

		void OnAutoGeneratingColumn(object sender, GridAutoGeneratingColumnEventArgs e)
		{
			string propertyName = e.Property.Name;
			int propertyNameLength = e.Property.Name.Length;

			string column = propertyName.Substring(5, propertyNameLength - 5);
			int columnNumber;
			bool successfullyParsed = Int32.TryParse(column, out columnNumber);
            if (successfullyParsed)
            {
				if (columnNumber > _report.Headers.Count)
				{
					e.Cancel = true;
				}
				else
				{
					string dataType = _report.Headers[columnNumber - 1].DataType;
					Debug.WriteLine(dataType);
					if (dataType == "decimal" || dataType == "int" || dataType == "money")
					{
						e.Column.HorizontalAlignment = LayoutAlignment.End;
						e.Column.Format = "N0";
						e.Column.Aggregate = GridAggregate.Sum;
					}
				}
            }
		}

		void onDataExist(bool exist) 
		{
			nodata.IsVisible = !exist;
			grid.IsVisible = exist;
		}

		void onLoading(bool loading) 
		{
			nodata.IsVisible = !loading;
			grid.IsVisible = !loading;
			layoutLoading.IsVisible = loading;
		}
	}
}
