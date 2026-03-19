using System.Text.Json;
using SkillBridge.DTOs.Job;

namespace SkillBridge.Services.Job
{
    public class JobAnalysisService : IJobAnalysisService
    {
        private const string JobsFileRelativePath = "Data/jobs.json";

        public async Task<JobAnalysisResponseDto> AnalyzeAsync(
            List<string> userSkills,
            string role,
            int pageNumber,
            int pageSize)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Role is required", nameof(role));
            }

            // Normalize user skills (lowercase + trim) and remove duplicates
            var normalizedUserSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (userSkills != null)
            {
                foreach (var skill in userSkills)
                {
                    if (!string.IsNullOrWhiteSpace(skill))
                    {
                        normalizedUserSkills.Add(skill.Trim().ToLowerInvariant());
                    }
                }
            }

            // Read and deserialize jobs.json
            var jobsPath = Path.Combine(AppContext.BaseDirectory, JobsFileRelativePath);
            if (!File.Exists(jobsPath))
            {
                // Fallback to current directory if running from project root
                jobsPath = Path.Combine(Directory.GetCurrentDirectory(), JobsFileRelativePath);
            }

            if (!File.Exists(jobsPath))
            {
                throw new FileNotFoundException($"Jobs configuration file not found at '{JobsFileRelativePath}'", jobsPath);
            }

            var json = await File.ReadAllTextAsync(jobsPath);
            var jobsConfig = JsonSerializer.Deserialize<JobsConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new JobsConfig();

            var filteredJobs = jobsConfig.jobs
                .Where(j => !string.IsNullOrWhiteSpace(j.role) &&
                            j.role.Equals(role, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var allVariantResults = new List<JobVariantResultDto>();
            var aggregatedMissingCount = new Dictionary<string, (int Count, string Canonical)>(StringComparer.OrdinalIgnoreCase);

            foreach (var job in filteredJobs)
            {
                var jobSkills = job.skills ?? new List<string>();

                var matched = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var missing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var jobSkill in jobSkills)
                {
                    if (string.IsNullOrWhiteSpace(jobSkill))
                    {
                        continue;
                    }

                    var trimmedSkill = jobSkill.Trim();
                    var normalizedSkill = trimmedSkill.ToLowerInvariant();

                    if (normalizedUserSkills.Contains(normalizedSkill))
                    {
                        matched.Add(trimmedSkill);
                    }
                    else
                    {
                        missing.Add(trimmedSkill);

                        if (aggregatedMissingCount.TryGetValue(normalizedSkill, out var existing))
                        {
                            aggregatedMissingCount[normalizedSkill] = (existing.Count + 1, existing.Canonical);
                        }
                        else
                        {
                            aggregatedMissingCount[normalizedSkill] = (1, trimmedSkill);
                        }
                    }
                }

                double matchPercentage = 0;
                if (jobSkills.Count > 0)
                {
                    matchPercentage = (double)matched.Count / jobSkills.Count * 100.0;
                }

                matchPercentage = Math.Round(matchPercentage, 2);

                var variantResult = new JobVariantResultDto
                {
                    Variant = job.variant ?? string.Empty,
                    MatchedSkills = matched.ToList(),
                    MissingSkills = missing.ToList(),
                    MatchPercentage = matchPercentage
                };

                allVariantResults.Add(variantResult);
            }

            // Order aggregated missing skills by frequency (descending) then name
            var aggregatedMissingSkills = aggregatedMissingCount
                .OrderByDescending(kvp => kvp.Value.Count)
                .ThenBy(kvp => kvp.Value.Canonical, StringComparer.OrdinalIgnoreCase)
                .Select(kvp => kvp.Value.Canonical)
                .ToList();

            // Pagination
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 5;
            }

            var skip = (pageNumber - 1) * pageSize;
            var pagedResults = allVariantResults
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            return new JobAnalysisResponseDto
            {
                Role = role,
                Results = pagedResults,
                AggregatedMissingSkills = aggregatedMissingSkills
            };
        }

        private class JobsConfig
        {
            public List<JobItem> jobs { get; set; } = new();
        }

        private class JobItem
        {
            public string role { get; set; } = string.Empty;
            public string variant { get; set; } = string.Empty;
            public List<string> skills { get; set; } = new();
        }
    }
}
