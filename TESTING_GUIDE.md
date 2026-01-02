# API Testing Guide

## Setup

Before testing, make sure:
1. Database is updated: `dotnet ef database update`
2. API is running: `dotnet run`
3. API is accessible at: `https://localhost:5001` or `http://localhost:5000`

---

## Authentication First

All endpoints require JWT token (except `Auth/register` and `Auth/login`).

### 1. Register User
**POST** `http://localhost:5000/api/auth/register`

**Headers:**
```
Content-Type: application/x-www-form-urlencoded
```

**Body (form-data):**
```
Name=John
Surname=Doe
UserName=johndoe
Password=Password123!
Email=john@example.com
ConfirmPassword=Password123!
```

**Response (201):**
```json
{}
```

### 2. Login to Get Token
**POST** `http://localhost:5000/api/auth/login`

**Headers:**
```
Content-Type: application/x-www-form-urlencoded
```

**Body (form-data):**
```
UserName=johndoe
Password=Password123!
```

**Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "johndoe",
  "expires": "2025-12-31T17:32:21.1234567Z"
}
```

**Copy the `token` value - you'll need it for all other requests!**

---

## Testing New Features

### A. Stories Endpoints

#### 1. Create Story
**POST** `http://localhost:5000/api/stories`

**Headers:**
```
Authorization: Bearer YOUR_TOKEN_HERE
```

**Body (form-data):**
- Key: `mediaFile`, Value: (select an image file)

**Response (200):**
```json
{}
```

#### 2. Get Active Stories
**GET** `http://localhost:5000/api/stories`

**Response (200):**
```json
[
  {
    "id": 1,
    "appUserId": "user-id-123",
    "userName": "johndoe",
    "userImageUrl": null,
    "mediaUrl": "https://...",
    "createdAt": "2025-12-30T17:32:21Z",
    "expiresAt": "2025-12-31T17:32:21Z"
  }
]
```

---

### B. Messages Endpoints

#### 1. Send Message
**POST** `http://localhost:5000/api/messages`

**Headers:**
```
Authorization: Bearer YOUR_TOKEN_HERE
Content-Type: application/x-www-form-urlencoded
```

**Body (form-data):**
```
ReceiverId=other-user-id-123
Content=Hello, this is my first message!
```

**Response (200):**
```json
{}
```

#### 2. Get Conversation History
**GET** `http://localhost:5000/api/messages/{otherUserId}`

Replace `{otherUserId}` with the actual user ID.

**Headers:**
```
Authorization: Bearer YOUR_TOKEN_HERE
```

**Response (200):**
```json
[
  {
    "id": 1,
    "senderId": "sender-id-123",
    "senderName": "johndoe",
    "receiverId": "receiver-id-123",
    "content": "Hello!",
    "createdAt": "2025-12-30T17:32:21Z",
    "isRead": false
  }
]
```

---

### C. Blogs - Infinite Scroll (Cursor-Based Pagination)

#### 1. Get Blogs with Cursor Pagination
**GET** `http://localhost:5000/api/blogs/feed?size=10&cursor=2025-12-30T17:32:21Z`

**Query Parameters:**
- `size` (optional, default 10): Number of posts to return
- `cursor` (optional): ISO DateTime. Returns posts created BEFORE this date

**Response (200):**
```json
[
  {
    "id": 1,
    "title": "My First Blog Post",
    "description": "...",
    "coverImageUrl": "...",
    "viewerCount": 42,
    "createdTime": "2025-12-30T17:32:21Z",
    "authorName": "johndoe",
    "categories": ["Tech", "News"]
  }
]
```

**For next page, use the `createdTime` of the last post as the new `cursor`**

#### 2. Get Blogs with Standard Pagination
**GET** `http://localhost:5000/api/blogs?page=1&size=10&search=tech&categoryId=1`

**Query Parameters:**
- `page` (optional, default 1): Page number
- `size` (optional, default 10): Items per page
- `search` (optional): Search by title or description
- `categoryId` (optional): Filter by category

---

### D. Blog Operations

#### 1. Create Blog
**POST** `http://localhost:5000/api/blogs`

**Headers:**
```
Authorization: Bearer YOUR_TOKEN_HERE
```

**Body (form-data):**
```
Title=My New Blog Post
Description=This is a great post about technology
CoverImageFile=(select image file)
VideoFile=(optional - select video file)
CategoryIds[0]=1
CategoryIds[1]=2
```

**Response (200):**
```json
{}
```

#### 2. Update Blog
**PUT** `http://localhost:5000/api/blogs/{blogId}`

Replace `{blogId}` with actual blog ID.

**Headers:**
```
Authorization: Bearer YOUR_TOKEN_HERE
```

**Body (form-data):**
```
Title=Updated Title
Description=Updated description
CoverImageFile=(optional - select new image)
VideoFile=(optional - select new video)
CategoryIds[0]=1
CategoryIds[1]=3
```

**Response (204):** No content

#### 3. Get Blog Details
**GET** `http://localhost:5000/api/blogs/{blogId}`

**Response (200):**
```json
{
  "id": 1,
  "title": "My Blog Post",
  "description": "...",
  "coverImageUrl": "...",
  "viewerCount": 100,
  "createdTime": "2025-12-30T17:32:21Z",
  "authorName": "johndoe",
  "categories": ["Tech"],
  "comments": []
}
```

#### 4. Like/Unlike Blog
**POST** `http://localhost:5000/api/blogs/like/{blogId}`

