param (
    [string]$Command = "check"
)

function RunBuild {
    dotnet build
}

function RunTest {
    dotnet test --no-restore
}

function RunFormattingCheck {
    dotnet format -v detailed --verify-no-changes --no-restore
}

function RunSecurityCheck {
    dotnet list package --vulnerable --include-transitive > vulnerable.log
    powershell -ExecutionPolicy Bypass -File ./security-check.ps1
}

switch ($Command) {
    "build" {
        RunBuild
    }
    "test" {
        RunBuild
        RunTest
    }
    "format" {
        RunBuild
        RunFormattingCheck
    }
    "security" {
        RunBuild
        RunSecurityCheck
    }
    "check" {
        RunTest
        RunFormattingCheck
        RunSecurityCheck
    }
    default {
        Write-Host "Command not recognized"
    }
}
