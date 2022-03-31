using System;
using System.Threading.Tasks;

namespace Kur.Managers
{
    public interface IConvertToExcel
    {
        public Task<byte[]> ConvertDbToExcel();
    }
}
