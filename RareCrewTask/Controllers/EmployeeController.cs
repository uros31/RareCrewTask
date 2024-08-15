using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System.Drawing.Drawing2D;
using System.Drawing;
using RareCrewTask.Services;
using RareCrewTask.Manager;
using RareCrewTask.Interfaces;

namespace RareCrewTask.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeManager _employeeManager;

        public EmployeeController(IEmployeeManager employeeManager)
        {
            _employeeManager = employeeManager;
        }

        public async Task<IActionResult> Index()
        {
            var employeesWorkedHours = await _employeeManager.GetEmployeesWorkedHoursDescAsync();
            return View(employeesWorkedHours);
        }
        public async Task<IActionResult> GeneratePieChart()
        {
            var chart = await _employeeManager.GeneratePieChartAsync();
            return File(chart, "image/png");
        }
    }


}
