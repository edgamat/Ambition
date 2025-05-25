Invoke-Expression -Command "dotnet list package --vulnerable" > vulnerable.log
$vulnerable = Select-String -Path "vulnerable.log" -Pattern "has the following vulnerable packages" -Quiet
if ($vulnerable) {
    Write-Output "Security vulnerabilities found in the command output."
    cat .\vulnerable.log
}
