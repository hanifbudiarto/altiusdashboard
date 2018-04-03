using System;
namespace AltiusDashboard
{
	public class DashboardModel
	{
		public string Name { get; set; }
		public double Value { get; set; }

		public DashboardModel(string name, double value)
		{
			this.Name = name;
			this.Value = value;
		}
	}
}
