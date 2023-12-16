# SecurityManager    
Ensures security and efficient session management by working with a Redis database. Responsible for token generation, validation, and handling module snapshots for data integrity.  


## Setup       
- docker build . -t securitymanager    
- docker run --name SecurityManager -p 9040:443 -p 8040:80 -d securitymanager  