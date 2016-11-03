#!/bin/bash

USERNAME=$(whoami)

function install_base {
	#upgrade
	sudo apt-get update -y
	sudo apt-get upgrade -y
	
	#CouchDB
	sudo apt-get install -y couchdb

	sudo sed -i 's/^.*bind_address = .*$/bind_address = 0.0.0.0/' /etc/couchdb/local.ini
	sudo sed -i 's/^.*admin = .*$/admin = pw/' /etc/couchdb/local.ini
	sudo systemctl restart couchdb
}

function install_pn532 {
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
	./configure --with-drivers=pn532_uart --sysconfdir=/etc --prefix=/usr
	sudo make clean
	sudo make install all
	popd
}

function install_mono {
	#mono
	sudo apt-get install -y mono-complete
	#avahi-daemon
	sudo apt-get install -y avahi-daemon
	#Dnssd Lib for Program
	sudo apt-get install -y libavahi-compat-libdnssd-dev
	#mono zeroconf
	sudo apt-get install -y libmono-zeroconf1.0-cil
	#set hostname and hostname for avahi
	sudo cp avahi-daemon.conf /etc/avahi/
	sudo systemctl restart avahi-daemon
}


function setup_autologin {
	#autologin
	TMPFILE=$(tempfile) || exit
	cat > "$TMPFILE" <<EOF
[Service]
ExecStart=
ExecStart=-/sbin/agetty --autologin $USERNAME --noclear %I \$TERM
EOF
	sudo mkdir -p /etc/systemd/system/getty@tty1.service.d
	sudo cp "$TMPFILE" /etc/systemd/system/getty@tty1.service.d/autologin.conf
	sudo chmod a+r /etc/systemd/system/getty@tty1.service.d/autologin.conf
}


function setup_autostart {
	chmod a+x $(pwd)/TimeRecordingTerminal/TimeRecordingTerminal/bin/Release/TimeRecordingTerminal.exe
	sed -i '/\/dev\/tty1/,/^fi$/d' $HOME/.profile
	cat >> $HOME/.profile <<EOF
if [ "\$(tty)" == "/dev/tty1" ]; then
	sudo hostname \$(ifconfig | grep "HWaddr" | cut -d" " -f11 | tr -d ":")
	$(pwd)/TimeRecordingTerminal/TimeRecordingTerminal/bin/Release/TimeRecordingTerminal.exe
fi
EOF
}

function setup_uart {
	sudo sed -i '/enable_uart=[10]/d' /boot/config.txt
	echo 'enable_uart=1' | sudo tee -a /boot/config.txt
}

install_base
install_pn532
install_mono
setup_autologin
setup_autostart
setup_uart

