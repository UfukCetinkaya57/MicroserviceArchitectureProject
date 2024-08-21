using FreeCourse.Services.Order.Domain.Core;
using System;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    // OrderItem sınıfı, bir sipariş içindeki ürünleri temsil eder.
    public class OrderItem : Entity
    {
        // Ürünün benzersiz kimliği.
        public string ProductId { get; set; }

        // Ürünün adı.
        public string ProductName { get; set; }

        // Ürünün görselinin URL'si.
        public string PictureUrl { get; set; }

        // Ürünün fiyatı.
        public decimal Price { get; set; }

        // Boş constructor, EF Core tarafından gereklidir.
        public OrderItem()
        {
        }

        // OrderItem sınıfının constructor'ı, ürün bilgilerini alır ve nesne oluşturur.
        public OrderItem(string productId, string productName, string pictureUrl, decimal price)
        {
            ProductId = productId;
            ProductName = productName;
            PictureUrl = pictureUrl;
            Price = price;
        }

        // Sipariş içindeki ürünün bilgilerini günceller.
        public void UpdateOrderItem(string productName, string pictureUrl, decimal price)
        {
            ProductName = productName;
            Price = price;
            PictureUrl = pictureUrl;
        }
    }
}
