using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AltiusDashboard
{
	public interface IDbDataFetcher
	{
		void reloadConnectionString();
		List<MainReport> getMainReport(string query);
		List<ItemReport> getItemReport(string query);
		string getQueryReport(string query);
		List<ChoiceItem> getChoiceData(string query);
		List<User> getUser(string query);
		ReportModel getReportData(string query);
		List<Branch> getBranch(string query);
		List<DashboardQuery> getDashboardQueries(string query);
		List<DashboardModel> getDashboard(string query);
		string getErrorMessage();
	}
}
