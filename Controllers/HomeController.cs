using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kur.Managers;
using Kur.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kur.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFoodManager _foodManager;
        private readonly ICryptographyManager _cryptographyManager;
        private readonly IWorkPortManagers _workPortManagers;
        private readonly IConvertToExcel _convertToExcelManager;

        public HomeController(
            IFoodManager manager,
            ICryptographyManager cryptographyManager,
            IWorkPortManagers workPortManagers,
            IConvertToExcel convertToExcelManager)
        {
            _foodManager = manager;
            _cryptographyManager = cryptographyManager;
            _workPortManagers = workPortManagers;
            _convertToExcelManager = convertToExcelManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var foods = _foodManager.GetAllFood();
            var view = new ViewModel()
            {
                Foods = foods
            };

            return View(view);
        }

        #region CRUD

        [HttpPost]
        public async Task<IActionResult> AddFood(Food food)
        {
            if (ModelState.IsValid)
            {
                await _foodManager.AddFoodAsync(food);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult UpdateFood(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var food = _foodManager.GetFood(Guid.Parse(id));
                return View(food);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFood(Food food)
        {
            if (ModelState.IsValid)
            {
                await _foodManager.UpdateFoodAsync(food);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFood(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                await _foodManager.DeleteFoodAsync(Guid.Parse(id));
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region cryptography

        [HttpPost]
        public async Task<IActionResult> EncryptFile(IFormFile uploadedFile, string algorithm, string password)
        {
            try
            {
                if (uploadedFile != null && algorithm != null && password != null)
                {

                    string fileName = uploadedFile!.FileName;
                    string ext = Path.GetExtension(fileName);
                    string fileType = "application/" + ext.Trim('.');
                    fileName = "EncryptFile" + ".cry";

                    byte[] data = new byte[uploadedFile.Length];
                    using (var stream = uploadedFile.OpenReadStream())
                    {
                        await stream.ReadAsync(data);
                    }

                    switch (algorithm)
                    {
                        case "aes":
                            var aesEncrypt = await Task.Run(() => _cryptographyManager.AESEncrypt(data, password, ext));
                            return File(aesEncrypt, fileType, fileName);
                        case "des":
                            var desEncrypt = await Task.Run(() => _cryptographyManager.DESEncrypt(data, password, ext));
                            return File(desEncrypt, fileType, fileName);
                    }
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DecryptFile(IFormFile uploadedFile, string algorithm, string password)
        {

            try
            {
                if (uploadedFile != null && algorithm != null && password != null)
                {
                    byte[] data = new byte[uploadedFile.Length];
                    using (var stream = uploadedFile.OpenReadStream())
                    {
                        await stream.ReadAsync(data);
                    }

                    switch (algorithm)
                    {
                        case "aes":
                            var aesDecrypt = await Task.Run(() => _cryptographyManager.AESDecrypt(data, password));
                            return File(aesDecrypt.data, "application/" + aesDecrypt.ext, "DecryptFile" + aesDecrypt.ext);
                        case "des":
                            var desDecrypt = await Task.Run(() => _cryptographyManager.DESDecrypt(data, password));
                            return File(desDecrypt.data, "application/" + desDecrypt.ext, "DecryptFile" + desDecrypt.ext);
                    }
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region WorkPorts

        [HttpGet]
        public IActionResult GetInfoPorts()
        {
            ViewData["MiniTitle"] = "Список активных TCP подключений";

            var result = _workPortManagers.GetActiveTcpConnections();
            if (result == null || result.Count() == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var view = new ViewModel()
            {
                PortInfos = result
            };

            return View(nameof(Index), view);
        }

        [HttpGet]
        public IActionResult GetInfoActiveTCPListeners()
        {
            ViewData["MiniTitle"] = "Список активных прослушивателей TCP";

            var result = _workPortManagers.GetActiveTcpListeners();
            if (result == null || result.Count() == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var view = new ViewModel()
            {
                EndPoints = result
            };

            return View(nameof(Index), view);
        }

        [HttpGet]
        public IActionResult GetInfoActiveUDPListeners()
        {
            ViewData["MiniTitle"] = "Список активных прослушивателей UDP";

            var result = _workPortManagers.GetActiveUdpListeners();
            if (result == null || result.Count() == 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var view = new ViewModel()
            {
                EndPoints = result
            };

            return View(nameof(Index), view);
        }

        #endregion

        #region ConvertToExcel

        [HttpGet]
        public async Task<IActionResult> GetFile()
        {
            var data = await _convertToExcelManager.ConvertDbToExcel();
            if(data != null)
            {
                string fileName = "Foods.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(data, contentType, fileName);
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion
    }
}
