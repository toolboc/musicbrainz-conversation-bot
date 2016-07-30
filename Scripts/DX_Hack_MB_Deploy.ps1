##### Variables
    ## Global
    $rgName = "DXHack_MusicBrainz"
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

$storageAccName = "jdcvhds"

#Enter a new user name and password in the pop-up window for the following
$cred = Get-Credential

#Get the storage account where the uploaded image is stored
$storageAcc = Get-AzureRmStorageAccount -ResourceGroupName $rgName -AccountName $storageAccName

#Set the VM name and size
#Use "Get-Help New-AzureRmVMConfig" to know the available options for -VMsize
$vmConfig = New-AzureRmVMConfig -VMName $vmName -VMSize $vmSize

#Set the Windows operating system configuration and add the NIC
$vm = Set-AzureRmVMOperatingSystem -VM $vm -Linux -ComputerName $ComputerName -Credential $cred

$vm = Add-AzureRmVMNetworkInterface -VM $vm -Id $nic.Id

#Create the OS disk URI
$osDiskUri = '{0}vhds/{1}{2}.vhd' -f $storageAcc.PrimaryEndpoints.Blob.ToString(), $vmName.ToLower(), $osDiskName

#Configure the OS disk to be created from the image (-CreateOption fromImage), and give the URL of the uploaded image VHD for the -SourceImageUri parameter
#You can find this URL in the result of the Add-AzureRmVhd cmdlet above

$urlOfUploadedImageVhd = "https://jdcvhds.blob.core.windows.net/vhds/MusicBrainz.vhd"

$vm = Set-AzureRmVMOSDisk -VM $vm -Name $osDiskName -VhdUri $osDiskUri -CreateOption fromImage -SourceImageUri $urlOfUploadedImageVhd -Linux

#Create the new VM
New-AzureRmVM -ResourceGroupName $rgName -Location $location -VM $vm

#Verify VM Creation
$vmList = "Get-AzureRMVM -ResourceGroupName $rgName"
$vmList.Name
