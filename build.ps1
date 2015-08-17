<#
	Copyright © 2015 Waldi Ravens

	This file is part of SE-WordListValidator.

	SE-WordListValidator is free software: you can redistribute it
	and/or modify it under the terms of the GNU General Public License
	as published by the Free Software Foundation, either version 3 of
	the License, or (at your option) any later version.

	SE-WordListValidator is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License along
	with SE-WordListValidator.  If not, see <http://www.gnu.org/licenses/>.
#>
param (
	[switch]$Build,
	[switch]$Clean,
	[switch]$Upkeep,
	[switch]$Verbose,
	[switch]$Archive,
	[switch]$UpdateAssemblyInfo
)
$ErrorActionPreference = 'stop'; $Error.Clear(); $Error.Capacity = 16
$TopLevelDirectoryName = (get-item -force -literalpath $myInvocation.myCommand.Path).DirectoryName

function Upkeep {
	filter XmlShortTagElements {
		if ($_.HasChildNodes) {
			$_.GetEnumerator() | XmlShortTagElements
		} elseif ($_.NodeType -eq [System.Xml.XmlNodeType]::Element) {
			if (!$_.IsEmpty) { $_.IsEmpty = $true }
		}
	}
	function XmlUpkeep {
	  Begin {
		$xmlEncoding		= new-object -type System.Text.UTF8Encoding -args $true, $true
		$xmlReaderSettings	= new-object -type System.Xml.XmlReaderSettings -property @{ CloseInput = $true; XmlResolver = $null }
		$xmlWriterSettings	= new-object -type System.Xml.XmlWriterSettings -property @{ CloseOutput = $false; Encoding = $xmlEncoding; Indent = $true; OmitXmlDeclaration = $false }
		$xmlDocument		= new-object -type System.Xml.XmlDocument -property @{ XmlResolver = $null }
		$xmlFileNames		= new-object -type System.Collections.ArrayList -args 100
	  }
	  Process {
		$file = $_
		$isXml = $true
		$xmlReader = [System.Xml.XmlReader]::Create($file.OpenRead(), $xmlReaderSettings)
		try {
			$xmlDocument.Load($xmlReader)
		} catch {
			if (!$_.Exception.InnerException -or $_.Exception.InnerException -isnot [System.Xml.XmlException]) {
				throw
			}
			$isXml = $false
		} finally {
			$xmlReader.Close()
		}
		if ($isXml) {
			$xmlDocument.DocumentElement | XmlShortTagElements
			$xmlStream = new-object -type System.IO.MemoryStream
			try {
				$xmlWriter = [System.Xml.XmlWriter]::Create($xmlStream, $xmlWriterSettings)
				try {
					$xmlDocument.Save($xmlWriter)
				} finally {
					$xmlWriter.Close()
					$xmlStream.Position = 0
				}
				if ($xmlStream.Length -eq $file.Length) {
					$fs = $file.OpenRead()
					try {
						$a = [System.Convert]::ToBase64String($sha512.ComputeHash($xmlStream))
						$b = [System.Convert]::ToBase64String($sha512.ComputeHash($fs))
						$isXml = ($a -cne $b)
					} finally {
						$fs.Close()
						$xmlStream.Position = 0
					}
				}
				if ($isXml) {
					$fs = $file.Create()
					try {
						$xmlStream.CopyTo($fs)
					} finally {
						$fs.Close()
						$xmlStream.Position = 0
					}
				}
			} catch {
				$Host.UI.WriteErrorLine("$($file.FullName.Substring($PWD.ProviderPath.Length).TrimStart('\')): $($_.Exception.Message)")
				exit 1
			} finally {
				$xmlStream.Close()
			}
			$null = $xmlFileNames.Add($file.FullName.Substring($PWD.ProviderPath.Length).TrimStart('\'))
		} else {
			return $file
		}
	  }
	  End {
		if ($Verbose -and $xmlFileNames.Count) {
			$Host.UI.WriteVerboseLine("Xml files:")
			$xmlFileNames.Sort([System.StringComparer]::InvariantCultureIgnoreCase)
			foreach ($name in $xmlFileNames) { $Host.UI.WriteVerboseLine("  ${name}") }
		}
	  }
	}
	function TextUpkeep {
	  Begin {
		$withBomEncoding = new-object -type System.Text.UTF8Encoding -args $true, $true
		$noBomEncoding = new-object -type System.Text.UTF8Encoding -args $false, $true
		$textFileNames = new-object -type System.Collections.ArrayList -args 100
		$textLine = new-object -type System.Text.StringBuilder -args 1000
		$bom = [char]0xFEFF
		$tab = [char]0x0009
		$sp = [char]0x0020
		$cr = [char]0x000D
		$lf = [char]0x000A
		$crlf = "${cr}${lf}"
	  }
	  Process {
		$file = $_
		$orig = get-content -encoding Byte -readcount 0 -literalpath $file
		$encoding = if ($file.Name.StartsWith('.git')) { $noBomEncoding } else { $withBomEncoding }
		try {
			$text = $encoding.GetString($orig).Replace($crlf, $lf).Replace($cr, $lf).TrimStart($bom).TrimEnd()
			$orig = [System.Convert]::ToBase64String($sha512.ComputeHash($orig))
		} catch {
			return $file
		}
		$expand = '.ps1', '.sln' -notcontains $file.Extension
		$text =  ($text.Split($lf) | foreach-object {
			$line = $_.TrimEnd()
			if ($expand -and $line.IndexOf($tab) -ge 0) {
				for ($textLine.Length = $i = 0; $i -lt $line.Length; $i++) {
					if (($c = $line[$i]) -ceq $tab) {
						do { $null = $textLine.Append($sp) } while ($textLine.Length % 4)
					} else {
						$null = $textLine.Append($c)
					}
				}
				$line = $textLine.ToString()
			}
			$line
		}) -join [System.Environment]::NewLine
		if ($text.Length) {
			$text += [System.Environment]::NewLine
		}
		$fs = $null
		try {
			$text = $encoding.GetPreamble() + $encoding.GetBytes($text)
			if ($text.Length -ne $file.Length -or [System.Convert]::ToBase64String($sha512.ComputeHash($text)) -cne $orig) {
				($fs = $file.Create()).Write($text, 0, $text.Length)
				$Host.UI.WriteLine("TXT: $($file.FullName.Substring($PWD.ProviderPath.Length).TrimStart('\'))")
			}
		} catch {
			$Host.UI.WriteErrorLine("$($file.FullName.Substring($PWD.ProviderPath.Length).TrimStart('\')): $($_.Exception.Message)")
			exit 1
		} finally {
			if ($fs) { $fs.Close() }
		}
		$null = $textFileNames.Add($file.FullName.Substring($PWD.ProviderPath.Length).TrimStart('\'))
	  }
	  End {
		if ($Verbose -and $textFileNames.Count) {
			$Host.UI.WriteVerboseLine("Text files:")
			$textFileNames.Sort([System.StringComparer]::InvariantCultureIgnoreCase)
			foreach ($name in $textFileNames) { $Host.UI.WriteVerboseLine("  ${name}") }
		}
	  }
	}
	function BinaryUpkeep {
	  Begin {
		$binFileNames = new-object -type System.Collections.ArrayList -args 100
	  }
	  Process {
		$null = $binFileNames.Add($_.FullName.Substring($PWD.ProviderPath.Length).TrimStart('\'))
	  }
	  End {
		if ($Verbose -and $binFileNames.Count) {
			$Host.UI.WriteVerboseLine("Binary files:")
			$binFileNames.Sort([System.StringComparer]::InvariantCultureIgnoreCase)
			foreach ($name in $binFileNames) { $Host.UI.WriteVerboseLine("  ${name}") }
		}
	  }
	}
	$sha512 = [System.Security.Cryptography.HashAlgorithm]::Create('SHA512')
	try {
		$files = @(git diff --diff-filter=AM --name-only HEAD | foreach-object { get-item -force -literalpath $_ })
		if ($files.Length) {
			$files | XmlUpkeep | TextUpkeep | BinaryUpkeep
		} elseif ($Upkeep) {
			git ls-tree --full-tree --name-only -r HEAD | foreach-object { get-item -force -literalpath $_ } | XmlUpkeep | TextUpkeep | BinaryUpkeep
		}
	} finally {
		$sha512.Dispose()
	}
}
function Create-Archive {
	$version = (select-string '^\[assembly: AssemblyVersion\("([^"]+)"\)\]' SE-WordListValidator\Properties\AssemblyInfo.cs).Matches[0].Groups[1].Value
	do { $guid = [System.Guid]::NewGuid() } while ($guid.Equals([System.Guid]::Empty))
	$zipfolder = $zipfile = $null
	try {
		$zipfile = new-item -force -type File -value $null -name "SE-WordListValidator-${version}.7z"
		$zipfolder = new-item -type Directory -name $guid
		copy-item -destination $zipfolder -literalpath README.txt, LICENSE.txt, SE-WordListValidator\bin\Release\SE-WordListValidator.exe
		$zipfile.Delete()
		$c = sevenz a -t7z -mx9 -mmt1 -m0=LZMA2:d=1024m:fb=273 -- "$($zipfile.FullName)" "$($zipfolder.FullName)\*" 2>&1
		if ($?) {
			explorer '/select,' $zipfile.FullName
			$zipfile = $null
		} else {
			$c | write-host -foregroundcolor yellow
		}
	} finally {
		if ($zipfolder) { try { $zipfolder.Delete($true) } catch { start-sleep -milliseconds 1500; try { $zipfolder.Delete($true) } catch { start-sleep -milliseconds 2500; try { $zipfolder.Delete($true) } catch {} } } }
		if ($zipfile) { $zipfile.Delete() }
	}
}
push-location -literalpath $TopLevelDirectoryName
try {
	if ($UpdateAssemblyInfo) {
		$githash = [string]::Empty
		$revno = '0'
		try {
			$githash = git rev-parse --verify HEAD 2>&1
			$revno = git describe --tags 2>&1
			$revno = "$(1 + "${revno}-0".Split('-')[1])"
		} catch {
			$Host.UI.WriteWarningLine($_.Exception.Message)
		}
		((get-content -encoding UTF8 -literalpath SE-WordListValidator\Properties\AssemblyInfo.template.cs) -creplace '\[GITHASH\]', $githash) -creplace '\[REVNO\]', $revno |
			set-content -encoding UTF8 -literalpath SE-WordListValidator\Properties\AssemblyInfo.cs
		exit 0
	}
#	get-process -erroraction:SilentlyContinue -processname MSBuild | stop-process
	get-childitem -force -recurse -include TestResults, Release, Debug, *.7z | remove-item -force -recurse
	if ($Upkeep -or (!$Clean -and !$Build)) {
		Upkeep
	}
	if ($Build -or (!$Upkeep -and !$Clean)) {
		MSBuild SE-WordListValidator\SE-WordListValidator.sln /m /v:minimal /t:Rebuild /p:"Configuration=Release;Platform=Any CPU"
	}
	if ($Archive) { Create-Archive }
} finally {
	pop-location
}
