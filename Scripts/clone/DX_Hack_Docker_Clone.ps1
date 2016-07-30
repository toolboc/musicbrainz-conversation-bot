#-----fill in these variables

$rgName = "DXHack"

$template = "C:\Users\Jessica Deen\Documents\GitHub\musicbrainz-conversation-bot\Scripts\DX_LinuxDocker_Template.json" # local file path specified

$vmName = "DockerClone"

$vhdName = "DXHackDockerClone"

$localfilepath = "C:\Users\Jessica Deen\Documents\GitHub\musicbrainz-conversation-bot\Images"

$sourceUri = "https://dxhack4423.blob.core.windows.net/system/Microsoft.Compute/Images/vhds/DXHackDockerClone-osDisk.ae2e253d-e28f-4ff9-8fb0-848f7f77e2db.vhd"
 

#-----stop the vm
azure vm deallocate -g $rgName -n $vmName

#-----generalize the image
azure vm generalize $rgName -n $vmName

#-----capture the image
azure vm capture $rgName $vmName $vhdName -t $template

#-----save VHD
Save-AzureRmVhd -ResourceGroupName $rgName -SourceUri $sourceUri -LocalFilePath $localfilepath\$vhdName
