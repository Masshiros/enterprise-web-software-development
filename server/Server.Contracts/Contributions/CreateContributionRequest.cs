﻿using Microsoft.AspNetCore.Http;

namespace Server.Contracts.Contributions
{
    public class CreateContributionRequest
    {
        public required string Title { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public List<IFormFile>? Files { get; set; }
        public Guid AcademicYearId { get; set; }

        // will remove late - just for test
        public Guid FacultyId { get; set; }
        public bool IsConfirmed { get; set; }

    }
}