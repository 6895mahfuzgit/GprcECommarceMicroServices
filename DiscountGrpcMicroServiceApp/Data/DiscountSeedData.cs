using DiscountGrpcMicroServiceApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscountGrpcMicroServiceApp.Data
{
    public  class DiscountSeedData
    {
        public static void SeedDiscountDataInDB(DiscountContext dbContext)
        {
            if (!dbContext.Discounts.Any())
            {
                var discountList = new List<Discount>
                {
                    new Discount {DiscountId=1,Code="D100",Amount=100 },
                    new Discount {DiscountId=2,Code="D200",Amount=200 },
                    new Discount {DiscountId=3,Code="D300",Amount=300 },
                    new Discount {DiscountId=4,Code="D400",Amount=400 },
                    new Discount {DiscountId=5,Code="D500",Amount=500 },
                    new Discount {DiscountId=6,Code="D600",Amount=600 },
                };

                dbContext.Discounts.AddRange(discountList);
                dbContext.SaveChanges();
            }
        }
    }
}
