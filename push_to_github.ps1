# PowerShell Script to push to GitHub
Set-Location "d:\DoAnCoSo\DoAnCoSo\Book2"

# Initialize git if not exists
if (!(Test-Path ".git")) {
    git init
    git add .
    git commit -m "Initial commit"
}

# Check if remote exists, if not add it
$remoteExists = git remote | Select-String "origin"
if (!$remoteExists) {
    git remote add origin https://github.com/deadpool7kk-star/NhomDoAnCoSoFixAndTest.git
} else {
    git remote set-url origin https://github.com/deadpool7kk-star/NhomDoAnCoSoFixAndTest.git
}

# Create and switch to branch VinhThai
git checkout -b VinhThai 2>$null
if ($LASTEXITCODE -ne 0) {
    git checkout VinhThai
}

# Add changes and push
git add .
git commit -m "Update project to VinhThai branch"
git push -u origin VinhThai --force

Write-Host "Done! Please check your GitHub repository." -ForegroundColor Green
