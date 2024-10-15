$ACRName = "collecto"
$ACRLoginServer = "$ACRName.azurecr.io"
$AppServiceName = "collecto"
$ResourceGroupName = "Collecto"
$DockerFilePath = ".\EmailCollector.Api\Dockerfile"
$NewVersionTag = "latest"
$BackupTag = "previous"
$APIImageName = "collecto-api"

function Push-DockerImage($tag) {
    docker build -t ${APIImageName} -f ${DockerFilePath} .
    docker tag "${APIImageName}:${NewVersionTag}" "${ACRLoginServer}/${APIImageName}:${NewVersionTag}"
    docker push "${ACRLoginServer}/${APIImageName}:${NewVersionTag}"
}

function Backup-ExistingImage() {
    # Re-tag the current image with the 'previous' tag inside ACR
    az acr repository tag copy --name ${ACRName} --image "${APIImageName}:${NewVersionTag}" --tag "${APIImageName}:${BackupTag}"
    Write-Host "Backed up the current image in ACR as ${BackupTag}."
}

# Choose Action: Deploy new version or Rollback
$action = Read-Host -Prompt "Type 'deploy' to deploy a new version or 'rollback' to roll back to the previous version"

if ($action -eq "deploy") {
    # Log in to ACR
    az acr login --name ${ACRName}

    # Backup the existing image before deploying the new one
    Backup-ExistingImage

    # Push the new version to ACR
    Push-DockerImage -tag ${NewVersionTag}

    # Update the Azure App Service with the new image
    az webapp config container set --name ${AppServiceName} --resource-group ${ResourceGroupName} --container-image-name "${ACRLoginServer}/${APIImageName}:${NewVersionTag}"

    Write-Host "Deployment of version ${NewVersionTag} is complete."

} elseif ($action -eq "rollback") {
    # Log in to ACR
    az acr login --name ${ACRName}

    # Swap the "previous" and "latest" tags in ACR
    # 1. Re-tag the current 'latest' as a temporary tag to avoid overwriting
    az acr repository tag copy --name ${ACRName} --image "${APIImageName}:${NewVersionTag}" --tag "${APIImageName}:rollback-temp"
    
    # 2. Re-tag 'previous' as 'latest' (perform the rollback)
    az acr repository tag copy --name ${ACRName} --image "${APIImageName}:${BackupTag}" --tag "${APIImageName}:${NewVersionTag}"

    # 3. Re-tag the original 'latest' (now rollback-temp) as 'previous'
    az acr repository tag copy --name ${ACRName} --image "${APIImageName}:rollback-temp" --tag "${APIImageName}:${BackupTag}"

    # 4. Remove the temporary tag
    az acr repository delete --name ${ACRName} --image "${APIImageName}:rollback-temp" --yes

    # Set the App Service to use the 'latest' tag (now pointing to the previously "previous" version)
    az webapp config container set --name ${AppServiceName} --resource-group ${ResourceGroupName} --container-image-name "${ACRLoginServer}/${APIImageName}:${NewVersionTag}"

    Write-Host "Rollback complete: swapped 'latest' with 'previous'."
} else {
    Write-Host "Invalid option. Please enter 'deploy' or 'rollback'."
}