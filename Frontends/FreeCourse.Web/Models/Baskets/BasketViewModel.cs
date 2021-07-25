using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Web.Models.Baskets
{
    public class BasketViewModel
    {
        public BasketViewModel()
        {
            _basketItems = new List<BasketItemViewModel>();
        }
        public string UserId { get; set; }
        public string DiscountCode { get; set; }
        public int? DiscountRate { get; set; }
        private List<BasketItemViewModel> _basketItems;
        public List<BasketItemViewModel> BasketItems
        {
            get
            {
                if (HasDiscount)
                {
                    //Örnek kurs fiyat 100 TL indirim %10
                    _basketItems.ForEach(x =>
                    {
                        var discountPrice = x.Price * ((decimal)DiscountRate.Value / 100);
                        x.AppliedDiscount(Math.Round(x.Price - discountPrice, 2)); //90.00 TL
                    });
                }
                return _basketItems;
            }
            set
            {
                _basketItems = value;
            }
        }

        public void CancelDiscount()
        {
            DiscountRate = null;
            DiscountCode = null;
        }

        public void ApplyDiscount(string code, int rate)
        {
            DiscountCode = code;
            DiscountRate = rate;
        }

        public decimal TotalPrice => _basketItems.Sum(x => x.GetCurrentPrice);
        public bool HasDiscount => !string.IsNullOrEmpty(DiscountCode) && DiscountRate.HasValue;
    }
}
