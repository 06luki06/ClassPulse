# Fails if the first line of a commit message file breaks the format from AGENTS.md.
# Allows the messages Git generates for merges and reverts, and fixup commits.
param(
    [Parameter(Mandatory = $true)]
    [string] $MessageFile
)

$firstLine = Get-Content -LiteralPath $MessageFile -TotalCount 1
$format = '^ISSUE [1-9][0-9]* (feat|fix|docs|refactor|test|build|chore): \S'
$generated = '^(Merge |Revert "|fixup! |squash! |amend! )'

if ($firstLine -cmatch $format -or $firstLine -cmatch $generated) {
    exit 0
}

Write-Output "Invalid commit message: $firstLine"
Write-Output 'Expected: ISSUE <number> <type>: <imperative summary>'
Write-Output 'Types: feat, fix, docs, refactor, test, build, chore'
exit 1
