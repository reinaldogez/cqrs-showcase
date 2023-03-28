$connectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=my(!)Password;"

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "Connected successfully to SQL Server."
    $connection.Close()
}
catch {
    Write-Host "Error connecting to SQL Server: $_.Exception.Message"
}