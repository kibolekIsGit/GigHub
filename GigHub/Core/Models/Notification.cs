using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GigHub.Core.Models
{
    public class Notification
    {
        public int Id { get;private set; }
        public DateTime DateTime { get; private set; }
        public NotificationType Type { get; private set; }
        public DateTime? OriginalDateTime { get; private set; }
        public string OriginalVenue { get; private set; }

        [Required]
        public Gig Gig { get; private set; }
        //mozna tez tak
        //[Required]
        //public Gig Gig { get; set; }
        //public int GigId { get; set; }



        //dodalismy przy refaktoringu metody cancel
        protected Notification()
        {

        }
        //dodalismy przy refaktoringu metody cancel
        private Notification(NotificationType type, Gig gig)
        {
            if (gig == null)
                throw new ArgumentNullException("gig");

            Type = type;
            Gig = gig;
            DateTime = DateTime.Now;
        }

        public static Notification GigCreated(Gig gig)
        {
            return new Notification(NotificationType.GigCreated, gig);

        }
        public static Notification GigUpdated(Gig newGig, DateTime orginalDateTime,string orginalVenue)
        {

            var notification = new Notification(NotificationType.GigUpdated, newGig);
            notification.OriginalDateTime = orginalDateTime;
            notification.OriginalVenue = orginalVenue;

            return notification;

        }

        public static Notification GigCanceled(Gig gig)
        {
            return new Notification(NotificationType.GigCanceled, gig);

        }

    }
}