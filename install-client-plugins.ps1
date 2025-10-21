# ================= SETTINGS =================
$DestinationBase = "./Assets"
$TmpDir = "./.tmp/"

Write-Host "Using base destination: $DestinationBase"

# Create the temporary directory if it doesn't exist
if (-not (Test-Path -Path $TmpDir)) {
    New-Item -ItemType Directory -Path $TmpDir | Out-Null
}

# ================= PROTOC =================
Write-Host "`n=== Downloading Unity Plugins (gRPC) ==="

$URL = "https://packages.grpc.io/archive/2019/11/6950e15882f28e43685e948a7e5227bfcef398cd-6d642d6c-a6fc-4897-a612-62b0a3c9026b/csharp/grpc_unity_package.2.26.0-dev.zip"
$Destination = "$DestinationBase/"

# Download
Write-Host "Downloading $URL..."
Invoke-WebRequest -Uri $URL -OutFile "$TmpDir/grpc_unity_package.2.26.0-dev.zip"

# Extract
Write-Host "Extracting $TmpDir/grpc_unity_package.2.26.0-dev.zip..."
Expand-Archive -Path "$TmpDir/grpc_unity_package.2.26.0-dev.zip" -DestinationPath $Destination -Force

if ($?) {
    Write-Host "DONE"
} else {
    Write-Host "Error: Failed to extract the contents of the zip file."
    exit 1
}

# ================= UniRx =================
Write-Host "`n=== Installing UniRx ==="

$URL = "https://github.com/neuecc/UniRx/archive/refs/tags/7.1.0.zip"
$Destination = "$DestinationBase/Plugins/UniRx/"

Write-Host "Downloading $URL..."
Invoke-WebRequest -Uri $URL -OutFile "$TmpDir/unirx.7.1.0.zip"

Write-Host "Extracting $TmpDir/unirx.7.1.0.zip..."
if (Test-Path "$TmpDir/UniRx-7.1.0/") {
    Remove-Item -Path "$TmpDir/UniRx-7.1.0/" -Recurse -Force
}
Expand-Archive -Path "$TmpDir/unirx.7.1.0.zip" -DestinationPath "$TmpDir/" -Force

Write-Host "Copying UniRx into Plugin folder..."
Copy-Item -Path "$TmpDir/UniRx-7.1.0/Assets/Plugins/UniRx/" -Destination $Destination -Recurse -Force
Write-Host "DONE"

# ================= Treeview =================
Write-Host "`n=== Installing Treeview ==="

$URL = "https://github.com/neomasterhub/Unity-Treeview.git"
$Destination = "$DestinationBase/Plugins/Treeview/"

Write-Host "Cloning repo from $URL..."
git clone $URL "$TmpDir/treeview/" | Out-Null

Write-Host "Copying TreeView into Plugin folder..."
Copy-Item -Path "$TmpDir/treeview/Assets/Treeview/" -Destination $Destination -Recurse -Force
Write-Host "DONE"

# ================= URDF Importer =================
Write-Host "`n=== Installing URDF Importer ==="

$URL = "https://github.com/Unity-Technologies/URDF-Importer.git"
$Destination = "$DestinationBase/Plugins/URDF-Importer/"

Write-Host "Cloning repo from $URL..."
git clone $URL "$TmpDir/urdfimporter/" | Out-Null

Write-Host "Copying URDF-Importer into Plugin folder..."
Copy-Item -Path "$TmpDir/urdfimporter/" -Destination $Destination -Recurse -Force
Write-Host "DONE"

# ================= ROS TCP Connector =================
Write-Host "`n=== Installing ROS-TCP-Connector ==="

$URL = "https://github.com/Unity-Technologies/ROS-TCP-Connector.git"
$Destination = "$DestinationBase/Plugins/ROS-TCP-Connector/"

Write-Host "Cloning repo from $URL..."
git clone $URL "$TmpDir/ros_tcp_connector/" | Out-Null

Write-Host "Copying ROS-TCP-Connector into Plugin folder..."
Copy-Item -Path "$TmpDir/ros_tcp_connector/" -Destination $Destination -Recurse -Force
Write-Host "DONE"

# ================= SimSharp =================
Write-Host "`n=== Installing SimSharp for Unity ==="

$URL = "https://github.com/Neroware/SimSharp.git"
$Destination = "$DestinationBase/Plugins/SimSharp/"

Write-Host "Cloning repo from $URL..."
git clone -b rz/unity $URL "$TmpDir/simsharp/" | Out-Null

Write-Host "Copying SimSharp into Plugin folder..."
Copy-Item -Path "$TmpDir/simsharp/" -Destination $Destination -Recurse -Force
Write-Host "DONE"

# ================= Cleanup =================
Write-Host "`nDeleting temp directory..."
Remove-Item -Recurse -Force -Path $TmpDir

Write-Host "`nAll done!"
