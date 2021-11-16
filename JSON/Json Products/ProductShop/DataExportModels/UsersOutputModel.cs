using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop.DataExportModels
{
    public class UsersOutputModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public IEnumerable<ProductWithBuyerOutputModel> SoldProducts { get; set; }
    }
}
