using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace OnlineOrderingApp
{
    class Program
    {
        static List<User> users = new List<User>();
        static List<Product> Products = new List<Product>();
        static List<Order> orders = new List<Order>();
        // static int nextUserId = 2; // Starts after Admin
        // static int nextProductId = 1;
        // static int nextOrderId = 1;

        static void Main(string[] args)
        {
            SeedData();
            Console.WriteLine("Welcome to the Online Ordering System!");

            while (true)
            {
                Console.WriteLine("1) Register as Customer  2) Login  3) Exit");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        RegisterCustomer();
                        break;
                    case "2":
                        Login();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
        }

        static void SeedData()
        {
            // Seed Products
            // Products.Add(new Product { Id = nextProductId++, Name = "Burger", Price = 5.99M });
            Products.Add(new Product { Id = 1, Name = "Burger", Price = 5.99M });
            Products.Add(new Product { Id = 2, Name = "Pizza", Price = 8.99M });
            Products.Add(new Product { Id = 3, Name = "Soda", Price = 1.99M });
            // Seed Admin use
            users.Add(new User { Id = 1, FullName = "Admin", Email = "admin@example.com", Password = "admin", Role = UserRole.Admin });
        }

        static void RegisterCustomer()
        {
            int cutomerId = 1;
            Console.WriteLine("Register as a new Customer:");
            Console.Write("Enter full name: ");
            string fullName = Console.ReadLine();
            Console.Write("Enter email: ");
            string email = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            users.Add(new User { Id = cutomerId, FullName = fullName, Email = email, Password = password, Role = UserRole.Customer });
            Console.WriteLine("Customer registered successfully.");
        }

        static void Login()
        {
            Console.WriteLine("Login:");
            Console.Write("Enter email: ");
            string email = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            var user = users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user == null)
            {
                Console.WriteLine("Invalid credentials. Please try again.");
                return;
            }

            Console.WriteLine($"Welcome, {user.FullName}!");

            switch (user.Role)
            {
                case UserRole.Admin:
                    AdminPanel();
                    break;
                case UserRole.Customer:
                    CustomerPanel();
                    break;
                case UserRole.Staff:
                    StaffPanel();
                    break;
            }
        }

        static void AdminPanel()
        {
            Console.WriteLine("Admin Panel: 1) Add Product  2) View Orders  3) Add Staff  4) Back");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddProduct();
                    break;
                case "2":
                    ViewAllOrders();
                    break;
                case "3":
                    AddStaff();
                    break;
                case "4":
                    EditProduct();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        static void EditProduct()
        {
            ViewAllProducts();
            Console.Write("Enter product ID to edit: ");
            int id = int.Parse(Console.ReadLine());
            var product = Products.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                Console.Write("Enter new name: ");
                product.Name = Console.ReadLine();
                Console.Write("Enter new price: ");
                product.Price = decimal.Parse(Console.ReadLine());
                Console.WriteLine("Product updated successfully.");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
        }

        static void AddProduct()
        {
            Console.Write("Enter product name: ");
            string name = Console.ReadLine();
            Console.Write("Enter product price: ");
            decimal price = decimal.Parse(Console.ReadLine());

            // Products.Add(new Product { Id = nextProductId++, Name = name, Price = price });
            Products.Add(new Product { Name = name, Price = price });
            Console.WriteLine("Product added successfully.");
        }

        static void ViewAllOrders()
        {
            if (!orders.Any())
            {
                Console.WriteLine("No orders available.");
                return;
            }

            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.Id}, Product: {order.Product.Name}, Status: {order.Status}");
            }
        }
        static void ViewAllProducts()
        {
            if (!orders.Any())
            {
                Console.WriteLine("No orders available.");
                return;
            }

            foreach (var product in Products)
            {
                Console.WriteLine($"Order ID: {product.Id}, Product: {product.Name}, Status: {product.Price}");
            }
        }

        public static bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains("."); // Basic validation; consider using regex for stricter checks
        }

        static void AddStaff()
        {
            Console.WriteLine("Add new Staff:");
            Console.Write("Enter full name: ");
            string fullName = Console.ReadLine();
            Console.Write("Enter email: ");
            string email = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();

            static string HashPassword(string password)
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    return BitConverter.ToString(bytes).Replace("-", "").ToLower(); // Convert byte array to a hex string
                }
            }

            if (!IsValidEmail(email))
            {
                Console.WriteLine("Invalid email format.");
                return; // Exit the method
            }

            if (password.Length < 6) // Arbitrary password length check
            {
                Console.WriteLine("Password must be at least 6 characters long.");
                return; // Exit the method
            }

            string hashedPassword = HashPassword(password);

            users.Add(new User { FullName = fullName, Email = email, Password = hashedPassword, Role = UserRole.Staff });
            Console.WriteLine("Staff added successfully.");

        }



        static void CustomerPanel()
        {
            foreach (var product in Products)
            {
                Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: {product.Price:C}");
            }

            Console.WriteLine("Enter product ID to order:");
            int productId = int.Parse(Console.ReadLine());

            var selectedProduct = Products.FirstOrDefault(p => p.Id == productId);
            if (selectedProduct != null)
            {
                orders.Add(new Order { Product = selectedProduct });
                Console.WriteLine("Order placed successfully.");
            }
            else
            {
                Console.WriteLine("Product not found.");
            }
        }

        static void StaffPanel()
        {
            var pendingOrders = orders.Where(o => o.Status == "Pending").ToList();
            if (!pendingOrders.Any())
            {
                Console.WriteLine("No pending orders.");
                return;
            }

            foreach (var order in pendingOrders)
            {
                Console.WriteLine($"Order ID: {order.Id}, Product: {order.Product.Name}, Status: {order.Status}");
            }

            Console.WriteLine("Enter order ID to mark as delivered:");
            int orderId = int.Parse(Console.ReadLine());

            var orderToMark = orders.FirstOrDefault(o => o.Id == orderId);
            if (orderToMark != null)
            {
                orderToMark.Status = "Delivered";
                Console.WriteLine("Order marked as delivered.");
            }
            else
            {
                Console.WriteLine("Order not found.");
            }
        }

        enum UserRole { Admin, Customer, Staff }

        class User
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public UserRole Role { get; set; }
        }

        class Product
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
        }

        class Order
        {
            public int Id { get; set; }
            public Product Product { get; set; }
            public string Status { get; set; } = "Pending";
        }
    }
}
