# E-Commerce Project

Welcome to the E-Commerce Project! This is a comprehensive solution designed to manage an e-commerce store, featuring a fully functional admin dashboard, product management, and customer interactions. 

This README will guide you through the setup and installation of the project on your local machine, covering dependencies, database configuration, and migration instructions.

---

## Features

### 1. **Admin Dashboard**
   - **Manage Products**: Admin users can view, add, update, and delete products from the store.
   - **Statistics**: View overall statistics such as the total number of users, products, and sales data.
   - **Product Categories**: Ability to organize products into categories.
   - **Manage Orders**:Ability to consult Customer or order and update their status
   - **Login**:Login with a predifined account  
   
### 2. **Customer Storefront**
   - **Product Browsing**: Customers can browse available products displayed as cards.
   - **Add to Cart**: Customers can add products to their shopping cart.
   - **View Cart**: A simple, user-friendly cart to review items before proceeding to checkout.
   - **Filters**:Filters for the category,name of the product,min price,max price.
   - **Consult Orders**:Consult the hisory of order and their Status (Shipped,pending,delivred)

### 3. **User Authentication**
   - **Login/Register**: Users can register new accounts and log in to the store.
   - **Role-based Access**: Differentiates between customer and admin users, providing access to different parts of the site.

### 4. **Order Management**
   - **Order Creation**: Customers can place orders for products in their cart.
   - **Order History**: View a history of past orders.     

### 5. **Email Notifications**
   - **Order Confirmation**: Users receive email confirmations for successful orders.
   - **Order Status**: Users recieve mail for the status updates
### 6. **Payment with stripe**
  - **Payment**: users can pay using stripe
---

## Prerequisites

Before setting up the project, ensure you have the following installed on your local machine:

- [.NET SDK](https://dotnet.microsoft.com/download) (Recommended version: 9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or any compatible database (PostgreSQL, MySQL, etc.)
- [Visual Studio](https://visualstudio.microsoft.com/) (optional, but recommended for development)
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/) (for running database migrations)

---

## Getting Started

### 1. Clone the Repository

To start using the project, first, clone the repository to your local machine:

```bash
git clone https://github.com/your-username/e-commerce-project.git
cd proj
```
### 2. Install dependencies
 run the following command
 ```bash
dotnet restore
```
### 3. Configure the Database Connection
The application requires a database connection to store and manage data. You need to update the connection string in the appsettings.json file.

Open the appsettings.json file in the root of the project.
Find the ConnectionStrings section, which looks like this:
json
```bash
"ConnectionStrings": {
  "DefaultConnection": "YourConnectionStringHere"
}
```
Replace "YourConnectionStringHere" with your actual database connection string. If you're using SQL Server, it might look something like this:
json
```bash
"DefaultConnection": "Server=localhost;Database=ECommerceDb;Trusted_Connection=True;"
```
If you're using a different database (e.g., PostgreSQL or MySQL), make sure to adjust the connection string accordingly.

### 4. Configure Email Settings (Optional)
If you wish to enable email notifications, you need to configure the SMTP settings for sending emails. Update the EmailSettings section in appsettings.json:

```bash
"EmailSettings": {
  "SmtpServer": "smtp.your-email-provider.com",
  "SmtpPort": 587,
  "SmtpUser": "your-email@example.com",
  "SmtpPassword": "your-email-password"
}
```

Replace the placeholders with your actual SMTP server information. This will allow the application to send email notifications for order confirmations and other alerts.

### 5. Run Database Migrations
Once your connection string is set up, it's time to apply the database migrations to set up your database schema.

Run the following command in the terminal:

```bash
dotnet ef database update
This will create the necessary tables in your database. If you encounter any issues, ensure your database is running and accessible, and that the connection string is correct.
```
### 6. Seed the Database (Optional)
You can seed the database with some initial data, such as default products or users, by running the following command:

```bash
dotnet ef database update --seed
```
This step is optional but useful for testing the application with some predefined data.

### 7. Run the Application
With everything set up, you can now run the application locally:

```bash
dotnet run
```
The application will start, and you can access it at http://localhost:5000 (or another port if specified). You should see the e-commerce storefront where customers can browse products, log in, and place orders.



### Project Structure
Here’s a breakdown of the main project folders and files:

Controllers: Contains the application logic for handling HTTP requests. Includes controllers for managing products, orders, and users.
Models: Defines the data structure for entities like products, orders, and users.
Views: Contains the Razor Views for rendering HTML pages. Organized by user type (admin and customer).
Data: Contains the application’s database context and migration files.
wwwroot: Stores static files like images, CSS, and JavaScript.
appsettings.json: Configuration file for connection strings, email settings, and other application settings.

### Contributing
We welcome contributions to improve this project! If you’d like to help out, feel free to fork the repository and submit a pull request. Before contributing, please make sure to:

Create an issue for any proposed changes.
Follow the existing code style and structure.
Ensure tests are updated or added for new features.
### License
This project is licensed under the MIT License - see the LICENSE file for details.

### Troubleshooting
If you run into any issues while setting up the project, here are some common solutions:

Issue: Database connection errors
Ensure your database is running and the connection string is correct.
If you're using SQL Server, verify that the server is accessible (e.g., localhost or a remote server).
Issue: Missing migrations
If you encounter an error regarding missing migrations, try running:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
This will generate the initial migrations and apply them to the database.

### Acknowledgments
Entity Framework Core for ORM functionality.
ASP.NET Core for building the web application.
Bootstrap for responsive design.
SendGrid for email notifications (if using SendGrid for email service).
