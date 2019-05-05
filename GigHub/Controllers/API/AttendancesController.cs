using GigHub.Core.Dtos;
using GigHub.Core.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using GigHub.Persistence;

namespace GigHub.Controllers.API
{
    




    [Authorize]
    public class AttendancesController : ApiController
    {
        private ApplicationDbContext _context;

        public AttendancesController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpPost]      
        public IHttpActionResult Attend(AttendanceDto dto)
        {

            var userId = User.Identity.GetUserId();
          

            if (_context.Attendances.Any(a => a.AttendeeId == userId && a.GigId == dto.GigID))
                return BadRequest("The attendance already exists.");

            var attendance = new Attendance
            {
                GigId = dto.GigID,
                AttendeeId = userId
            };
            _context.Attendances.Add(attendance);
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete]
        public IHttpActionResult DeleteAttedance(int id)
        {
            var userId = User.Identity.GetUserId();

            var attendance = _context.Attendances
                .SingleOrDefault(a => a.AttendeeId == userId && a.GigId == id);

            if(attendance==null)
            {
                return NotFound();

            }

            _context.Attendances.Remove(attendance);
            _context.SaveChanges();

            return Ok(id);

        }

    }
}
