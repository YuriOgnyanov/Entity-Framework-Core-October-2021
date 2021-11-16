using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DataExportModels
{
    public class ProductWithBuyerOutputModel
    {
        public string Name { get; set; }

        public decimal Price { get; set; }

        public string BuyerFirstName { get; set; }
        public string BuyerLastName { get; set; }
    }
}
