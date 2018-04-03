using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AltiusDashboard
{
	public class ReportModel
	{
		public ObservableCollection<Model> Data { get; set; }
		public List<Column> Headers { get; set; }
		public List<Summary> Sum { get; set; }

		public ReportModel() 
		{
			Data = new ObservableCollection<Model>();
			Headers = new List<Column>();
			Sum = new List<Summary>();
		}
	}
}
