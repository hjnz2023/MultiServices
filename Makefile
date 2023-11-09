run_service2:
	docker run --rm -it -p 8002:80 \
	  -e ASPNETCORE_URLS="http://+" \
	  -e NEW_RELIC_LICENSE_KEY=$${NEW_RELIC_LICENSE_KEY} \
	  service2

run_myservice:
	docker run --rm -it -p 8000:80 -p 8001:443 \
	  -e NEW_RELIC_LICENSE_KEY=$${NEW_RELIC_LICENSE_KEY} \
	  -e ASPNETCORE_URLS="https://+;http://+" \
	  -e ASPNETCORE_HTTPS_PORT=8001 \
	  -e ASPNETCORE_Kestrel__Certificates__Default__Password=$${ASPNETCORE_Kestrel__Certificates__Default__Password} \
	  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx \
	  -v ${HOME}/.aspnet/https:/https/ myservice

release:
	dotnet publish -c Release

verify_swagger:
	st run --checks all https://localhost:8001/swagger/v1/swagger.json --report --request-tls-verify=false

build_myservice:
	docker build --rm -f Dockerfile -t myservice --build-arg="GIT_COMMIT_SHA=$$(git rev-parse HEAD)" .

build_service2:
	docker build --rm -f Dockerfile.service2 -t service2 --build-arg="GIT_COMMIT_SHA=$$(git rev-parse HEAD)" .