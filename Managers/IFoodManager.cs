using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kur.Models;

namespace Kur.Managers
{
    public interface IFoodManager
    {
        public Task<bool> AddFoodAsync(Food entity);
        public Task<bool> UpdateFoodAsync(Food food);
        public Task<bool> DeleteFoodAsync(Guid id);
        public Food GetFood(Guid id);
        public List<Food> GetAllFood();
    }
}
