# ---- Existing Resources ----

$rgName = "LVMTest"

$vNet = "LVMTest"

$subnet = ”Default”

# ---- New Network Resources ----

$pip = "CoolVHDTest"

$nic = "LVMNic2"

$vmName2 = "LVMTest2"

# ---- Network Creation ---- 
azure network public-ip create $rgName $pip -l "westus" 
azure network nic create $rgName $nic -k $subnet -m $vNet -p $pip -l "westus" 
azure network nic create $rgName $nic -k $subnet -m $vNet -p $pip -l "westus"

# --- Run the following to grab the ID of the new Nic 
azure network nic show $rgName $nic

# ---- Deployment ---- 
$deployName = "LVMDemo"
$template = "C:\Templates\Template1.json" # local file path specified
azure group deployment create $rgName $deployName -f $template