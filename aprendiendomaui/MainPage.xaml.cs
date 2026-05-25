using System.Collections.ObjectModel;

namespace aprendiendomaui
{
    public partial class MainPage : ContentPage
    {
        private readonly ObservableCollection<Employee> employees = [];
        private readonly ObservableCollection<AttendanceRecord> attendanceRecords = [];

        public MainPage()
        {
            InitializeComponent();

            EmployeesCollection.ItemsSource = employees;
            AttendanceCollection.ItemsSource = attendanceRecords;
            EmployeePicker.ItemsSource = employees;
            EmployeePicker.ItemDisplayBinding = new Binding(nameof(Employee.Name));
            AttendanceDatePicker.Date = DateTime.Today;
            TodayLabel.Text = DateTime.Today.ToString("dd/MM/yyyy");

            LoadDemoData();
            RefreshEmployeeSummary();
        }

        private void OnAddEmployeeClicked(object? sender, EventArgs e)
        {
            var name = NameEntry.Text?.Trim();
            var role = RoleEntry.Text?.Trim();
            var department = DepartmentEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowFormMessage("Escribe el nombre del empleado.");
                return;
            }

            var employee = new Employee(
                name,
                string.IsNullOrWhiteSpace(role) ? "Sin cargo" : role,
                string.IsNullOrWhiteSpace(department) ? "Sin departamento" : department);

            employees.Add(employee);
            EmployeePicker.SelectedItem = employee;

            NameEntry.Text = string.Empty;
            RoleEntry.Text = string.Empty;
            DepartmentEntry.Text = string.Empty;

            RefreshEmployeeSummary();
            ShowFormMessage($"Empleado agregado: {employee.Name}");
        }

        private void OnPresentClicked(object? sender, EventArgs e)
        {
            RegisterAttendance("Presente");
        }

        private void OnLateClicked(object? sender, EventArgs e)
        {
            RegisterAttendance("Tarde");
        }

        private void OnAbsentClicked(object? sender, EventArgs e)
        {
            RegisterAttendance("Ausente");
        }

        private void RegisterAttendance(string status)
        {
            if (EmployeePicker.SelectedItem is not Employee employee)
            {
                ShowAttendanceMessage("Selecciona un empleado primero.");
                return;
            }

            var date = AttendanceDatePicker.Date;
            employee.LastAttendance = $"{status} - {date:dd/MM}";

            attendanceRecords.Insert(0, new AttendanceRecord(employee.Name, status, date));
            EmployeesCollection.ItemsSource = null;
            EmployeesCollection.ItemsSource = employees;

            RefreshEmployeeSummary();
            ShowAttendanceMessage($"{employee.Name}: {status} el {date:dd/MM/yyyy}");
        }

        private void RefreshEmployeeSummary()
        {
            EmployeeCountLabel.Text = employees.Count.ToString();
            HeroEmployeeCountLabel.Text = employees.Count.ToString();
            HeroAttendanceCountLabel.Text = attendanceRecords.Count.ToString();
        }

        private void LoadDemoData()
        {
            var firstEmployee = new Employee("Ana Martinez", "Supervisora", "Operaciones")
            {
                LastAttendance = "Presente - " + DateTime.Today.ToString("dd/MM")
            };

            var secondEmployee = new Employee("Carlos Rojas", "Tecnico", "Soporte");

            employees.Add(firstEmployee);
            employees.Add(secondEmployee);
            attendanceRecords.Add(new AttendanceRecord(firstEmployee.Name, "Presente", DateTime.Today));
            EmployeePicker.SelectedItem = firstEmployee;
        }

        private void ShowFormMessage(string message)
        {
            FormMessageLabel.Text = message;
            FormMessageLabel.IsVisible = true;
        }

        private void ShowAttendanceMessage(string message)
        {
            AttendanceMessageLabel.Text = message;
            AttendanceMessageLabel.IsVisible = true;
        }
    }

    public class Employee
    {
        public Employee(string name, string role, string department)
        {
            Name = name;
            Role = role;
            Department = department;
        }

        public string Name { get; }
        public string Role { get; }
        public string Department { get; }
        public string Detail => string.Join(" - ", Role, Department);
        public string Initials => string.Join(
            string.Empty,
            Name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(part => part[0]))
            .ToUpperInvariant();

        public string LastAttendance { get; set; } = "Sin marca";
    }

    public class AttendanceRecord
    {
        public AttendanceRecord(string employeeName, string status, DateTime date)
        {
            EmployeeName = employeeName;
            Status = status;
            Date = date;
        }

        public string EmployeeName { get; }
        public string Status { get; }
        public DateTime Date { get; }
        public string DateText => Date.ToString("dd/MM/yyyy");
        public string DayText => Date.ToString("dd");
        public string MonthText => Date.ToString("MMM").ToUpperInvariant();
    }
}
