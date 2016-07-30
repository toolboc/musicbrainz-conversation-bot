##### Variables
    ## Global
    $rgName = "DXHack_MusicBrainz"
    $location = "westus"

    ## Storage
    $storageName = "MusicBrainz"
    $storageType = "Standard_GRS"

    ## Network
    $nicname = "MB1"
    $subnet1Name = "MBSub1"
    $vnetName = "MuscBrainzNet"
    $vnetAddressPrefix = "10.0.0.0/16"
    $vnetSubnetAddressPrefix = "10.0.0.0/24"

    ## Compute
    $vmName = "MusicBrainz"
    $computerName = "MBComputer"
    $vmSize = "Standard_A2"
    $osDiskName = $vmName + "osDisk"

##### Resource Group
    New-AzureResourceGroup -Name $rgName -Location $location

##### Storage
    $storageacc = New-AzureStorageAccount -ResourceGroupName $rgName -Name $storageName -Type $storageType -Location $location

##### Network
    $pip = New-AzurePublicIpAddress -Name $nicname -ResourceGroupName $rgName -Location $location -AllocationMethod Dynamic
    $subnetconfig = New-AzureVirtualNetworkSubnetConfig -Name $subnet1Name -AddressPrefix $vnetSubnetAddressPrefix
    $vnet = New-AzureVirtualNetwork -Name $vnetName -ResourceGroupName $rgName -Location $location -AddressPrefix $vnetAddressPrefix -Subnet $subnetconfig
    $nic = New-AzureNetworkInterface -Name $nicname -ResourceGroupName $rgName -Location $location -SubnetId $vnet.Subnets[0].Id -PublicIpAddressId $pip.Id

##### Compute
    ## Setup local VM object
    $cred = Get-Credential
    $vm = New-AzureVMConfig -VMName $vmName -VMSize $vmSize
    $vm = Set-AzureVMOperatingSystem -VM $vm -Windows -ComputerName $computerName -Credential $cred -ProvisionVMAgent -EnableAutoUpdate

    $vm = Add-AzureVMNetworkInterface -VM $vm -Id $nic.Id

    $osDiskUri = "http://test.blob.core.windows.net/vmcontainer10798c80-131-1231-a94a-f9d2a712251f/osDisk.10798c80-2919-4100-a94a-f9d2a712251f.vhd"
    $imageUri = "http://test.blob.core.windows.net/system/Microsoft.Compute/Images/captured/image-osDisk.8b021d87-913c-4f94-a01a-944ad92d7388.vhd"
    $vm = Set-AzureVMOSDisk -VM $vm -Name $osDiskName -VhdUri $osDiskUri -CreateOption fromImage -SourceImageUri $imageUri -Windows

    $dataImageUri = "http://test.blob.core.windows.net/system/Microsoft.Compute/Images/captured/image-dataDisk-0.8b021d87-913c-4f94-a01a-944ad92d7388.vhd"
    $dataDiskUri = "http://test.blob.core.windows.net/vmcontainer10798c80-sa11-41sa-dsad-f9d2a712251f/dataDisk-0.10798c80-2919-4100-a94a-f9d2a712251f.vhd"
    $vm = Add-AzureVMDataDisk -VM $vm -Name "dd1" -VhdUri $dataDiskUri -SourceImageUri $dataImageUri -Lun 0 -CreateOption fromImage

    ## Create the VM in Azure
    New-AzureVM -ResourceGroupName $rgName -Location $location -VM $vm -Verbose