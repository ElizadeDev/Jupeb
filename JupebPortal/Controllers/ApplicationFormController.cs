using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JupebPortal.Data;
using JupebPortal.Models;
using JupebPortal.Repository.Interface;
using Microsoft.AspNetCore.Identity;
using JupebPortal.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using JupebPortal.Repository.AppServices;
using JupebPortal.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;

namespace JupebPortal.Controllers
{
    public class ApplicationFormController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly IApplicationFormService _service;
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IWebHostEnvironment _iWebHostEnvironment;

        public ApplicationFormController(ApplicationDbContext context, IApplicationFormService service, UserManager<ApplicationUser> userManager, IEmailService emailService, IWebHostEnvironment iWebHostEnvironment)
        {
            _context = context;
            _service = service;
            _userManager = userManager;
            _emailService = emailService;
            _iWebHostEnvironment = iWebHostEnvironment;
        }

        // GET: ApplicationForm
        [Authorize(Roles = "Admin,Admission officer")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ApplicationForms
                .Include(a => a.Programme1).Include(a => a.Programme2)
                .Include(a => a.OLevelScores)
                    .ThenInclude(p => p.Subject);
                
            var entries = await applicationDbContext.ToListAsync();
            return View(entries);
        }

        // GET: ApplicationForm/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null || _context.ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }

        //    var ApplicationForm = await _context.ApplicationForm
        //        .Include(a => a.Programme1)
        //        .Include(a => a.Programme2)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ApplicationForm);
        //}

