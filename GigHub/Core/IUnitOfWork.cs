using GigHub.Core.Repositories;
using GigHub.Persistence.Repositories;

namespace GigHub.Core
{
    public interface IUnitOfWork
    {
        IAttendanceRepository Attendance { get; }
        IFollowingRepository Following { get; }
        IGenreRepository Genre { get; }
        IGigRepository Gigs { get; }

        void Complete();
    }
}