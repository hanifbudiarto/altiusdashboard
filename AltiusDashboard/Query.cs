using System;
namespace AltiusDashboard
{
	public class Query
	{
		public static string getMainReport() 
		{
			return "select dept from Reportlist r, MenuGrup m" +
				" where m.ObjectName like 'R%' and m.ObjectName = 'R' + cast(r.id as varchar(100))" +
				" group by r.Dept order by r.Dept";
		}

		public static string getItemReport(string dept)
		{
			return "select r.ID, r.Title from Reportlist r, MenuGrup m" +
				" where m.ObjectName like 'R%' and m.ObjectName = 'R' + cast(r.id as varchar(100))" +
				"and r.Dept = '" + dept + "' group by r.ID, r.Title order by r.Title";
		}

		public static string getQueryReport(string id) 
		{
			return "select query1, class from Reportlist where id ='" + id + "'";
		}

		public static string getBranchQuery(string username)
		{
			return "SELECT k.Cabang,k.NamaCabang,s.Kode from Staff s,StaffCabang scb,  " +
				"Kategori k where s.Email = '" + username + "'  and s.kode = scb.staff and k.Cabang = scb.Cabang ";
		}

		public static string getDashboardQuery() 
		{
			return "SELECT d.ID, d.Title, dr.ReportList, r.Title as Judul, r.Query1 from DashboardReportList dr " +
				"left join Dashboard d " +
				"on d.ID = dr.Dashboard " +
				"left join ReportList r " +
				"on r.ID = dr.ReportList " +
				"order by d.ID asc";
		}

		public static string getItemReportByKeyword(string keyword) 
		{
			return "select r.ID, r.Title from Reportlist r, MenuGrup m" +
				" where m.ObjectName like 'R%' and m.ObjectName = 'R' + cast(r.id as varchar(100))" +
				"and r.Title like '%" + keyword + "%' group by r.ID, r.Title order by r.Title";
		}
	}
}
