sudo apt-get update
sudo apt-get upgrade

sudo apt-get -y install docker.io
sudo apt-get -y install docker

ln -sf /usr/bin/docker.io /usr/local/bin/docker
sed 0i '$acomplete -F _docker docker' /etc/bash_completion.d/docker.io

sudo service docker start

sudo docker pull ubuntu

sudo docker run hello-world
