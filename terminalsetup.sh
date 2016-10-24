sudo cp /home/pi/sources.list /etc/apt/sources.list
sudo apt-get update -y
sudo apt-get upgrade -y
#CouchDB
sudo apt-get install -y couchdb
#PN532Reader
sudo cd /home/pi
sudo mkdir libnfc
sudo wget https://libnfc.googlecode.com/files/libnfc-1.7.0.tar.bz2
sudo tar -jxvf libnfc-1.7.0.tar.bz2
sudo cd libnfc-libnfc-1.7.0
sudo mkdir /etc/nfc
sudo mkdir /etc/nfc/devices.d
sudo cp contrib/libnfc/pn532_uart_on_rpi.conf.sample /etc/nfc/devices.d/pn532_u$
sudo echo /etc/nfc/devices.d/pn532_uart_on_rpi.config "allow_intrusive_scan = t$
sudo apt-get install -y autoconf libtool
sudo apt-get install -y libpcsclite-dev libusb-dev
sudo autoreconf -vis
sudo ./configure --with-drivers=pn532_uart --sysconfdir=/etc --prefi=/usr
sudo make clean
sudo make install all
#mono
sudo apt-get install -y mono-complete
#avahi-daemon
sudo apt-get install -y avahi-daemon
#Dnssd Lib for Program
sudo apt-get install -y libavahi-compat-libdnssd-dev
#set hostname and hostname for avahi
sudo cp /home/pi/avahi-daemon.conf /etc/avahi/
cd

#add TimeRecordingTerminal to autostart
touch tempbash
head -n -1 .bashrc tempbash; mv tempbash .bashrc
crontab -u pi -l | { cat; echo "@reboot sudo mono /home/pi/TimeRecordingTermina$
#add hostname.sh to autostart on reboot to set hostname to macaddress
crontab -u pi -l | { cat; echo "@reboot sudo sh /home/pi/hostname.sh &"; } | cr$

#setup for services
sudo systemctl enable systemd-networkd-wait-online.service
sudo systemctl enable timerecordingterminal.service

