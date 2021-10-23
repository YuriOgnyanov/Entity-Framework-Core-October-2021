using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();

            var information = GetEmployeesByFirstNameStartingWithSa(context);
            Console.WriteLine(information);

        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context) 
        {
            var sb = new StringBuilder();

            var employees = context.Employees.Where(x => x.FirstName.StartsWith("Sa")).Select(x => new
            {
                firstName = x.FirstName,
                lastName = x.LastName,
                jobTitle = x.JobTitle,
                salary = x.Salary
            }).OrderBy(x => x.firstName)
            .ThenBy(x => x.lastName)
            .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.firstName} {employee.lastName} - {employee.jobTitle} - (${employee.salary:F2})");
            }

            return sb.ToString().Trim();
        }
        public static string GetLatestProjects(SoftUniContext context) 
        {
            var sb = new StringBuilder();

            var projects = context.Projects.OrderByDescending(project => project.StartDate)
                .Take(10)
                .OrderBy(project => project.Name);
            
            foreach (var project in projects)
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().Trim();
        }
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context) 
        {
            var sb = new StringBuilder();

            var departments = context.Departments.Where(x => x.Employees.Count() > 5)
                .Select(x => new
                {
                    departmentName = x.Name,
                    managerFirstName = x.Manager.FirstName,
                    managerLastName = x.Manager.LastName,
                    employees = x.Employees.Select(x => new
                    {
                        employeeFirstName = x.FirstName,
                        employeeLastName = x.LastName,
                        employeeJobTitle = x.JobTitle
                    })
                }).OrderBy(x => x.employees.Count())
                .ThenBy(x => x.departmentName);
                

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.departmentName} - {department.managerFirstName} {department.managerLastName}");

                foreach (var employee in department.employees.OrderBy(x => x.employeeFirstName).ThenBy(x => x.employeeLastName))
                {
                    sb.AppendLine($"{employee.employeeFirstName} {employee.employeeLastName} - {employee.employeeJobTitle}");
                }
            }

            return sb.ToString().Trim();
        }
        public static string GetEmployee147(SoftUniContext context) 
        {
            var sb = new StringBuilder();

            var employeeId = context.Employees.Where(x => x.EmployeeId == 147)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    jobTitle = x.JobTitle,
                    projectEmployee = x.EmployeesProjects.Select(x => new { projectName = x.Project.Name })
                }).ToList();


            foreach (var employee in employeeId)
            {
                sb.AppendLine($"{employee.firstName} {employee.lastName} - {employee.jobTitle}");

                foreach (var project in employee.projectEmployee.OrderBy(x => x.projectName))
                {
                    sb.AppendLine($"{project.projectName}");
                }
            }

            

            return sb.ToString().Trim();
        }

        public static string GetAddressesByTown(SoftUniContext context) 
        {
            var sb = new StringBuilder();

            var addresses = context.Addresses.Select(x => new
            {
                addressText = x.AddressText,
                townName = x.Town.Name,
                employeeCount = x.Employees.Count()
            }).OrderByDescending(x => x.employeeCount)
            .ThenBy(x => x.townName)
            .ThenBy(x => x.addressText)
            .ToList();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.addressText}, {address.townName} - {address.employeeCount} employees");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context) 
        {
            var sb = new StringBuilder();

            var employees = context.Employees.Where(x => x.EmployeesProjects
            .Any(x => x.Project.StartDate.Year >= 2001 && x.Project.StartDate.Year <= 2003))
                .Select(x => new 
                {
                    employeeFirstName = x.FirstName,
                    employeeLastName = x.LastName,
                    managerFirstName = x.Manager.FirstName,
                    managerLastName = x.Manager.LastName,
                    project = x.EmployeesProjects.Select(x => new { projectName = x.Project.Name, startDate = x.Project.StartDate, endDate = x.Project.EndDate})
                })
                .Take(10).ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.employeeFirstName} {employee.employeeLastName} - Manager: {employee.managerFirstName} {employee.managerLastName}");

                foreach (var project in employee.project)
                {
                    var projectInfo = string.Empty;

                    if (project.endDate is null)
                    {
                        projectInfo = $"--{project.projectName} - {project.startDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - not finished";
                    }
                    else
                    {
                        projectInfo = $"--{project.projectName} - {project.startDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {project.endDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}";
                    }

                    sb.AppendLine(projectInfo);
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context) 
        {
            var sb = new StringBuilder();

            var adress = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };
            
            context.Addresses.Add(adress);
            context.SaveChanges();

            var employee = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

            employee.AddressId = adress.AddressId;
            context.SaveChanges();


            var employeeAddresses = context.Employees.Select(x => new
            {
                x.Address.AddressText,
                x.Address.AddressId
            }).OrderByDescending(x => x.AddressId)
            .Take(10)
            .ToList();

            

            foreach (var item in employeeAddresses)
            {
                sb.AppendLine($"{item.AddressText}");
            }

            return sb.ToString().Trim();
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employeeWithDepartment = context.Employees.Select(x => new
            {
                name = x.FirstName,
                lastName = x.LastName,
                departmentName = x.Department.Name,
                salary = x.Salary
            }).Where(x => x.departmentName == "Research and Development")
              .OrderBy(x => x.salary)
              .ThenByDescending(x => x.name)
              .ToList();

            foreach (var employee in employeeWithDepartment)
            {
                sb.AppendLine($"{employee.name} {employee.lastName} from {employee.departmentName} - ${employee.salary:F2}");
            }

            return sb.ToString().Trim();
        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context) 
        {
            var sb = new StringBuilder();

            var employeeWithSalary = context.Employees.Select(x => new
            {
                name = x.FirstName,
                salary = x.Salary
            }).Where(x => x.salary > 50000)
              .OrderBy(x => x.name)
              .ToList();

            foreach (var employee in employeeWithSalary)
            {
                sb.AppendLine($"{employee.name} - {employee.salary:F2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employeesFullInfo = context.Employees.Select(x => new
            {
                name = x.FirstName,
                lastName = x.LastName,
                middleName = x.MiddleName,
                jobTittle = x.JobTitle,
                salary = x.Salary
            }).ToList();

            foreach (var employee in employeesFullInfo)
            {
                sb.AppendLine($"{employee.name} {employee.lastName} {employee.middleName} {employee.jobTittle} {employee.salary:F2}");
            }

            return sb.ToString().Trim();
        }
    }
}
