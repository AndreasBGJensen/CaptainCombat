# This Script relies on the your ssh key being saved in CAPCOMB_SSH_KEY
# Use 'export CAPCOMB_SSH_KEY=<PATH TO YOUR SSH KEY HERE>' prior to running this script

set -e

printf "Cleaning...\n"

ssh -i "$CAPCOMB_SSH_KEY" captaincombat_user@maltebp.dk "rm -rf ./CaptainCombat"

printf "Uploading...\n"

scp -q -r -i "$CAPCOMB_SSH_KEY" Server/bin/Debug captaincombat_user@maltebp.dk:./CaptainCombat

printf "Done!"