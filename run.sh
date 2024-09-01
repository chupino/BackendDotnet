git clone https://github.com/chupino/BackendDotnet.git backend
cd backend
docker-compose up --build -d

if [ $? -eq 0 ]; then
	echo "bien"
else
	echo "mal"
	exit 1
fi
