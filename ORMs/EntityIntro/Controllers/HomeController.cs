using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using EntityIntro.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EntityIntro.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;

        public HomeController(MyContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public ViewResult Index()
        {
            // // get all vampires from database
            ViewBag.AllVampires = _context.Vampires
                .Include(v => v.Victims)
                .ToList();
            // // get ONE vampire from the database, with an id of 2
            // Vampire SecondVampire = _context.Vampires.FirstOrDefault(v => v.VampireId == 2);
            // // get all vampires who have killed fewer than 3 victims
            // List<Vampire> FewerThanThreeVictims = _context.Vampires.Where(v => v.Victims < 3).ToList();

            

            return View("Index");
        }

        [HttpGet("vampire/new")]
        public IActionResult NewVampire()
        {
            return View("NewVampire");
        }

        [HttpPost("vampire/new")]
        public IActionResult SubmitVampire(Vampire FromForm)
        {
            if(ModelState.IsValid)
            {
                return RedirectToAction("NewVampire");
            }
            else
            {
                return NewVampire();
            }
        }

        [HttpGet("vampire/{VampireId}")]
        public IActionResult OneVampire(int VampireId)
        {
            Vampire ToDisplay = _context.Vampires
                .Include(v => v.Victims)
                .FirstOrDefault(v => v.VampireId == VampireId);
            return View("OneVampire", ToDisplay);
        }


        [HttpPost("createvictim")]
        public IActionResult CreateVictim(Victim FromForm)
        {
            _context.Add(FromForm);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost("create")]
        public IActionResult CreateVampire(Vampire FromForm)
        {
            // Adding a new vampire to the database
            // Vampire Dracula = new Vampire()
            // {
            //     Name = "Dracula",
            //     Birthday = new DateTime(1431, 6, 6),
            //     Victims = 234
            // }; 

            _context.Add(FromForm);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost("update/{VampireId}")]
        public IActionResult UpdateVampire(int VampireId, Vampire FromForm)
        {
            if(_context.Vampires.Any(v => v.VampireId == VampireId)) 
            {
                return RedirectToAction("Index");
            }
            // Retrieved.Name = FromForm.Name;
            // Retrieved.Birthday = FromForm.Birthday;
            // Retrieved.Victims = FromForm.Victims;
            // Retrieved.UpdatedAt = FromForm.UpdatedAt;

            // _context.SaveChanges();

            FromForm.VampireId = VampireId;
            _context.Update(FromForm);
            _context.Entry(FromForm).Property("CreatedAt").IsModified = false;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet("delete/{VampireId}")]
        public IActionResult DeleteVampire(int VampireId)
        {
            // logic to determine if we should actually delete

            Vampire ToDelete = _context.Vampires.FirstOrDefault(v => v.VampireId == VampireId);

            if(ToDelete == null)
            {
                return RedirectToAction("Index");
            }

            _context.Remove(ToDelete);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}