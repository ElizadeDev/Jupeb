using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JupebPortal.Controllers.Admin
{
	public class SubjectController: Controller
	{
		private readonly ISubjectService _services;

		public SubjectController(ISubjectService services)
		{
			_services = services;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _services.GetAll());
		}

        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Subject model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _services.Add(model);
            if (result)
            {
                TempData["msg"] = "Added Successfully";
                return RedirectToAction(nameof(Add));
            }
            TempData["msg"] = "Error has occured on server side";
            return View(model);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var data = await _services.GetById(id);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Subject model)
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

        public async Task<IActionResult> Delete(int id)
        {
            await _services.Delete(id);
            return RedirectToAction("Index");
        }

    }
}
