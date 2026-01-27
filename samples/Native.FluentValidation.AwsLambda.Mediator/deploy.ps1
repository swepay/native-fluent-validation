param(
    [string]$StackName = "native-fluentvalidation-lambda",
    [string]$Region = "us-east-1"
)

sam build
sam deploy --stack-name $StackName --region $Region --capabilities CAPABILITY_IAM
