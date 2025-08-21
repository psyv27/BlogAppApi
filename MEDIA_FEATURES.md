# Media Upload Features

## Overview
This update adds support for photo and video uploads to both blog posts and comments.

## New Features

### Blog Posts
- **Photo Upload**: Upload images via `CoverImageFile` or provide URL via `CoverImageUrl`
- **Video Upload**: Upload videos via `VideoFile` or provide URL via `VideoUrl`
- Supported image formats: .jpg, .jpeg, .png, .gif, .bmp, .webp (max 5MB)
- Supported video formats: .mp4, .avi, .mov, .wmv, .flv, .webm, .mkv (max 100MB)

### Comments
- **Photo Attachment**: Upload images via `ImageFile` or provide URL via `ImageUrl`
- **Video Attachment**: Upload videos via `VideoFile` or provide URL via `VideoUrl`
- Same file format and size restrictions as blog posts

## API Endpoints

### Create Blog with Media
```
POST /api/blogs
Content-Type: multipart/form-data

Fields:
- Title (string, required)
- Description (string, required)
- CoverImageUrl (string, optional)
- CoverImageFile (file, optional)
- VideoUrl (string, optional)
- VideoFile (file, optional)
- CategoryIds (array of integers, required)
```

### Add Comment with Media
```
POST /api/blogs/comment/{blogId}
Content-Type: multipart/form-data

Fields:
- Text (string, required)
- ImageUrl (string, optional)
- ImageFile (file, optional)
- VideoUrl (string, optional)
- VideoFile (file, optional)
- ParrentId (integer, optional - for nested comments)
```

## File Storage
- Uploaded files are stored in `wwwroot/uploads/`
- Images: `wwwroot/uploads/images/`
- Videos: `wwwroot/uploads/videos/`
- Files are given unique names with GUID prefixes
- Static file serving is enabled to access uploaded media

## Database Changes
- Added `VideoUrl` column to `Blogs` table
- Added `ImageUrl` and `VideoUrl` columns to `Comments` table

## Validation
- At least one media source (URL or file) must be provided for blog posts
- File size and format validation is performed server-side
- Comprehensive FluentValidation rules ensure data integrity