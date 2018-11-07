using System.Linq;

using Microsoft.EntityFrameworkCore;

using VERSUS.Infrastructure.Models;

namespace VERSUS.Infrastructure.Services
{
    public class ReviewService : IReviewService
    {
        private readonly SiteDbContext _dbContext;

        public ReviewService(SiteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Review AddReview(string content)
        {
            var review = _dbContext.Reviews.Add(new Review
            {
                Content = content
            }).Entity;

            _dbContext.SaveChanges();

            return review;
        }

        public IQueryable<Review> GetReviews()
        {
            return _dbContext.Reviews
                    .AsNoTracking();
        }
    }
}