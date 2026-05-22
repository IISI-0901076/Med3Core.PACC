# ==========================================
# NuGet Whitelist & Blacklist Checker
# FINAL / STABLE / PowerShell 5.1 SAFE
# ==========================================
$ErrorActionPreference = "Stop"

Write-Host "🔍 Checking NuGet packages against whitelist / blacklist..."

# --------------------------------------------------
# Locate repo root & whitelist.md
# --------------------------------------------------
$repoRoot = Split-Path -Parent $PSScriptRoot
$whitelistPath = Join-Path $repoRoot "whitelist.md"

if (-not (Test-Path $whitelistPath)) {
    Write-Error "❌ whitelist.md not found at repo root."
}

# Read whitelist.md (UTF8 + BOM safe)
$content = Get-Content $whitelistPath -Raw -Encoding UTF8

# --------------------------------------------------
# Helpers: extract block by marker
# --------------------------------------------------
function Get-MarkerBlock {
    param (
        [string]$Text,
        [string]$Start,
        [string]$End
    )

    if (-not ($Text.Contains($Start) -and $Text.Contains($End))) {
        return @()
    }

    $s = $Text.IndexOf($Start)
    $e = $Text.IndexOf($End)

    if ($e -le $s) {
        return @()
    }

    $block = $Text.Substring(
        $s + $Start.Length,
        $e - ($s + $Start.Length)
    )

    return ($block -split "`n")
}

# --------------------------------------------------
# Parse backend whitelist
# --------------------------------------------------
$whitelistLines = Get-MarkerBlock `
    -Text  $content `
    -Start '<!-- BACKEND-NUGET-WHITELIST-START -->' `
    -End   '<!-- BACKEND-NUGET-WHITELIST-END -->'

if ($whitelistLines.Count -eq 0) {
    Write-Error "❌ Cannot find BACKEND-NUGET-WHITELIST marker block."
}

$allowedPackages = @()

foreach ($line in $whitelistLines) {

    $line = $line.Trim()
    if (-not $line.StartsWith('|')) { continue }
    if ($line -match '^\|\s*-+') { continue }

    $cols = $line -split '\|'
    if ($cols.Count -ge 3) {

        # 第二欄：程式庫名稱
        $pkg = $cols[2].Trim().ToLowerInvariant()

        if ($pkg -match '^[a-z0-9][a-z0-9\.\-_*\+]+$') {
            $allowedPackages += $pkg
        }
    }
}

$allowedPackages = $allowedPackages | Sort-Object -Unique

if ($allowedPackages.Count -eq 0) {
    Write-Error "❌ No valid backend NuGet packages found in whitelist."
}

Write-Host "✅ Allowed packages:"
$allowedPackages | ForEach-Object { Write-Host "  - $_" }

# --------------------------------------------------
# Parse backend blacklist
# --------------------------------------------------
$blacklistLines = Get-MarkerBlock `
    -Text  $content `
    -Start '<!-- BACKEND-NUGET-BLACKLIST-START -->' `
    -End   '<!-- BACKEND-NUGET-BLACKLIST-END -->'

$blacklist = @()

foreach ($line in $blacklistLines) {

    $line = $line.Trim()
    if (-not $line.StartsWith('|')) { continue }
    if ($line -match '^\|\s*-+') { continue }

    $cols = $line -split '\|'
    if ($cols.Count -ge 2) {

        # 第一欄：套件名稱
        $pkg = $cols[1].Trim().ToLowerInvariant()

        if ($pkg -match '^[a-z0-9][a-z0-9\.\-_*\+]+$') {
            $blacklist += $pkg
        }
    }
}

$blacklist = $blacklist | Sort-Object -Unique

if ($blacklist.Count -gt 0) {
    Write-Host "🚫 Blacklisted packages:"
    $blacklist | ForEach-Object { Write-Host "  - $_" }
}

# --------------------------------------------------
# Ignored packages (framework / tooling / test)
# --------------------------------------------------
$ignoredPackages = @(
    'microsoft.net.test.sdk',
    'microsoft.visualstudio.*',
    'microsoft.aspnetcore.*',
    'swashbuckle.aspnetcore.*'
)

# --------------------------------------------------
# Scan all csproj files
# --------------------------------------------------
$violations = @()
$csprojFiles = Get-ChildItem -Recurse -Filter *.csproj

foreach ($csproj in $csprojFiles) {

    [xml]$xml = Get-Content $csproj.FullName
    $packageRefs = $xml.SelectNodes('//PackageReference')

    foreach ($pkg in $packageRefs) {

        $pkgName = $null

        if ($pkg.Attributes['Include']) {
            $pkgName = $pkg.Attributes['Include'].Value
        }
        elseif ($pkg.Attributes['Update']) {
            $pkgName = $pkg.Attributes['Update'].Value
        }
        else {
            $includeNode = $pkg.SelectSingleNode('Include')
            if ($includeNode) {
                $pkgName = $includeNode.InnerText
            }
        }

        if ([string]::IsNullOrWhiteSpace($pkgName)) {
            continue
        }

        $pkgName = $pkgName.ToLowerInvariant()

        # ------------------------------------------
        # 1️⃣ Blacklist = absolute deny
        # ------------------------------------------
        foreach ($blocked in $blacklist) {
            if ($pkgName -like $blocked) {
                $violations += "❌ BLACKLISTED: $pkgName  (in $($csproj.FullName))"
                continue
            }
        }

        # ------------------------------------------
        # 2️⃣ Ignore framework / tooling
        # ------------------------------------------
        $matched = $false
        foreach ($ignore in $ignoredPackages) {
            if ($pkgName -like $ignore) {
                $matched = $true
                break
            }
        }

        # ------------------------------------------
        # 3️⃣ Whitelist check
        # ------------------------------------------
        if (-not $matched) {
            foreach ($allowed in $allowedPackages) {
                if ($pkgName -like $allowed) {
                    $matched = $true
                    break
                }
            }
        }

        if (-not $matched) {
            $violations += "$pkgName  (in $($csproj.FullName))"
        }
    }
}

# --------------------------------------------------
# Result
# --------------------------------------------------
if ($violations.Count -gt 0) {
    Write-Host ""
    Write-Error "❌ NuGet policy violation detected:`n$($violations -join "`n")"
}

Write-Host ""
Write-Host "🎉 NuGet whitelist / blacklist check passed."
exit 0
