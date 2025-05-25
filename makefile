# Define targets and their dependencies
.PHONY: check
check: build test security format

# Step 1: Build the .NET project
.PHONY: build
build:
	dotnet build

# Step 2a: Run unit tests
.PHONY: test
test: build
	dotnet test --no-build

# Step 2b: Check for vulnerable packages
.PHONY: security
security: build
	dotnet list package --vulnerable > vulnerable.log
	pwsh -NoProfile -ExecutionPolicy Bypass -Command "$vulnerable = Select-String -Path 'vulnerable.log' -Pattern 'has the following vulnerable packages' -Quiet; if ($vulnerable) { Write-Output 'Security vulnerabilities found in the command output.'; Get-Content vulnerable.log; exit 1 }"

# Step 2c: Check formatting issues
.PHONY: format
format: build
	dotnet format --verify-no-changes --no-restore


#	pwsh -NoProfile -ExecutionPolicy Bypass -File ./security-check.ps1
