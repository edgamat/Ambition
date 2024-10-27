param (
    [Parameter(Mandatory)][string]$FeatureName
)

$jsonFilePath = '.\src\Ambition.Accounting\appsettings.json'

$jsonContent = Get-Content -Path $jsonFilePath -Raw | ConvertFrom-Json

$newFeature = @{
    EnabledFor = @(
        @{
            Name = "TimeWindow"
            Parameters = @{
                Start = [DateTime]::UtcNow.ToString("R")
            }
        }
    )
}

if ($jsonContent.FeatureManagement.$FeatureName) {
    Write-Output "$FeatureName already exists"
    Exit
}

$jsonContent.FeatureManagement | Add-Member -MemberType NoteProperty -Name $FeatureName -Value $newFeature

$jsonString = $jsonContent | ConvertTo-Json -Depth 100 -Compress:$false

$jsonString | Set-Content -Path $jsonFilePath
