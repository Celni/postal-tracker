start docker-compose -f docker-compose-app.yml up --force-recreate --build --remove-orphans --renew-anon-volumes

rem API
start http://localhost:9000/swagger/index.html
rem RabbitMQ UI
start http://localhost:15672