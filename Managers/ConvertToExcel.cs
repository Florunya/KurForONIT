using System;
using System.Linq;
using System.Threading.Tasks;
using Kur.Storeges;
using OfficeOpenXml;

namespace Kur.Managers
{
    public class ConvertToExcel : IConvertToExcel
    {
        private readonly FoodDbContext _dbContext;
        public ConvertToExcel(FoodDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<byte[]> ConvertDbToExcel()
        {
            var foods = _dbContext.Foods.ToList();
            if(foods == null && foods.Count == 0)
            {
                return null;
            }

            try
            {
                byte[] excelData;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Foods");
                    sheet.Cells[1, 1].Value = "Наименование";
                    sheet.Cells[1, 2].Value = "Тип";
                    sheet.Cells[1, 3].Value = "Калории";
                    sheet.Cells[1, 4].Value = "Цена";

                    for (int i = 0; i < foods.Count; i++)
                    {
                        sheet.Cells[i + 2, 1].Value = foods[i].Name;
                        sheet.Cells[i + 2, 2].Value = foods[i].Type;
                        sheet.Cells[i + 2, 3].Value = foods[i].Calories;
                        sheet.Cells[i + 2, 4].Value = foods[i].Price;
                    }

                    excelData = await package.GetAsByteArrayAsync();
                }

                return excelData;
            }
            catch
            {
                return null;
            }
        }
    }
}
