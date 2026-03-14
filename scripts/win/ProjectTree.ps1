$OutputFile = "scripts/output/ProjectStructure.md"

$dir = Split-Path $OutputFile
if (!(Test-Path $dir)) {
    New-Item -ItemType Directory -Path $dir -Force | Out-Null
}

$ignoreDirs = @(
".git",".vs","bin","obj","node_modules","scripts","TestResults","coverage","wwwroot"
)

function Show-Tree($path, $prefix="") {

    $items = Get-ChildItem $path | Where-Object {
        $ignoreDirs -notcontains $_.Name
    }

    for ($i=0; $i -lt $items.Count; $i++) {

        $item = $items[$i]
        $isLast = $i -eq ($items.Count - 1)

        if ($isLast) {
            $line = "$prefix\-- $($item.Name)"
            $newPrefix = "$prefix    "
        }
        else {
            $line = "$prefix+-- $($item.Name)"
            $newPrefix = "$prefix|   "
        }

        Add-Content -Path $OutputFile -Value $line -Encoding UTF8

        if ($item.PSIsContainer) {
            Show-Tree $item.FullName $newPrefix
        }
    }
}

$header = @"
# Project Structure

_Generated automatically on $(Get-Date -Format "yyyy-MM-dd HH:mm:ss")_

"@

Set-Content $OutputFile $header -Encoding UTF8

Add-Content $OutputFile '```' -Encoding UTF8
Add-Content $OutputFile '.' -Encoding UTF8

Show-Tree (Get-Location)

Add-Content $OutputFile '```' -Encoding UTF8

Write-Host "Project structure generated at $OutputFile"