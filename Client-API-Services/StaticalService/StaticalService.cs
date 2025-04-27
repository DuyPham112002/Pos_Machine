using Client_DBAccess.Dapper;
using Client_ViewModel.Dashboard;
using Client_ViewModel.DashBoard;
using Client_ViewModel.Order;
using Client_ViewModel.Product;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Client_API_Services.StaticalService
{
	public interface IStaticalService
	{
		Task<IEnumerable<NumberSatical>> GetNumberStaticAsync();
		Task<LineChartViewModel> GetParamLineChartAsync(int type);
		Task<AppointmentChartViewModel> GetParamAppointmentChartAsync();
		Task<SurgeryChartViewModel> GetParamSurgeryChartAsync();
		Task<DiseasesChartViewModel> GetParamDiseasesChartAsync(int type);
		Task<IEnumerable<OrderViewModel>> GetTopOrderAsync(int type);
		Task<IEnumerable<ProductViewModel>> GetTopProductAsync(int type);
	}
	public class StaticalService : IStaticalService
	{
		private readonly IDapperContext _dapper;
		private readonly string _queryNumStatical = @"
							SELECT 
								CAST(1 AS int) AS [index], 
								CAST('Employee' AS VARCHAR(50)) AS [Title], 
								CAST(
												SUM(
													CASE WHEN E.AccId IS NOT NULL THEN 1 ELSE 0 END +
													CASE WHEN M.AccId IS NOT NULL THEN 1 ELSE 0 END
												) AS FLOAT
								) AS [Number]
								FROM 
												Account AS A WITH (NOLOCK)
								LEFT JOIN 
												Employee AS E WITH (NOLOCK) ON E.AccId = A.Id AND A.IsActive = 1
								LEFT JOIN 
												Manager AS M WITH (NOLOCK) ON M.AccId = A.Id AND A.IsActive = 1
								WHERE 
												A.IsActive = 1 

								UNION

								SELECT 
												CAST(2 AS int) AS [index], 
												CAST('Product' AS VARCHAR(50)) AS [Title], 
												CAST(COUNT(*) AS FLOAT) AS [Number]
								FROM 
												Product AS P WITH (NOLOCK)
								WHERE 
												P.IsActive = 1

								UNION 

								SELECT 
												CAST(3 AS int) AS [index], 
												CAST('Order' AS VARCHAR(50)) AS [Title], 
												CAST(COUNT(*) AS FLOAT) AS [Number]
								FROM 
												[Order] AS O WITH (NOLOCK)
								WHERE 
												MONTH(O.CreatedOn) = MONTH(GETDATE())

								UNION 

								SELECT  
												CAST(4 AS int) AS [index], 
												CAST('Revenue' AS VARCHAR(50)) AS [Title], 
												CAST(ISNULL(Sum(O.Total),0) AS FLOAT) AS [Number]
								FROM 
												[Order] AS O WITH (NOLOCK)
								WHERE 
												MONTH(O.CreatedOn) = MONTH(GETDATE()) And O.Status = 2
			";
		public StaticalService(IDapperContext dapper)
        {
            _dapper = dapper;
        }

		public async Task<IEnumerable<NumberSatical>> GetNumberStaticAsync()
		{
			
			using (var conection = _dapper.CreateConnection())
			{
				try
				{
					var result = await conection.QueryAsync<NumberSatical>(_queryNumStatical);
					return result.ToList();
				}
				catch (Exception ex)
				{
					return null;
				}
			}
		}

        public async Task<LineChartViewModel> GetParamLineChartAsync(int type)
        {
            var lineChartViewModel = new LineChartViewModel();
            string[] tables = { "Doanh Thu" };

            try
            {
                List<string> labels = GetLabelsByType(type);
                lineChartViewModel.Labels = labels;

                using (var connection = _dapper.CreateConnection())
                {
                    foreach (var table in tables)
                    {
                        List<double> data = await GetDataByType(connection, labels, type);
                        lineChartViewModel.Series.Add(new series
                        {
                            name = table,
                            data = data
                        });
                    }
                }

                lineChartViewModel.MaxValue = lineChartViewModel.Series.Max(p => p.data.Max());
                return lineChartViewModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private List<string> GetLabelsByType(int type)
        {
            return type switch
            {
                1 => new List<string> { "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy", "Chủ Nhật" },
                2 => GetWeeksInCurrentMonth(),
                3 => GetMonthsInCurrentYear(),
                _ => GetLastFiveYears()
            };
        }

        private async Task<List<double>> GetDataByType(IDbConnection connection, List<string> labels, int type)
        {
            var data = new List<double>();

            foreach (var label in labels)
            {
                string query = type switch
                {
                    1 => $"SELECT CAST(ISNULL(SUM(O.Total), 0) AS FLOAT) FROM [Order] AS O WITH(NOLOCK) WHERE CONVERT(date, O.CreatedOn) = CONVERT(date, '{GetCurrentWeekDates().OrderBy(p => p.Date).ToList()[labels.IndexOf(label)]}') AND O.Status = 2",
                    2 => $"SELECT CAST(ISNULL(SUM(O.Total), 0) AS FLOAT) FROM [Order] AS O WITH(NOLOCK) WHERE {label.Replace("Tuần", "").Trim()} = DATEPART(WEEK, O.CreatedOn) AND O.Status = 2",
                    3 => $"SELECT CAST(ISNULL(SUM(O.Total), 0) AS FLOAT) FROM [Order] AS O WITH(NOLOCK) WHERE {label.Replace("Tháng", "").Trim()} = MONTH(O.CreatedOn) AND O.Status = 2",
                    _ => $"SELECT CAST(ISNULL(SUM(O.Total), 0) AS FLOAT) FROM [Order] AS O WITH(NOLOCK) WHERE {label} = DATEPART(YEAR, O.CreatedOn) AND O.Status = 2"
                };

                double number = await connection.QuerySingleAsync<double>(query);
                data.Add(number);
            }

            return data;
        }


        public async Task<AppointmentChartViewModel> GetParamAppointmentChartAsync()
		{
			AppointmentChartViewModel appointmentChart = new AppointmentChartViewModel();
			try
			{
				string datecondition = "";
				string query = "";
				List<DateTime> CurrentDates = GetCurrentWeekDates().OrderBy(p => p.Date).ToList();
				using (var conection = _dapper.CreateConnection())
				{
					for (int i = 0; i < CurrentDates.Count; i++)
					{
						datecondition = CurrentDates[i].ToString();
						query = $"select CAST(ISNULL(Sum(O.Total),0) AS FLOAT) as Total from [Order] as O with(nolock) where CONVERT(date, '{datecondition}') = CONVERT(date, O.CreatedOn) and O.Status = 2";
						double number = await conection.QuerySingleAsync<double>(query);
						appointmentChart.data.Add(new Param
						{
							x = CurrentDates[i].ToString("dddd", new CultureInfo("vi-VN")),
							y = number
						});
					}
                    datecondition = DateTime.Now.ToString("yyyy-MM-dd");
                    query = $"select CAST(ISNULL(Sum(O.Total),0) AS FLOAT) as Total from [Order] as O with(nolock) WHERE DATEPART(WEEK, O.CreatedOn) = DATEPART(WEEK, '{datecondition}') AND DATEPART(YEAR, O.CreatedOn) = DATEPART(YEAR, '{datecondition}') and O.Status = 2";
                    double sumThisweek = await conection.QuerySingleAsync<double>(query);

                    datecondition = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
                    query = $"select CAST(ISNULL(Sum(O.Total),0) AS FLOAT) as Total from [Order] as O with(nolock) WHERE DATEPART(WEEK, O.CreatedOn) = DATEPART(WEEK, '{datecondition}') AND DATEPART(YEAR, O.CreatedOn) = DATEPART(YEAR, '{datecondition}') and O.Status = 2";
                    double sumLastweek = await conection.QuerySingleAsync<double>(query);

                    appointmentChart.sinceLastWeek = sumLastweek != 0
                        ? Math.Round(((sumThisweek - sumLastweek) / Math.Abs(sumLastweek)) * 100, 2)
                        : 0;
                }
				return appointmentChart;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<SurgeryChartViewModel> GetParamSurgeryChartAsync()
		{
			SurgeryChartViewModel surgeryChart = new SurgeryChartViewModel();
			try
			{
				string datecondition = "";
				string query = "";
				List<DateTime> CurrentDates = GetEndOfWeeksInCurrentMonth().OrderBy(p => p.Date).ToList();
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				Calendar calendar = cultureInfo.Calendar;
				using (var conection = _dapper.CreateConnection())
				{
					for (int i = 0; i < CurrentDates.Count; i++)
					{
						datecondition = CurrentDates[i].ToString();
						query = $"select CAST( ISNULL(Sum(O.Total),0) AS FLOAT) as Total from [Order] as O with(nolock) WHERE DATEPART(WEEK, O.CreatedOn) = DATEPART(WEEK, '{datecondition}') AND DATEPART(YEAR, O.CreatedOn) = DATEPART(YEAR, '{datecondition}') and O.Status = 2";
						double number = await conection.QuerySingleAsync<double>(query);
						surgeryChart.data.Add(new data
						{
							x = "Tuần" + " " + calendar.GetWeekOfYear(CurrentDates[i], CalendarWeekRule.FirstDay, DayOfWeek.Monday).ToString(),
							y = number
						});
					}
					datecondition = DateTime.Now.ToString("yyyy-MM-dd");
					query = $"select CAST( ISNULL(Sum(O.Total),0) AS FLOAT) as Total from [Order] as O with(nolock) WHERE DATEPART(MONTH, O.CreatedOn) = DATEPART(MONTH,'{datecondition}') AND DATEPART(YEAR, O.CreatedOn) = DATEPART(YEAR, '{datecondition}') and O.Status = 2";
					double sumThisMonth = await conection.QuerySingleAsync<double>(query);
					datecondition = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
					query = $"select CAST( ISNULL(Sum(O.Total),0) AS FLOAT) as Total from [Order] as O with(nolock) WHERE DATEPART(MONTH, O.CreatedOn) = DATEPART(MONTH,'{datecondition}') AND DATEPART(YEAR, O.CreatedOn) = DATEPART(YEAR, '{datecondition}') and O.Status = 2";
					double sumLastMonth = await conection.QuerySingleAsync<double>(query);
					surgeryChart.sinceLastMonth = Math.Round(sumLastMonth > 0 ? ((double)(sumThisMonth - sumLastMonth) / sumLastMonth) * 100 : 0);
				}
				return surgeryChart;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<DiseasesChartViewModel> GetParamDiseasesChartAsync(int type)
		{
			DiseasesChartViewModel diseasesChart = new DiseasesChartViewModel();
			try
			{
				string[] tables = { "Đã Thanh Toán", "Chưa Thanh Toán", "Đã Hủy" };
				List<int> data = new List<int>();
				using (var conection = _dapper.CreateConnection())
				{
					foreach (var table in tables)
					{
						int status = table == "Đã Thanh Toán" ? 2 : table == "Đã Hủy" ? 3 : 1;
						string condition = type == 1 ? $"where CONVERT(date, O.CreatedOn) = CONVERT(date, '{DateTime.Now.ToString()}') and O.Status = {status}" :
							type == 2 ? $"where DATEPART(WEEK, O.CreatedOn) = DATEPART(WEEK, '{DateTime.Now.ToString()}') AND DATEPART(YEAR, O.CreatedOn) = DATEPART(YEAR, '{DateTime.Now.ToString()}') and O.Status = {status}" :
							type == 3 ? $"where DATEPART(MONTH, O.CreatedOn) = DATEPART(MONTH, '{DateTime.Now.ToString()}') AND DATEPART(YEAR, O.CreatedOn) = DATEPART(YEAR, '{DateTime.Now.ToString()}') and O.Status = {status}" :
                            type == 4 ? $"where DATEPART(YEAR, O.CreatedOn) = DATEPART(YEAR, '{DateTime.Now.ToString()}') and O.Status = {status}" :
                            $"Where O.Status = {status}";
                        string query = $"select ISNULL(Count(*), 0) as [Number] from [Order] as O with(nolock) {condition} ";
						int number = await conection.QuerySingleAsync<int>(query);
						data.Add(number);
					}

				}
				diseasesChart.Series = data;
				return diseasesChart;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return null;
			}
		}

        public async Task<IEnumerable<OrderViewModel>> GetTopOrderAsync(int type)
        {
            try
            {
                string datePart = type switch
                {
                    1 => "DAY",
                    2 => "WEEK",
                    3 => "MONTH",
                    4 => "YEAR",
                    _ => null
                };

                string dateCondition = datePart != null
                    ? $"and DATEPART({datePart}, GETDATE()) = DATEPART({datePart}, O.CreatedOn)"
                    : string.Empty;

                string query = $@"
					select top(5) O.Id, O.Code, O.Total, O.Status, O.Note, O.CreatedOn , sum(D.Quantity) as Quantity
					from [Order] as O with(nolock)
					join OrderDetail as D on O.Id = D.OrderId and D.IsActive = 1
					where O.Status = 2 {dateCondition}
					group by O.Id, O.Code, O.Total, O.Status, O.Note, O.CreatedOn
					order by O.Total desc";

                using var connection = _dapper.CreateConnection();
                return (await connection.QueryAsync<OrderViewModel>(query)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<ProductViewModel>> GetTopProductAsync(int type)
        {
            try
            {
                string datePart = type switch
                {
                    1 => "DAY",
                    2 => "WEEK",
                    3 => "MONTH",
                    4 => "YEAR",
                    _ => null
                };

                string dateCondition = datePart != null
                    ? $"and DATEPART({datePart}, GETDATE()) = DATEPART({datePart}, O.CreatedOn)"
                    : string.Empty;

                string query = $@"
            select top 5 
                P.Id, P.name, P.Price, 
                ISNULL(REPLACE(I.ImageUrl, 'Images/', ''), '') AS [Images], 
                sum(D.Quantity) as Quantity
            from [Order] as O with(nolock)
            join OrderDetail as D with(nolock) on O.Id = D.OrderId
            join Product as P with(nolock) on D.ProductId = P.Id
            left join Image as I with(nolock) on I.ImgSetId = P.ImgSetId and I.IsActive = 1
            where O.Status = 2 {dateCondition}
            group by P.Id, P.name, P.Price, I.ImageUrl";

                using var connection = _dapper.CreateConnection();
                return (await connection.QueryAsync<ProductViewModel>(query)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        private List<DateTime> GetCurrentWeekDates()
		{
			List<DateTime> weekDates = new List<DateTime>();
			DateTime today = DateTime.Today;

			int daysToMonday = (int)today.DayOfWeek - (int)DayOfWeek.Monday;
			if (daysToMonday < 0) daysToMonday += 7;

			DateTime startOfWeek = today.AddDays(-daysToMonday);
			for (int i = 0; i < 7; i++)
			{
				weekDates.Add(startOfWeek.AddDays(i));
			}
			return weekDates;
		}

		private List<DateTime> GetEndOfWeeksInCurrentMonth()
		{
			List<DateTime> endOfWeeks = new List<DateTime>();
			DateTime today = DateTime.Today;

			DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
			DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

			int daysToMonday = (int)firstDayOfMonth.DayOfWeek - (int)DayOfWeek.Monday;
			if (daysToMonday > 0)
			{
				daysToMonday -= 7;
			}

			DateTime startOfWeek = firstDayOfMonth.AddDays(daysToMonday);

			while (startOfWeek <= lastDayOfMonth)
			{
				DateTime endOfWeek = startOfWeek.AddDays(6);
				if (endOfWeek <= lastDayOfMonth)
				{
					endOfWeeks.Add(endOfWeek);
				}
				startOfWeek = startOfWeek.AddDays(7);
			}
			return endOfWeeks;
		}

        private List<string> GetWeeksInCurrentMonth()
        {
            var weeks = new List<string>();
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            CultureInfo ci = CultureInfo.CurrentCulture;

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            int firstWeek = ci.Calendar.GetWeekOfYear(firstDayOfMonth, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int lastWeek = ci.Calendar.GetWeekOfYear(lastDayOfMonth, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            for (int weekNum = firstWeek; weekNum <= lastWeek; weekNum++)
            {
                weeks.Add($"Tuần {weekNum}");
            }

            return weeks;
        }

        private List<string> GetMonthsInCurrentYear()
        {
            var months = new List<string>();
            int year = DateTime.Now.Year;

            for (int month = 1; month <= 12; month++)
            {
                months.Add($"Tháng {month}");
            }

            return months;
        }

        private List<string> GetLastFiveYears()
        {
            var years = new List<string>();
            int currentYear = DateTime.Now.Year;

            for (int i = 4; i >= 0; i--)
            {
                years.Add((currentYear - i).ToString());
            }

            return years;
        }
    }
}
