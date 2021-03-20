using System.Collections.Generic;

namespace ShoppingCartGrpcMicroserviceApp.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public ShoppingCart()
        {

        }

        public ShoppingCart(string userName)
        {
            UserName = userName;
        }

        public float TotalPrice
        {
            get
            {
                float totalPrice = 0;
                foreach (var item in Items)
                {
                    totalPrice += (item.Quantity * item.Price);
                }
                return totalPrice;
            }
        }
    }
}
