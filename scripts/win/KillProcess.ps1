$port = 8150

Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue |
    Select-Object -ExpandProperty OwningProcess -Unique |
    Where-Object { $_ -ne 0 } |
    ForEach-Object {
        if (Get-Process -Id $_ -ErrorAction SilentlyContinue) {
            Write-Host "Killing PID $_"
            Stop-Process -Id $_ -Force
        }
    }