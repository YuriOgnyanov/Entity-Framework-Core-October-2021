using AutoMapper;
using ProductShop.DataTransferObject;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UsersInputModel, User>();
            this.CreateMap<ProductsInputModel, Product>();
            this.CreateMap<CategoriesInputModel, Category>();
            this.CreateMap<CategoryProductsInputModel, CategoryProduct>();
        }

        
    }
}
