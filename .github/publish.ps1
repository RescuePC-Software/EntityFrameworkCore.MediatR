$csproj = Join-Path $PSScriptRoot "..\Source\RescuePC.Software.EntityFrameworkCore.MediatR\RescuePC.Software.EntityFrameworkCore.MediatR.csproj"
$xml = [xml](Get-Content $csproj)
$version = $xml.Project.PropertyGroup.Version

if (-not $version) {
	Write-Error "Nie znaleziono elementu <Version> w pliku $csproj."
	exit 1
}

Write-Host "Wersja odczytana z csproj: $version" -ForegroundColor DarkCyan
$tag = "v$version"

$status = git status --porcelain
if ($status) {
	Write-Error "Są niezacommitowane zmiany. Zrób commit przed publikacją."
	exit 1
}

Write-Host "Wypychanie brancha..." -ForegroundColor Cyan
git push origin
if ($LASTEXITCODE -ne 0) {
	Write-Error "Nie udało się wypchnąć brancha."
	exit 1
}

Write-Host "Budowanie paczki NuGet $version..." -ForegroundColor Cyan
$artifacts = Join-Path $PSScriptRoot "..\artifacts"
dotnet pack $csproj --configuration Release --output $artifacts /p:Version=$version
if ($LASTEXITCODE -ne 0) {
	Write-Error "Nie udało się zbudować paczki."
	exit 1
}

$nupkg = Get-ChildItem $artifacts -Filter "*.nupkg" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $nupkg) {
	Write-Error "Nie znaleziono pliku .nupkg w $artifacts."
	exit 1
}

Write-Host "Wypychanie paczki $($nupkg.Name) do GitHub Packages..." -ForegroundColor Cyan
dotnet nuget push $nupkg.FullName `
	--source "RescuePC-GitHub" `
	--skip-duplicate
if ($LASTEXITCODE -ne 0) {
	Write-Error "Nie udało się wypchnąć paczki."
	exit 1
}

Write-Host "Tworzenie taga $tag..." -ForegroundColor Cyan
git tag $tag
if ($LASTEXITCODE -ne 0) {
	Write-Error "Nie udało się utworzyć taga $tag."
	exit 1
}

git push origin $tag
if ($LASTEXITCODE -ne 0) {
	Write-Error "Nie udało się wypchnąć taga $tag."
	exit 1
}

Write-Host "Paczka $($nupkg.Name) została opublikowana, tag $tag wypchnięty." -ForegroundColor Green