**Headers:**
```
Authorization: Bearer YOUR_TOKEN_HERE
```

**Response (200):**
```json
{}
```

#### 5. Delete Blog
**DELETE** `http://localhost:5000/api/blogs/{blogId}`

**Headers:**
```
Authorization: Bearer YOUR_TOKEN_HERE
```

**Response (202):** Accepted

---

### E. Comments

#### 1. Add Comment to Blog
**POST** `http://localhost:5000/api/blogs/comment/{blogId}`

**Headers:**
```
Authorization: Bearer YOUR_TOKEN_HERE
```

**Body (form-data):**
```
Text=This is a great post!
ImageFile=(optional)
VideoFile=(optional)
```

---

## SignalR Real-Time Chat Testing

### Using HTML Test Page

Create a file `test-signalr.html` in your project root:

```html
<!DOCTYPE html>
<html>
<head>
    <title>SignalR Chat Test</title>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>
</head>
<body>
    <h1>SignalR Chat Test</h1>
    
    <div>
        <label>Token:</label>
        <input type="password" id="token" placeholder="Paste your JWT token here" style="width: 500px;">
        <button onclick="connect()">Connect to Chat</button>
    </div>

    <div style="margin-top: 20px;">
        <label>Receiver ID:</label>
        <input type="text" id="receiverId" placeholder="User ID to chat with">
    </div>

    <div style="margin-top: 10px;">
        <label>Message:</label>
        <input type="text" id="messageText" placeholder="Type your message">
        <button onclick="sendMessage()">Send</button>
    </div>

    <h3>Chat Log:</h3>
    <div id="messages" style="border: 1px solid #ccc; padding: 10px; height: 300px; overflow-y: auto;">
    </div>

    <script>
        let connection;

        async function connect() {
            const token = document.getElementById("token").value;
            
            connection = new signalR.HubConnectionBuilder()
                .withUrl("http://localhost:5000/chatHub", {
                    accessTokenFactory: () => token
                })
                .withAutomaticReconnect()
                .build();

            connection.on("ReceiveMessage", (senderId, content) => {
                addMessageToLog(`${senderId}: ${content}`);
            });

            try {
                await connection.start();
                addMessageToLog("Connected to chat!");
            } catch (err) {
                addMessageToLog("Error: " + err);
            }
        }

        function sendMessage() {
            const receiverId = document.getElementById("receiverId").value;
            const content = document.getElementById("messageText").value;

            if (!connection) {
                alert("Not connected! Click Connect first.");
                return;
            }

            connection.invoke("SendMessage", receiverId, content)
                .catch(err => addMessageToLog("Send error: " + err));
            
            document.getElementById("messageText").value = "";
        }

        function addMessageToLog(message) {
            const div = document.getElementById("messages");
            const p = document.createElement("p");
            p.textContent = new Date().toLocaleTimeString() + " - " + message;
            div.appendChild(p);
            div.scrollTop = div.scrollHeight;
        }
    </script>
</body>
</html>
```

### Steps to Test SignalR:

1. **Register 2 users:**
   - User 1: `johndoe` (get token1)
   - User 2: `janedoe` (get token2)

2. **Open `test-signalr.html` in browser (or use VS Code Live Server)**

3. **First chat window (User 1):**
   - Paste `token1` in Token field
   - Click "Connect to Chat"
   - Paste `User2 ID` in Receiver ID field
   - Type a message and click "Send"

4. **Second browser tab/window (User 2):**
   - Paste `token2` in Token field
   - Click "Connect to Chat"
   - You should see User 1's message appear!
   - Send a reply

---

## Postman Collection

Import this JSON into Postman as a collection:

```json
{
  "info": {
    "name": "BlogApp API Tests",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Auth",
      "item": [
        {
          "name": "Register",
          "request": {
            "method": "POST",
            "url": "http://localhost:5000/api/auth/register",
            "body": {
              "mode": "formdata"
            }
          }
        },
        {
          "name": "Login",
          "request": {
            "method": "POST",
            "url": "http://localhost:5000/api/auth/login",
            "body": {
              "mode": "formdata"
            }
          }
        }
      ]
    },
    {
      "name": "Stories",
      "item": [
        {
          "name": "Create Story",
          "request": {
            "method": "POST",
            "url": "http://localhost:5000/api/stories",
            "header": [
              {
                "key": "Authorization",
                "value": "Bearer {{token}}"
              }
            ],
            "body": {
              "mode": "formdata"
            }
          }
        },
        {
          "name": "Get Stories",
          "request": {
            "method": "GET",
            "url": "http://localhost:5000/api/stories"
          }
        }
      ]
    }
  ]
}
```

---

## Common Issues & Solutions

### Issue: "No 'Access-Control-Allow-Origin' header"
**Solution:** CORS is configured for `http://127.0.0.1:5500`. 
- Either use that URL
- Or update `Program.cs` CORS policy to include your frontend URL

### Issue: "Invalid token"
**Solution:** 
- Make sure you're using the full token string (including all dots)
- Token expires after a certain time, get a new one if needed

### Issue: "User not found"
**Solution:** 
- Make sure the userId exists in the database
- Register users first before sending messages

### Issue: SignalR not connecting
**Solution:**
- Check that `app.MapHub<ChatHub>("/chatHub");` is in Program.cs
- Make sure token is valid (not expired)
- Check browser console for detailed errors

