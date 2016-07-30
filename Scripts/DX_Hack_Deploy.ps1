##### Variables
    ## Global
    $rgName = "DXHack_MusicBrainz1"
    $location = "westus"

    ## Storage
    $storageName = "musicbrainz0730" #use lowercase and numbers
    $storageType = "Standard_GRS"

    ## Network
    $nicname = "MB1"
    $subnetName1 = "MBSub1"
    $vnetName = "MuscBrainzNet"
    $vnetAddressPrefix = "10.0.0.0/16"
    $vnetSubnetAddressPrefix = "10.0.0.0/24"

    ## Compute
    $vmName = "MusicBrainz"
    $computerName = "MBComputer"
    $vmSize = "Standard_A2"
    $osDiskName = $vmName + "osDisk"

##### Resource Group
    New-AzureRMResourceGroup -Name $rgName -Location $location

##### Storage
    $storageacc = New-AzureRmStorageAccount -Location $location -Name $storageName -ResourceGroupName $rgName -SkuName $storageType


##### Network
    $subnetconfig = New-AzureRmVirtualNetworkSubnetConfig -AddressPrefix $vnetSubnetAddressPrefix -Name $subnetName1
    $vnet = New-AzureRmVirtualNetwork -AddressPrefix $vnetAddressPrefix -Location $location -Name $vnetName -ResourceGroupName $rgName -subnet $subnetconfig    
    $pip = New-AzureRmPublicIpAddress -AllocationMethod Dynamic -ResourceGroupName $rgName -Location $location -Name $nicname
    $nic = New-AzureRmNetworkInterface -Location $location -Name $nicname -ResourceGroupName $rgName -SubnetId $vnet.Subnets[0].Id -PublicIpAddressId $pip.Id

##### Compute
    ## Setup local VM object
    $cred = Get-Credential
    $vm = New-AzureRmVMConfig -VMName $vmName -VMSize $vmsize
    $vm = Set-AzureRmVMOperatingSystem -VM $vm -Linux -ComputerName $ComputerName -Credential $cred
    $vm = Add-AzureRmVMNetworkInterface -VM $vm -Id $nic.Id
    $vm = Set-AzureRmVMOSDisk -CreateOption fromImage -Name $osDiskName -VhdUri $osDiskUri -VM $vm -Linux -SourceImageUri $imageUri
    $osDiskUri = "https://dxhack4423.blob.core.windows.net/system/Microsoft.Compute/Images/vhds/DXHackDockerClone-osDisk.ae2e253d-e28f-4ff9-8fb0-848f7f77e2db.vhd"
    $imageUri = "https://dxhack4423.blob.core.windows.net/vhds/DockerClone201662994043.vhd"
 
    ## Create the VM in Azure
    New-AzureRmVM -Location $location -ResourceGroupName $rgName -VM $vm -Verbose


    ##optional musicbrainz Image Test
    $mediaLocation = "jdcvhds.blob.core.windows.net/vhds/musicbrainz.vhd"
    Add-AzureVMImage -ImageName musicBrainz -MediaLocation $mediaLocation -OS Linux