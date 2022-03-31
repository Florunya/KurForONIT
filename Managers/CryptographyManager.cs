using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Kur.Managers
{
    public class CryptographyManager : ICryptographyManager
    {
        private readonly IWebHostEnvironment _env;
        private readonly string WayPath;

        public CryptographyManager(IWebHostEnvironment env)
        {
            _env = env;
            WayPath = Path.Combine(_env.ContentRootPath, "wwwroot", "documents");
        }

        #region AES

        public async Task<(byte[] data, string ext)> AESDecrypt(byte[] data, string password)
        {
            if (data == null || data.Length <= 0)
            {
                throw new Exception("Данных нет");
            }

            var Key = CreateHashMD5(password);

            string FileName = Path.Combine(WayPath, $"{Guid.NewGuid()}.txt");
            await File.WriteAllBytesAsync(FileName, data);

            var fileLines = (await File.ReadAllLinesAsync(FileName)).ToList();
            var str = fileLines[fileLines.Count() - 3];
            var IV = Encoding.BigEndianUnicode.GetBytes(str);
            var str2 = fileLines[fileLines.Count() - 2];
            var lenght = int.Parse(str2);
            var Ext = fileLines[fileLines.Count() - 1];
            
            var fileBytes = await File.ReadAllBytesAsync(FileName);
            var originFileBytes = new byte[lenght];
            Array.Copy(fileBytes, 0, originFileBytes, 0, lenght);

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            byte[] decrypted;
            using (Aes aesAlg = Aes.Create())
            {
                byte[] encrypted = new byte[lenght];

                aesAlg.Key = Key;
                aesAlg.IV = IV;

                decrypted = aesAlg.DecryptCbc(originFileBytes, IV, PaddingMode.PKCS7);
            }

            return (decrypted, Ext);
        }

        public async Task<byte[]> AESEncrypt(byte[] data, string password, string ext)
        {
            if (data == null || data.Length <= 0) return null;
            if (password == null || password.Length <= 0) return null;

            var Key = CreateHashMD5(password);
            byte[] IV;

            byte[] encrypted;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                encrypted = aesAlg.EncryptCbc(data, aesAlg.IV, PaddingMode.PKCS7);
            }

            string FileName = Path.Combine(WayPath, $"{Guid.NewGuid()}.txt");
            await File.WriteAllBytesAsync(FileName, encrypted);

            using (StreamWriter sw = File.AppendText(FileName))
            {
                var str = Encoding.BigEndianUnicode.GetString(IV);
                await sw.WriteAsync($"\n{str}");
                await sw.WriteAsync($"\n{encrypted.Length}");
                await sw.WriteAsync($"\n{ext}");
            }

            var result = await File.ReadAllBytesAsync(FileName);

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            return result;
        }

        #endregion

        #region DES

        public async Task<(byte[] data, string ext)> DESDecrypt(byte[] data, string password)
        {
            if (data == null || data.Length <= 0)
            {
                throw new Exception("Данных нет");
            }

            string FileName = Path.Combine(WayPath, $"{Guid.NewGuid()}.txt");
            await File.WriteAllBytesAsync(FileName, data);

            var fileLines = (await File.ReadAllLinesAsync(FileName)).ToList();
            var str = fileLines[fileLines.Count() - 3];
            var IV = Encoding.BigEndianUnicode.GetBytes(str);
            var str2 = fileLines[fileLines.Count() - 2];
            var lenght = int.Parse(str2);
            var Ext = fileLines[fileLines.Count() - 1];

            var fileBytes = await File.ReadAllBytesAsync(FileName);
            var originFileBytes = new byte[lenght];
            Array.Copy(fileBytes, 0, originFileBytes, 0, lenght);

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            byte[] decrypted;
            using (DES desAlg = DES.Create())
            {
                byte[] encrypted = new byte[lenght];

                var Key = new byte[8];
                Array.Copy(CreateHashMD5(password), 0, Key, 0, 8);
                desAlg.Key = Key;
                desAlg.IV = IV;

                decrypted = desAlg.DecryptCbc(originFileBytes, IV, PaddingMode.PKCS7);
            }

            return (decrypted, Ext);
        }

        public async Task<byte[]> DESEncrypt(byte[] data, string password, string ext)
        {
            if (data == null || data.Length <= 0) return null;
            if (password == null || password.Length <= 0) return null;

            byte[] IV;

            byte[] encrypted;
            using (DES desAlg = DES.Create())
            {
                var Key = new byte[8];
                Array.Copy(CreateHashMD5(password), 0, Key, 0, 8);
                desAlg.Key = Key;
                desAlg.GenerateIV();
                IV = desAlg.IV;

                encrypted = desAlg.EncryptCbc(data, desAlg.IV, PaddingMode.PKCS7);
            }

            string FileName = Path.Combine(WayPath, $"{Guid.NewGuid()}.txt");
            await File.WriteAllBytesAsync(FileName, encrypted);

            using (StreamWriter sw = File.AppendText(FileName))
            {
                var str = Encoding.BigEndianUnicode.GetString(IV);
                await sw.WriteAsync($"\n{str}");
                await sw.WriteAsync($"\n{encrypted.Length}");
                await sw.WriteAsync($"\n{ext}");
            }

            var result = await File.ReadAllBytesAsync(FileName);

            if (File.Exists(FileName))
            {
                File.Delete(FileName);
            }

            return result;
        }
        #endregion

        protected byte[] CreateHashMD5(string password)
        {
            byte[] hash;

            using (MD5 md5 = MD5.Create())
            {
                hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return hash;
        }
    }
}
