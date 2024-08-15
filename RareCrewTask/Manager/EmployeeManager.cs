using RareCrewTask.Models;
using RareCrewTask.Services;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using RareCrewTask.Interfaces;


namespace RareCrewTask.Manager
{
    public class EmployeeManager : IEmployeeManager
    {
        private readonly EmployeeService _employeeService;

        public EmployeeManager(
            EmployeeService employeeService)
        {

            _employeeService = employeeService;
        }

        public async Task<List<EmployeeWorkHoursModel>> GetEmployeesWorkedHoursDescAsync()
        {
            var employeeTimeTracks = await _employeeService.GetEmployeeTimeTracksAsync();

            var workedHoursGorupByEmployee = GetWorkedHoursGroupedByEmployee(employeeTimeTracks);

            workedHoursGorupByEmployee = workedHoursGorupByEmployee.OrderByDescending(e => e.TotalTimeWorked).ToList();

            return workedHoursGorupByEmployee;
        }

        public async Task<byte[]> GeneratePieChartAsync()
        {
            var employeeTimeTracks = await _employeeService.GetEmployeeTimeTracksAsync();

            var workedHoursGorupByEmployee = GetWorkedHoursGroupedByEmployee(employeeTimeTracks);

            var totalWorkedHours = workedHoursGorupByEmployee.Sum(e => e.TotalTimeWorked);

            using var chart = new Bitmap(600, 600);

            return GenerateAndSaveChart(workedHoursGorupByEmployee, chart, totalWorkedHours);
        }

        private List<EmployeeWorkHoursModel> GetWorkedHoursGroupedByEmployee(List<EmployeeTimeTrackModel> employeeTimeTracks)
        {
            var groupedByEmployee = employeeTimeTracks
                .Where(it => it.StartTimeUtc.HasValue && it.EndTimeUtc.HasValue)
                .GroupBy(it => it.EmployeeName)
                .Select(it => new EmployeeWorkHoursModel
                {
                    EmployeeName = it.Key ?? "Without Name",
                    TotalTimeWorked = Math.Round(it.Sum(e => (e.EndTimeUtc.Value - e.StartTimeUtc.Value).TotalHours), 2)

                }).ToList();

            return groupedByEmployee ?? new List<EmployeeWorkHoursModel>();
        }

        private byte[] GenerateAndSaveChart(
            List<EmployeeWorkHoursModel> employees,
            Bitmap chart,
            double totalWorkedHours)
        {
            using var graphics = Graphics.FromImage(chart);
            var random = new Random();
            var startAngle = 0f;
            var usedColors = new List<SolidBrush>();

            foreach (var employee in employees)
            {
                var sweepAngle = (float)(employee.TotalTimeWorked * 360.0 / totalWorkedHours);
                var color = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
                while (usedColors.Contains(color))
                {
                    color = new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
                }
                usedColors.Add(color);
                var middleAngle = startAngle + sweepAngle / 2 - (0.01 * (startAngle + sweepAngle / 2));
                var x = (float)(300 + 100 * Math.Cos(middleAngle * Math.PI / 180));
                var y = (float)(300 + 100 * Math.Sin(middleAngle * Math.PI / 180));

                graphics.FillPie(color, 0, 0, 600, 600, startAngle, sweepAngle);
                var workPercentage = (employee.TotalTimeWorked * 100 / totalWorkedHours);
                DrawRotatedText(graphics, middleAngle, $"{employee.EmployeeName} - {Math.Round(workPercentage, 2)}%", x, y, new Font("Arial", 10), new SolidBrush(Color.Black));
                startAngle += sweepAngle;
            }

            return SaveImage(chart);
        }

        private byte[] SaveImage(
            Bitmap chart)
        {
            if (chart is null)
                throw new ArgumentNullException(nameof(chart));

            using var memoryStream = new MemoryStream();
            chart.Save(memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        }


        private void DrawRotatedText(
            Graphics graphics,
            double angle,
            string text,
            float x,
            float y,
            Font font,
            Brush brush)
        {
            if (graphics is null)
                return;

            GraphicsState state = graphics.Save();
            graphics.ResetTransform();
            graphics.RotateTransform((float)angle);
            graphics.TranslateTransform(x, y, MatrixOrder.Append);
            graphics.DrawString(text, font, brush, 0, 0);
            graphics.Restore(state);
        }
    }
}
