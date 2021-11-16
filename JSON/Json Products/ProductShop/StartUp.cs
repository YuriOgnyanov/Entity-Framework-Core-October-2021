using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DataExportModels;
using ProductShop.DataTransferObject;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var dbContext = new ProductShopContext();
            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //var json = File.ReadAllText("../../../Datasets/categories-products.json");

            var result = GetCategoriesByProductsCount(dbContext);
            Console.WriteLine(result);
        }
        
        public static string GetCategoriesByProductsCount(ProductShopContext context) 
        {
            var categories = context.Categories.Select(x => new CategoriesByProductsCountOutputModel
            {
                Category = x.Name,
                ProductsCount = x.CategoryProducts.Count(c => c.CategoryId == x.Id),
                AveragePrice = x.CategoryProducts.Average(a => a.Product.Price).ToString("F2"),
                TotalRevenue = x.CategoryProducts.Sum(a => a.Product.Price).ToString("F2")
            })
             .OrderByDescending(x => x.ProductsCount);

            var fileName = "categoriesProduct.json";
            var convertToJson = JsonConvert.SerializeObject(categories, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            File.WriteAllText(fileName, convertToJson);

            return File.ReadAllText(fileName);
        }
        public static string GetSoldProducts(ProductShopContext context) 
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Count > 0 && x.ProductsSold.Any(b => b.Buyer != null))
                .Select(x => new UsersOutputModel
                {
                    FirstName = x.FirstName, // Gloriq
                    LastName = x.LastName,   // Alexander
                    SoldProducts = x.ProductsSold.Select(p => new ProductWithBuyerOutputModel
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })

                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName);

            //Json SerializeObject
            var fileName = "soldProduct.json";
            var exportToJson = JsonConvert.SerializeObject(users, Formatting.Indented, new JsonSerializerSettings 
            { 
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            File.WriteAllText(fileName, exportToJson);

            return File.ReadAllText(fileName);
        }
        public static string GetProductsInRange(ProductShopContext context) 
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(x => new ProductsOutputModel
                {
                    name = x.Name,
                    price = x.Price,
                    seller = x.Seller.FirstName + " " + x.Seller.LastName
                })
                .OrderBy(x => x.price)
                .ToList();

            var fileName = "products.json";
            var exportToJson = JsonConvert.SerializeObject(products, Formatting.Indented);
            File.WriteAllText(fileName, exportToJson);

            return File.ReadAllText(fileName);
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson) 
        {
            DataMapper();

            var dtoCategoryProducts = JsonConvert.DeserializeObject<IEnumerable<CategoryProductsInputModel>>(inputJson);
            var categoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(dtoCategoryProducts);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count()}";
        }
        public static string ImportCategories(ProductShopContext context, string inputJson) 
        {
            DataMapper();

            var dtoCategories = JsonConvert.DeserializeObject<IEnumerable<CategoriesInputModel>>(inputJson);
            var categories = mapper.Map<IEnumerable<Category>>(dtoCategories.Where(x => x.Name != null));
            

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }
        public static string ImportProducts(ProductShopContext context, string inputJson) 
        {
            DataMapper();

            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductsInputModel>>(inputJson);
            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            DataMapper();
            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UsersInputModel>>(inputJson);
            var users = mapper.Map<IEnumerable<User>>(dtoUsers);
            context.Users.AddRange(users);
            context.SaveChanges();
            
            return $"Successfully imported {users.Count()}";
        }

        private static void DataMapper()
        {
            var config = new MapperConfiguration(x =>
            {
                x.AddProfile<ProductShopProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}