param($installPath, $toolsPath, $package, $project)
foreach ($reference in $project.Object.References)
{
    if($reference.Name.StartsWith("Codeable.Foundation"))
    {
        if($reference.CopyLocal -eq $true)
        {
            $reference.CopyLocal = $false;
        }
        else
        {
            $reference.CopyLocal = $true;
        }
    }
}