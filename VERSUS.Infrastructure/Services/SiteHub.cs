﻿using Microsoft.AspNetCore.SignalR;

namespace VERSUS.Infrastructure.Services
{
    public class SiteHub : Hub
    {
        private readonly IReviewService _reviewService;

        public SiteHub(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        //public async Task SendMessage(string message)
        //{
        //    await Clients.All.SendAsync("displaymessage", message);
        //}

        public string ReturnSomething(SiteViewModel siteViewModel)
        {
            return "test string" + _reviewService.AddReview(siteViewModel.Announcement).Content;
        }

        public class SiteViewModel
        {
            public string Announcement { get; set; }
        }
    }
}