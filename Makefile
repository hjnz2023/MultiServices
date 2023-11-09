GIT_COMMIT_SHA=$(shell git rev-parse HEAD)

release:
	dotnet publish -c Release

verify_swagger:
	st run --checks all https://localhost:8001/swagger/v1/swagger.json --report --request-tls-verify=false

up:
	export GIT_COMMIT_SHA=$${GIT_COMMIT_SHA} && docker-compose up -d --build

down:
	docker-compose down --volumes

printenv:
	export GIT_COMMIT_SHA=$${GIT_COMMIT_SHA} && printenv