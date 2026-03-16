# PowerShell script to rename Enums to C+Name convention and update references

$basePath = "d:\Clone\WebUI\src\WebUI\blazorwasm"

$enums = @(
    "BadgeStyle",
    "BadgeVariant",
    "OrderStatus",
    "PaymentStatus",
    "ToastLevel",
    "ThemeMode",
    "LanguageSelectorMode",
    "LanguageCode",
    "ButtonType",
    "TooltipTrigger",
    "TooltipPlacement",
    "ThemeSwitchVariant",
    "SwitchSize",
    "SwitchLabelPosition",
    "SkeletonAnimation",
    "SkeletonVariant",
    "SelectPlacement",
    "ValidationStatus",
    "SelectMode",
    "SectionVariant",
    "SidebarState",
    "DropdownAlignment",
    "EditorCommand",
    "NavLinkMatchMode",
    "ButtonStyle",
    "ButtonSize",
    "ButtonIconPosition",
    "FormFieldLayout",
    "EmojiCategory"
)

# 1. Update text content in files
Write-Host "Updating references in files..."
$filesToScan = Get-ChildItem -Path $basePath -Include *.cs, *.razor -Recurse

foreach ($file in $filesToScan) {
    $content = Get-Content $file.FullName
    $changed = $false

    foreach ($enum in $enums) {
        $oldName = $enum
        $newName = "C" + $enum
        
        # Use word boundary to match exact type name
        if ($content -match "\b$oldName\b") {
            $content = $content -replace "\b$oldName\b", $newName
            $changed = $true
            Write-Host "Replacing $oldName -> $newName in $($file.FullName)"
        }
    }

    if ($changed) {
        $content | Set-Content $file.FullName
    }
}

# 2. Rename files that match exactly the Enum name
Write-Host "Renaming enum files..."
foreach ($enum in $enums) {
    $oldName = $enum
    $newName = "C" + $enum
    
    $filesToRename = Get-ChildItem -Path $basePath -Filter "$oldName.cs" -Recurse
    foreach ($file in $filesToRename) {
        $newPath = Join-Path $file.DirectoryName "$newName.cs"
        Write-Host "Renaming file $($file.FullName) -> $newPath"
        Rename-Item $file.FullName "$newName.cs"
    }
}

Write-Host "Enums renamed successfully according to convention!"
