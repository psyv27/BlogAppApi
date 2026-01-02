# BlogApp API - Complete Testing Guide

## ?? Overview

This guide provides comprehensive instructions for testing all new features:
- ? **Stories** (Instagram-style, 24-hour expiry)
- ? **Real-Time Chat** (SignalR)
- ? **Infinite Scroll** (Cursor-based pagination)
- ? **Blog Management** (CRUD with categories and likes)
- ? **Comments & Interactions**

---

## ?? Quick Start

### Option 1: Automated Setup (Windows)
```bash
# Run the setup script
setup-and-test.bat
```

### Option 2: Manual Setup

**Step 1: Install Dependencies**
```bash
cd BlogApp.Api
dotnet restore
```

**Step 2: Update Database**
```bash
cd BlogApp.DAL
dotnet ef database update --startup-project ../BlogApp.Api
cd ..
```

**Step 3: Run API**
```bash
cd BlogApp.Api
dotnet run
# API will start at http://localhost:5000 (HTTP) or https://localhost:5001 (HTTPS)
```

---

## ?? Testing Methods

### Method 1: Interactive HTML Tester (Recommended for Beginners)

1. **Open `test-signalr.html`** in your browser
   - Use VS Code Live Server extension, or
   - Simply double-click the file

2. **Features:**
   - Beautiful UI for chat testing
   - REST API endpoint tester
   - Real-time status indicators

### Method 2: Postman Collection

1. **Download Postman** from https://www.postman.com/downloads/

2. **Import Collection:**
   - Open Postman
   - Click "Import"
   - Select `BlogApp.postman_collection.json`
   - Set the `token` variable with your JWT token

3. **Test All Endpoints:**
   - Authentication ? Login to get token
   - Stories ? Create and view stories
   - Messages ? Send and retrieve messages
   - Blogs ? Full CRUD operations

### Method 3: cURL Commands

```bash
# Register User
curl -X POST "http://localhost:5000/api/auth/register" \
  -F "Name=John" \
  -F "Surname=Doe" \
  -F "UserName=johndoe" \
  -F "Email=john@example.com" \
  -F "Password=Password123!" \
  -F "ConfirmPassword=Password123!"

# Login and Get Token
curl -X POST "http://localhost:5000/api/auth/login" \
  -F "UserName=johndoe" \
  -F "Password=Password123!"

# Get Active Stories
curl "http://localhost:5000/api/stories"

# Get Blogs with Infinite Scroll
curl "http://localhost:5000/api/blogs/feed?size=10"
```

---

## ?? Step-by-Step Testing

### 1. Authentication Setup

#### Register User 1:
```
POST /api/auth/register

Name: John
Surname: Doe
UserName: johndoe
Email: john@example.com
Password: Password123!
ConfirmPassword: Password123!
```

#### Register User 2:
```
POST /api/auth/register

Name: Jane
Surname: Smith
UserName: janesmith
Email: jane@example.com
Password: Password123!
ConfirmPassword: Password123!
```

#### Login to Get Tokens:
```
POST /api/auth/login
UserName: johndoe
Password: Password123!
```
**Save the returned token for User 1**

Repeat for User 2 with `janesmith` / `Password123!`

---

### 2. Testing Stories

#### Create a Story
```
POST /api/stories

Headers:
  Authorization: Bearer USER_TOKEN

Body (form-data):
  MediaFile: [Select any image file from your computer]
```

**Expected Response:** 200 OK

#### View All Stories
```
GET /api/stories

Response Example:
[
  {
    "id": 1,
    "appUserId": "550e8400-e29b-41d4-a716-446655440000",
    "userName": "johndoe",
    "userImageUrl": null,
    "mediaUrl": "https://...",
    "createdAt": "2025-12-30T17:32:21Z",
    "expiresAt": "2025-12-31T17:32:21Z"
  }
]
```

**Note:** Stories automatically disappear after 24 hours

---

### 3. Testing Messages (REST API First)

#### Send a Message
```
POST /api/messages

Headers:
  Authorization: Bearer USER_1_TOKEN
  Content-Type: application/x-www-form-urlencoded

Body:
  ReceiverId: [USER_2_ID_FROM_REGISTER_RESPONSE]
  Content: Hello Jane! How are you?
```

**Expected Response:** 200 OK

#### Get Conversation History
```
GET /api/messages/{USER_2_ID}

Headers:
  Authorization: Bearer USER_1_TOKEN

Response:
[
  {
    "id": 1,
    "senderId": "user-1-id",
    "senderName": "johndoe",
    "receiverId": "user-2-id",
    "content": "Hello Jane! How are you?",
    "createdAt": "2025-12-30T17:32:21Z",
    "isRead": false
  }
]
```

---

### 4. Testing SignalR Real-Time Chat

This is where the magic happens! ??

#### Using HTML Tester:

**Terminal 1 - User 1 (Browser Tab 1):**
1. Open `test-signalr.html`
2. Paste User 1's JWT token
3. Click "Connect to Chat"
4. Paste User 2's ID in "Receiver User ID" field
5. Type a message and click "Send Message"

**Terminal 2 - User 2 (Browser Tab 2):**
1. Open `test-signalr.html` in another tab
2. Paste User 2's JWT token
3. Click "Connect to Chat"
4. Paste User 1's ID in "Receiver User ID" field

