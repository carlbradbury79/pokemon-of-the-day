# PokemonScheduler Lambda

This project contains a .NET 8 AWS Lambda function that is triggered on a daily schedule. Its purpose is to select a random Pokémon, fetch its data from the public [PokeAPI](https://pokeapi.co/), and save the resulting JSON data to an S3 bucket.

This serves as the "Pokémon of the Day" data source for the corresponding `PokemonApi` function.

## How it Works

1.  **Trigger**: The function is designed to be triggered by an Amazon EventBridge (CloudWatch Events) schedule. A typical schedule would be a cron expression to run it once per day (e.g., `cron(0 10 * * ? *)` for 10:00 AM UTC daily).
2.  **Execution**:
    - A random Pokémon ID between 1 and 1025 is generated.
    - A GET request is made to `https://pokeapi.co/api/v2/pokemon/{id}` to fetch the Pokémon's data.
    - The JSON response is saved to an S3 bucket. The object key is formatted as `YYYY-MM-DD.json` (e.g., `2026-03-08.json`).

## Configuration

The function requires a single environment variable to be configured:

- `BUCKET_NAME`: The name of the S3 bucket where the daily Pokémon JSON file will be stored.

This can be set for local development in the `aws-lambda-tools-defaults.json` file and must be configured in the Lambda function's environment settings when deployed to AWS.

## Deployment

This project is set up for easy deployment using the AWS Toolkit for Visual Studio or the .NET Core CLI. The `aws-lambda-tools-defaults.json` file contains the default deployment settings.

To deploy from the command line, navigate to the project directory and run:

```bash
dotnet lambda deploy-function
```

The tool will use the values in `aws-lambda-tools-defaults.json` to configure the function during deployment.
