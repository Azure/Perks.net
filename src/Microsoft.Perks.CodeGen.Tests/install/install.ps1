
pushd $psscriptroot 

# remove anything here. 
$null = rmdir -recurse -force -ea 0 ..\..\Microsoft.Perks.CodeGen\obj\ 
$null = rmdir -recurse -force -ea 0 ..\..\Microsoft.Perks.CodeGen\bin\
$null = rmdir -recurse -force -ea 0 .\install\obj\ 
$null = rmdir -recurse -force -ea 0 .\install\bin\
$null = mkdir -ea 0 "..\..\Microsoft.Perks.CodeGen\bin\Release"
$null = mkdir -ea 0 "..\..\Microsoft.Perks.CodeGen\bin\Debug" 

# remove installed copies of the tool too.
$null = rmdir -recurse -force -ea 0  "$env:USERPROFILE\.nuget\packages\dotnet-Perks.CodeGen\"
$null = rmdir -recurse -force -ea 0  "$env:USERPROFILE\.nuget\packages\.tools\dotnet-Perks.CodeGen\"

# build this tool
dotnet restore ..\..\Microsoft.Perks.CodeGen
dotnet pack ..\..\Microsoft.Perks.CodeGen

# trick dotnet into installing the package that we just built.
dotnet restore 
popd 