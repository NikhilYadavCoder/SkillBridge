# 🚀 SKILLBRIDGE FRONTEND DOCUMENTATION (FULL IMPLEMENTATION GUIDE)

---

# 📌 1. PROJECT OVERVIEW

SkillBridge is an AI-powered platform that:

* Extracts structured data from resumes
* Matches user skills with job roles
* Identifies missing skills
* Generates a learning roadmap
* Provides mock interview questions

Frontend is built using:

* React (Vite)
* Axios for API communication
* Context API for authentication

Backend:

* .NET Web API (already implemented)

---

# 🎯 2. CORE FEATURES

### ✅ Authentication

* Login / Register
* JWT-based authentication

### ✅ Resume Processing

* Upload PDF
* Extract and store:

  * Skills
  * Education
  * Projects
  * Experience
  * Certifications

### ✅ Job Analysis

* Compare user skills with jobs.json
* Output:

  * Match %
  * Missing skills
  * Variant-wise comparison

### ✅ Learning Roadmap

* AI-generated steps to improve missing skills

### ✅ Interview Questions

* Generated based on user skills

---

# 🔁 3. COMPLETE USER FLOW

1. User registers/logs in
2. Uploads resume
3. Backend extracts and stores profile data
4. User selects role
5. System analyzes skills
6. Displays:

   * Match %
   * Missing skills
   * Roadmap
   * Interview questions

---

# 🏗️ 4. FRONTEND FOLDER STRUCTURE

```
Frontend/
 └── src/
     ├── api/
     │    └── axios.js
     │
     ├── context/
     │    └── AuthContext.jsx
     │
     ├── pages/
     │    ├── Login.jsx
     │    ├── Register.jsx
     │    ├── Dashboard.jsx
     │    ├── UploadResume.jsx
     │    ├── Analysis.jsx
     │
     ├── components/
     │    ├── Navbar.jsx
     │    ├── RoleSelector.jsx
     │    ├── MatchCard.jsx
     │    ├── SkillsList.jsx
     │    ├── RoadmapCard.jsx
     │    ├── InterviewCard.jsx
     │
     ├── utils/
     │    └── constants.js
     │
     ├── App.jsx
     └── main.jsx
```

---

# 🔌 5. API ENDPOINTS

## 🔐 Auth

* POST `/api/auth/register`
* POST `/api/auth/login`

## 📄 Resume

* POST `/api/resume/upload`

## 📊 Job Analysis

* POST `/api/job/analyze`

## 🧠 Roadmap

* POST `/api/roadmap/generate`

## 🎤 Interview

* POST `/api/interview/generate`

---

# 🔑 6. AUTHENTICATION FLOW

## Store Token

```js
localStorage.setItem("token", token)
```

## Attach Token in Headers

```http
Authorization: Bearer <token>
```

## Logout

```js
localStorage.removeItem("token")
```

---

# ⚙️ 7. AXIOS CONFIGURATION

* Base URL: backend URL
* Interceptor:

  * Attach JWT automatically
  * Handle 401 errors (logout)

---

# 🧠 8. STATE MANAGEMENT

Use:

* React `useState`
* React Context (Auth only)

AuthContext stores:

```js
{
  user,
  token,
  login(),
  logout()
}
```

---

# 🎨 9. UI PAGES DESIGN

---

## 🔹 Login Page

* Email + Password
* On success → Dashboard

**Fields (from backend `LoginDto`):**

- `email` (string, required, must be a valid email)
- `password` (string, required)

Payload example:

```json
{
  "email": "user@example.com",
  "password": "StrongPassword123!"
}
```

---

## 🔹 Register Page

* Name, Email, Password, Confirm Password, optional profile info

**Fields (from backend `RegisterDto`):**

- `name` (string, required)
- `email` (string, required, valid email)
- `dateOfBirth` (DateTime?, optional)
- `gender` (string?, optional)
- `contact` (string?, optional)
- `password` (string, required)
- `confirmPassword` (string, required, must match `password`)

Payload example:

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "dateOfBirth": "2000-01-01T00:00:00Z",
  "gender": "Male",
  "contact": "+1-123-456-7890",
  "password": "StrongPassword123!",
  "confirmPassword": "StrongPassword123!"
}
```

---

## 🔹 Dashboard

* Upload Resume button
* Analyze button

---

## 🔹 Upload Resume Page

* File input (PDF only)
* Submit button

API:

```http
POST /api/resume/upload
```

---

## 🔹 Analysis Page (CORE UI)

### Sections:

### ✅ Match Percentage

* Progress bar

---

### ✅ Role Variants

Example:

```text
ASP.NET Developer → 55%
Django Developer → 37%
```

---

### ✅ Missing Skills

Display as tags:

```text
JWT | Docker | REST API
```

---

### ✅ Learning Roadmap

Step-by-step:

```text
Step 1 → Learn JWT
Step 2 → Build API
Step 3 → Deploy
```

---

### ✅ Interview Questions

Expandable list:

```text
Q1
Q2
Q3
```

---

# 🔄 10. DATA FLOW

```text
Upload Resume
   ↓
Backend extracts data
   ↓
Stored in DB (UserProfile)
   ↓
Analyze API
   ↓
Frontend displays results
```

---

# ⚠️ 11. EDGE CASE HANDLING (VERY IMPORTANT)

---

## 🧾 Resume Issues

### ❌ Empty / invalid PDF

→ Show:

```text
Invalid resume file
```

---

## 🧠 AI Failure

If AI fails:

* Backend fallback runs
* Frontend must still display results

---

## 📉 No Experience

```text
"No experience available"
```

---

## 📜 No Certifications

```text
"No certifications available"
```

---

## 📚 No Projects

```text
"No projects available"
```

---

## 📊 No Skills Found

```text
"No skills extracted"
```

---

## 🧩 No Matching Jobs

```text
"No matching roles found"
```

---

## 🧠 No Roadmap

```text
"No roadmap generated"
```

---

## 🎤 No Interview Questions

```text
"No interview questions available"
```

---

## 🔐 Unauthorized (401)

* Auto logout
* Redirect to login

---

## 🌐 Network Error

```text
"Server not reachable"
```

---

## 📦 Empty API Response

* Show fallback UI
* Avoid crashes

---

# 🚫 12. COMMON MISTAKES TO AVOID

* ❌ Hardcoding API URLs
* ❌ Mixing API calls inside components
* ❌ Not handling loading states
* ❌ Not handling null/empty data
* ❌ Overcomplicated state management

---

# ✅ 13. BEST PRACTICES

* Use reusable components
* Keep API logic separate
* Show loading spinners
* Use clear error messages
* Keep UI simple

---

# 🧪 14. TESTING FLOW

1. Login/Register
2. Upload resume
3. Check DB update
4. Analyze API
5. Verify UI rendering

---

# 🚀 15. DEVELOPMENT ORDER

Follow strictly:

```text
1. Axios setup
2. Auth (Login/Register)
3. Resume upload
4. Job analysis
5. UI display
6. Roadmap
7. Interview questions
```

---

# 🏁 FINAL OUTPUT

User sees:

```text
✔ Match Percentage
✔ Missing Skills
✔ Learning Roadmap
✔ Interview Questions
✔ Profile Auto-filled
```

---

# 🔥 END GOAL

A full-stack AI-powered career assistant that:

* Understands resumes
* Suggests improvements
* Prepares users for jobs

---

# 📌 READY FOR IMPLEMENTATION

Use this document with GitHub Copilot to:

* Generate components
* Maintain structure
* Avoid errors
* Build production-ready UI

---
