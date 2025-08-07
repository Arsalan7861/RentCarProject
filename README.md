# RentCarProject

Welcome to the RentCarProject! This project is designed to provide a comprehensive car rental system with both client-side and server-side components. Built using **.NET CORE 9** and **Angular 20**, this application aims to facilitate easy car rentals for users.

## Table of Contents
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Features
- **User-Friendly Dashboard**: 
  - A clean and intuitive interface that allows users to easily navigate through the car rental options.
  - Provides quick access to available cars, booking history, and user account settings.

- **Car Management**:
  - Admin users can add, edit, and delete car listings.
  - Each car listing includes details such as make, model, year, rental price, and availability status.
  - Users can filter cars based on various criteria like price range, car type, and availability.

- **Booking System**:
  - Users can select a car and choose rental dates to check availability.
  - The system calculates the total rental cost based on the selected duration and any applicable discounts.
  - Users can view their booking history and manage upcoming reservations.

- **User Authentication**:
  - Secure login and registration system for users and administrators.
  - Password recovery options to help users regain access to their accounts.

- **Responsive Design**:
  - The application is designed to work seamlessly on various devices, including desktops, tablets, and smartphones.
  - Adapts layout and functionality for optimal user experience across different screen sizes.

- **Admin Dashboard**:
  - Special features for administrators to manage users, view analytics.
  - Ability to monitor active bookings and manage car inventory.

  ![Dashboard Screenshot](https://github.com/Arsalan7861/RentCarProject/blob/master/Screenshots/dashboard.png)

  ![Vehicles Screenshot](https://github.com/Arsalan7861/RentCarProject/blob/master/Screenshots/vehicles.png)
- **UI**:
  - UI part is under development and It will be developed soon.  

## Technology Stack
- **Frontend:** Angular 20
- **Backend:** .NET CORE 9

## Installation

To set up the project locally, follow these steps:

1. Clone the repository:
   ```bash
   git clone https://github.com/Arsalan7861/RentCarProject.git
   ```
   
2. Navigate to the project directory:
   ```bash
   cd RentCarProject
   ```

3. Open the **client-side** project in Visual Studio Code:
   - Navigate to the client folder and open it with VS Code.

4. Open the **server-side** project in Visual Studio:
   - Navigate to the server folder and open the solution file (.slnx) in Visual Studio.

5. Restore the dependencies for the server-side:
   ```bash
   dotnet restore
   ```

6. Run the server-side application:
   ```bash
   dotnet run
   ```
   ### Important:
     Don't forget to update the **connection string**

## Usage

Once the application is running, you can access the dashboard through your web browser at `http://localhost:4200`. From there, you can explore the features and functionalities of the car rental system.

## Contributing

Contributions are welcome! If you have suggestions for improvements or new features, feel free to create an issue or submit a pull request.

1. Fork the repository.
2. Create your feature branch:
   ```bash
   git checkout -b feature/YourFeature
   ```
3. Commit your changes:
   ```bash
   git commit -m 'Add some feature'
   ```
4. Push to the branch:
   ```bash
   git push origin feature/YourFeature
   ```
5. Open a pull request.

## License

This project does not have a specified license. Please feel free to use it for personal or educational purposes.
