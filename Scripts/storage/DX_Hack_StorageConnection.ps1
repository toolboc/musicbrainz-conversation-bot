$ENV:AZURE_STORAGE_ACCOUNT = 'dxhack4423'
$ENV:AZURE_sTORAGE_ACCESS_KEY = 'HVSlIRF1yb5px+tuyzTiavv2/iGtmCeYrDI35YGaZc48bCeIS+W0QJknzEdw26MPPoA1MmFwcY6p7ew5seu53A=='

$imagename = "musicbrainz"
$blobUrl = "https://dxhack4423.blob.core.windows.net"
$localvhdPath = "C:\Users\Jessica Deen\Desktop\musicbrainz\musicbrainz.vhd"
$container_name = "vhds"
$destinationfolder = "C:\USers\Jessica Deen\Desktop"

echo "Uploading the image..."
azure storage blob upload $localvhdPath $container_name $imagename

echo "Listing the image to confirm successful upload..."
azure storage blob list $container_name

#optional local download command
echo "Downloading the image to local folder..."
azure storage blob download $container_name $imagename $destinationfolder

