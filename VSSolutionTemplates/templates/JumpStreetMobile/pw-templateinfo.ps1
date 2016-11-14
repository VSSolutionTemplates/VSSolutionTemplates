[cmdletbinding()]
param()

$templateInfo = New-Object -TypeName psobject -Property @{
    Name = 'JumpStreetMobile'
    Type = 'ProjectTemplate'
    Description = 'Project Jump Street is an application accelerator for building Azure Mobile Applications.'
    DefaultProjectName = 'MyJumpStreetMobileProject'
    CreateNewFolder = $false
    AfterInstall = {
        Update-PWPackagesPathInProjectFiles -slnRoot ($SolutionRoot)
    }
}

$templateInfo | replace (
    ('JumpStreetMobile', {"$ProjectName"}, {"$DefaultProjectName"} ,@('*.sln','*.ps1','*.vstemplate','*.csproj','*.bak','*.cs','*.xml','*.plist','*.projitems','*.shproj','*.xaml','*.config','*.appxmanifest','*.deployproj','json') ),

    ('8EBB17C5-5B87-466B-99BE-709C04F71BC8', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('B095DC2E-19D7-4852-9450-6774808B626E', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('e651c0cb-f5fb-4257-9289-ef45f3c1a02c', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('1dfffd59-6b32-4937-bfde-1e10c11d22c3', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('4D2348EA-44AA-479F-80FB-EF67D64F4F3A', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('0A7800A3-784F-4822-8956-7BAC2C4D194E', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('6B0A711C-8401-4240-BA08-A8198EFC271E', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json'))
)

# when the template is run any filename with the given string will be updated
$templateInfo | update-filename (
    ,('JumpStreetMobile', {"$ProjectName"},$null,@('*.sln','*.csproj','*.bak','*.projitems','*.shproj','*.cs','*.deployproj','json'))
)
# excludes files from the template
$templateInfo | exclude-file 'pw-*.*','*.user','*.suo','*.userosscache','project.lock.json','*.vs*scc'
# excludes folders from the template
$templateInfo | exclude-folder '.vs','artifacts','packages','bin','obj' 

# This will register the template with pecan-waffle
Set-TemplateInfo -templateInfo $templateInfo

<#
Use this one-liner to figure out the include expression for the project name
> Get-ChildItem VSSolutionTemplates\templates * -Recurse -File|select-string 'JumpStreetMobile' -SimpleMatch|Select-Object -ExpandProperty path -Unique|% { Get-Item $_ | Select-Object -ExpandProperty extension}|Select-Object -Unique|%{ Write-Host "'*$_'," -NoNewline }


'.sln';'.vstemplate';'.csproj';'.bak';'.cs';'.xml';'.plist';'.projitems';'.shproj';'.xaml';'.config';'.pubxml';'.appxmanifest'

Use this one-liner to figure out the guids in your template
> Get-ChildItem .\VSSolutionTemplates\templates *.*proj -Recurse -File|Select-Object -ExpandProperty fullname -Unique|% { ([xml](Get-Content $_)).Project.PropertyGroup.ProjectGuid|Select-Object -Unique|%{ '({0}, {{"$ProjectId"}}, {{[System.Guid]::NewGuid()}},@("*.*proj")),' -f $_ }}

({8EBB17C5-5B87-466B-99BE-709C04F71BC8}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({B095DC2E-19D7-4852-9450-6774808B626E}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
(e651c0cb-f5fb-4257-9289-ef45f3c1a02c, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
(1dfffd59-6b32-4937-bfde-1e10c11d22c3, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({4D2348EA-44AA-479F-80FB-EF67D64F4F3A}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({0A7800A3-784F-4822-8956-7BAC2C4D194E}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({6B0A711C-8401-4240-BA08-A8198EFC271E}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),


use this one-liner to figure out the include statement for update-filename
Get-ChildItem C:\temp\pean-waffle\dest\new3 *jumpstreetmobile* -Recurse -File|Select-Object -ExpandProperty extension -Unique|%{ write-host ( '''{0}'',' -f $_) -NoNewline }

'.csproj','.bak','.projitems','.shproj','.cs'
#>