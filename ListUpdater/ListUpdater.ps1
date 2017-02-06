[cmdletbinding(SupportsShouldProcess=$True)]
Param(
[Parameter(Mandatory=$True)]
[string]$csvPath,

[Parameter(Mandatory=$True)]
[string]$distributionListName,

[Parameter(Mandatory=$True)]
[bool]$addNames,

[Parameter(Mandatory=$True)]
[bool]$removeNames,

[Parameter(Mandatory=$True)]
[bool]$testing
)

#set WhatIf preference if just testing
$WhatIfPreference = $testing

#Connect to Exchange
.$env:ExchangeInstallPath\bin\RemoteExchange.ps1
Connect-ExchangeServer -auto -ClientApplication:ManagementShell

#import CSV
$contacts = import-csv $csvPath

#check if distribution group exists and create if needed (after confirming)
$groupExists = Get-DistributionGroup -Id $distributionListName -ErrorAction 'SilentlyContinue'
if (-not $groupExists){
    New-DistributionGroup -Name $distributionListName -Confirm
}


#if adding names, check to see if name exists already, and if not, create and add
if (($addNames -eq $True) -and ($removeNames -eq $false)){
    foreach($c in $contacts){
        $MailContactCheck = Get-MailContact -Identity $c.email -ErrorAction SilentlyContinue
        $MailContactExists = $?
        if ($MailContactExists -eq $False){
            New-MailContact -Name ($c.first + " " + $c.last) -ExternalEmailAddress $c.email
            Add-DistributionGroupMember -Identity $distributionListName -Member $c.email
        }
        else {
            $name = ($c.first + " " + $c.last)
            Write-Host "$name already exists"
        }
    }
}

#if removing names, check to see if name exists already, anf if so, remove from list and delete
elseif (($addNames -eq $False) -and ($removeNames -eq $True)){
    foreach($c in $contacts){
        $MailContactCheck = Get-MailContact -ExternalEmailAddress $c.email -ErrorAction SilentlyContinue
        $MailContactExists = $?
        if ($MailContactExists -eq $True){
            Remove-DistributionGroupMember -Identity $distributionListName -Member $c.email
            Remove-MailContact -Identity ($c.first + " " + $c.last)
        }
        else {
            $name = ($c.first + " " + $c.last)
            Write-Host "$name doesn't exist"
        }
    }
}


