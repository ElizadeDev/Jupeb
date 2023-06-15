using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JupebPortal.Controllers
{
	public class ProgrammeController : Controller
	{
		private readonly IProgramServices _services;

		public ProgrammeController(IProgramServices services)
		{
			_services = services;
		}
		[HttpGet]
		public async Task<IActionResult> Index()
		{
			//var models = new Program();
			//return View(models);
			return View(await _services.GetAll());
		}

		public IActionResult Add(bool isSuccess = false)
		{
            ViewBag.IsSuccess = isSuccess;
            return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(Programme model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var result = await _services.Add(model);
			if (result)
			{
				TempData["msg"] = "Added Successfully";
				return RedirectToAction(nameof(Add), new { isSuccess = true });
			}
			TempData["msg"] = "Error has occured on server side";
			return View(model);
		}

        public async Task<IActionResult> Delete(int id)
        {
            await _services.Delete(id);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int id)
        {
            var data = await _services.GetById(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Programme model)
        {
            if (ModelState.IsValid)
            {
                var data = await _services.Update(model);
                if (data)
                {
                    TempData["msg"] = "Updated Successfully";
                    return RedirectToAction("Index");
                }
                TempData["msg"] = "Server Error!";
                return RedirectToAction("Index");
            }
            return View(model);
        }


    }
}
