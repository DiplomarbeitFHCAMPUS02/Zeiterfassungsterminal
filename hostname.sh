sleep 10
sudo hostname $(ifconfig | grep "HWaddr" | cut -d" " -f11 | tr -s ":" ".")