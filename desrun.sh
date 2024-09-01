rm -rf backend
docker rm -f $(docker ps -lq)
docker rmi -f backend-backend