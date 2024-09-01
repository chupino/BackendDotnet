echo "Inializando EC2, espere 😄"
sudo yam update -y
sudo yam install docker -y
sudo yam install git -y
sudo systemctl start docker
sudo usermod -aG docker $USER
newgrp docker
echo "EC2 con los componentes necesarios..., toca iniciar el back 😛"

touch run.sh desrun.sh

echo "git clone https://github.com/chupino/BackendDotnet.git backend
cd backend
docker build -t backend .

if [ $? -eq 0 ]; then
        echo "bien"
else
        echo "mal"
        exit 1
fi

docker run -dp 8000:80 backend" > run.sh

echo "rm -rf backend
docker rm -f $(docker ps -lq)
docker rmi -f backend" > desrun.sh

chmod 777 *.sh

./run.sh

echo "Todo funca 😏"

