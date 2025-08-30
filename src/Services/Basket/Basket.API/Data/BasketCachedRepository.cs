
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data
{
    public class BasketCachedRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
    {
        public async Task<bool> DeleteBasket(string UserName, CancellationToken token = default)
        {
            await repository.DeleteBasket(UserName, token);
            await cache.RemoveAsync(UserName, token);

            return true;
        }

        public async Task<ShoppingCart> GetBasket(string UserName, CancellationToken token = default)
        {
            var cachedBasketd = await cache.GetStringAsync(UserName, token);
            if(!string.IsNullOrEmpty(cachedBasketd))
            {
                return JsonSerializer.Deserialize<ShoppingCart>(cachedBasketd)!;
            }

            var basket = await repository.GetBasket(UserName, token);
            await cache.SetStringAsync(UserName, JsonSerializer.Serialize(basket));
            return basket;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart Basket, CancellationToken token = default)
        {
            var result = await repository.StoreBasket(Basket, token);
            await cache.SetStringAsync(Basket.UserName, JsonSerializer.Serialize(Basket));
            return result;
        }
    }
}
