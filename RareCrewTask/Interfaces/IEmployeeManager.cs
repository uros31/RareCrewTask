using RareCrewTask.Models;

namespace RareCrewTask.Interfaces
{
    public interface IEmployeeManager
    {
        Task<List<EmployeeWorkHoursModel>> GetEmployeesWorkedHoursDescAsync();
        Task<byte[]> GeneratePieChartAsync();
    }
}
