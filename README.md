# SkillBridge

SkillBridge is an ASP.NET Core 8 Web API that lets users register/login, upload resumes as PDFs, and manage a skills profile. Resumes are parsed using the Groq LLM API when available, with a deterministic, text‑only fallback parser that works even without any AI key configured.

## Features

- **User authentication**
  - Register with name, email, optional profile info, and password.
  - Passwords hashed with SHA256 before storage.
  - Login returns a JWT access token.
  - JWT‑based authorization via `Bearer {token}` header.

- **User profile & skills**
  - One‑to‑one `User` ↔ `UserProfile` relationship.
  - Profile stores the user’s skills as a JSON array string.
  - Endpoints to create, read, update, and delete the profile for the current user.

- **Resume upload & parsing**
  - Accepts PDF files up to 5 MB.
  - Validates content type/extension and size.
  - Extracts raw text from PDFs using **UglyToad.PdfPig**.
  - Sends extracted text to Groq’s LLM API when an API key is configured.
  - Falls back to a deterministic parser when Groq API is not configured or fails.

- **Deterministic AI fallback parser** (in `Services/AI/GroqService.cs`)
  - Works purely on the extracted resume text, with **no hallucinated data**.
  - Normalizes compressed text (e.g., `IndianInstitute` → `Indian Institute`) and cleans special characters/extra whitespace.
  - Extracts structured data into `ResumeParsedDto`:
    - **Skills**: universal scan across SKILLS, PROJECTS, and EXPERIENCE sections.
      - Uses a whitelist of 60+ known technologies (languages, frameworks, tools, clouds, DBs, etc.).
      - Recognizes techs from comma/pipe/plus separated lists and patterns like `Built using ...`, `Technologies: ...`, `Stack: ...`.
      - Normalizes variations (e.g., `ReactJS` → `React`, `NodeJS` → `Node.js`, `Dotnet` → `.NET`, `MSSQL` → `SQL Server`, `Github` → `GitHub`).
      - Filters out non‑tech words (`Developer`, `Type`, `Script`, soft skills, etc.).
    - **Experience**: STRICT rules.
      - Only taken from explicit `WORK EXPERIENCE` or `EXPERIENCE` sections.
      - Requires lines that look like `Role - Company` or `Role at Company` and contain a recognizable job role word (Developer, Engineer, Manager, etc.).
      - Projects, coursework, and generic bullets are **not** turned into experience.
      - If nothing valid is found, returns `"No experience available"`.
    - **Education**:
      - Only from an `EDUCATION` section.
      - Detects degrees (B.Tech, BE, BSc, MSc, MCA, MBA, Bachelor, Master, Diploma, PhD, etc.).
      - Strips out years (e.g., 2022), ranges, and CGPA/GPA patterns.
      - Cleans coursework/metadata and formats entries as `Degree ... - Institute` when possible.
      - Handles compressed institution names by re‑inserting spaces during normalization.
    - **Projects**:
      - Extracted from `PROJECTS` section.
      - Focuses on project names (short phrases, stripping numbers and long descriptions).
      - Handles names separated by pipes (`|`) or bullet lists.
    - **Certifications**:
      - Only from `CERTIFICATIONS` section.
      - Ignores rankings from competitive programming platforms.
      - If none are found, returns `"No certifications available"`.
  - If the resume text is empty, the fallback returns empty lists with the default experience/certifications messages.

- **API documentation**
  - Swagger/OpenAPI configured with Bearer JWT support.
  - Swagger UI available at `/swagger` when running in Development.

## Project Structure

Key folders and files:

- `Program.cs`
  - Configures services, middleware, Swagger, authentication, and routing.
  - Adds `AppDbContext` with SQL Server using `ConnectionStrings:DefaultConnection`.
  - Registers services:
    - `IAuthService` → `AuthService`
    - `IResumeService` → `ResumeService`
    - `IGroqService` as a typed `HttpClient`
  - Configures JWT bearer auth using values from `Jwt` configuration.

- `SkillBridge.csproj`
  - Targets `.NET 8.0` with nullable reference types and implicit usings.
  - Package references:
    - `Microsoft.AspNetCore.Authentication.JwtBearer`
    - `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design`, `Microsoft.EntityFrameworkCore.Tools`
    - `Swashbuckle.AspNetCore` (Swagger)
    - `System.IdentityModel.Tokens.Jwt`
    - `UglyToad.PdfPig` for PDF text extraction
    - `Google.Apis` (included for potential future integrations).

