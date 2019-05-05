using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GigHub.Core.Models
{
    public class Gig
    {
        public int Id { get; set; }

        public bool IsCanceled { get; private set; }

        public ApplicationUser Artist { get; set; }

        //[Required]
        public string ArtistId { get; set; }

        public DateTime DateTime { get; set; }

        //[Required]
        //[StringLength(255)]
        public string Venue { get; set; }

       
        public Gendre Gendre { get; set; }

        //[Required]
        public byte GendreId { get; set; }


        //dodalismy przy refaktoringu metody cancel
        public ICollection<Attendance> Attendances { get; private set; }


        //dodalismy przy refaktoringu metody cancel
        public Gig()
        {
            Attendances = new Collection<Attendance>();
        }

        public void Cancel()
        {
            IsCanceled = true;

            // var notification = new Notification(NotificationType.GigCanceled, this);
            var notification = Notification.GigCanceled(this);

            foreach (var attendee in Attendances.Select(a => a.Attendee))
            {
                
                attendee.Notify(notification);
            }
        }

      

       public void Modify(DateTime dateTime, string venue, byte genre)
        {
            var notification = Notification.GigUpdated(this,DateTime,Venue);
            //notification.OriginalDateTime = DateTime;
            //notification.OriginalVenue = Venue;

            Venue = venue;
            DateTime = dateTime;
            GendreId = genre;


            foreach (var attende in Attendances.Select(a => a.Attendee))
                attende.Notify(notification);

        }
    }

  
}