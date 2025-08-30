using Basket.API.Basket.GetBasket;

namespace Basket.API.Data
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(string UserName, CancellationToken token = default);
        Task<ShoppingCart> StoreBasket(ShoppingCart Basket, CancellationToken token = default);
        Task<bool> DeleteBasket(string UserName, CancellationToken token = default);
    }
}
