using System.Threading.Tasks;

namespace Kur.Managers
{
    public interface ICryptographyManager
    {
        public Task<byte[]> AESEncrypt(byte[] data, string password, string ext);
        public Task<(byte[] data, string ext)> AESDecrypt(byte[] data, string password);

        public Task<byte[]> DESEncrypt(byte[] data, string password, string ext);
        public Task<(byte[] data, string ext)> DESDecrypt(byte[] data, string password);
    }
}
