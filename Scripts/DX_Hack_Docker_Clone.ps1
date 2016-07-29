#-----fill in these variables

$rgName = "DXHack"

$template = "/Users/jessicadeen/GitHub/musicbrainz-conversation-bot/Scripts/DX_LinuxDocker_Template.json" # local file path specified

$vmName = "DockerClone"

$vhdName = "DXHackDockerClone"

 

#-----stop the vm
azure vm deallocate -g $rgName -n $vmName

#-----generalize the image
azure vm generalize $rgName -n $vmName

#-----capture the image
azure vm capture $rgName $vmName $vhdName -t $template