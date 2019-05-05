﻿using GigHub.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using GigHub.Core.ViewModels;
using Microsoft.AspNet.Identity;
using GigHub.Persistence.Repositories;
using GigHub.Persistence;

namespace GigHub.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private AttendanceRepository _attendencesRepository;

        public HomeController()
        {
            _context = new ApplicationDbContext();
            _attendencesRepository = new AttendanceRepository(_context);
        }

        public ActionResult Index(string query = null)
        {
            var upcomingGigs = _context.Gigs
                .Include(g => g.Artist)
                .Include(g => g.Gendre)
                .Where(g => g.DateTime > DateTime.Now && !g.IsCanceled);

            if (!String.IsNullOrWhiteSpace(query))
            {
                upcomingGigs = upcomingGigs
                    .Where(g =>
                            g.Artist.Name.Contains(query) ||
                            g.Gendre.Name.Contains(query) ||
                            g.Venue.Contains(query));




            }
            var userId = User.Identity.GetUserId();
            var attendances = _attendencesRepository.GetFutureAttendances(userId)
                .ToLookup(a => a.GigId);

            //var attendances = _context.Attendances
            //    .Where(a => a.AttendeeId == userId && a.Gig.DateTime > DateTime.Now)
            //    .ToList()
            //    .ToLookup(a => a.GigId);
            var viewModel = new GigsViewModel
            {
                UpcomingGigs = upcomingGigs,
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Upcoming Gigs",
                SearchTerm=query,
                Attendances=attendances
            };

            return View("Gigs", viewModel);
            //return View(upcomingGigs);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}