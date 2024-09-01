sudo yam update -y
sudo yam install docker -y
sudo yam install git -y
sudo systemctl start docker
sudo usermod -aG docker $USER
newgrp docker
