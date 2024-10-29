param (
    [string]$Command
)

function RunBuild {
    dotnet build
}

function RunRebuild {
    dotnet build --no-incremental
}

function RunTest {
    dotnet test
}

function RunE2E {
    dotnet test --filter TestCategory=e2e
}

function RunStart {
    # dotnet run --project .\src\One80.ImsApps.DelayedShotgunMailer -lp https
    $tasks = @(
        { dotnet run --project .\src\Ambition.Api -lp https } 
        { dotnet run --project .\src\Ambition.Accounting -lp https } 
        { dotnet run --project .\src\Mercury.Email -lp https }
    )
    
    $tasks | ForEach-Object { Start-Job -ScriptBlock $_ }
}

function RunLint {
    dotnet format -v detailed --verify-no-changes --no-restore
}

function RunLintFix {
    dotnet format -v detailed --no-restore
}

switch ($Command) {
    "build" {
        RunBuild
    }
    "rebuild" {
        RunRebuild
    }
    "test" {
        RunTest
    }
    "e2e" {
        RunE2E
    }
    "start" {
        RunStart
    }
    "lint" {
        RunLint
    }
    "lint-fix" {
        RunLintFix
    }
    "check" {
        RunTest
        RunLint
    }
    default {
        Write-Host "Command not recognized. Use 'build', 'rebuild', 'test', 'e2e', 'start', 'lint', 'lint-fix', or 'check'."
    }
}