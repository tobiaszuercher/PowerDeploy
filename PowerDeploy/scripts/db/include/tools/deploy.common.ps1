function xmlPeek($filePath, $xpath)
{ 
    [xml] $fileXml = Get-Content $filePath 
    $found = $fileXml.SelectSingleNode($xpath)

    if ($found.GetType().Name -eq 'XmlAttribute') { return $found.Value }

    return $found.InnerText
} 

function Write-Welcome
{
	$package_name = xmlPeek $package_xml "package/@id"
	$package_version = xmlPeek $package_xml "package/@version"
	$package_env = xmlPeek $package_xml "package/@environment"


	Write-Host ""
	Write-Host ""
	Write-Host "Welcome to your deploy shell!"
	Write-Host ""
	Write-Host "  Package: " -nonewline
	Write-Host $package_name v$package_version -ForegroundColor Red -nonewline
	Write-Host (" targeting {0}" -f $package_env.ToUpper())
	Write-Host "" 
}