


# SkillBridge - Complete User & Developer Guide

**VIDEO DEMO LINK** - https://drive.google.com/file/d/1IreoWJMpkwNZQck4-Km9WlC_vkZ0wOiF/view?usp=sharing

## 🎯 What is SkillBridge? (Simple Explanation)

**SkillBridge is like a personal career coach that lives on the internet.**

Imagine you want to become a React Developer, but you're currently a C# developer. SkillBridge helps by:

1. **Reading Your Resume** - Upload your resume (PDF), and the AI reads it to understand your current skills
2. **Finding Gaps** - It compares your skills with what's needed for your dream job
3. **Creating a Learning Plan** - It shows you exactly what to learn and in what order
4. **Practicing for Interviews** - It generates mock interview questions for your target role

**In short:** Resume → Your Skills → Dream Job → Learning Path → Interview Prep

---

## 📚 Table of Contents

- [Quick Start (5 minutes)](#-quick-start-5-minutes)
- [What Problem Does It Solve?](#-what-problem-does-it-solve)
- [Key Features Explained](#-key-features-explained)
- [Getting Started (Installation)](#-getting-started-installation)
- [Running the Application](#-running-the-application)
- [Project Structure & What Each Folder Does](#-project-structure--what-each-folder-does)
- [How to Use the Application](#-how-to-use-the-application)
- [API Reference (For Developers)](#-api-reference-for-developers)
- [Deploying to Production](#-deploying-to-production)
- [Troubleshooting & Common Issues](#-troubleshooting--common-issues)
- [Glossary: Technical Terms Made Simple](#-glossary-technical-terms-made-simple)

---

## ⚡ Quick Start (5 minutes)

### Prerequisites Checklist
- [ ] Windows 10/11, Mac, or Linux
- [ ] .NET 8.0 SDK installed ([Download here](https://dotnet.microsoft.com/download/dotnet/8.0))
- [ ] Node.js 18+ installed ([Download here](https://nodejs.org/))
- [ ] PostgreSQL installed ([Download here](https://www.postgresql.org/download/))
- [ ] Git installed ([Download here](https://git-scm.com/))

### Steps to Run Locally

```bash
# 1. Clone the project
git clone <your-repo-url>
cd SKILLBRIDGE-PROJECT

# 2. Set up the database
# Start PostgreSQL and create a database called SkillBridgeDb

# 3. Install backend dependencies and run migrations
cd SkillBridge
dotnet ef database update
dotnet run
# Backend now running at https://localhost:7275

# 4. In a new terminal, install and run frontend
cd ../Frontend
npm install
npm run dev
# Frontend now running at http://localhost:3000

# 5. Open browser and go to http://localhost:3000
```

**Done!** You now have SkillBridge running locally.

---

## 🤔 What Problem Does It Solve?

### The Challenge
Imagine you're a developer who wants to switch careers:
- You know C# and .NET
- You want to become a React Developer
- But you don't know what React skills you need
- You don't know the best learning order
- You're worried you might waste time learning the wrong things

### The Solution (SkillBridge)
SkillBridge answers all these questions:
1. ✅ **Current Skills**: Upload your resume → SkillBridge reads it
2. ✅ **Gap Analysis**: Shows exactly what skills are missing for React Developer role
3. ✅ **Learning Path**: Creates a step-by-step plan (Week 1: JS Basics, Week 2: React Components, etc.)
4. ✅ **Interview Ready**: Generates practice interview questions specific to React role

**Result**: Instead of guessing, you have a clear roadmap with 85% confidence you're learning the right things.

---

## ✨ Key Features Explained

### 1. **Secure User Registration & Login**

**What it does:**
- Create an account with email and password
- Your password is encrypted (secured with SHA256)
- Login generates a security token (JWT)
- Only you can access your profile

**Behind the scenes:**
```
Your Password → SHA256 Encryption → Locked Safe
When you login → Password → Encryption → Matches? → Generate Token → Access Granted
```

### 2. **Resume Upload & Parsing**

**What it does:**
- Upload your PDF resume
- AI reads the resume automatically
- Extracts: Skills, Experience, Education, Projects
- Saves to your profile

**How it works:**
```
Step 1: Upload PDF
   ↓
Step 2: Extract Text from PDF (UglyToad.PdfPig)
   ↓
Step 3: Send to Groq AI for parsing (with fallback if AI unavailable)
   ↓
Step 4: Structured Data (Skills list, Experience timeline)
   ↓
Step 5: Saved to Your Profile
```

**What if AI is offline?**
- Don't worry! SkillBridge has a backup parser
- It uses pattern matching (looks for keywords)
- Works offline, slightly less accurate but still useful

### 3. **Job Role Discovery**

**What it does:**
- Browse available job roles (Backend, Frontend, Full Stack, etc.)
- See job variants (React Developer, Angular Developer, etc.)
- See required skills for each role

**Example:**
```
Frontend Developer
├─ React Developer (requires: React, JS, TypeScript, CSS, API Integration)
├─ Angular Developer (requires: Angular, TypeScript, RxJS, etc.)
└─ Vue Developer (requires: Vue, JavaScript, etc.)
```

### 4. **Skill Gap Analysis**

**What it does:**
- Compare your skills with target job
- Calculate match percentage
- Show which skills you have
- Show which skills you're missing
- Prioritize missing skills by importance

**Example:**
```
You have: [C#, JavaScript, HTML]
React Developer needs: [React, JavaScript, TypeScript, CSS, Redux]

✅ Match: JavaScript
❌ Missing: React, TypeScript, CSS, Redux

Match Score: 20% (1 out of 5 skills)
Top Priority: Learn React first
```

### 5. **AI-Generated Learning Roadmap**

**What it does:**
- Creates personalized learning path
- Breaks down into phases (each 2-4 weeks)
- Includes study resources
- Realistic time estimates

**Example Roadmap:**
```
PHASE 1: JavaScript Fundamentals (Weeks 1-2)
├─ Topics: Variables, Functions, Async/Await
├─ Resources: FreeCodeCamp course, Codewars challenges
└─ Time: 2-3 weeks

PHASE 2: React Basics (Weeks 3-5)
├─ Topics: Components, Hooks, State Management
├─ Resources: Official React docs, tutorial projects
└─ Time: 3-4 weeks

PHASE 3: Advanced React & TypeScript (Weeks 6-8)
├─ Topics: Redux, Context API, TypeScript integration
├─ Resources: Redux docs, advanced tutorials
└─ Time: 4-5 weeks
```

### 6. **Interview Preparation**

**What it does:**
- Generates practice interview questions
- Questions targeted to your role
- Includes difficulty levels (Easy, Medium, Hard)
- Hints provided for each question

**Example Questions:**
```
1. "What is a React Hook?" (Easy)
   Hint: Think about state management in functional components
   
2. "Explain Redux and why it's useful" (Medium)
   Hint: Think about state management in large apps
   
3. "Design a scalable Redux architecture" (Hard)
   Hint: Consider middleware, selectors, and performance
```

---

## 🔧 Getting Started (Installation)

### Step 1: Prerequisites

#### Windows 10/11

**Install .NET 8.0:**
1. Go to https://dotnet.microsoft.com/download/dotnet/8.0
2. Click "Download .NET 8.0 SDK" (Windows installer)
3. Run installer, follow prompts
4. Verify: Open Command Prompt, type `dotnet --version`

**Install Node.js:**
1. Go to https://nodejs.org/
2. Download LTS version (Long Term Support)
3. Run installer, follow prompts
4. Verify: Open Command Prompt, type `node --version`

**Install PostgreSQL:**
1. Go to https://www.postgresql.org/download/
2. Download Windows installer
3. Run installer, set password for 'postgres' user
4. Note: Default port is 5432
5. After install, open pgAdmin (graphical tool) to manage databases

**Install Git:**
1. Go to https://git-scm.com/
2. Download Windows installer
3. Run installer, accept defaults
4. Verify: Open Command Prompt, type `git --version`

#### Mac

```bash
# Using Homebrew (if not installed: /bin/bash -c "$(curl -fsSL ...)")
brew install dotnet-sdk node postgresql git

# Verify installations
dotnet --version
node --version
postgres --version
git --version
```

#### Linux (Ubuntu/Debian)

```bash
# Install .NET 8.0
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Install Node.js
sudo apt-get update
sudo apt-get install -y nodejs npm

# Install PostgreSQL
sudo apt-get install -y postgresql postgresql-contrib

# Install Git
sudo apt-get install -y git
```

### Step 2: Clone the Repository

```bash
# Navigate to where you want the project
cd ~/projects  # or your preferred location

# Clone the repository
git clone https://github.com/your-username/skillbridge.git
cd skillbridge

# You now have:
# ├─ SkillBridge/ (Backend)
# ├─ Frontend/ (Frontend)
# └─ other files
```

### Step 3: Set Up the Database

**Windows:**

1. Open pgAdmin (installed with PostgreSQL)
2. Right-click "Databases" → Create → Database
3. Name: `SkillBridgeDb`
4. Click Save

**Mac/Linux:**

```bash
# Connect to PostgreSQL
psql -U postgres

# Inside psql prompt, create database
CREATE DATABASE SkillBridgeDb;
\q  # Exit psql
```

### Step 4: Configure Connection String

**File:** `SkillBridge/appsettings.Development.json`

Update the connection string to match your PostgreSQL setup:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=SkillBridgeDb;Username=postgres;Password=your-password;Ssl Mode=Prefer;Trust Server Certificate=true"
  }
}
```

**Note:** Replace `your-password` with the password you set during PostgreSQL installation.

### Step 5: Create Database Tables

```bash
cd SkillBridge
dotnet ef database update
```

This creates the necessary tables (Users, UserProfiles) automatically.

---

## ▶️ Running the Application

### Option 1: Full Development Setup (Recommended for Development)

**Terminal 1 - Backend:**
```bash
cd SkillBridge
dotnet run
# Output: Application started at https://localhost:7275
```

**Terminal 2 - Frontend:**
```bash
cd Frontend
npm run dev
# Output: Local: http://localhost:3000
```

**Terminal 3 - Database:**
- PostgreSQL should be running as a service (automatic on most systems)
- If not running, start it:
  ```bash
  # Windows
  net start postgresql-x64-15  # or your version
  
  # Mac
  brew services start postgresql
  
  # Linux
  sudo systemctl start postgresql
  ```

**Open in Browser:**
- Frontend: http://localhost:3000
- Backend API: https://localhost:7275
- Swagger API Docs: https://localhost:7275/swagger

### Option 2: Quick Production Build

```bash
# Build frontend (produces optimized files)
cd Frontend
npm run build

# Build backend
cd ../SkillBridge
dotnet build -c Release

# Run backend (it serves the frontend too)
dotnet run --configuration Release
```

---

## 📁 Project Structure & What Each Folder Does

### Overview Tree

```
SKILLBRIDGE-PROJECT/
├── SkillBridge/                    ← BACKEND (ASP.NET Core API)
│   ├── Program.cs                  ← Startup configuration
│   ├── Controllers/                ← API endpoints
│   │   ├── AuthController.cs       ← Login/Register
│   │   ├── ProfileController.cs    ← User profile management
│   │   ├── ResumeController.cs     ← Resume upload/parsing
│   │   └── JobController.cs        ← Job analysis, roadmaps, interviews
│   ├── Services/                   ← Business logic
│   │   ├── Auth/                   ← Authentication logic
│   │   ├── Resume/                 ← Resume parsing logic
│   │   ├── Job/                    ← Job analysis logic
│   │   ├── AI/                     ← Groq LLM integration
│   │   └── Profile/                ← Profile management logic
│   ├── Models/                     ← Database entity classes
│   │   ├── User.cs                 ← User data model
│   │   ├── UserProfile.cs          ← Profile data model
│   │   └── AI/                     ← AI-related models
│   ├── DTOs/                       ← Data Transfer Objects (API contracts)
│   │   ├── Auth/                   ← Login/Register DTOs
│   │   ├── Resume/                 ← Resume DTOs
│   │   └── Job/                    ← Job analysis DTOs
│   ├── Data/                       ← Database & data
│   │   ├── AppDbContext.cs         ← EF Core database configuration
│   │   └── jobs.json               ← Job roles database
│   ├── Migrations/                 ← Database version history
│   ├── appsettings.json            ← Configuration (default)
│   ├── appsettings.Development.json ← Configuration (development)
│   ├── Dockerfile                  ← Docker container instructions
│   └── SkillBridge.csproj          ← Project file (dependencies)
│
├── Frontend/                       ← FRONTEND (React SPA)
│   ├── src/
│   │   ├── main.jsx                ← Application entry point
│   │   ├── App.jsx                 ← Main app component
│   │   ├── pages/                  ← Full page components
│   │   │   ├── Landing.jsx         ← Home page
│   │   │   ├── Login.jsx           ← Login page
│   │   │   ├── Register.jsx        ← Registration page
│   │   │   ├── Dashboard.jsx       ← Main dashboard
│   │   │   ├── UploadResume.jsx    ← Resume upload page
│   │   │   ├── Analysis.jsx        ← Results page
│   │   │   └── Profile.jsx         ← Profile management page
│   │   ├── components/             ← Reusable UI components
│   │   │   ├── Navbar.jsx          ← Header navigation
│   │   │   ├── MatchCard.jsx       ← Job match display
│   │   │   ├── RoadmapCard.jsx     ← Learning roadmap display
│   │   │   └── ...more components
│   │   ├── context/                ← Authentication state management
│   │   │   └── AuthContext.jsx     ← Auth state & functions
│   │   ├── api/                    ← API communication
│   │   │   └── axios.js            ← HTTP client configuration
│   │   └── assets/                 ← Images, fonts, etc.
│   ├── dist/                       ← Built/optimized files (production)
│   ├── public/                     ← Static files (favicon, etc.)
│   ├── package.json                ← Dependencies & scripts
│   ├── vite.config.js              ← Build configuration
│   └── index.html                  ← HTML template
│
├── DESIGN_DOCUMENTATION.md         ← Technical design (for developers)
├── README.md                       ← Project overview
└── .gitignore                      ← Files to exclude from git
```

### Detailed Folder Explanations

#### Backend: `SkillBridge/`

**Controllers/** - Where API requests arrive
```
Think of it like: Doors to your business
- AuthController = Doors for login/register
- ProfileController = Doors for profile management
- ResumeController = Doors for resume handling
- JobController = Doors for job analysis
```

**Services/** - Where work actually happens
```
Think of it like: Workers behind the doors
- AuthService = Worker who authenticates users
- ResumeService = Worker who reads PDF files
- GroqService = Worker who talks to AI/Groq
- JobAnalysisService = Worker who compares skills to jobs
```

**Models/** - How data looks
```
Think of it like: Data containers
- User = Container for user info (name, email, password)
- UserProfile = Container for profile info (skills, experience)
```

**DTOs/** - Data format for API
```
Think of it like: Envelopes for sending data
- LoginDto = Envelope containing email + password
- RegisterDto = Envelope containing name + email + password
- ResumeParsedDto = Envelope containing extracted resume info
```

**Data/** - Database stuff
```
- AppDbContext = Instructions for how database works
- jobs.json = List of all job roles and required skills
```

#### Frontend: `Frontend/`

**pages/** - Full screens user sees
```
- Landing.jsx = Home/welcome screen
- Login.jsx = Login screen
- Register.jsx = Sign-up screen
- Dashboard.jsx = Main screen after login
- UploadResume.jsx = Resume upload screen
- Analysis.jsx = Results screen
- Profile.jsx = User profile screen
```

**components/** - Reusable pieces
```
- Navbar.jsx = Top navigation bar (reused on every page)
- MatchCard.jsx = Card showing job match (reused for each job)
- RoadmapCard.jsx = Card showing roadmap phase (reused for each phase)
```

**context/** - Shared state
```
- AuthContext.jsx = Stores login info globally
  Think: "Is user logged in? Who is the user?"
  Used by: All pages/components need to know
```

**api/** - Talks to backend
```
- axios.js = Configured HTTP client
  Handles: Sending requests to backend API
  Adds: Authorization headers, error handling
```

---

##  How to Use the Application

### User Journey: From Sign-Up to Career Change

#### Step 1: Create Account

1. Open http://localhost:3000
2. Click "Sign Up" or "Register"
3. Enter:
   - Full Name: "John Doe"
   - Email: "john@example.com"
   - Password: "SecurePassword123"
   - Confirm Password: "SecurePassword123"
4. Click "Register"
5. Success message appears

**Behind the scenes:**
- Password gets hashed (encrypted) so SkillBridge never knows your actual password
- User record created in database
- Ready to login

#### Step 2: Login

1. Click "Login"
2. Enter email and password
3. Click "Login"
4. Redirect to Dashboard

**Behind the scenes:**
- Backend verifies email + password match
- Creates JWT token (security pass good for 60 minutes)
- Frontend stores token
- All future requests include token

#### Step 3: Upload Resume

1. Click "Upload Resume" in navigation
2. Select your PDF resume from computer
3. Click "Upload"
4. Wait for processing (10-30 seconds)

**What happens:**
- File uploaded to backend
- Backend extracts text from PDF
- AI reads text (or fallback parser if no AI)
- Structured data extracted:
  - Skills: ["Python", "Django", "PostgreSQL"]
  - Experience: [{"role": "Backend Dev", "company": "TechCorp", "years": "2"}]
  - Education: [{"school": "University", "degree": "BS Computer Science"}]
- Data saved to your profile
- Results displayed

#### Step 4: Choose Target Role

1. Click "Analyze Jobs" or "Job Analysis"
2. Browse available roles
   - Backend Developer (ASP.NET, Django, Spring Boot variants)
   - Frontend Developer (React, Angular, Vue variants)
   - Full Stack Developer (MERN, MEAN, .NET variants)
3. Select target role and variant
   - Example: "Frontend Developer" → "React Developer"
4. Click "Analyze"

**What happens:**
- Your profile skills loaded
- Target job skills loaded
- Comparison calculated
- Results displayed

#### Step 5: View Analysis Results

**You see:**
```
Match Score: 42%

Your Skills That Match (3/7):
- JavaScript
- HTML/CSS
- API Integration

 Skills You Need to Learn (4/7):
1. React (Priority: HIGH)
2. TypeScript (Priority: HIGH)
3. Redux (Priority: MEDIUM)
4. Testing (Priority: MEDIUM)

Recommendations:
- Learn React in the next 2 weeks (highest priority)
- Combine with TypeScript learning
- Then dive into state management (Redux)
```

#### Step 6: Get Learning Roadmap

1. Click "Generate Roadmap" on analysis results
2. Wait for AI to create plan
3. Receive structured learning path

**You see:**
```
PHASE 1: JavaScript Fundamentals (2-3 weeks)
├─ Core Concepts
│  ├─ Variables, Data Types, Operators
│  ├─ Functions & Scope
│  └─ Async/Await & Promises
├─ Resources
│  ├─ FreeCodeCamp JavaScript Course
│  ├─ JavaScript.info
│  └─ Codewars Challenges (Kyu 6-7)
└─ Estimated Time: 2-3 weeks

PHASE 2: React Basics (3-4 weeks)
├─ Core Concepts
│  ├─ JSX Syntax
│  ├─ Components (Functional & Class)
│  ├─ Hooks (useState, useEffect)
│  └─ Props & Component Communication
├─ Resources
│  ├─ Official React Documentation
│  ├─ React Tutorial Project: Todo App
│  └─ Scrimba React Course
└─ Estimated Time: 3-4 weeks

PHASE 3: Advanced React & TypeScript (4-5 weeks)
├─ Core Concepts
│  ├─ Context API vs Redux
│  ├─ State Management Patterns
│  ├─ TypeScript Integration
│  └─ Testing (Jest, React Testing Library)
├─ Resources
│  ├─ Redux Official Docs
│  ├─ "State Management with Redux" Course
│  └─ Advanced React Patterns
└─ Estimated Time: 4-5 weeks
```

#### Step 7: Practice for Interview

1. Click "Practice Interview" on analysis results
2. Receive mock interview questions

**You see:**
```
MOCK INTERVIEW: React Developer

Question 1 (Easy): "What is JSX and why is it useful?"
Hint: Think about how React combines HTML-like syntax with JavaScript
Your Answer: [Text box to type your answer]

Question 2 (Medium): "Explain the difference between state and props"
Hint: Consider which direction data flows (parent to child vs internal)
Your Answer: [Text box to type your answer]

Question 3 (Hard): "Design a scalable React component architecture for a large application"
Hint: Think about component composition, reusability, and state management
Your Answer: [Text box to type your answer]
```

#### Step 8: Manage Profile

1. Click "Profile" in navigation
2. View your current skills
3. Click "Edit Skills" to update
4. Add new skills or remove old ones
5. Click "Save"

**What happens:**
- Skills list updated in database
- Next time you analyze jobs, uses new skills
- History of skills kept (for future progress tracking)

---

## 🔌 API Reference (For Developers)

### Authentication APIs

#### Register New User
```
POST /api/Auth/register

Request:
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "SecurePassword123",
  "confirmPassword": "SecurePassword123"
}

Response (Success - 200):
{
  "message": "User registered successfully"
}

Response (Error - 400):
{
  "message": "Passwords do not match"
}

Response (Error - 409):
{
  "message": "User already exists"
}
```

#### Login User
```
POST /api/Auth/login

Request:
{
  "email": "john@example.com",
  "password": "SecurePassword123"
}

Response (Success - 200):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}

Response (Error - 401):
{
  "message": "Invalid credentials"
}

How to use token:
Add to all future requests in header:
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Profile APIs

#### Get Your Profile
```
GET /api/Profile

Headers:
Authorization: Bearer {token}

Response (Success - 200):
{
  "id": 1,
  "userId": 1,
  "skills": ["C#", "React", "PostgreSQL"]
}

Response (Error - 401):
{"message": "Unauthorized"}

Response (Error - 404):
{"message": "Profile not found"}
```

#### Create Profile
```
POST /api/Profile

Headers:
Authorization: Bearer {token}

Request:
{
  "skills": ["C#", "React", "SQL"]
}

Response (Success - 201):
{
  "id": 1,
  "userId": 1,
  "skills": ["C#", "React", "SQL"]
}
```

#### Update Profile Skills
```
PUT /api/Profile/update

Headers:
Authorization: Bearer {token}

Request:
{
  "skills": ["C#", "React", "SQL", "Docker", "Kubernetes"]
}

Response (Success - 200):
{
  "message": "Skills updated successfully",
  "id": 1,
  "userId": 1,
  "skills": ["C#", "React", "SQL", "Docker", "Kubernetes"]
}
```

### Resume APIs

#### Upload Resume
```
POST /api/Resume/upload

Headers:
Authorization: Bearer {token}
Content-Type: multipart/form-data

Body:
file: [PDF file, max 5MB]

Response (Success - 200):
{
  "skills": ["Python", "Django", "PostgreSQL", "REST API"],
  "experience": [
    {
      "company": "TechCorp",
      "role": "Senior Backend Developer",
      "years": 3
    }
  ],
  "education": [
    {
      "school": "State University",
      "degree": "Bachelor of Science",
      "field": "Computer Science"
    }
  ],
  "projects": [
    {
      "name": "E-Commerce Platform",
      "description": "Built REST API serving 100k+ users"
    }
  ],
  "certifications": ["AWS Solutions Architect", "Kubernetes Administrator"]
}

Response (Error - 400):
{
  "message": "File size exceeds 5MB limit"
}

Response (Error - 400):
{
  "message": "Only PDF files are accepted"
}
```

### Job Analysis APIs

#### Get Available Roles
```
GET /api/Job/roles

Headers:
Authorization: Bearer {token}

Response (Success - 200):
[
  {
    "role": "Backend Developer",
    "variants": [
      {
        "name": "ASP.NET Backend Developer",
        "skills": ["C#", "ASP.NET", ".NET", "SQL", "Entity Framework"]
      },
      {
        "name": "Django Backend Developer",
        "skills": ["Python", "Django", "PostgreSQL", "REST API"]
      }
    ]
  },
  {
    "role": "Frontend Developer",
    "variants": [
      {
        "name": "React Developer",
        "skills": ["React", "JavaScript", "TypeScript", "CSS", "Redux"]
      }
    ]
  }
]
```

#### Analyze Job Match
```
POST /api/Job/analyze

Headers:
Authorization: Bearer {token}

Request:
{
  "userSkills": ["C#", "React", "SQL"],
  "targetRole": "Frontend Developer",
  "targetVariant": "React Developer"
}

Response (Success - 200):
{
  "matchPercentage": 40,
  "matchedSkills": ["React", "JavaScript"],
  "missingSkills": ["TypeScript", "Redux", "CSS"],
  "recommendations": [
    {
      "skill": "TypeScript",
      "priority": "HIGH",
      "reason": "Required for modern React development"
    }
  ]
}
```

#### Generate Learning Roadmap
```
POST /api/Job/roadmap

Headers:
Authorization: Bearer {token}

Request:
{
  "skills": ["JavaScript", "React"]
}

Response (Success - 200):
{
  "roadmap": [
    {
      "phase": 1,
      "title": "React Fundamentals",
      "description": "Learn core React concepts and hooks",
      "duration": "2-3 weeks",
      "resources": [
        {
          "type": "course",
          "name": "React Official Tutorial",
          "url": "https://react.dev"
        }
      ]
    },
    {
      "phase": 2,
      "title": "State Management",
      "description": "Master Redux and Context API",
      "duration": "3-4 weeks",
      "resources": [...]
    }
  ]
}
```

#### Generate Interview Questions
```
POST /api/Job/interview

Headers:
Authorization: Bearer {token}

Request:
{
  "skills": ["React", "JavaScript", "TypeScript"]
}

Response (Success - 200):
{
  "questions": [
    {
      "id": 1,
      "question": "What is the purpose of React Hooks?",
      "difficulty": "easy",
      "hints": [
        "They allow you to use state in functional components",
        "They were introduced in React 16.8"
      ]
    },
    {
      "id": 2,
      "question": "Explain the difference between useState and useReducer",
      "difficulty": "medium",
      "hints": [
        "useReducer is useful for complex state logic",
        "useState is simpler for single values"
      ]
    }
  ]
}
```

### Error Codes Reference

| Code | Meaning | What to do |
|------|---------|-----------|
| 200 | Success | ✅ Request worked |
| 201 | Created | ✅ Resource created successfully |
| 400 | Bad Request | ❌ Check your request data |
| 401 | Unauthorized | ❌ Missing or invalid token |
| 404 | Not Found | ❌ Resource doesn't exist |
| 409 | Conflict | ❌ Resource already exists |
| 500 | Server Error | ❌ Backend error, try later |

---

## 🌐 Deploying to Production

### What is Production?

**Development:** Your computer, for testing
**Production:** Internet-accessible website for real users

### Deployment to Render (Easy Method)

#### Prerequisites
- GitHub account (https://github.com)
- Render account (https://render.com)
- Code pushed to GitHub

#### Steps

**Step 1: Push Code to GitHub**
```bash
cd SKILLBRIDGE-PROJECT
git add .
git commit -m "Ready for deployment"
git push origin main
```

**Step 2: Create Render Account**
1. Go to https://render.com
2. Click "Sign up"
3. Use GitHub account to sign up (easier)

**Step 3: Create Web Service**
1. Dashboard → New → Web Service
2. Connect to your GitHub repo
3. Select repository

**Step 4: Configure Service**
```
Name: skillbridge-api
Environment: Docker
Build Command: (Leave empty, uses Dockerfile)
Start Command: (Leave empty, uses Dockerfile)
Plan: Free tier (or Paid)
```

**Step 5: Add Environment Variables**

In Render dashboard, add these variables:

```
ASPNETCORE_ENVIRONMENT = Production
ConnectionStrings__DefaultConnection = postgresql://username:password@db.render.com:5432/skillbridgedb
FrontendUrl = https://your-frontend-domain.com
Jwt__Key = your-secret-key-min-32-characters
Jwt__Issuer = SkillBridgeAPI
Jwt__Audience = SkillBridgeUsers
Groq__ApiKey = gsk_... (your Groq API key)
```

**Step 6: Deploy**
1. Click "Deploy"
2. Watch logs for any errors
3. Once "Service is live", it's running!
4. Backend URL: https://skillbridge-api.render.com

**Step 7: Deploy Frontend (Separate Service)**

**Option A: Deploy Frontend to Vercel (Recommended)**
1. Go to https://vercel.com
2. Connect GitHub repo
3. Select `Frontend` folder as root
4. Deploy
5. Update `FrontendUrl` in backend environment variable

**Option B: Serve Frontend from Backend**
- Frontend `/dist` already included in Docker
- Backend serves static files automatically
- Access at: https://skillbridge-api.render.com

---

## ❓ Troubleshooting & Common Issues

### Issue 1: "Cannot connect to database"

**Error Message:**
```
An exception occurred in the database while updating the entries...
Connection refused at localhost:5432
```

**Causes & Solutions:**
```
✓ PostgreSQL not installed?
  → Download and install from postgresql.org

✓ PostgreSQL not running?
  Windows: net start postgresql-x64-15
  Mac: brew services start postgresql
  Linux: sudo systemctl start postgresql

✓ Wrong connection string?
  → Check appsettings.json
  → Verify: host, port, username, password, database name

✓ Database doesn't exist?
  → Create in pgAdmin or psql:
    CREATE DATABASE SkillBridgeDb;
```

### Issue 2: "CORS Error" in Frontend

**Error Message:**
```
Access to XMLHttpRequest at 'https://localhost:7275/api/Auth/login' 
has been blocked by CORS policy
```

**Cause:** Frontend and backend on different ports

**Solution:**
```
Make sure appsettings.Development.json has:
{
  "FrontendUrl": "http://localhost:3000"
}

And Program.cs includes Frontend URL in CORS policy
```

### Issue 3: "Invalid token" after login

**Error Message:**
```
401 Unauthorized - Token validation failed
```

**Causes & Solutions:**
```
✓ Token expired?
  → Login again (generates new token)

✓ Different secret key?
  → Ensure Jwt:Key in appsettings.json is consistent

✓ Frontend not sending token?
  → Check: Authorization header includes "Bearer {token}"
```

### Issue 4: PDF upload fails

**Error Message:**
```
400 Bad Request - File size exceeds 5MB limit
or
Unsupported file format. Only PDF files are accepted.
```

**Solutions:**
```
✓ File too large?
  → Compress PDF or split into multiple files

✓ Not a PDF?
  → Ensure file ends with .pdf

✓ File corrupted?
  → Try opening PDF normally
  → If can't open, file is corrupted
```

### Issue 5: Groq AI not working

**Error Message:**
```
Failed to connect to Groq API
or
Resume parsing less accurate
```

**Solutions:**
```
✓ No Groq API key?
  → Get free key from https://console.groq.com
  → Add to appsettings.json: "Groq": { "ApiKey": "gsk_..." }

✓ API key invalid?
  → Verify key hasn't expired
  → Get fresh key from console.groq.com

✓ API rate limited?
  → Free tier has limits
  → Use fallback parser (still works offline)

✓ Network issue?
  → Check internet connection
  → Fallback parser automatically takes over
```

### Issue 6: Frontend won't start

**Error Message:**
```
Cannot find module '@vitejs/plugin-react'
or
npm ERR! missing ...
```

**Solution:**
```bash
cd Frontend
rm -rf node_modules package-lock.json
npm install
npm run dev
```

### Issue 7: "dotnet: command not found"

**Solution:**
```bash
# Reinstall .NET SDK
# OR check PATH environment variable includes .NET SDK path

# Verify installation
dotnet --version

# If still not working
# Restart terminal/computer after installation
```

---

## 📖 Glossary: Technical Terms Made Simple

### A-M

**API** (Application Programming Interface)
- Think of it as: A menu at a restaurant
- You order (request) → Kitchen processes → You get food (response)
- Your frontend "orders" from backend API

**Authentication**
- Think of it as: Proving who you are
- Like showing ID at airport
- Username + password → Verified → You're in

**Backend**
- Think of it as: The kitchen of a restaurant
- Where work happens (cooking = processing data)
- Hidden from customers (you don't see it)

**JWT** (JSON Web Token)
- Think of it as: A security badge
- Generated after login
- Good for 60 minutes
- Needed for all protected operations

**Database**
- Think of it as: A library
- Stores all information
- Can search and retrieve quickly
- PostgreSQL = Our library system

**DTO** (Data Transfer Object)
- Think of it as: An envelope for sending data
- Defines what data goes in/out
- Prevents sending unwanted data

**Frontend**
- Think of it as: The dining area of a restaurant
- What customers see and interact with
- React = Our dining area

**HTTP/HTTPS**
- Think of it as: Mail system
- HTTP = Regular mail (anyone can read)
- HTTPS = Secure mail (encrypted, private)
- Port numbers = Different mail boxes (80, 443, 3000, 5432)

**JSON** (JavaScript Object Notation)
- Think of it as: A standard way to write information
- Like: `{ "name": "John", "age": 30 }`
- Used for all API communication

**Middleware**
- Think of it as: Security guards at building entrance
- Check everyone who enters
- Can allow, deny, or modify requests

### N-Z

**ORM** (Object-Relational Mapping)
- Think of it as: A translator
- Translates database language to program language
- Entity Framework = Our translator

**Pagination**
- Think of it as: Breaking a book into chapters
- Shows data in chunks instead of all at once
- Better for performance

**REST API** (Representational State Transfer)
- Think of it as: Standardized menu format
- Uses HTTP methods: GET (read), POST (create), PUT (update), DELETE (remove)
- Predictable and easy to use

**Regex** (Regular Expression)
- Think of it as: A pattern matcher
- Finds text matching a pattern
- Example: Find all email addresses in text

**Routing**
- Think of it as: Road signs
- Directs traffic to right place
- Frontend routing: http://localhost:3000/login → Login page
- Backend routing: /api/Auth/login → AuthController

**SHA256**
- Think of it as: A one-way blender
- Put password in → Get hash out
- Can't reverse it (can't get password from hash)
- Used to securely store passwords

**SPA** (Single Page Application)
- Think of it as: Avoiding page reloads
- React loads once, then updates content
- Faster than traditional websites

**Swagger**
- Think of it as: An interactive menu guide
- Shows all API endpoints
- Let's you test them without coding
- Access at: https://localhost:7275/swagger

**Token** (JWT Token)
- Think of it as: An airport security pass
- Proves you're logged in
- Expires after time period
- Needed for protected operations

**UUID** / **GUID**
- Think of it as: Universal ID numbers
- Every object gets unique ID
- Like: `550e8400-e29b-41d4-a716-446655440000`
- Used for database records

**Vite**
- Think of it as: A fast cooker for frontend
- Builds React code super fast
- Makes optimized files for production

---

## 📞 Getting Help

### Finding Answers

1. **Error Message?**
   - See Troubleshooting section above
   - Search Google for error message

2. **Feature Not Working?**
   - Check logs: Backend console shows detailed errors
   - Check browser console: Frontend console shows errors
   - Read error message carefully

3. **Want to Learn More?**
   - Read DESIGN_DOCUMENTATION.md for technical details
   - Visit documentation sites:
     - React: https://react.dev
     - .NET: https://docs.microsoft.com/dotnet
     - PostgreSQL: https://postgresql.org/docs
     - Groq: https://console.groq.com/docs

4. **Report Bug?**
   - Create GitHub Issue with:
     - What you did
     - What happened
     - What should happen
     - Error message (copy full text)

---

## 🎓 Learning Resources

### For Using SkillBridge
- This README (you're reading it!)
- DESIGN_DOCUMENTATION.md for architecture
- Swagger Docs at /swagger endpoint

### For Development
- **C#/.NET**: https://docs.microsoft.com/dotnet
- **React**: https://react.dev
- **PostgreSQL**: https://postgresql.org
- **Entity Framework**: https://docs.microsoft.com/ef

### For Deployment
- **Render Docs**: https://render.com/docs
- **Docker Docs**: https://docs.docker.com
- **GitHub Actions**: https://docs.github.com/actions

---

## 📝 License & Attribution

**SkillBridge** - Open Source Project

Feel free to use, modify, and share!

---

## 📊 Quick Statistics

| Metric | Value |
|--------|-------|
| **Backend Language** | C# / .NET 8.0 |
| **Frontend Language** | JavaScript / React 19.2.4 |
| **Database** | PostgreSQL |
| **API Style** | RESTful |
| **Authentication** | JWT |
| **AI Provider** | Groq (with fallback) |
| **Deployment** | Docker + Render |
| **Build Tool (Frontend)** | Vite |

---

## 🎉 You're Ready!

Congratulations! You now understand:
- ✅ What SkillBridge does
- ✅ How to install it
- ✅ How to run it
- ✅ How to use it
- ✅ How to deploy it
- ✅ What to do when errors happen

**Next Steps:**
1. Follow "Quick Start" to get it running
2. Upload a resume
3. Analyze a job
4. Get a learning roadmap
5. Start your career transition!

---

**Questions?** Check the [Troubleshooting](#troubleshooting--common-issues) section or create a GitHub issue.

**Version:** 1.0  
**Last Updated:** March 21, 2025  
**Maintained By:** SkillBridge Development Team