- `Data/AppDbContext.cs`
  - EF Core `DbContext` with `DbSet<User>` and `DbSet<UserProfile>`.
  - Configures a one‑to‑one relationship: each `User` has one `UserProfile` with cascade delete.

- `Models/User.cs`
  - User entity with fields: `Id`, `Name`, `Email`, `DateOfBirth`, `Gender`, `Contact`, `PasswordHash`, `CreatedAt` and navigation property `UserProfile`.

- `Models/UserProfile.cs`
  - User profile entity with `Id`, `UserId`, `Skills` (JSON string), `UpdatedAt`, and navigation back to `User`.

- `DTOs/Auth`
  - `RegisterDto`: required `Name`, `Email`, `Password`, `ConfirmPassword`, plus optional profile info. Validation attributes ensure proper email format and matching passwords.
  - `LoginDto`: required `Email` and `Password` with basic validation.

- `DTOs/Profile/UpdateSkillsDto.cs`
  - Represents a list of skills (`List<string> Skills`) with validation and `JsonPropertyName("skills")` for JSON binding.

- `DTOs/Resume`
  - `ResumeParsedDto`: lists for `skills`, `experience`, `education`, `projects`, `certifications`.
  - `ResumeExtractionResultDto`: wraps `ExtractedText` and `StructuredData` (a `ResumeParsedDto`).

- `Services/Auth/AuthService.cs`
  - Handles registration and login.
  - On register: checks for existing email, validates password match, hashes password with SHA256, saves user.
  - On login: looks up by email, compares hashed password, and on success issues a JWT.
  - JWT claims include `NameIdentifier`, `Email`, and `Name`.
  - Reads signing key, issuer, audience, and expiry from the `Jwt` configuration section.

- `Services/Profile/UserProfileService.cs`
  - Uses `AppDbContext` to manage `UserProfile` records.
  - Serializes/deserializes skills as JSON string.
  - Methods: `GetProfileAsync`, `CreateProfileAsync`, `UpdateProfileAsync`, `DeleteProfileAsync`.

- `Services/Resume/ResumeService.cs`
  - Validates uploaded files (null, empty, size limit, PDF type/extension).
  - Uses `PdfDocument.Open` (PdfPig) to read all pages and concatenate text.
  - Cleans extracted text by collapsing multiple newlines/spaces and trimming lines.
  - Uses `IGroqService.ExtractResumeDataAsync` to parse structured data.
  - Public methods:
    - `ExtractTextFromPdfAsync(IFormFile)`
    - `ExtractAndParseResumeAsync(IFormFile)`
    - `ProcessResumeAsync(IFormFile)` (returns a combined result: message, file name, text length, raw text, and structured data).

- `Services/AI/GroqService.cs`
  - If `Groq:ApiKey` is set and not a placeholder, calls Groq’s `/openai/v1/chat/completions` endpoint with `llama-3.1-8b-instant` and a strict JSON‑only prompt.
  - Cleans the model response by removing Markdown code fences and extracting the JSON object between the first `{` and last `}`.
  - Deserializes into `ResumeParsedDto`.
  - On any failure, falls back to the deterministic parser (`ExtractFromResumeText`).

- `Controllers/AuthController.cs`
  - `[Route("api/[controller]")]`, `[Authorize]` at controller level; individual actions for register/login are `[AllowAnonymous]`.
  - `POST api/Auth/register` → `RegisterDto` → uses `IAuthService.RegisterAsync` and returns appropriate HTTP codes for conflicts/validation.
  - `POST api/Auth/login` → `LoginDto` → uses `IAuthService.LoginAsync` and returns `{ token }` on success.

- `Controllers/ProfileController.cs`
  - `[Route("api/[controller]")]`, `[Authorize]`.
  - Extracts current user ID from JWT `NameIdentifier` claim.
  - `GET api/Profile` → returns current user’s profile or 404.
  - `POST api/Profile` → creates a profile with skills for the current user; conflicts if one already exists.
  - `PUT api/Profile` → updates skills.
  - `DELETE api/Profile` → deletes the profile.

