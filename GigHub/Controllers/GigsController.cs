using GigHub.Core.Models;
using GigHub.Persistence;
using GigHub.Persistence.Repositories;
using GigHub.Core.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GigHub.Core;

namespace GigHub.Controllers
{
    public class GigsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        //private readonly AttendanceRepository _attendanceRepository;
        //private readonly GigRepository _gigRepository;
        //private readonly FollowingRepository _followingRepository;
        //private readonly GenreRepository _genreRepository;
        private readonly IUnitOfWork _unitOfWork;

        public GigsController(IUnitOfWork unitOfWork)
        {
            //_context = new ApplicationDbContext();
            //_attendanceRepository = new AttendanceRepository(_context);
            //_gigRepository = new GigRepository(_context);
            //_followingRepository = new FollowingRepository(_context);
            //_genreRepository = new GenreRepository(_context);
            //_unitOfWork = new UnitOfWork(new ApplicationDbContext());
            _unitOfWork = unitOfWork;

        }
        [Authorize]
        public ActionResult Mine()
        {
            var userId = User.Identity.GetUserId();


            var gigs = _unitOfWork.Gigs.GetUpcomingGigsByArtist(userId);


            //var gigs = _context.Gigs
            //    .Where(g => 
            //    g.ArtistId == userId && 
            //    g.DateTime > DateTime.Now && 
            //    !g.IsCanceled)
            //    .Include(g=>g.Gendre)
            //    .ToList();

            return View(gigs);
        }


        [Authorize]
        public ActionResult Attending()
        {
            var userId = User.Identity.GetUserId();


            var gigs = _unitOfWork.Gigs.GetGigsUserAttending(userId);
            //var gigs = _context.Attendances
            //    .Where(a => a.AttendeeId == userId)
            //    .Select(a => a.Gig)
            //    .Include(g=>g.Artist)
            //    .Include(g=>g.Gendre)
            //    .ToList();

            var attendances = _unitOfWork.Attendance.GetFutureAttendances(userId)
                .ToLookup(a => a.GigId);

            var viewModel = new GigsViewModel
            {
                UpcomingGigs = gigs,
                ShowActions = User.Identity.IsAuthenticated,
                Heading = "Gig I'm Attending",
                Attendances = attendances

            };

            return View("Gigs", viewModel);
        }

       
        [HttpPost]
        public ActionResult Search(GigsViewModel viewModel)
        {

            return RedirectToAction("Index", "Home", new { query = viewModel.SearchTerm });
        }


        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new GigFormViewModel
            {
                Genres = _unitOfWork.Genre.GetGenres(),
                //Genres = _context.Gendres.ToList(),
                Heading = "Add a Gig"
            };

            return View("GigForm",viewModel);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            var userId = User.Identity.GetUserId();


            var gig = _unitOfWork.Gigs.GetGig(id);

            if (gig == null)
                return HttpNotFound();

            if (gig.ArtistId != userId)
                return new HttpUnauthorizedResult();

            //var gig = _context.Gigs.Single(g => g.Id == id && g.ArtistId==userId);



            var viewModel = new GigFormViewModel
            {
                Id=gig.Id,
                Genres = _unitOfWork.Genre.GetGenres(),
                Date = gig.DateTime.ToString("d MMM yyyy"),
                Time=gig.DateTime.ToString("HH:mm"),
                Genre=gig.GendreId,
                Venue=gig.Venue,
                Heading="Edit a Gig"
            
            };

            return View("GigForm",viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GigFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                viewModel.Genres = _unitOfWork.Genre.GetGenres();
                return View("GigForm", viewModel);
            }

            var gig = new Gig
            {
                ArtistId = User.Identity.GetUserId(),
                DateTime = viewModel.GetDateTime(),
                GendreId = viewModel.Genre,
                Venue = viewModel.Venue
            };

            _unitOfWork.Gigs.Add(gig);
            _unitOfWork.Complete();

            //_context.Gigs.Add(gig);
            //_context.SaveChanges();


            return RedirectToAction("Mine", "Gigs");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(GigFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {

                viewModel.Genres = _unitOfWork.Genre.GetGenres();
                return View("GigForm", viewModel);
            }

            var userId = User.Identity.GetUserId();



            var gig = _unitOfWork.Gigs.GetGigWithAttendees(viewModel.Id);

            if (gig == null)
                return HttpNotFound();

            if (gig.ArtistId != userId)
                return new HttpUnauthorizedResult();
            
            //var gig = _context.Gigs
            //    .Include(g => g.Attendances.Select(a => a.Attendee))
            //    .Single(g => g.Id == viewModel.Id && g.ArtistId == userId);

            gig.Modify(viewModel.GetDateTime(), viewModel.Venue, viewModel.Genre);

            //var gig = _context.Gigs.Single(g => g.Id == viewModel.Id && g.ArtistId == userId);
            //gig.Venue = viewModel.Venue;
            //gig.DateTime = viewModel.GetDateTime();
            //gig.GendreId = viewModel.Genre;



            _unitOfWork.Complete();
            //_context.SaveChanges();


            return RedirectToAction("Mine", "Gigs");
        }

        public ActionResult Details(int id)
        {

            var gig = _unitOfWork.Gigs.GetGig(id);

            //var gig = _context.Gigs
            //    .Include(g => g.Artist)
            //    .Include(g => g.Gendre)
            //    .SingleOrDefault(g => g.Id == id);

            if (gig == null)
                return HttpNotFound();

            var viewModel = new GigDetailsViewModel { Gig = gig };

            if(User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();


                viewModel.IsAttending = _unitOfWork.Attendance
                    .GetAttendance(gig.Id, userId) != null;

                //viewModel.IsAttending = _context.Attendances
                //    .Any(a => a.GigId == gig.Id && a.AttendeeId == userId);

                viewModel.IsFollowing = _unitOfWork.Following
                    .GetFollowing(userId, gig.ArtistId) != null;


                //viewModel.IsFollowing = _context.Followings
                //    .Any(a => a.FolloweeId == gig.ArtistId && a.FollowerId == userId);



            }


            return View("Details", viewModel);
        }

      

        


    }
}