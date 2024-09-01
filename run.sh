git clone https://github.com/chupino/BackendDotnet.git backend
cd backend
docker build -t backend .

if [ $? -eq 0 ]; then
	echo "bien"
else
	echo "mal"
	exit 1
fi

docker run -dp 8000:80 backend