        private void GetPrograms()
        {
            var programs = _context.Programmes.ToList();

            if (programs.Count > 0)
            {
                ViewData["Programme1Id"] = new SelectList(programs, "id", "Name");
                ViewData["Programme2Id"] = new SelectList(programs, "id", "Name");
            }
            else
            {
                ViewData["Programme1Id"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["Programme2Id"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        // GET: ApplicationForm/Create
        //[Authorize(Roles = "applicant")]
        public async Task<IActionResult> Create()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var userDetails = await _userManager.FindByIdAsync(user.ToString());
            ViewBag.SurName = userDetails.Surname;
            ViewBag.FirstName = userDetails.FirstName;
            ViewBag.Email = userDetails.Email;
            ViewBag.OtherName = userDetails.OtherName;

            GetPrograms();

            var newForm = new JupebFormViewModel();


            var userForm = await _context.ApplicationForms.FirstOrDefaultAsync(m => m.UserId == user);

            if (userForm != null)
            {
                var existingForm = new JupebFormViewModel
                {
                    Email = userForm.Email,
                    FirstName = userForm.FirstName,
                    OtherName = userForm.OtherName,
                    Surname = userForm.Surname,
                    DateOfBirth = userForm.DateOfBirth,
                    Address = userForm.Address,
                    Gender = userForm.Gender,
                    MaritalStatus = userForm.MaritalStatus,

                    Phone = userForm.Phone,
                    PlaceOfBirth = userForm.PlaceOfBirth,
                    Religion = userForm.Religion,
                    StateOfOrigin = userForm.StateOfOrigin,

                    Programme1Id = userForm.Programme1Id,
                    Programme2Id = userForm.Programme2Id,

                    GuardEmail = userForm.GuardEmail,
                    GuardName = userForm.GuardName,
                    GuardPhone = userForm.GuardPhone,
                };
                return View(existingForm);
            }


            return View(newForm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JupebFormViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            model.User = user;
            model.UserId = user.Id;
            ModelState.Clear(); // Clear the model state to ensure the new values are validated

            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                var userForm = await _context.ApplicationForms.FirstOrDefaultAsync(m => m.UserId == user.Id);

                if (userForm != null)
                {
                    userForm.DateOfBirth = model.DateOfBirth;
                    userForm.Address = model.Address;
                    userForm.Gender = model.Gender;
                    userForm.MaritalStatus = model.MaritalStatus;

                    userForm.Phone = model.Phone;
                    userForm.PlaceOfBirth = model.PlaceOfBirth;
                    userForm.Religion = model.Religion;
                    userForm.StateOfOrigin = model.StateOfOrigin;
                    userForm.ModifiedAt = DateTime.UtcNow;
                   
                    userForm.Programme1Id = model.Programme1Id;
                    userForm.Programme2Id = model.Programme2Id;

                    userForm.GuardEmail = model.GuardEmail;
                    userForm.GuardName = model.GuardName;
                    userForm.GuardPhone = model.GuardPhone;

                    if (model.Photo != null)
                    {
                        userForm.PicturePath = UploadPhotoFile(model);
                    }
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newform = new ApplicationForm
                    {
                        UserId = model.UserId,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        OtherName = model.OtherName,
                        Surname = model.Surname,
                        DateOfBirth = model.DateOfBirth,
                        Address = model.Address,
                        Gender = model.Gender,
                        MaritalStatus = model.MaritalStatus,

                        Phone = model.Phone,
                        PlaceOfBirth = model.PlaceOfBirth,
                        Religion = model.Religion,
                        StateOfOrigin = model.StateOfOrigin,
                        PicturePath = UploadPhotoFile(model),

                        Programme1Id = model.Programme1Id,
                        Programme2Id = model.Programme2Id,

                        GuardEmail = model.GuardEmail,
                        GuardName = model.GuardName,
                        GuardPhone = model.GuardPhone,

                    };

                    _context.ApplicationForms.Add(newform);

                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction("Create", "OLevelForm");
            }
           
            ViewBag.SurName = user.Surname;
            ViewBag.FirstName = user.FirstName;
            ViewBag.Email = user.Email;
            ViewBag.OtherName = user.OtherName;

            GetPrograms();

            return View(nameof(Create), model);


        }

        private string? UploadPhotoFile(JupebFormViewModel model)
        {
            string fileName = null;
            if (model.Photo != null)
            {    //getting file path(folder inside wwwroot)

                string uploadsFolder = Path.Combine(_iWebHostEnvironment.WebRootPath, "ApplicantPhotos");

                //making the image name unique
                fileName = model.UserId + "_passport" + Path.GetExtension(model.Photo.FileName);
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fs);
                }

            }
            else
            {
                return null;
            }

            return fileName;
        }



        
        // GET: ApplicationForm/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null || _context.ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }

        //    var ApplicationForm = await _context.ApplicationForm.FindAsync(id);
        //    if (ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["Programme1Id"] = new SelectList(_context.Programs, "id", "id", ApplicationForm.Programme1Id);
        //    ViewData["Programme2Id"] = new SelectList(_context.Programs, "id", "id", ApplicationForm.Programme2Id);
        //    return View(ApplicationForm);
        //}

        public async Task<IActionResult> validateSubjectCount()
        {
            var user = await _userManager.GetUserAsync(User);

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (applicationForm == null)
            {
                return NotFound();
            }

            var oLevelsubjects = await _context.ApplicantOLevels
                .Where(s => s.ApplicationFormFK == applicationForm.Id)
                .Select(s => new
                {
                    s.SubjectId,
                    s.Grade
                })
                .ToListAsync();
            if (oLevelsubjects == null)
            {
                return BadRequest("Please fill your O' level subjects.\n Select 'AR' as grade if awaiting results.");
            }
            if (oLevelsubjects.Count < 5)
            {
                return BadRequest("Please supply up to 5 O' level subjects.");
            }

            //TODO set isSubmitted to true here
            applicationForm.isSubmitted = true;
            applicationForm.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            //Send Registration Sucessfull Email
            try
            {
                UserEmailOptions options = new UserEmailOptions
                {
                    ToEmails = new List<string>() { user.Email },
                    PlaceHolders = new List<KeyValuePair<string, string>>()
                        {
                          new KeyValuePair<string, string>("{{UserName}}", user.FirstName)
                       }

                };
           
                await _emailService.PDSendEmailRegistrationSuccess(options);

            }
            catch
            {

            }
            return Ok();
        }

        [Authorize(Roles = "applicant")]
        public async Task<IActionResult> Edit(int? id)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userDetails = await _userManager.FindByIdAsync(user.ToString());
            ViewBag.SurName = userDetails.Surname;
            ViewBag.FirstName = userDetails.FirstName;
            ViewBag.Email = userDetails.Email;
            ViewBag.OtherName = userDetails.OtherName;

            var programs = _context.Programmes.ToList();

            if (programs.Count > 0)
            {
                ViewData["Programme1Id"] = new SelectList(programs, "id", "Name");
                ViewData["Programme2Id"] = new SelectList(programs, "id", "Name");
            }
            else
            {
                ViewData["Programme1Id"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["Programme2Id"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            var userForm = new ApplicationForm();
            

            if (_context.ApplicationForms == null)
            {
                return RedirectToAction("Create", "ApplicationForm");
            }
            
            userForm = await _context.ApplicationForms.FirstOrDefaultAsync(m => m.UserId == user);

            if (userForm == null)
            {
                return RedirectToAction("Create", "ApplicationForm");
            }
            
            return View(userForm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApplicationForm ApplicationForm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.GetUserAsync(User);
            ApplicationForm.User = user;
            ModelState.Clear(); // Clear the model state to ensure the new values are validated

            TryValidateModel(ApplicationForm);
            if (userId != ApplicationForm.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ApplicationForm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationFormExists(ApplicationForm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Create", "UTMEForm");
            }
            ViewData["Programme1Id"] = new SelectList(_context.Programmes, "id", "Name", ApplicationForm.Programme1Id);
            ViewData["Programme2Id"] = new SelectList(_context.Programmes, "id", "Name", ApplicationForm.Programme2Id);
            return View(ApplicationForm);
        }


        //// GET: ApplicationForm/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }

        //    var ApplicationForm = await _context.ApplicationForm
        //        .Include(a => a.Programme1)
        //        .Include(a => a.Programme2)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (ApplicationForm == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(ApplicationForm);
        //}

        //// POST: ApplicationForm/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.ApplicationForm == null)
        //    {
        //        return Problem("Entity set 'ApplicationDbContext.ApplicationForm'  is null.");
        //    }
        //    var ApplicationForm = await _context.ApplicationForm.FindAsync(id);
        //    if (ApplicationForm != null)
        //    {
        //        _context.ApplicationForm.Remove(ApplicationForm);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool ApplicationFormExists(int id)
        {
            return (_context.ApplicationForms?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
