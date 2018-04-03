using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using C1.CollectionView;
using Xamarin.Forms;

namespace AltiusDashboard
{
	public partial class FilterForm : ContentPage
	{
		private TaskCompletionSource<bool> _completion;
		public ObservableCollection<StringFilter> Filters { get; set; }

		public FilterForm()
		{
			InitializeComponent();
			Filters = new ObservableCollection<StringFilter>();

			var operators = new List<FilterOperation>();
			operators.Add(FilterOperation.Contains);
			operators.Add(FilterOperation.StartsWith);
			operators.Add(FilterOperation.EndsWith);
			operators.Add(FilterOperation.EqualText);

			grid.AutoGeneratingColumn += (sender, e) => 
			{
				if (e.Property.Name == "Field") 
				{
					e.Cancel = true;
				}

				if (e.Property.Name == "FieldName") 
				{
					e.Column.IsReadOnly = true;
					e.Column.Width = GridLength.Auto;
					e.Column.Header = "Field";
				}

				if (e.Property.Name == "Operation")
				{
					e.Column.Width = GridLength.Auto;
					e.Column.DataMap = new C1.Xamarin.Forms.Grid.GridDataMap()
					{
						ItemsSource = operators
					};
				}

				if (e.Property.Name == "Value") 
				{
					e.Column.Width = GridLength.Star;
				}
			};
			grid.ItemsSource = Filters;
		}

		private async void FilterClicked(object sender, EventArgs e) 
		{
			grid.FinishEditing();
			await Navigation.PopModalAsync(true);
			_completion.TrySetResult(true);
		}

		private async void CancelClicked(object sender, EventArgs e) 
		{
			if (_completion != null)
			{
				_completion.TrySetCanceled();				           
			}

			await Navigation.PopModalAsync();
		}

		public async Task ShowAsync(INavigation navigation) 
		{
			_completion = new TaskCompletionSource<bool>();
			await navigation.PushModalAsync(this, true);
			await _completion.Task;
		}

		protected override bool OnBackButtonPressed()
		{
			if (_completion != null) 
			{
				_completion.TrySetCanceled();
			}
			return base.OnBackButtonPressed();
		}
	}

	public class StringFilter
	{
		public string Field { get; set; }
		public string FieldName { get; set; }
		public FilterOperation Operation { get; set; }
		public string Value { get; set; }	
	}
}
