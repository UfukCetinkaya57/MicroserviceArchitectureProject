namespace FreeCourse.Services.Basket.Dtos
{
    public class BasketDto
    {
        public string Userld { get; set; }
        public string DiscountCode { get; set; }
        public List<BasketItemDto> basketitems{get; set; }
        public decimal TotalPrice {
            get => basketitems.Sum(x => x.Price * x.Quantity); 
        }
    }
}
