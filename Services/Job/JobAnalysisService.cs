using System.Text.Json;
using SkillBridge.DTOs.Job;
using SkillBridge.DTOs.Resume;

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

            var normalizedUserSkills = NormalizeUserSkills(userSkills);
            var filteredJobs = await LoadJobsForRoleAsync(role);

            return BuildAnalysisResult(filteredJobs, normalizedUserSkills, role, pageNumber, pageSize);
        }

        public async Task<JobAnalysisResponseDto> AnalyzeFromResumeAsync(
            ResumeParsedDto resumeData,
            string resumeText,
            string role,
            int pageNumber,
            int pageSize)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Role is required", nameof(role));
            }

            resumeData ??= new ResumeParsedDto();

            // STEP 1: load jobs for role
            var filteredJobs = await LoadJobsForRoleAsync(role);

            // STEP 2: master job skill set
            var masterJobSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var job in filteredJobs)
            {
                if (job.skills == null) continue;
                foreach (var skill in job.skills)
                {
                    if (!string.IsNullOrWhiteSpace(skill))
                    {
                        masterJobSkills.Add(skill.Trim());
                    }
                }
            }

            // STEP 3: extract user skills from parsed skills + resume text scan
            var normalizedUserSkills = NormalizeUserSkills(resumeData.skills);

            if (!string.IsNullOrWhiteSpace(resumeText) && masterJobSkills.Count > 0)
            {
                var text = resumeText;
                foreach (var skill in masterJobSkills)
                {
                    if (string.IsNullOrWhiteSpace(skill)) continue;

                    if (text.IndexOf(skill, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        normalizedUserSkills.Add(skill.Trim().ToLowerInvariant());
                    }
                }
            }

            // STEP 4–7: reuse core matching logic
            return BuildAnalysisResult(filteredJobs, normalizedUserSkills, role, pageNumber, pageSize);
        }

        private static HashSet<string> NormalizeUserSkills(List<string>? userSkills)
        {
            var normalizedUserSkills = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (userSkills == null)
            {
                return normalizedUserSkills;
            }

            foreach (var skill in userSkills)
            {
                if (!string.IsNullOrWhiteSpace(skill))
                {
                    normalizedUserSkills.Add(skill.Trim().ToLowerInvariant());
                }
            }

            return normalizedUserSkills;
        }

        private async Task<List<JobItem>> LoadJobsForRoleAsync(string role)
        {
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

            return jobsConfig.jobs
                .Where(j => !string.IsNullOrWhiteSpace(j.role) &&
                            j.role.Equals(role, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        private JobAnalysisResponseDto BuildAnalysisResult(
            List<JobItem> filteredJobs,
            HashSet<string> normalizedUserSkills,
            string role,
            int pageNumber,
            int pageSize)
        {
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
