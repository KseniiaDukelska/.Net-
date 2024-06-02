//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Rocky_DataAccess; // Replace with your actual namespace
//using Rocky_Models.Models;


//namespace Rocky.Services

//{
//    public class DataExtractionService
//    {
//        private readonly ApplicationDbContext _context;

//        public DataExtractionService(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public List<UserInteraction> GetUserInteractions()
//        {
//            var likes = _context.Likes
//                .Select(l => new UserInteraction
//                {
//                    UserId = l.ApplicationUserId,
//                    EvId = l.ProductId,
//                    Rating = 3 // Neutral rating for likes
//                }).ToList();

//            var purchases = _context.OrderDetails
//                .Include(od => od.OrderHeader)  // Ensure we include user data
//                .Select(od => new UserInteraction
//                {
//                    UserId = od.OrderHeader.CreatedBy.UserId, // Assuming you have a way to get user from OrderHeader
//                    EventId = od.ProductId,
//                    Rating = 5 // High rating for purchases
//                }).ToList();

//            return likes.Concat(purchases).ToList();
//        }

//        public List<Event> GetEvents()
//        {
//            return _context.Products
//                .Select(p => new Event
//                {
//                    EventId = p.Id,
//                    EventDetails = p.Name,
//                    Price = p.Price,
//                    Image = p.Image,
//                    Place = p.Place,
//                    StartTime = p.StartTime
//                }).ToList();
//        }
//    }
//}
