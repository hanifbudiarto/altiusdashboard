using System;
using System.Diagnostics;
using System.Collections.Generic;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace AltiusDashboard
{
    public partial class ShowChartPage : ContentPage
    {
        ShowChartPage() {}

        public ShowChartPage(string title, string query) 
        {
            InitializeComponent();


            Title = title;
            Debug.WriteLine(query);

            getDataReport(query);
        }

        async Task getDataReport(string query)
        {
            onLoading(true);
            List<DashboardModel> list = await Task.Run(() => DependencyService.Get<IDbDataFetcher>().getDashboard(query));
            if (list != null && list.Count > 0) {
                chart.ItemsSource = list;
                nodata.IsVisible = false;
            }
            else {
                nodata.IsVisible = true;
            }
            onLoading(false);
        }

        void onLoading(bool loading)
        {
            chart.IsVisible = !loading;
            layoutLoading.IsVisible = loading;
        }
    }
}
