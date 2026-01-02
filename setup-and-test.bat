@echo off
REM BlogApp API - Setup & Testing Script

echo.
echo ========================================
echo   BlogApp API - Complete Setup
echo ========================================
echo.

REM Check if dotnet CLI is available
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET CLI not found. Please install .NET 8 SDK first.
    echo Download from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [Step 1] Building the project...
dotnet build
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo ? Build successful!

echo.
echo [Step 2] Creating database migration...
cd BlogApp.DAL
dotnet ef migrations add AddSocialFeaturesComprehensive --startup-project ../BlogApp.Api
if errorlevel 1 (
    echo WARNING: Migration may have failed or already exists
)
cd ..
echo ? Migration checked!

echo.
echo [Step 3] Applying database migrations...
cd BlogApp.DAL
dotnet ef database update --startup-project ../BlogApp.Api
if errorlevel 1 (
    echo ERROR: Database update failed!
    pause
    exit /b 1
)
cd ..
echo ? Database updated successfully!

echo.
echo ========================================
echo   Setup Complete!
echo ========================================
echo.
echo Next steps:
echo 1. Start the API:
echo    cd BlogApp.Api
echo    dotnet run
echo.
echo 2. Open test-signalr.html in your browser for interactive testing
echo    (Use Live Server extension in VS Code)
echo.
echo 3. Import BlogApp.postman_collection.json in Postman for REST API testing
echo.
echo 4. Read TESTING_GUIDE.md for detailed testing instructions
echo.
pause
