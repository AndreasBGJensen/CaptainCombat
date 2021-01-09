# This Script relies on the your ssh key being saved in CAPCOMB_SSH_KEY
# Use 'export CAPCOMB_SSH_KEY=<PATH TO YOUR SSH KEY HERE>' prior to running this script

scp -r -i "$CAPCOMB_SSH_KEY" RemoteServer/bin/Debug captaincombat_user@maltebp.dk:./CaptainCombat