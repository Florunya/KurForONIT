using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kur.Models;
using Kur.Storeges;
using System.Linq;

namespace Kur.Managers
{
    public class FoodManager : IFoodManager
    {
        private readonly FoodDbContext _dbContext;
        public FoodManager(FoodDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddFoodAsync(Food food)
        {
            try
            {
                food.Id = Guid.NewGuid();
                await _dbContext.Foods.AddAsync(food);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        public async Task<bool> DeleteFoodAsync(Guid id)
        {
            try
            {
                var food = _dbContext.Foods.FirstOrDefault(x => x.Id == id);
                _dbContext.Foods.Remove(food);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Food> GetAllFood()
        {
            try
            {
                var foods = _dbContext.Foods.ToList();
                return foods;
            }
            catch
            {
                return null;
            }
        }

        public Food GetFood(Guid id)
        {
            try
            {
                var food = _dbContext.Foods.FirstOrDefault(x => x.Id == id);
                return food;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateFoodAsync(Food food)
        {
            try
            {
                var entity = _dbContext.Foods.FirstOrDefault(x => x.Id == food.Id);
                if (entity == null) return false;

                entity.Name = food.Name;
                entity.Type = food.Type;
                entity.Calories = food.Calories;
                entity.Price = food.Price;

                _dbContext.Foods.Update(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
