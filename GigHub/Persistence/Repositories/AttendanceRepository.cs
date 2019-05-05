﻿using GigHub.Core.Models;
using GigHub.Core.Repositories;
using GigHub.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GigHub.Persistence.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Attendance> GetFutureAttendances(string userId)
        {
            return _context.Attendances
                            .Where(a => a.AttendeeId == userId && a.Gig.DateTime > DateTime.Now)
                            .ToList();
        }

        public Attendance GetAttendance(int gigId, string userId)
        {
            return _context.Attendances
                    .SingleOrDefault(a => a.GigId == gigId && a.AttendeeId == userId);
        }
    }
}