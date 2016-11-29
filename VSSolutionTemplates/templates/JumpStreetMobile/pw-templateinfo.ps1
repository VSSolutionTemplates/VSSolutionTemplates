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

    ('3AFA591D-C115-4AC9-9519-F788373ECDE9', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('5FA0219F-B063-4D07-800B-CC38BDB5B916', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('34B9099E-4B00-4707-BFDE-0B95997F8893', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('0483302A-9154-434A-93EB-5633AAFD4409', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('059786BD-D4DA-4770-8624-13A84BFC97AC', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('6B0A711C-8401-4240-BA08-A8198EFC271E', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('209FA716-A7AD-4095-BD70-C8710FC66FA7', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json')),
    ('68D82A27-F8EA-4550-9AC7-766CC4CA164E', {$null}, {[System.Guid]::NewGuid()},@('*.sln','*.ps1','*.csproj','*.bak','*.projitems','*.shproj','*.deployproj','json'))
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

({3AFA591D-C115-4AC9-9519-F788373ECDE9}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({5FA0219F-B063-4D07-800B-CC38BDB5B916}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({34B9099E-4B00-4707-BFDE-0B95997F8893}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({0483302A-9154-434A-93EB-5633AAFD4409}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({059786BD-D4DA-4770-8624-13A84BFC97AC}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({6B0A711C-8401-4240-BA08-A8198EFC271E}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({209FA716-A7AD-4095-BD70-C8710FC66FA7}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),
({68D82A27-F8EA-4550-9AC7-766CC4CA164E}, {"$ProjectId"}, {[System.Guid]::NewGuid()},@("*.*proj")),


use this one-liner to figure out the include statement for update-filename
Get-ChildItem C:\temp\pean-waffle\dest\new3 *jumpstreetmobile* -Recurse -File|Select-Object -ExpandProperty extension -Unique|%{ write-host ( '''{0}'',' -f $_) -NoNewline }

'.csproj','.bak','.projitems','.shproj','.cs'
#>