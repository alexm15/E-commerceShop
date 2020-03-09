using Webshop.UI.App_Data;
using Webshop.UI.Models;

namespace Webshop.UI.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebshopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebshopContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            if (!context.Products.Any())
            {
                //Categories
                var electronics = new Category {Name = "Electronics"};
                var books = new Category {Name = "Books"};
                var officeSupplies = new Category {Name = "Office Supplies"};
                var moviesAndTv = new Category {Name = "Movies and TV-Shows"};
                context.Categories.AddOrUpdate(electronics);
                context.Categories.AddOrUpdate(books);
                context.Categories.AddOrUpdate(officeSupplies);
                context.Categories.AddOrUpdate(moviesAndTv);
                context.SaveChanges();

                //Products
                var asus = new Product {Name = "ASUS X554L Laptop", Description = "Laptop computer", Price = 5499M};
                var hp = new Product {Name = "HP Pavillion XE455", Description = "Newest Laptop with high specs", Price = 9299M};
                var harryPotterBook = new Product
                {
                    Name = "Harry Potter and the Chamber of Secrects",
                    Description = "A Book by J.K Rowling Second Book", Price = 49M
                };
                var montyPythonMovie = new Product {Name = "Monty Python's Life of Brian", Description = "A Comedy movie", Price = 49M};
                var w5Wipes = new Product {Name = "W5 Glasses Wipes", Description = "For cleaning glasses", Price = 15M};
                var terminatorGenysis = new Product
                {
                    Name = "Terminator Genysis", Description = "The fifth movies in the Terminator Movie Series",
                    Price = 99M
                };
                var howIMetYourMother = new Product
                {
                    Name = "How I Met Your Mother Complete Series",
                    Description = "View all the loved episodes of the complete series", Price = 569M
                };
                var theDayAfterTomorrow = new Product
                {
                    Name = "The Day After Tommorrow",
                    Description = "A big climate change changes the whole world. But not for the better", Price = 128M
                };
                context.Products.AddOrUpdate(asus);
                context.Products.AddOrUpdate(hp);
                context.Products.AddOrUpdate(harryPotterBook);
                context.Products.AddOrUpdate(montyPythonMovie);
                context.Products.AddOrUpdate(w5Wipes);
                context.Products.AddOrUpdate(terminatorGenysis);
                context.Products.AddOrUpdate(howIMetYourMother);
                context.Products.AddOrUpdate(theDayAfterTomorrow);
                context.SaveChanges();

                //CategoryEntries
                electronics.Products.Add(asus);
                electronics.Products.Add(hp);
                books.Products.Add(harryPotterBook);
                officeSupplies.Products.Add(w5Wipes);
                moviesAndTv.Products.Add(terminatorGenysis);
                moviesAndTv.Products.Add(howIMetYourMother);
                moviesAndTv.Products.Add(theDayAfterTomorrow);
                context.SaveChanges();
            }
        }
    }
}
