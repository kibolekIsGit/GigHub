using GigHub.Core.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using GigHub.Persistence;

namespace GigHub.Controllers.API
{
    [Authorize]
    public class GigsController : ApiController
    {
        public ApplicationDbContext _context;

        public GigsController()
        {
            _context = new ApplicationDbContext();   
        }

        [HttpDelete]
        public IHttpActionResult Cancel(int id)
        {
            var userId = User.Identity.GetUserId();

      

            var gig = _context.Gigs
                .Include(g=>g.Attendances.Select(a=>a.Attendee))
                .Single(g => g.Id == id && g.ArtistId == userId);


            //var gig = _context.Gigs.Single(g => g.Id==id && g.ArtistId == userId);

            if (gig.IsCanceled)
                return NotFound();

            gig.Cancel();


            //gig.IsCanceled = true;

            //var notification = new Notification(NotificationType.GigCanceled, gig);
            ////var notification = new Notification
            ////{
            ////    DateTime = DateTime.Now,
            ////    Gig = gig,
            ////    Type = NotificationType.GigCanceled

            ////};

            ////var attendees = _context.Attendances
            ////    .Where(a => a.GigId == gig.Id)
            ////    .Select(a => a.Attendee)
            ////    .ToList();

            //foreach (var attendee in gig.Attendances.Select(a=>a.Attendee))
            ////foreach( var attendee in attendees)
            //{
            //    attendee.Notify(notification);

                
            //}



            _context.SaveChanges();

            return Ok();


        }


    }
}
