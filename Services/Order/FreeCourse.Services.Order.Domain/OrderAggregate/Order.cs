using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    // EF Core özellikleri:
    // -- Owned Types: Address sınıfı Order tarafından sahiplenilen bir türdür.
    // -- Shadow Property: EF Core tarafından yönetilen, sınıfta bulunmayan fakat veritabanında var olan özelliklerdir.
    // -- Backing Field: Koleksiyonlar ve diğer private alanlar için EF Core'un arka planda kullandığı alanlardır.
    public class Order : Entity, IAggregateRoot
    {
        // Siparişin oluşturulma tarihi.
        public DateTime CreatedDate { get; private set; }

        // Siparişin teslim edileceği adres.
        public Address Address { get; private set; }

        // Siparişi veren kullanıcının ID'si.
        public string BuyerId { get; private set; }

        // Siparişe ait ürünlerin listesi (private field olarak tanımlanmıştır, EF Core tarafından kullanılır).
        private readonly List<OrderItem> _orderItems;

        // Siparişe ait ürünlerin sadece okunabilir koleksiyonu (public olarak dışarıya açılmıştır).
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        // Boş constructor EF Core için gereklidir. Order sınıfı EF Core tarafından instantiate edilebilir.
        public Order()
        {
        }

        // Order sınıfının constructor'ı, siparişe ait buyerId ve address bilgisini alır.
        // Sipariş oluşturulduğunda ürünler listesi boş başlar.
        public Order(string buyerId, Address address)
        {
            _orderItems = new List<OrderItem>();
            CreatedDate = DateTime.Now;
            BuyerId = buyerId;
            Address = address;
        }

        // Siparişe yeni bir ürün ekler. Eğer ürün zaten varsa eklenmez.
        public void AddOrderItem(string productId, string productName, decimal price, string pictureUrl)
        {
            var existProduct = _orderItems.Any(x => x.ProductId == productId);

            if (!existProduct)
            {
                var newOrderItem = new OrderItem(productId, productName, pictureUrl, price);
                _orderItems.Add(newOrderItem);
            }
        }

        // Siparişin toplam fiyatını döner (ürünlerin fiyatlarının toplamı).
        public decimal GetTotalPrice => _orderItems.Sum(x => x.Price);
    }
}