**Result:** 
- User 1's message appears in real-time in User 2's chat (no page refresh needed!)
- User 2 can reply, and User 1 sees it instantly

---

### 5. Testing Infinite Scroll (Blogs)

#### First, Create Some Blogs

```
POST /api/blogs

Headers:
  Authorization: Bearer USER_TOKEN

Body (form-data):
  Title: First Blog Post
  Description: This is my first blog post
  CoverImageFile: [Select image]
  CategoryIds: 1
```

Repeat several times with different titles.

#### Test Infinite Scroll

```
GET /api/blogs/feed?size=5

Response:
[
  {
    "id": 5,
    "title": "Fifth Blog",
    "description": "...",
    "coverImageUrl": "...",
    "viewerCount": 0,
    "createdTime": "2025-12-30T18:00:00Z",
    "authorName": "johndoe",
    "categories": [...]
  },
  ...
]
```

**For Next Page:**
Use the `createdTime` of the last post as the `cursor` parameter:

```
GET /api/blogs/feed?size=5&cursor=2025-12-30T18:00:00Z
```

This returns posts created BEFORE that timestamp (true infinite scroll behavior).

---

### 6. Testing Blog Interactions

#### Like a Blog
```
POST /api/blogs/like/1

Headers:
  Authorization: Bearer USER_TOKEN

Response: 200 OK
```

Call the same endpoint again to **unlike**.

#### Update Blog
```
PUT /api/blogs/1

Headers:
  Authorization: Bearer USER_TOKEN

Body (form-data):
  Title: Updated Title
  Description: Updated description
  CategoryIds: 1
```

#### Add Comment
```
POST /api/blogs/comment/1

Headers:
  Authorization: Bearer USER_TOKEN

Body (form-data):
  Text: This is a great blog post!
```

---

## ?? Troubleshooting

### Issue: "No 'Access-Control-Allow-Origin' header"
**Solution:** 
- Make sure API is running
- Check that CORS is configured for your frontend URL in `Program.cs`

### Issue: "Invalid token" / "401 Unauthorized"
**Solutions:**
- Token may be expired (get a new one)
- Token may be incomplete or corrupted (paste the full token)
- Check token format is `Bearer TOKEN` (not just the token)

### Issue: SignalR not connecting
**Checklist:**
- ? API is running
- ? Token is valid and not expired
- ? Check browser console for errors (F12 ? Console tab)
- ? Verify `app.MapHub<ChatHub>("/chatHub");` is in `Program.cs`
- ? User exists in database

### Issue: "User not found"
**Solution:**
- Make sure you registered both users
- Copy the exact User ID from the login response
- Don't modify the User ID (it's a GUID)

### Issue: "Database does not exist"
**Solution:**
```bash
cd BlogApp.DAL
dotnet ef database update --startup-project ../BlogApp.Api
cd ..
```

---

## ?? Database Schema Overview

### Stories Table
```
Id (PK)
AppUserId (FK ? AspNetUsers)
MediaUrl
CreatedAt
ExpiresAt
IsDeleted
```

### Messages Table
```
Id (PK)
SenderId (FK ? AspNetUsers)
ReceiverId (FK ? AspNetUsers)
Content
CreatedAt
IsRead
IsDeleted
```

### BlogLikes Table
```
Id (PK)
BlogId (FK ? Blogs)
AppUserId (FK ? AspNetUsers)
IsDeleted
```

---

## ?? Useful Commands

```bash
# Run API in debug mode
dotnet run --configuration Debug

# Run API with hot reload
dotnet watch run

# Check database
dotnet ef dbcontext info --startup-project ../BlogApp.Api

# View migrations
dotnet ef migrations list --startup-project ../BlogApp.Api

# Rollback last migration (if needed)
dotnet ef database update PreviousMigration --startup-project ../BlogApp.Api

# Add new migration (if you modify entities)
dotnet ef migrations add YourMigrationName --startup-project ../BlogApp.Api
```

---

## ? Checklist for Complete Testing

- [ ] User Registration (2 users minimum)
- [ ] User Login (get tokens)
- [ ] Create Story
- [ ] View Stories
- [ ] Send Message (REST API)
- [ ] Get Conversation History
- [ ] SignalR Chat (real-time)
- [ ] Create Blog
- [ ] Get Blogs (infinite scroll)
- [ ] Get Blog Details (check viewerCount increments)
- [ ] Update Blog
- [ ] Like Blog
- [ ] Add Comment to Blog
- [ ] Delete Blog

---

## ?? Additional Resources

- [SignalR Documentation](https://learn.microsoft.com/en-us/aspnet/core/signalr/introduction)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [JWT Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization)
- [Postman Documentation](https://learning.postman.com/)

---

## ?? Next Steps for Production

- [ ] Add rate limiting
- [ ] Implement message encryption
- [ ] Add file upload validation (size, type)
- [ ] Implement story analytics
- [ ] Add user blocking/muting
- [ ] Implement read receipts for messages
- [ ] Add push notifications
- [ ] Cache frequently accessed data (Redis)
- [ ] Add logging and monitoring
- [ ] Security audit & penetration testing

---

Happy Testing! ??
