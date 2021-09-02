using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AssignmentServer.BlazorApp.Controllers
{
    public class ContentController : Controller
    {
        public IActionResult VideoReview(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return StatusCode(404);

            var videoPath = $"Cabinet/Assignments/{Id}/review.mp4";

            if (!System.IO.File.Exists(videoPath))
                return StatusCode(404);

            var stream = System.IO.File.OpenRead(videoPath);
            return File(stream, "video/mp4");
        }
    }
}
