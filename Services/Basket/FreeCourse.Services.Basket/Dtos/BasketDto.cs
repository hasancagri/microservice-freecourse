using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Services.Basket.Dtos
{
    public class BasketDto
    {
        public string UserId { get; set; }
        public string DiscountCode { get; set; }
        public decimal? DiscountRate { get; set; }
        public List<BasketItemDto> BasketItems { get; set; }
        public decimal TotalPrice => BasketItems.Sum(x => x.Price * x.Quantity);
    }
}