- `Controllers/ResumeController.cs`
  - `[Route("api/[controller]")]`, `[Authorize]`.
  - `POST api/Resume/upload` (multipart form upload):
    - Validates file presence.
    - Calls `IResumeService.ProcessResumeAsync`.
    - Returns detailed error messages for invalid files and 500 on unexpected exceptions.

- `Migrations/`
  - Contains EF Core migrations (e.g., `20260319104641_first`) and `AppDbContextModelSnapshot` to define and evolve the database schema.

- `appsettings.json`
  - Connection string under `ConnectionStrings:DefaultConnection` for SQL Server.
  - Logging configuration.
  - `Jwt` configuration: `Key`, `Issuer`, `Audience`, `ExpiryMinutes` (placeholder key to be overridden for real deployments).
  - `Groq:ApiKey` placeholder (use a real key via a secure local config or environment variable).

- `appsettings.Development.json`
  - Development logging overrides.

- `appsettings.local.json` (not in git, but loaded in `Program.cs` if present)
  - Intended for **local secrets** only (real DB connection string, JWT key, Groq API key, etc.).
  - Keep this file out of source control.

- `Properties/launchSettings.json`
  - Configures HTTP/HTTPS profiles and IIS Express, with `ASPNETCORE_ENVIRONMENT=Development` and Swagger as the launch URL.

## Running the Project

### Prerequisites

- .NET 8 SDK
- SQL Server (local or remote) reachable by the configured `DefaultConnection`.

### Setup

1. **Clone the repository**

   ```bash
   git clone https://github.com/NikhilYadavCoder/SkillBridge.git
   cd SkillBridge
   ```

2. **Configure connection string and secrets**

   - Option 1: Edit `appsettings.json` (for local only) with your SQL Server connection string and JWT key.
   - Option 2 (recommended): Create `appsettings.local.json` (ignored by git) and override:

     ```json
     {
       "ConnectionStrings": {
         "DefaultConnection": "<your-connection-string>"
       },
       "Jwt": {
         "Key": "<strong-secret-key>",
         "Issuer": "SkillBridgeAPI",
         "Audience": "SkillBridgeUsers",
         "ExpiryMinutes": 60
       },
       "Groq": {
         "ApiKey": "<your-groq-api-key>"
       }
     }
     ```

3. **Apply migrations and create the database**

   From the project root:

   ```bash
   dotnet ef database update
   ```

   (Requires the EF Core CLI tools; install with `dotnet tool install --global dotnet-ef` if needed.)

4. **Run the API**

   ```bash
   dotnet run
   ```

   By default, the app will start on the URLs from `launchSettings.json` (e.g. `http://localhost:5106`), and Swagger UI will be at `http://localhost:5106/swagger` in Development.

## API Overview

- **Auth**
  - `POST api/Auth/register`
    - Body: `RegisterDto` (JSON) with `name`, `email`, `password`, `confirmPassword`, optional `dateOfBirth`, `gender`, `contact`.
  - `POST api/Auth/login`
    - Body: `LoginDto` (JSON) with `email`, `password`.
    - Response: `{ "token": "<jwt-token>" }`.

- **Profile** (requires `Authorization: Bearer {token}`)
  - `GET api/Profile` — get current user profile.
  - `POST api/Profile` — create skills profile for current user.
  - `PUT api/Profile` — update skills list.
  - `DELETE api/Profile` — delete profile.

- **Resume** (requires `Authorization: Bearer {token}`)
  - `POST api/Resume/upload`
    - Content type: `multipart/form-data` with a `file` field (PDF).
    - Response includes:
      - `message`
      - `fileName`
      - `textLength`
      - `extractedText`
      - `structuredData` (`ResumeParsedDto` from Groq or fallback parser).

## Notes & Future Improvements

- For production, replace SHA256 password hashing with a stronger algorithm (e.g., ASP.NET Identity with PBKDF2/BCrypt/Argon2).
- Move secrets entirely to environment variables or a secret manager (Azure Key Vault, etc.).
- Expand tests around the deterministic parser to cover more resume formats.
- The `Google.Apis` package is currently unused and can be leveraged or removed depending on future needs.
