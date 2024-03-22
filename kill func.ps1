$CONTAINER_ID = docker ps -q --filter "label=killprogram"
$KILLFUNCTIONS = docker inspect --format '{{ index .Config.Labels \"killprogram\" }}' $CONTAINER_ID
docker exec -it $CONTAINER_ID sh -c $KILLFUNCTIONS
