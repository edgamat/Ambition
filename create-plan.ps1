$url = "https://localhost:7289/maintenance-plan"

$payload = @{
  productId = "45a340d8-7bd1-4f56-95b3-8ca192ef6094"
  customerId = "4f8b1c36-047b-4c5c-99e4-bf5446856d14"
  description = "Plan for my Widget"
  userName = "matte"
  effectiveOn = "2024-11-01T13:10:01.016Z"
} | ConvertTo-Json -Depth 10

$headers = @{
  "Accept" = "application/json"
  "Content-Type" = "application/json"
}

$response = Invoke-WebRequest -Uri $url -Method POST -Body $payload -Headers $headers

$response.Headers

$response.Content
