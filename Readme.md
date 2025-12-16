# REDNIT
### Join, team up, endless fun awaits!

## reach us by your device
[rednit.janipang.dev]([URL](https://rednit.janipang.dev))

## run app in local
- install dependencies
`dotnet restore`
- run app
`dotnet watch run`

## run app in docker container
### way1 - build & run manually
- build image
`docker build . -t rednit-img:dev-1.00`
- run container (please enter absolute path T_T)
`docker run -d -p 5000:5000 -v <REDNIT_DIR>\Datacenter:/app/Datacenter --name rednit rednit-img:dev-1.00`

### way2 - using docker compose
- run command
`docker compose up -d`
