using System.Linq;

using VERSUS.Infrastructure.Models;

namespace VERSUS.Infrastructure.Services
{
    public interface IReviewService : IDataService<Review>
    {
        IQueryable<Review> GetReviews();

        Review AddReview(string content);
    }
}