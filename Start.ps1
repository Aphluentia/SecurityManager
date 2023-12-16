# Script.ps1

# Change to the directory containing the docker-compose file
cd .\SecurityManager\

# Run docker-build
docker build . -t securitymanager

# Run docker run container
docker run --name SecurityManager -p 9040:443 -p 8040:80 -d securitymanager  
