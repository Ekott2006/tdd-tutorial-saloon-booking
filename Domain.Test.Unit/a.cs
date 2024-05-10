
    // ALL EMPLOYEES
    // When No Employees return false
    // When Employees but no shift return false
    // When Employees and shift but no match period return false
    // When employees and shift but appointment return false
    // When employees shift and no appointment return true and verify insertion (Ensure random)
    // ONE EMPLOYEE
    // When no/invalid employee return false
    // When employee but no shift return false -- REMOVE LATER
    // When Employees and shift but no match period return false
    // When employee and shift but no appointment return false -- REMOVE LATER

        
    // [Fact]
    // public async Task Create_NoEmployees_False()
    // {
    //     // Arrange
    //     const string firstName = "Nsikak";
    //     const string lastName = "Ekott";
    //     EmployeeRepository employeeRepository = new(Context);
    //     CustomerRepository customerRepository = new(Context);
    //     EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
    //     CreateBookingService service = new(employeeRepository, customerRepository, employeeHelper);
    //
    //     // Act
    //     bool result = await service.Create(new CreateBookingServiceParameter(firstName, lastName,
    //         new Period(DateTime.Now, DateTime.Now.AddHours(1))));
    //
    //     // Assert
    //     Assert.False(result);
    //     Assert.Empty(await Context.Appointments.ToListAsync());
    // }
    //
    // [Fact]
    // public async Task Create_EmployeesNoShift_False()
    // {
    //     // Arrange
    //     const string firstName = "Nsikak";
    //     const string lastName = "Ekott";
    //     List<Employee> employees = new EmployeeFaker().Generate(3);
    //     await Context.Employees.AddRangeAsync(employees);
    //     await Context.SaveChangesAsync();
    //
    //     EmployeeRepository employeeRepository = new(Context);
    //     CustomerRepository customerRepository = new(Context);
    //     EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
    //     
    //     EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
    //     CreateBookingService service = new(employeeRepository, customerRepository, employeeHelper);
    //
    //     // Act
    //     bool result = await service.Create(new CreateBookingServiceParameter(firstName, lastName,
    //         new Period(DateTime.Now, DateTime.Now.AddHours(1))));
    //
    //     // Assert
    //     Assert.False(result);
    //     Assert.Empty(await Context.Appointments.ToListAsync());
    // }
    //
    // [Fact]
    // public async Task Create_EmployeesNoShiftOneEmployee_False()
    // {
    //     // Arrange
    //     const string firstName = "Nsikak";
    //     const string lastName = "Ekott";
    //     List<Employee> employees = new EmployeeFaker().Generate(3);
    //     await Context.Employees.AddRangeAsync(employees);
    //     await Context.SaveChangesAsync();
    //
    //     EmployeeRepository employeeRepository = new(Context);
    //     CustomerRepository customerRepository = new(Context);
    //     EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
    //     CreateBookingService service = new(employeeRepository, customerRepository, employeeHelper);
    //
    //     // Act
    //     bool result = await service.Create(new CreateBookingServiceParameter(firstName, lastName,
    //         new Period(DateTime.Now, DateTime.Now.AddHours(1)), employees.First().Id));
    //
    //     // Assert
    //     Assert.False(result);
    //     Assert.Empty(await Context.Appointments.ToListAsync());
    // }
    //
    // [Fact]
    // public async Task Create_EmployeesNoShiftInvalidId_False()
    // {
    //     // Arrange
    //     const string firstName = "Nsikak";
    //     const string lastName = "Ekott";
    //     List<Employee> employees = new EmployeeFaker().Generate(3);
    //     await Context.Employees.AddRangeAsync(employees);
    //     await Context.SaveChangesAsync();
    //
    //     EmployeeRepository employeeRepository = new(Context);
    //     CustomerRepository customerRepository = new(Context);
    //     EmployeeHelper employeeHelper = new(new ShiftRepository(Context), new AppointmentRepository(Context));
    //     CreateBookingService service = new(employeeRepository, customerRepository, employeeHelper);
    //
    //     // Act
    //     bool result = await service.Create(new CreateBookingServiceParameter(firstName, lastName,
    //         new Period(DateTime.Now, DateTime.Now.AddHours(1)), Guid.NewGuid()));
    //
    //     // Assert
    //     Assert.False(result);
    //     Assert.Empty(await Context.Appointments.ToListAsync());
    // }
    //