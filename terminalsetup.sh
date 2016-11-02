#!/bin/bash

#upgrade
sudo apt-get update -y
sudo apt-get upgrade -y

#CouchDB
sudo apt-get install -y couchdb


#PN532Reader
pushd .
cd
mkdir -p libnfc
wget https://bintray.com/artifact/download/nfc-tools/sources/libnfc-1.7.1.tar.bz2
tar -jxvf libnfc-1.7.1.tar.bz2
cd libnfc-1.7.1
sudo mkdir -p /etc/nfc
sudo mkdir -p /etc/nfc/devices.d
TMPFILE=$(tempfile) || exit
cp contrib/libnfc/pn532_uart_on_rpi.conf.sample "$TMPFILE"
echo "allow_intrusive_scan = true" >> "$TMPFILE"
sudo cp "$TMPFILE" /etc/nfc/devices.d/pn532_uart_on_rpi.conf
rm "$TMPFILE"
sudo apt-get install -y autoconf libtool
sudo apt-get install -y libpcsclite-dev libusb-dev
autoreconf -vis
./configure --with-drivers=pn532_uart --sysconfdir=/etc --prefi=/usr
sudo make clean
sudo make install all
popd

#mono
sudo apt-get install -y mono-complete
#avahi-daemon
sudo apt-get install -y avahi-daemon
#Dnssd Lib for Program
sudo apt-get install -y libavahi-compat-libdnssd-dev
#set hostname and hostname for avahi
sudo cp avahi-daemon.conf /etc/avahi/

###add TimeRecordingTerminal to autostart
##touch tempbash
##head -n -1 .bashrc tempbash; mv tempbash .bashrc
##crontab -u pi -l | { cat; echo "@reboot sudo mono /home/pi/TimeRecordingTermina$
###add hostname.sh to autostart on reboot to set hostname to macaddress
##crontab -u pi -l | { cat; echo "@reboot sudo sh /home/pi/hostname.sh &"; } | cr$

###setup for services
##sudo systemctl enable systemd-networkd-wait-online.service
##sudo systemctl enable timerecordingterminal.service

