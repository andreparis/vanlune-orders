$StackName = "vanlune-orders"

rm -r C:\Projects\vanlune\vanlune-order\Orders.Application\bin\Release\netcoreapp3.1\publish

dotnet publish C:\Projects\vanlune\vanlune-order -c release

$7zipPath = "$env:ProgramFiles\7-Zip\7z.exe"
if (-not (Test-Path -Path $7zipPath -PathType Leaf)) {
    throw "7 zip file '$7zipPath' not found"
}
Set-Alias 7zip $7zipPath
$Source = "C:\Projects\vanlune\vanlune-order\Orders.Application\bin\Release\netcoreapp3.1\publish\*"
$Target = "C:\Projects\vanlune\vanlune-order\Orders.Application\bin\Release\netcoreapp3.1\publish\Orders.zip"

7zip a -mx=9 $Target $Source

Get-Job | Wait-Job

aws s3 cp C:/Projects/vanlune/vanlune-order/Orders.Application/template-orders.yaml s3://vanlune-bin-dev
aws s3 cp C:\Projects\vanlune\vanlune-order\Orders.Application\bin\Release\netcoreapp3.1\publish\Orders.zip s3://vanlune-bin-dev

Get-Job | Wait-Job

$exists = aws cloudformation describe-stacks --stack-name $StackName
if ($exists)
{
	aws cloudformation  update-stack --stack-name $StackName --template-url https://vanlune-bin-dev.s3.amazonaws.com/template-orders.yaml --capabilities CAPABILITY_IAM CAPABILITY_AUTO_EXPAND
}
else
{
	aws cloudformation create-stack  --stack-name $StackName --template-url https://vanlune-bin-dev.s3.amazonaws.com/template-orders.yaml --capabilities CAPABILITY_IAM CAPABILITY_AUTO_EXPAND
}
aws cloudformation wait stack-update-complete --stack-name $StackName

aws lambda update-function-code --function-name vanlune-orders-assign                 --s3-bucket vanlune-bin-dev --s3-key Orders.zip
aws lambda update-function-code --function-name vanlune-orders-create         --s3-bucket vanlune-bin-dev --s3-key Orders.zip
aws lambda update-function-code --function-name vanlune-orders-get-all        --s3-bucket vanlune-bin-dev --s3-key Orders.zip
aws lambda update-function-code --function-name vanlune-orders-get-by-filters          --s3-bucket vanlune-bin-dev --s3-key Orders.zip
aws lambda update-function-code --function-name vanlune-orders-get-status               --s3-bucket vanlune-bin-dev --s3-key Orders.zip
aws lambda update-function-code --function-name vanlune-orders-get-user-email       --s3-bucket vanlune-bin-dev --s3-key Orders.zip
aws lambda update-function-code --function-name vanlune-orders-get-user-id       --s3-bucket vanlune-bin-dev --s3-key Orders.zip
aws lambda update-function-code --function-name vanlune-orders-paypal --s3-bucket vanlune-bin-dev --s3-key Orders.zip
aws lambda update-function-code --function-name vanlune-orders-update           --s3-bucket vanlune-bin-dev --s3-key Orders.zip