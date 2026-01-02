# ?? BlogApp API - Quick Reference Card

## API Base URL
```
HTTP:  http://localhost:5000
HTTPS: https://localhost:5001
```

## Authentication
All endpoints (except `/auth/*`) require:
```
Header: Authorization: Bearer YOUR_JWT_TOKEN
```

---

## ?? Endpoints Summary

### Auth (No Token Required)
| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Get JWT token |

### Stories
| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| GET | `/api/stories` | ? | View all active stories (24h) |
| POST | `/api/stories` | ? | Upload new story |

### Messages
| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| POST | `/api/messages` | ? | Send message |
| GET | `/api/messages/{userId}` | ? | Get conversation history |

### SignalR
| Event | Hub | Auth | Purpose |
|-------|-----|------|---------|
| `SendMessage(receiverId, content)` | `/chatHub` | ? | Real-time chat |
| `ReceiveMessage(senderId, content)` | `/chatHub` | ? | Receive real-time messages |

### Blogs
| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| GET | `/api/blogs?page=1&size=10` | ? | Get blogs (pagination) |
| GET | `/api/blogs/feed?size=10&cursor=DATE` | ? | Get blogs (infinite scroll) |
| GET | `/api/blogs/{id}` | ? | Get blog details |
| POST | `/api/blogs` | ? | Create blog |
| PUT | `/api/blogs/{id}` | ? | Update blog |
| DELETE | `/api/blogs/{id}` | ? | Delete blog |
| POST | `/api/blogs/like/{id}` | ? | Like/Unlike blog |
| POST | `/api/blogs/comment/{id}` | ? | Add comment |

### Categories
| Method | Endpoint | Auth | Purpose |
|--------|----------|------|---------|
| GET | `/api/categories` | ? | Get all categories |
| GET | `/api/categories/{id}` | ? | Get category details |
| POST | `/api/categories` | ? | Create category (Admin) |
| PUT | `/api/categories/{id}` | ? | Update category (Admin) |
| DELETE | `/api/categories/{id}` | ? | Delete category (Admin) |

---

## ?? Request/Response Examples

### Register User
```bash
POST /api/auth/register
Content-Type: application/x-www-form-urlencoded

Name=John&Surname=Doe&UserName=johndoe&Email=john@example.com&Password=Pass123!&ConfirmPassword=Pass123!
```

### Login
```bash
POST /api/auth/login
Content-Type: application/x-www-form-urlencoded

UserName=johndoe&Password=Pass123!
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "johndoe",
  "expires": "2025-12-31T17:32:21Z"
}
```

### Create Story
```bash
POST /api/stories
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: multipart/form-data

[Binary image data]
```

### Send Message
```bash
POST /api/messages
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
Content-Type: application/x-www-form-urlencoded

ReceiverId=550e8400-e29b-41d4-a716-446655440000&Content=Hello!
```

### Get Blogs (Infinite Scroll)
```bash
GET /api/blogs/feed?size=10&cursor=2025-12-30T18:00:00Z
```

Response:
```json
[
  {
    "id": 1,
    "title": "My Blog Post",
    "description": "...",
    "coverImageUrl": "...",
    "viewerCount": 42,
    "createdTime": "2025-12-30T18:00:00Z",
    "authorName": "johndoe",
    "categories": ["Tech", "News"]
  }
]
```

### Like Blog
```bash
POST /api/blogs/like/1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## ?? Testing Tools

### 1. HTML Tester (Recommended)
```bash
# Open in browser
test-signalr.html
```
? SignalR testing
? API testing  
? Beautiful UI

### 2. Postman
```bash
# Import collection
BlogApp.postman_collection.json
```
? Full API testing
? Environment variables
? Test automation

### 3. cURL
```bash
curl -X GET "http://localhost:5000/api/stories"
```
? Command line
? CI/CD integration

### 4. VS Code REST Client
Install extension: `REST Client`

Create `test.rest` file:
```
GET http://localhost:5000/api/stories

###

POST http://localhost:5000/api/auth/login
Content-Type: application/x-www-form-urlencoded

UserName=johndoe&Password=Pass123!
```

Then click "Send Request" above each endpoint

---

## ?? Security Notes

### Never Commit Sensitive Data
```
? JWT tokens
? Database passwords
? API keys
```

### Store Tokens Securely
```javascript
// For Web Apps: Use HttpOnly cookies
// For Mobile: Use secure storage
// For Testing: Use environment variables
```

### Token Expiration
Default: 1 hour

If expired, get a new token:
```bash
POST /api/auth/login
UserName=johndoe&Password=Pass123!
```

---

## ?? Common Errors & Solutions

| Error | Cause | Solution |
|-------|-------|----------|
| 401 Unauthorized | Invalid/expired token | Get new token from /login |
| 403 Forbidden | Missing admin role | Check user permissions |
| 404 Not Found | Resource doesn't exist | Verify ID is correct |
| 500 Internal Server | Server error | Check API logs |
| CORS error | Domain not allowed | Update CORS policy |
| SignalR disconnected | Token expired | Reconnect with new token |

---

## ?? Testing Sequence

1. **Register 2 users**
   ```bash
   POST /auth/register (User 1)
   POST /auth/register (User 2)
   ```

2. **Login both users**
   ```bash
   POST /auth/login (Get tokens)
   ```

3. **Test Stories**
   ```bash
   POST /stories (Create with User 1 token)
   GET /stories (View all)
   ```

4. **Test Messages**
   ```bash
   POST /messages (Send with User 1 token)
   GET /messages/{userId} (Get history)
   ```

5. **Test SignalR**
   ```javascript
   // Connect User 1: signalR.connect(token1)
   // Connect User 2: signalR.connect(token2)
   // User 1 sends: signalR.invoke("SendMessage", userId2, "Hello")
   // User 2 receives in real-time: ReceiveMessage event
   ```

6. **Test Blogs**
   ```bash
   POST /blogs (Create blog)
   GET /blogs/feed (Infinite scroll)
   POST /blogs/like/{id} (Like blog)
   ```

---

## ?? Performance Tips

### Infinite Scroll Implementation
```javascript
// Load first page
GET /api/blogs/feed?size=10

// On scroll to bottom, load next page
const lastPostTime = blogs[blogs.length - 1].createdTime;
GET /api/blogs/feed?size=10&cursor=${lastPostTime}
```

### Message Pagination
```javascript
// Get conversation (latest messages first)
GET /api/messages/{userId}

// Implement pagination server-side if needed
```

### Story Auto-Expiry
Stories automatically excluded from query after 24 hours:
```sql
-- Automatic with ExpiresAt > NOW()
WHERE ExpiresAt > GETUTCDATE()
```

---

## ?? Support & Resources

### API Documentation
- [Swagger UI](http://localhost:5000/swagger/index.html) (When running)

### Project Files
- `TESTING_GUIDE.md` - Detailed testing instructions
- `TESTING_COMPLETE_GUIDE.md` - Step-by-step guide
- `test-signalr.html` - Interactive HTML tester
- `BlogApp.postman_collection.json` - Postman collection

### Git Repository
```bash
git remote -v
# origin: https://github.com/psyv27/BlogAppApi
```

---

## ?? Deployment Checklist

- [ ] Update connection string for production database
- [ ] Change JWT secret key
- [ ] Enable HTTPS
- [ ] Configure CORS for production domain
- [ ] Set up logging
- [ ] Configure SignalR for scale-out (Redis)
- [ ] Run security audit
- [ ] Load testing
- [ ] Database backups
- [ ] Monitoring alerts

---

**Last Updated:** December 30, 2025
**API Version:** .NET 8
**Status:** ? Ready for Testing
