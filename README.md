# Barber Booking App (ASP.NET with TDD)

This project implements a barber booking application using ASP.NET and Test-Driven Development (TDD) practices.

## Project Setup

1. **Prerequisites:**

   - Microsoft Visual Studio (or compatible IDE)
   - ASP.NET Core development environment

2. **Cloning the Repository:**
   Use Git to clone this repository:

   ```bash
   git clone https://your-repository-url.git
   ```

3. **Restoring Dependencies:**
   Run the following command in your terminal to restore the project's dependencies:

   ```bash
   dotnet restore
   ```

## Running the Application

1. **Open in Visual Studio:**
   Open the project in Visual Studio or your preferred IDE.

2. **Run the Application:**
   Press `F5` or use the "Run" menu option to start the app.

## Features

- Users can browse available barbers.
- Users can search for barbers by name or availability.
- Users can view a barber's profile and services offered.
- Users can schedule appointments with barbers (subject to implementation).
- Appointments can be managed (viewing, rescheduling, canceling) (subject to implementation).

## TDD Practices

- This project emphasizes TDD principles. Unit tests are written for core functionalities before implementing the corresponding code.
- Unit tests reside in the `Domain.Test.Unit` folder, following a naming convention that reflects the components being tested (e.g., `AppointmentServiceTests.cs`).
- Consider using a testing framework like xUnit or NUnit for writing and managing tests.

## Further Development

This README provides a starting point. Consider expanding on the following aspects:

- Complete the implementation of user authentication, appointment bookings, and management functionalities.
- Implement additional features as needed.
- Add documentation for specific controllers, services, and models.
- Provide deployment instructions if applicable.
