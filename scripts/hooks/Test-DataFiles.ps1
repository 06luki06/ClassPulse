# Fails if any given path looks like a database, backup or export.
# The app stores personal data about children, and such files must never be committed.
# Checks only the path names and never opens a file.
param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $Paths
)

$pattern = '\.((db|sqlite|sqlite3)(-wal|-shm|-journal)?|bak|backup|csv|xls|xlsx)$|(^|[\\/])classpulse\.db'
$blocked = @($Paths | Where-Object { $_ -match $pattern })

if ($blocked.Count -gt 0) {
    Write-Output 'These files look like databases, backups or exports and must not be committed:'
    $blocked | ForEach-Object { Write-Output "  $_" }
    exit 1
}

exit 0
