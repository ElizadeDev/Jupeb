using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JupebPortal.Data;
using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using JupebPortal.Enums;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace JupebPortal.Controllers
{
    public class OLevelFormController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly IOLevelFormService _service;
		private readonly UserManager<ApplicationUser> _userManager;

		public OLevelFormController(ApplicationDbContext context, IOLevelFormService service, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_service = service;
			_userManager = userManager;
		}

        // GET: ApplicationForm

        public async Task<IActionResult> Index(string id)
        {
            
            var model = await _context.ApplicantOLevels.ToListAsync();
            return View(model);
        }

        // GET: ApplicationForm/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }

        //    var applicationFormModel = await _context.ApplicationForm
        //        .Include(a => a.Programme1)
        //        .Include(a => a.Programme2)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (applicationFormModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(applicationFormModel);
        //}

        // GET: ApplicationForm/Create

        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);

            var subject = _context.Subjects.ToList();

            if (subject.Count > 0)
            {
                ViewBag.Subjects = new SelectList(subject, "id", "Name"); 
            }
            else
            {
                ViewBag.Subjects = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (applicationForm == null)
            {
                return RedirectToAction("Create", "ApplicationForm");
            }

            var oLevelGrades1 = await _context.ApplicantOLevels.Where(m => m.ApplicationFormFK == applicationForm.Id && m.Sitting == "1").ToListAsync();
            ViewBag.existingSitting1 = oLevelGrades1;

            var oLevelGrades2 = await _context.ApplicantOLevels.Where(m => m.ApplicationFormFK == applicationForm.Id && m.Sitting == "2").ToListAsync();
            ViewBag.existingSitting2 = oLevelGrades2;

            return View();

        }
        
            
        

        // POST: ApplicationForm/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,UserId,Surname,FirstName,OtherName,Email,Phone,Address,City,Gender,DOB,State,Nationality,GuardName,GuardEmail,GuardAddress,GuardPhone,EntryMode,UTMEregNo,Programme1Id,Programme2Id")] ApplicationFormModel applicationFormModel)
        public async Task<IActionResult> Create(ApplicantOLevel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (applicationForm == null)
            {
                return NotFound();
            }

            model.ApplicationFormFK = applicationForm.Id;
            model.ApplicationForm = applicationForm;

            ModelState.Clear(); // Clear the model state to ensure the new values are validated

            TryValidateModel(model);

            if (!ModelState.IsValid)
            {
                return BadRequest("");
            }
            var applicantEntries = await _context.ApplicantOLevels
                .Where(s => s.ApplicationFormFK == model.ApplicationFormFK && s.Sitting == model.Sitting)
                .ToListAsync();

            bool shouldSave = applicantEntries.All(s => 
                s.ExamBody == model.ExamBody &&
                s.ExamYear == model.ExamYear &&
                s.ExamNo == model.ExamNo);

            if (!shouldSave)
            {
                return BadRequest("Exam Body, Exam Year, and Exam Number must be the same for all subjects in a sitting");
            }

            //Check for duplicate subject

            bool duplicateSubject = applicantEntries.Any(s =>
               s.SubjectId == model.SubjectId);
            if (duplicateSubject)
            {
                return BadRequest("Subject already added for this sitting");
            }

            var result = await _service.Create(model);
           
            return Ok();
        }

        // GET: ApplicationForm/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }

        //    var applicationFormModel = await _context.ApplicationForm.FindAsync(id);
        //    if (applicationFormModel == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["Programme1Id"] = new SelectList(_context.Programs, "id", "id", applicationFormModel.Programme1Id);
        //    ViewData["Programme2Id"] = new SelectList(_context.Programs, "id", "id", applicationFormModel.Programme2Id);
        //    return View(applicationFormModel);
        //}

        //// POST: ApplicationForm/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Surname,FirstName,OtherName,Email,Phone,Address,City,Gender,DOB,State,Nationality,GuardName,GuardEmail,GuardAddress,GuardPhone,EntryMode,UTMEregNo,Programme1Id,Programme2Id")] ApplicationFormModel applicationFormModel)
        //{
        //    if (id != applicationFormModel.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(applicationFormModel);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ApplicationFormModelExists(applicationFormModel.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["Programme1Id"] = new SelectList(_context.Programs, "id", "id", applicationFormModel.Programme1Id);
        //    ViewData["Programme2Id"] = new SelectList(_context.Programs, "id", "id", applicationFormModel.Programme2Id);
        //    return View(applicationFormModel);
        //}

        //// GET: ApplicationForm/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }

        //    var applicationFormModel = await _context.ApplicationForm
        //        .Include(a => a.Programme1)
        //        .Include(a => a.Programme2)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (applicationFormModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(applicationFormModel);
        //}

        // POST: ApplicationForm/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.ApplicantOlevelSubject == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.ApplicantOlevelSubject'  is null.");
        //    }
        //    var item = await _context.ApplicantOlevelSubject.FindAsync(id);
        //    if (item != null)
        //    {
        //        _context.ApplicantOlevelSubject.Remove(item);
        //    }

        //    await _context.SaveChangesAsync();
        //    return View(nameof(Index));
        //}

        [HttpPost]
        public async Task<IActionResult> DeleteSubject(int id, string sitting)
        {
            if (_context.ApplicantOLevels == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ApplicantSubject'  is null.");
            }

            var user = await _userManager.GetUserAsync(User);

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (applicationForm == null)
            {
                return NotFound();
            }

            var item = await _context.ApplicantOLevels
                .FirstOrDefaultAsync(s => s.ApplicationFormFK == applicationForm.Id && s.SubjectId == id && s.Sitting == sitting);

            if (item != null)
            {
                _context.ApplicantOLevels.Remove(item);
                await _context.SaveChangesAsync();
            }


            var result = await _context.ApplicantOLevels
                .Where(s => s.ApplicationFormFK == applicationForm.Id && s.Sitting == sitting)
                .Select(s => new
                {
                    Subject = s.Subject.Name,
                    s.SubjectId,
                    grade = s.Grade
                })
                .ToListAsync();

            var updatedResult = result.Select(item => new
            {
                item.Subject,
                item.SubjectId,
                gradeName = Enum.GetName(typeof(Grade), item.grade)
            }).ToList();

            return Json(updatedResult);
        }

        //private bool ApplicationFormModelExists(Guid id)
        //{
        //  return (_context.ApplicationForm?.Any(e => e.Id = id)).GetValueOrDefault();
        //}
    }
}
