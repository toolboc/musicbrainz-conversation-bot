#begin
# Update with the name of your subscription.
$SubscriptionName = "Visual Studio Enterprise"

# Give a name to your new storage account. It must be lowercase!
$StorageAccountName = "jdcvhds"

# Choose "West US" as an example.
$Location = "West US"

# Give a name to your new container.
$ContainerName = "vhds"

# Have an image file and a source directory in your local computer.
$ImageToUpload = "C:\Users\Jessica Deen\Desktop\musicbrainz\musicbrainz.vhd"

# A destination directory in your local computer.
$DestinationFolder = "C:\DownloadImages"

$rgName = "vhds"

$blobName = "musicBrainz.vhd"

# Add your Azure account to the local PowerShell environment.
Add-AzureRMAccount

# Set a default Azure subscription.
Select-AzureRmSubscription -SubscriptionName $SubscriptionName

# Set a default storage account.

Get-AzureRmStorageAccount -Name $storageAccountName -ResourceGroupName $rgName

# Get Storage Account Key - copy first one
Get-AzureRmStorageAccountKey -Name $storageAccountName -ResourceGroupName $rgname

$ctx = "[paste storage acccount key here]"

# Upload a blob into a container.
Set-AzureStorageBlobContent -Container $ContainerName -File $ImageToUpload -blob $blobName -BlobType Page -context $ctx

# List all blobs in a container.
Get-AzureStorageBlob -Container $ContainerName

#optional musicbrainz Add Blob to VMImage
$mediaLocation = "jdcvhds.blob.core.windows.net/vhds/musicbrainz.vhd"
Add-AzureVMImage -ImageName musicBrainz -MediaLocation $mediaLocation -OS Linux


#end