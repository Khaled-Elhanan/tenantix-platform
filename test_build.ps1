$output = dotnet build "c:\Users\khald\source\repos\Tenantix Platform\Tenantix.Infrastructure\Tenantix.Infrastructure.csproj" 2>&1
$output | Out-File "c:\Users\khald\source\repos\Tenantix Platform\build_output_full.txt"
