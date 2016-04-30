
function Get-ScriptDirectory{                                               
    split-path (((Get-Variable MyInvocation -Scope 1).Value).MyCommand.Path)
}                                                                           
$scriptdir = (Get-ScriptDirectory)
$templatesFolder = (Join-Path $scriptdir 'JumpStreetMobileVs\templates')

$xmlfrag = @'
    <Content Include="{0}">
      <IncludeInVSIX>true</IncludeInVSIX>
    </Content>
'@

$files = Get-ChildItem -Path $templatesFolder -Recurse -File -Exclude '*.sln','*.user','bin\**\*'

$oldpath = Get-Location
try{
    Set-Location $templatesFolder
    
    foreach($file in ($files.FullName|sort-object)){
        # create the xml that should be added to the project file
        $path = 'templates\{0}' -f ((Resolve-Path $file -Relative).TrimStart('.\'))
        $xmlfrag -f $path
    }
}
finally{
    Set-Location $oldpath
}